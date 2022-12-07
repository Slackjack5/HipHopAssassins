Shader "examples/week 3/translate"
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

            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv = i.uv * 2 - 1; //Centering our UV
                float time = _Time.x*15; //Need to Animate the object over time
                float3 color = 0; //Setting Color to Black

                //How do we move this rectangle?
                float2 translate = 0;
                translate.x += sin(time);
                translate.y += cos(time); //The relationship of Sign and Cosign will result in circular motion.
                translate *= 0.5; //Scaling the circular path, so now its narrower

                uv += translate;

                color += float3(uv.x, 0, uv.y); //I'm adding to the color the coordinates of our UV
                color == float3(1,0,1) * rectangle(uv, float2(0.25,0.5)); //Create a rectangle that 
                //We are defining our color first in the float3 , then multiplying our shape into the color.
                //Since the rectangle is jst a mask.

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
