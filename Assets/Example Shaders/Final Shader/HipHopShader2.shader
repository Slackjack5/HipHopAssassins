Shader "examples/week 3/HipHopShader2"
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
                float4 screenPos: TEXCOORD1;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
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
            //Shader toy Shader I converted https://www.shadertoy.com/view/Dd2XRK
            float4 frag (Interpolators i) : SV_Target
            {
                // Normalized pixel coordinates (from 0 to 1)
                float2 uv =   i.screenPos.xy/ i.screenPos.w - 0.5;
                float3 aspect = _ScreenParams.x/_ScreenParams.y;
                uv.x *= aspect;
                float2 uv2 = i.uv * 2 - 1;
                float time = _Time.y;
                float distx = _Amplitude * sin(_Frequency * uv.y + _Scale * time);
                float disty = _Amplitude * cos(_Frequency * uv.x + _Scale * time);
                uv += distx;
                uv2 += disty*distx;
                
                // Time varying pixel color
                float3 col = 0.5 + 0.5*cos(_Time.y+uv.xyx+float3(0,2,4));
                float3 color2 = tex2D(_Texture, uv2).rgb;
                // Output to screen
                float3 color = float4(col,1.0);
                return float4(color*color2,1.0);
            }
            ENDCG
        }
    }
}
