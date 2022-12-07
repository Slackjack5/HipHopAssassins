﻿Shader "examples/week 3/rotate"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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

            float rectangle (float2 uv, float2 scale) {
                float2 s = scale * 0.5;
                float2 shaper = float2(step(-s.x, uv.x), step(-s.y, uv.y));
                shaper *= float2(1-step(s.x, uv.x), 1-step(s.y, uv.y));
                return shaper.x * shaper.y;
            }

            #define PI 3.14159265

            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv = i.uv * 2 - 1;
                float time = _Time.x * 15;
                float3 color = 0;

                float angle = time % 2*PI; //We are doing 2 PI , because in Radianse 2PI is 360 Degrees
                angle = frac(time) * (2*PI); //Better way of doing it
                
                float2x2 rotate2D = float2x2( //This is our matrix
                    cos(angle), -sin(angle),
                    sin(angle), cos(angle)
                );

                uv = mul(rotate2D,uv); //This function multiplies matricies for us
                color+=float3(uv.x,0,uv.y);
                color += rectangle(uv, float2(0.25, 0.5));

                

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
