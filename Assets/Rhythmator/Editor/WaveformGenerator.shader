Shader "Unlit/WaveformGenerator"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SampleTex("Sample Texture", 2D) = "white" {}
        _NumChannels("Channels", Int) = 1 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _NumChannels;
            sampler2D _SampleTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                float4 samples = tex2D(_SampleTex, float2(i.uv.x, 0));
                if (_NumChannels == 1) {
                    float s = pow(samples.r, 1 / 1.5) + 0.01;

                    float y = i.uv.y - 0.5;
                    y = abs(y);
                    y *= 2;

                    float v = smoothstep(-.01, 0.01, s - y);
                    return lerp(float4(0, 0, 0, 1), float4(1, 1, 0, 1), v);
                }
                else if (_NumChannels == 2) {
                    float s1 = pow(samples.r,1/1.5) + 0.01;
                    float s2 = pow(samples.g,1/1.5) + 0.01;

                    float y1 = i.uv.y;
                    float y2 = i.uv.y;

                    y1 -= 0.25;
                    y1 = abs(y1);
                    y1 *= 4;

                    y2 -= 0.75;
                    y2 = abs(y2);
                    y2 *= 4;

                    float v = smoothstep(-.01, 0.01, s1 - y1);
                    v += smoothstep(-.01, .01, s2 - y2);
                    return lerp(float4(0, 0, 0, 1), float4(1, 1, 0, 1), v);
                }
                
                return float4(0, 0, 0, 1);
                
            }
            ENDCG
        }
    }
}
