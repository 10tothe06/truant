Shader "Custom/GrassBladeBuiltIn"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.18, 0.42, 0.12, 1)
        _TipColor  ("Tip Color",  Color) = (0.45, 0.75, 0.22, 1)
        _WindStrength ("Wind Strength", Float) = 0.45
        _WindDirection ("Wind Direction", Vector) = (1, 0, 0.35, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200
        Cull Off          // important for thin blades

        // ===================== Forward Base =====================
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_instancing
            #pragma target 4.5

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct GrassBlade
            {
                float3 position;
                float  rotation;
                float  height;
                float  width;
                float  bend;
                float  colorVariation;
            };

            StructuredBuffer<GrassBlade> _GrassBlades;

            float4 _BaseColor;
            float4 _TipColor;
            float  _WindStrength;
            float4 _WindDirection;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float3 normal : NORMAL;
                uint   id     : SV_InstanceID;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal   : TEXCOORD2;
                float  colorVar : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            float3 RotateY(float3 v, float angle)
            {
                float s, c;
                sincos(angle, s, c);
                return float3(v.x * c - v.z * s, v.y, v.x * s + v.z * c);
            }

            v2f vert(appdata v)
            {
                v2f o;

                GrassBlade blade = _GrassBlades[v.id];

                float3 pos = v.vertex.xyz;

                // Scale
                pos.x *= blade.width;
                pos.y *= blade.height;

                // Natural bend
                float t = v.uv.y;
                pos.z += blade.bend * t * t * blade.height;

                // Wind
                float phase = (blade.position.x + blade.position.z) * 0.12 + _Time.y * 1.6;
                float wind = sin(phase) * 0.55 + sin(phase * 1.73 + 2.1) * 0.3;
                wind *= _WindStrength * t * t;

                // Rotate
                pos = RotateY(pos, blade.rotation);

                float3 windDir = normalize(float3(_WindDirection.x, 0, _WindDirection.y));
                pos += windDir * wind * blade.height;

                float3 worldPos = blade.position + pos;

                o.pos      = UnityWorldToClipPos(worldPos);
                o.worldPos = worldPos;
                o.uv       = v.uv;
                o.normal   = UnityObjectToWorldNormal(RotateY(v.normal, blade.rotation));
                o.colorVar = blade.colorVariation;

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Color
                float3 col = lerp(_BaseColor.rgb, _TipColor.rgb, round(i.uv.y / 0.15) * 0.15);
                col *= lerp(0.72, 1.18, i.colorVar);

                // Lighting
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = saturate(dot(i.normal, lightDir));
                float atten = SHADOW_ATTENUATION(i);

                float3 lighting = _LightColor0.rgb * (NdotL * 0.75 + 0.25) * atten;
                lighting += ShadeSH9(float4(i.normal, 1)); // ambient

                return fixed4(col * lighting, 1);
            }
            ENDCG
        }

        // ===================== Shadow Caster =====================
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            #pragma target 4.5

            #include "UnityCG.cginc"

            struct GrassBlade
            {
                float3 position;
                float  rotation;
                float  height;
                float  width;
                float  bend;
                float  colorVariation;
            };

            StructuredBuffer<GrassBlade> _GrassBlades;

            float3 RotateY(float3 v, float angle)
            {
                float s, c;
                sincos(angle, s, c);
                return float3(v.x * c - v.z * s, v.y, v.x * s + v.z * c);
            }

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v, uint id : SV_InstanceID)
            {
                v2f o;
                GrassBlade blade = _GrassBlades[id];

                float3 pos = v.vertex.xyz;
                pos.x *= blade.width;
                pos.y *= blade.height;

                float t = v.texcoord.y;
                pos.z += blade.bend * t * t * blade.height;

                pos = RotateY(pos, blade.rotation);
                float3 worldPos = blade.position + pos;

                v.vertex = float4(worldPos, 1);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}