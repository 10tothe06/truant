Shader "Custom/pixelate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            int old_screen_width;
            int old_screen_height;

            int new_screen_width;
            int new_screen_height;

            fixed4 frag (v2f i) : SV_Target
            {
                // TODO: double check the math on this

                old_screen_width = 1920;
                old_screen_height = 1080;

                new_screen_width = 1920/4;
                new_screen_height = 1080/4;

                float old_per_new_width = old_screen_width / new_screen_width;
                float old_per_new_height = old_screen_height / new_screen_height;

                float2 uv = float2(round(i.uv.x * old_screen_width / old_per_new_width) * old_per_new_width / old_screen_width, round(i.uv.y / old_per_new_height * old_screen_height) * old_per_new_height / old_screen_height);
                fixed4 col = tex2D(_MainTex, uv);

                return col;
                
            }
            ENDCG
        }
    }
}
