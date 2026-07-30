Shader "Custom/fog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        // some properties are static, user will not be able to change them via the settings menu
        _FogMultiplier ("Fog Strength Modifier", Float) = 100

        low_res ("Low Resolution?", Int) = 1
        gamma ("GammA", Float) = 0
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

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float _FogMultiplier;

            int low_res;
            float gamma;

            fixed4 frag (v2f i) : SV_Target
            {    
                // the original screen color
                fixed4 col = 0;
                if (low_res == 1) {
                    col = tex2D(_MainTex, round(i.uv * 500.0) / 500.0);
                } else {
                    col = tex2D(_MainTex, i.uv);
                }
                
                float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
                
                if (depth < 0.9 || i.viewVector.y < 0.2) {
                    col = lerp(col, float4(0,0,0,1), clamp(depth * _FogMultiplier, 0, 1));
                    col *= 1 + gamma;
                }

                return col;
            }
            ENDCG
        }
    }
}
