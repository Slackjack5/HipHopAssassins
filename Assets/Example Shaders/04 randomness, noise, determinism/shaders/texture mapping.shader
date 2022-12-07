Shader "examples/week 4/texture mapping"
{
    Properties
    {
        _tex ("my texture",2D) = "white" {}
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
            sampler2D _tex; float4 _tex_ST;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos: TEXCOORD1;
                float4 screenPos: TEXCOORD2;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }
            //Chowder Effect (From the show lol)
            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv = i.uv;
                float3 color = 0;

                //uv = i.worldPos.xy;
                uv - i.screenPos.xy / i.screenPos.w;
                float aspect = _ScreenParams.x  / _ScreenParams.y; //Width , Height
                uv.x *= aspect;
                color = tex2D(_tex,TRANSFORM_TEX(uv,_tex)).rgb;

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
