Shader "examples/week 12/ray marching"
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

            #define MAX_STEPS 100
            #define MAX_DIST 10
            #define MIN_DIST 0.001

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 hitPos : TEXCOORD1;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.hitPos = mul(unity_ObjectToWorld, v.vertex); //gives us the world position

                return o;
            }

            //get dist defines everything that is in the scene
            float get_dist(float3 pos)
            {
                float radius = 0.5;
                float3 spherePos = float3(0, 0, 0);

                float dSphere = distance(spherePos, pos) - radius;

                float planeYPos = -0.5;
                float dPlane = abs(pos.y - planeYPos);

                return min(dSphere, dPlane);        //return minimum distance between two objs
            }

            float ray_march(float3 rayOrigin, float3 rayDir)
            {
                float marchDist = 0;

                for (int i = 0; 1 < MAX_STEPS; i++)
                {
                    //gets position each step
                    float3 pos = rayOrigin + rayDir * marchDist;

                    //calcule distance to surface
                    float distToSurf = get_dist(pos);

                    //march forward
                    marchDist += distToSurf;

                    if (distToSurf < MIN_DIST || marchDist > MAX_DIST)
                    {
                        break; //break out of for loop
                    }
                }

                return marchDist;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float3 color = 0;
                float3 camPos = _WorldSpaceCameraPos;           //gives world camera pos
                float3 rayDir = normalize(i.hitPos - camPos);   //getting direction

                //distance
                float d = ray_march(camPos, rayDir);

                float depth = 1 - (d / MAX_DIST);

                color = depth.rrr;

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
