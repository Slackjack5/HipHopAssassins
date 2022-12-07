Shader "examples/week 3/homework template"
{
    Properties 
    {
        _hour ("hour", Float) = 0
        _minute ("minute", Float) = 0
        _second ("second", Float) = 0
        _Texture ("my texture",2D) = "white" {}
        _Texture2 ("my texture",2D) = "white" {}
        _Texture3 ("my texture",2D) = "white" {}

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
            sampler2D _Texture2;
            sampler2D _Texture3;



            #define TAU 6.28318530718

            float _hour;
            float _minute;
            float _second;

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

            
            float rectangle (float2 uv, float2 scale) {
                float2 s = scale * 0.5;
                float2 shaper = float2(step(-s.x, uv.x), step(-s.y, uv.y));
                shaper *= float2(1-step(s.x, uv.x), 1-step(s.y, uv.y));
                return shaper.x * shaper.y;
            }

            float4 frag (Interpolators i) : SV_Target
            {


                
                float warpStrength = 3;
                float2 uv = i.uv * 2 - 1;
                float time = _Time.z;
                
                float angle = frac(time) * (TAU); //Better way of doing it

                float2x2 rotate2D = float2x2( //This is our matrix
                    cos(angle), -sin(angle),
                    sin(angle), cos(angle)
                );



                uv.x += cos(uv.yx + float2(time, 1.5)) * 0.1; //Swizzling example
                uv.y += sin(uv.yx + float2(0, 3)) * 0.1; //Swizzling example

                

                float scaleMagnitude = (sin(time*.25)+.25);
                float2 scale = float2(scaleMagnitude, scaleMagnitude);

                uv *= scale;

                
                float3 t1 = tex2D(_Texture, uv).rgb;
                float3 t2 = tex2D(_Texture2, uv).rgb;
                float3 mask = tex2D(_Texture3, uv).rgb;
                // adding 0.5 changes the range to 0 - 1 so we can now set our angle based on a percent
                float polar1 = (atan2(uv.y, uv.x) / TAU) + 0.5;
                
                // creating a copy of our polar coordinate system to modify to render the hour hand
                
                float mA = polar1;
                mA += (frac(mA + (_minute / 60) + 0.25));
                
                float hA = polar1;
                // here is where we add our percent rotation to our coordinates based on the _hour value we're getting sent to the shader from the c# script. adding 0.25 at the end just makes sure that the starting point for the rotation is at the "12 o-clock" position
                hA = tex2D(_Texture, polar1)+frac(hA + (_hour / mA )); //(_hour / 12) + 0.25);

                float sA = polar1;
                sA = frac(sA + (_second / mA) + 0.25);
                //We know here that the X and Y are going to be moving independently
                hA += cos(uv.yx + float2(time, 1.5)) * warpStrength; //Swizzling example
                sA += sin(uv.yx + float2(0, 3)) * warpStrength; //Swizzling example

                // blending the values by adding them together with differing weights
                float3 color = 0;
                color = lerp(t1,t2,mask)*rectangle(hA + sA + mA, float2(1, 2));
                return float4(color.rgb,1);//_hour/24, _minute/60, _second/60, 1.0);
            }
            ENDCG
        }
    }
}
