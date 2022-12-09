Shader "examples/week 3/HipHopShader1"
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
            //Shader toy shader I converted https://www.shadertoy.com/view/3ttSzr
            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv =   i.screenPos.xy/ i.screenPos.w - 0.5;
                float3 aspect = _ScreenParams.x/_ScreenParams.y;
                uv.x *= aspect;
               
                for(float i = 2; i < 12; i++){
                uv.y += i * 0.5 / i * 
                  sin(uv.x * i * i + _Time.y * 0.1) * sin(uv.y * i * i + _Time.y  * 1);
                }
                
                float3 col;
                col.r  = uv.y - 0;
                col.g = uv.y + 0;
                col.b = uv.y + 0;
                
                float3 color2 = tex2D(_Texture, uv).rgb;

                if(col.r>=0.9)
                {
                    return float4(col*color2,1.0);
                }
                else
                {
                    return float4(0,0,0,1.0);
                }
                
            }
            ENDCG
        }
    }
}
// Nest of Polygons, mla, 2022
/*
const float PI = 3.141592654;

// Rotate vector p by angle t.
vec2 rotate(vec2 p, float t) {
  return cos(t)*p + sin(t)*vec2(-p.y,p.x);
}

            // Signed distance from p segment qr. Positive distance for
            // points on the left of the line looking from q to r.
            float ssegment(vec2 p, vec2 q, vec2 r) {
              p -= q; r -= q;
              float k = dot(p,r)/dot(r,r);
              k = clamp(k,0.0,1.0);
              vec2 closestpoint = k*r;
              float dist = distance(p,closestpoint);
              vec2 normal = vec2(-r.y,r.x);
              float s = dot(p,normal) >= 0.0 ? 1.0 : -1.0;
              return s*dist;
            }

            vec3 h2rgb(float h) {
              vec3 rgb = clamp( abs(mod(h*6.0+vec3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );
              return rgb*rgb*(3.0-2.0*rgb); // cubic smoothing	
            }

            float polydist(float N, vec2 p, float r) {
                float phi = PI/N; // angle of half sector
                float theta = atan(p.x,-p.y);// negative y-axis is theta = 0
                theta = mod(theta+phi,2.0*phi)-phi;
                //theta /= 2.0*phi; theta -= round(theta); theta *= 2.0*phi; // Equivalent
                theta = abs(theta); // Mirror symmetry
                // With dihedral symmetry, the closest point is always in the
                // fundamental region, so can limit attention just to there
                // for finding the SDF.
                vec2 p1 = length(p)*vec2(cos(theta),sin(theta));
                vec2 q0 = r*vec2(cos(phi),sin(phi)), q1 = r*vec2(cos(phi),0);
                return ssegment(p1,q0,q1);
            }

            float px;
            float N = 11.0;
            float M = 18.0;
            float poly(float k, vec2 p) {
                float k0 = k;
                k /= M;
                k += 0.25*iTime;
                k = exp(k);
                float r = exp(k+0.1*iTime);
                //p = rotate(p,0.5*iTime*sin(0.05*k0-0.1*sin(0.01*iTime)));
                //p = rotate(p,0.5*sin(3.0*iTime+0.1*k0));
                p = rotate(p,0.5*iTime*sin(k0));
                float d = polydist(N,p,k);
                return d;
            }

            vec3 getcol(float k) {
              return 0.25+0.75*h2rgb(0.01*iTime+0.5*fract(100.0*sin(k)));
            }

            void mainImage(out vec4 fragColor, vec2 fragCoord) {
                vec2 p = 2.0*(2.0*fragCoord-iResolution.xy)/iResolution.y;
                px = fwidth(length(p));
                float k = length(p);
                k = log(k);
                k -= 0.25*iTime;
                k *= M;
                k = round(k);
                float d0 = poly(k-0.5,p);
                float d1 = poly(k+0.5,p);
                
                float d = min(abs(d0),abs(d1));
                vec3 col;
                if (d0 < 0.0) col = getcol(k-0.5);
                else if (d1 < 0.0) col = getcol(k+0.5);
                else col = getcol(k+1.5);
                col = mix(vec3(0),col,vec3(smoothstep(0.0,px,abs(d)-0.005)));
                col = pow(col,vec3(0.4545));
                fragColor = vec4(col,1.0);
            }
*/