Shader "examples/week 7/lambert"
{
    Properties 
    {
        _surfaceColor("surface color", color) = (0.7 , 0.1, 0.6)
    }
    SubShader
    {
        Tags { "LightMode" = "ForwardBase"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            float3 _surfaceColor;
            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float normal : TEXCOORD0;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.normal = v.normal;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float3 color = 0;
                float3 normal = normalize(i.normal);
                float3 lightColor = _LightColor0;
                float3 lightDirection = _WorldSpaceLightPos0;

                float falloff = max(dot(lightDirection , normal),0.0);

                color = _surfaceColor * falloff * lightColor;
                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
