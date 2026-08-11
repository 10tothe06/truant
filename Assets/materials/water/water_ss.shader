// this shader handles the screenspace (ss) pass of the water implementation

// it also handles fog above the water, because the water's depth information is required for that pass as well

Shader "Custom/water_ss"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // color data from the camera FOR THE SCENE (no water)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewVector : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                //calculating the forward vector of the camera
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                
                return o;
            }

            // input textures
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _WaterColor;
            sampler2D _WaterDepth;

            // visual data
            float3 waterCol;
            float waveAngles[12];
            float baseWaveFrequency;
            int isUnderWater;
            float3 sunDir;
            float timeValue;

            float getHeight(float3 pos) {
                float a = 50;
                float f = 0.05;

                float sum = 0;
                int iterations = 12;

                for (int i = 0; i < iterations; i++) {
                    float angle = waveAngles[i];

                    float x = cos(angle) * (pos.x * baseWaveFrequency) - sin(angle) * (pos.z * baseWaveFrequency);
                    x += timeValue*4;
                    
                    sum += pow(2.71828, sin(x*f) - 1) * a;

                    f *= 1.5;
                    a = lerp(a, 1, 0.5);
                }

                return sum/iterations;
            }

            float getGodrays(float3 pos) {
                float a = 50;
                float f = 1;

                float sum = 0;
                int iterations = 12;

                for (int i = 0; i < iterations; i++) {
                    float angle = waveAngles[i];

                    float x = cos(angle) * (pos.x * baseWaveFrequency) - sin(angle) * (pos.z * baseWaveFrequency);
                    x += timeValue/30;
                    
                    sum += pow(2.71828, sin(x*f) - 1) * a;

                    f *= 1.5;
                    a = lerp(a, 3, 0.5);
                }

                return sum/iterations;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                fixed4 waterColor = tex2D(_WaterColor, i.uv);
                fixed4 sceneColor = tex2D(_MainTex, i.uv);

                float waterDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_WaterDepth, i.uv)) * length(i.viewVector);
                float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * length(i.viewVector);

                float3 testPos = (rayOrigin + rayDir*1);
                float height = getHeight(testPos);
                if (height - testPos.y > 0) { // we're under the water

                    float distanceThroughWater = sceneDepth;
                    float lerpFactor = 0;
                    if (waterDepth < sceneDepth && waterDepth != 0) {
                        distanceThroughWater = waterDepth;
                        lerpFactor = 0.95;
                    }

                    float colorOffset = 0;

                    float fogLerpOffset = 0;
                    if (waterDepth < 500) {
                        fogLerpOffset = clamp(waterColor.r/max(waterDepth, 45)*10 - 0.1, 0, 1);
                    }

                    float theta = atan2(rayDir.z, rayDir.x);
                    colorOffset = (getGodrays(float3((rayOrigin.x + rayDir.x * 10), 0, (rayOrigin.z + rayDir.z * 10))) + 1)/55*(1-abs(rayDir.y));
                    
                    return lerp(float4(waterCol + colorOffset, 1), lerp(sceneColor, waterColor, lerpFactor), exp(-distanceThroughWater/20)+fogLerpOffset);
                }
                else {
                    // the fog pass
                    // sceneColor = lerp(sceneColor, 0.5, clamp(sceneDepth/100, 0, 1));
                    // waterColor = lerp(waterColor, 0.5, clamp(waterDepth/100, 0, 1));
                }

                if (waterDepth < sceneDepth) { // looking through water
                    float shiftAmt = 20;
                    float2 shiftVector = float2(shiftAmt/1920,shiftAmt/1080);

                    sceneColor = tex2D(_MainTex, i.uv + shiftVector);
                    sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv + shiftVector)) * length(i.viewVector);

                    float distanceThroughWater = sceneDepth - waterDepth;

                    shiftAmt = 50;
                    shiftVector = float2(shiftAmt/1920,shiftAmt/1080) * distanceThroughWater * 2;

                    sceneColor = tex2D(_MainTex, i.uv + shiftVector);
                    sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv + shiftVector)) * length(i.viewVector);

                    distanceThroughWater = sceneDepth - waterDepth;

                    if (waterDepth < sceneDepth) {
                        return lerp(waterColor, sceneColor, exp(-distanceThroughWater*2));
                    } else {
                        waterColor = tex2D(_WaterColor, i.uv);
                        sceneColor = tex2D(_MainTex, i.uv);

                        waterDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_WaterDepth, i.uv)) * length(i.viewVector);
                        sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * length(i.viewVector);

                        if (waterDepth < sceneDepth) {
                            return waterColor;
                        } else {
                            return sceneColor;
                        }
                    }
                    
                } else { // not looking through water at all

                    return sceneColor;
                }
            }
            ENDCG
        }
    }
}
