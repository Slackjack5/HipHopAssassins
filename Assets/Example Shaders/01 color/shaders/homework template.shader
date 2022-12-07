Shader "examples/week 1/homework template"
{
    Properties
    {
        [NoScaleOffset] _tex1 ("texture one", 2D) = "white" {}
        [NoScaleOffset] _tex2 ("texture two", 2D) = "white" {}
        [NoScaleOffset] _mask ("mask", 2D) = "white" {}
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
            uniform sampler2D _tex1;
            uniform sampler2D _tex2;
            uniform sampler2D _mask;
            
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
            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv = i.uv;
                // sample the color data from each of the three textures and store them in float3 variables
                float3 t1 = tex2D(_tex1, uv).rgb;
                float3 t2 = tex2D(_tex2, uv).rgb;
                float3 mask = tex2D(_mask, uv).rgb;
                float3 color = 0;
                // add your code here
                color = ((t1) / max(t2,t1)*mask); // here's a very simple example. delete this and make your own logic.
                
                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}