    Shader "Custom/water_vertex" {
        Properties{
            
            [Header(Tesselation)]
            [Space(10)]
            _MaxTessDistance("Max Tessellation Distance", Range(10,100)) = 50
            _Tess("Tessellation", Range(1,32)) = 20

            [Space(50)]
            
            [Header(Colors)]
            [Space(10)]
            _ColorBand ("Color Banding", Range(0,100)) = 0.5
            _SpaceBand ("Space Banding", Range(0,100)) = 0.5
            _LightCol ("Light Color", Color) = (0.5,0.5,0.5,1)
            _DarkCol ("Dark Color", Color) = (0.5,0.5,0.5,1)
            
            [Header(Foam)]
            [Space(10)]
            _FoamStrength  ("Foam Strength", Range(0,1)) = 0.5
            _FoamScale  ("Foam Scale", Range(0.1,10)) = 0.5
            _Foam1 ("Foam Texture 1", 2D) = "white" {}
            _Foam2 ("Foam Texture 2", 2D) = "white" {}
            
            [Space(50)]
            
            [Header(Normals)]
            [Space(10)]
            _NormalScale  ("Normal Scale", Range(0.1,10)) = 0.5
            _SpecularStrength  ("Specular Strength", Range(0.1,10)) = 0.5
            _SpecularPower  ("Specular Power", Range(1,200)) = 100
            _SpecularNormal ("Normal Map", 2D) = "white" {}
        }
    
        SubShader{
            Tags { 
                "RenderType" = "Transparent" 
                "Queue" = "Transparent" 
            }

            ZWrite Off

            LOD 200
    
            CGPROGRAM
    
            // custom lighting function that uses a texture ramp based
            // on angle between light direction and normal
            #pragma surface surf ToonRamp vertex:vert addshadow nolightmap tessellate:tessDistance fullforwardshadows alpha
            #pragma target 4.0
            #pragma require tessellation tessHW
            #include "Tessellation.cginc"

            sampler2D _MainTex, _Foam1, _Foam2, _SpecularNormal;

            float4 _LightCol;
            float _ColorBand;
            float _SpaceBand;
            float4 _DarkCol;
            float3 sunDir;
            int isUnderWater;
            float timeValue;

            // specular settings
            float _SpecularStrength;
            float _SpecularPower;
    
            inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
            {
                lightDir = normalize(lightDir);
                viewDir = normalize(viewDir);

                half4 c;
                
                if (!isUnderWater) {
                    c.rgb = lerp(_DarkCol, _LightCol, round(s.Albedo.r / _ColorBand) * _ColorBand) + pow(clamp(dot(normalize((sunDir + viewDir)), s.Normal), 0, 1), _SpecularPower)*_SpecularStrength + s.Alpha;
                } else {
                    c.rgb = lerp(_DarkCol, _LightCol, round(s.Albedo.r / _ColorBand) * _ColorBand) + pow(clamp(dot(normalize(lerp(-viewDir, s.Normal, 0.2)), sunDir), 0, 1), 10) + s.Alpha;
                }
                
                c.a = 0.9;
                return c;
            }
            uniform float3 _Position;
            uniform sampler2D _GlobalEffectRT;
            uniform float _OrthographicCamSize;
            float baseWaveFrequency;
    
            float _Tess;
            float _MaxTessDistance;
            float waveAngles[3];
            

            // foam settings
            float _FoamStrength;
            float _FoamScale;

            float _NormalScale;
    
            float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess)
            {
                float3 worldPosition = mul(unity_ObjectToWorld, vertex).xyz;
                float dist = distance(worldPosition, _WorldSpaceCameraPos);
                float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0);
                return f * tess;
            }
    
            float4 DistanceBasedTess(float4 v0, float4 v1, float4 v2, float minDist, float maxDist, float tess)
            {
                float3 f;
                f.x = CalcDistanceTessFactor(v0, minDist, maxDist, tess);
                f.y = CalcDistanceTessFactor(v1, minDist, maxDist, tess);
                f.z = CalcDistanceTessFactor(v2, minDist, maxDist, tess);
    
                return UnityCalcTriEdgeTessFactors(f);
            }
    
            float4 tessDistance(appdata_full v0, appdata_full v1, appdata_full v2)
            {
                float minDist = 10.0;
                float maxDist = _MaxTessDistance;
    
                return DistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
            }
    
            struct Input {
                float2 uv_MainTex : TEXCOORD0;
                float3 worldPos; // world position built-in value
                float3 viewDir;// view direction built-in value we're using for rimlight
            };

            float getHeight(float3 pos) {
                float a = 0.7;
                float f = 2;

                float sum = 0;
                int iterations = 3;

                for (int i = 0; i < iterations; i++) {
                    float angle = waveAngles[i];

                    float x = cos(angle) * (pos.x * baseWaveFrequency) - sin(angle) * (pos.z * baseWaveFrequency);
                    x += timeValue;
                    
                    sum += pow(2.71828, sin(x*f) - 1) * a;

                    f *= 1.5;
                    a = lerp(a, 0.2, 0.5);
                }

                return sum/iterations;
            }

            float3 getNormal(float3 pos) {
                float a = 0.7;
                float f = 2;

                float3 sum = 0;
                int iterations = 3;

                for (int i = 0; i < iterations; i++) {
                    float  angle = waveAngles[i];

                    float x = cos(angle) * (pos.x * baseWaveFrequency) - sin(angle) * (pos.z * baseWaveFrequency);
                    x += timeValue;

                    float slope = pow(2.71828, sin(x*f) - 1) * cos(x * f);
                    sum += normalize(float3(cos(-angle) * slope, 1, -sin(-angle) * slope)) * a;

                    f *= 1.5;
                    a = lerp(a, 0.2, 0.5);
                }

                return normalize(sum);
            }
    
            void vert(inout appdata_full v)
            {   
                
                float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                float waterHeight = getHeight(worldPosition);
                
                // move vertices up where snow is, and where there is no path   
                v.vertex.xyz += float3(0,1,0) * (waterHeight-3.5);
                v.normal = getNormal(worldPosition);
            }
    
            void surf(Input IN, inout SurfaceOutput o) {
                IN.worldPos = float3(round(IN.worldPos.x / _SpaceBand) * _SpaceBand, round(IN.worldPos.y / _SpaceBand) * _SpaceBand, round(IN.worldPos.z / _SpaceBand) * _SpaceBand);

                float heightTerm = clamp(IN.worldPos.y - 6, 0, 10)/5;
                float diffuseTerm = lerp(0, 1, clamp(dot(sunDir, o.Normal)-0.5, 0, 1))/2;
                float distanceTerm = clamp(2/length(IN.worldPos - _WorldSpaceCameraPos), 0, 0.3);

                float2 x = IN.worldPos.xz/50;

                //x = float2(round(x.x / _ColorBand) * _ColorBand, round(x.y / _ColorBand) * _ColorBand);
                float t = timeValue/30;

                float foamValue = tex2D(_Foam1, x/2*_FoamScale + t/2).r + tex2D(_Foam2, x/3*_FoamScale-t/3).r + tex2D(_Foam1, x*_FoamScale+t/3).r;
                if (isUnderWater) {
                    foamValue *= 0.4; // foam is less apparent when viewing from the bottom
                }

                foamValue *= _FoamStrength;

                o.Albedo = heightTerm + diffuseTerm + distanceTerm;
                o.Normal = normalize(lerp(o.Normal, UnpackNormal(tex2D(_SpecularNormal, x*_NormalScale + t)), 0.3));
                o.Alpha = foamValue * 0.25;
                o.Emission = 0;
            }
            ENDCG
    
        }
        
        Fallback "Diffuse"
    }