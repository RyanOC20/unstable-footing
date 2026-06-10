Shader "Custom/NightSkyboxStars"
{
    Properties
    {
        _SkyColorZenith  ("Sky Color (Zenith)",  Color) = (0.00, 0.01, 0.05, 1)
        _SkyColorHorizon ("Sky Color (Horizon)", Color) = (0.02, 0.04, 0.12, 1)
        _HorizonSharpness("Horizon Sharpness",   Float) = 3.0
        _StarDensity     ("Star Density",        Float) = 1.0
        _StarBrightness  ("Star Brightness",     Float) = 1.5
        _StarSharpness   ("Star Sharpness",      Float) = 5.0
        _MoonColor       ("Moon Color",          Color) = (0.88, 0.92, 1.00, 1)
        _MoonSize        ("Moon Size",           Float) = 0.018
        _MoonGlowSize    ("Moon Glow Size",      Float) = 0.12
        _MoonDirX        ("Moon Dir X",          Float) = 0.25
        _MoonDirY        ("Moon Dir Y",          Float) = 0.65
        _MoonDirZ        ("Moon Dir Z",          Float) = 0.45
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _SkyColorZenith;
            float4 _SkyColorHorizon;
            float  _HorizonSharpness;
            float  _StarDensity;
            float  _StarBrightness;
            float  _StarSharpness;
            float4 _MoonColor;
            float  _MoonSize;
            float  _MoonGlowSize;
            float  _MoonDirX;
            float  _MoonDirY;
            float  _MoonDirZ;

            struct appdata { float4 vertex : POSITION; };
            struct v2f
            {
                float4 pos      : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.worldDir = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
                return o;
            }

            // Reliable sin-based hash — good distribution, handles large integer inputs well
            float3 hash3(float3 p)
            {
                p = float3(
                    dot(p, float3(127.1, 311.7,  74.7)),
                    dot(p, float3(269.5, 183.3, 246.1)),
                    dot(p, float3(113.5, 271.9, 124.6))
                );
                return frac(sin(p) * 43758.5453);
            }

            // Returns the brightness of the single brightest star visible in direction `dir`.
            // Uses max() — NOT += — so 27 cells never accumulate into white.
            float Stars(float3 dir)
            {
                float3 p  = dir * _StarDensity;
                float3 pi = floor(p);

                float best = 0.0;
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    float3 cell    = pi + float3(dx, dy, dz);
                    float3 rand    = hash3(cell);
                    float3 starDir = normalize(cell + rand - 0.5);
                    float  d       = dot(dir, starDir);
                    float  star    = smoothstep(_StarSharpness, 1.0, d) * (0.3 + rand.z * 0.7);
                    best           = max(best, star);
                }
                return best * _StarBrightness;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.worldDir);

                // Sky gradient — zenith to horizon
                float  t   = saturate(pow(saturate(dir.y), 1.0 / _HorizonSharpness));
                float3 sky = lerp(_SkyColorHorizon.rgb, _SkyColorZenith.rgb, t);

                // Stars — fade to zero at and below the horizon
                float starFade = saturate(dir.y * 8.0);
                sky += Stars(dir) * starFade;

                // Moon disc + atmospheric glow
                float3 moonDir = normalize(float3(_MoonDirX, _MoonDirY, _MoonDirZ));
                float  moonDot = dot(dir, moonDir);
                float  disc    = smoothstep(1.0 - _MoonSize, 1.0, moonDot);
                float  glow    = smoothstep(1.0 - _MoonGlowSize, 1.0 - _MoonSize * 1.5, moonDot) * 0.18;
                sky  = lerp(sky, _MoonColor.rgb * 2.2, disc);
                sky += _MoonColor.rgb * glow;

                return fixed4(sky, 1.0);
            }
            ENDCG
        }
    }
}
