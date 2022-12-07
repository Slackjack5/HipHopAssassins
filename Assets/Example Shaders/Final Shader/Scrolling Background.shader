Shader "examples/week 3/Scrolling Background"
{
    Properties{
        _Texture ("my texture",2D) = "white" {}
        _Amplitude ("Shader Amplitude",Range(0,1)) = 1
        _Frequency ("Shader Frequency",Range(0,5)) = 2.5
        _Scale ("Shader Scale",Range(0,1)) = 1
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
            float _Scale;
            float _Amplitude;
            float _Frequency;
        
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
                float2 uv = i.uv; //* 2 - 1;
                float2 uv2 = i.uv * 2 - 1;
                float time = _Time.y;
                float time2 = time.x/2;
                float3 color = 0;
                float3 color2 = 0;

                float distx = _Amplitude * sin(_Frequency * uv.y + _Scale * time);
                float disty = _Amplitude * cos(_Frequency * uv.x + _Scale * time);

                float warpStrength = 0.33;
                //We know here that the X and Y are going to be moving independently
                uv += distx;
                uv2 += disty;//(sin(uv.yx + float2(0, 3)*time) * warpStrength);;//cos(uv.yx + float2(time, 1.5)) * warpStrength; //Swizzling example
                //uv += sin(uv.yx + float2(0, 3)) * warpStrength; //Swizzling example

                color = tex2D(_Texture, uv).rgb;
                color2 = tex2D(_Texture, uv2).rgb;
                return float4(color*color2, 1.0);
            }
            ENDCG
        }
    }
}
