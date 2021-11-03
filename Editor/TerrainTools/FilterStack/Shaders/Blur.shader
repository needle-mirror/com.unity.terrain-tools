    Shader "Hidden/TerrainTools/Blur" {

    Properties { _MainTex ("Texture", any) = "" {} }

    SubShader {

        ZTest Always Cull Off ZWrite Off

        CGINCLUDE

            #include "UnityCG.cginc"
            #include "Packages/com.unity.terrain-tools/Shaders/TerrainTools.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height

            float4 _SmoothWeights; // centered, min, max, unused

            float2 _BlurDirection;
            int _KernelSize;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

        ENDCG

        Pass
        {
            Name "Blur"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 c = tex2D(_MainTex, uv);
                float4 h = tex2D(_MainTex, uv);
                
                float divisor = 1.0f;
                int kernelSize = _KernelSize; // todo: subpixel?

                // separate axis guassian blur
                for(int x = 0; x < kernelSize; ++x)
                {
                    float2 offset = _MainTex_TexelSize.xy * abs(sign(_BlurDirection)) * (x + 1);
                    float weight = (float)(kernelSize - x) / (float)(kernelSize + 1);
                    float iib0 = saturate(uv + offset) == uv + offset ? 1 : 0;
                    float iib1 = saturate(uv - offset) == uv - offset ? 1 : 0;
                    h += tex2D(_MainTex, uv + offset) * weight * iib0;
                    h += tex2D(_MainTex, uv - offset) * weight * iib1;
                    divisor += weight * (iib0 + iib1);
                }
            
                h /= divisor;
                
                h = float4(
                    dot(float3(h.r, min(h.r, c.r), max(h.r, c.r)), _SmoothWeights.xyz),
                    dot(float3(h.g, min(h.g, c.g), max(h.g, c.g)), _SmoothWeights.xyz),
                    dot(float3(h.b, min(h.b, c.b), max(h.b, c.b)), _SmoothWeights.xyz),
                    dot(float3(h.a, min(h.a, c.a), max(h.a, c.a)), _SmoothWeights.xyz)
                );
                
                return h;
            }
            ENDCG
        }
    }
    Fallback Off
}
