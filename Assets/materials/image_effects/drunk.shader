Shader "Custom/drunk"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amt ("Amount", Float) = 0
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
            float _Amt;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 toCenter = float2(0.5,0.5) - i.uv;

                float amp = 4;
                float freq = 7;

                float2 uv = i.uv + _Amt * float2(sin(_Time.x*freq)*amp, sin(_Time.x*freq)*amp) * 0.1 * max(0, 0.5 - length(toCenter));
                fixed4 col = tex2D(_MainTex, uv);
                
                return col;
            }
            ENDCG
        }
    }
}
