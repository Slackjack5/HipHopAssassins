﻿Shader "examples/week 3/warping"
{
    Properties{
        _Texture ("my texture",2D) = "white" {}
        }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Texture;
        
            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float circle (float2 uv, float size) {
                return smoothstep(0.0, 0.005, 1 - length(uv) / size);
            }

            float2x2 rotate2D (float angle) {
                return float2x2 (
                    cos(angle), -sin(angle),
                    sin(angle),  cos(angle)
                );
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv = i.uv * 2 - 1;
                float time = _Time.y;
                float3 color = 0;

                float warpStrength = 0.33;
                //We know here that the X and Y are going to be moving independently
                uv += cos(uv.yx + float2(time, 1.5)) * warpStrength; //Swizzling example
                uv += sin(uv.yx + float2(0, 3)) * warpStrength; //Swizzling example

                float circ = circle(uv,0.5);
                color += circ.rrr;

                color = tex2D(_Texture, uv).rgb;

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
