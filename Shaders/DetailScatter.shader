Shader "Hidden/TerrainEngine/DetailScatter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
                ZTest Always Cull Off ZWrite Off

        CGINCLUDE

            #include "UnityCG.cginc"
            #include "Packages/com.unity.terrain-tools/Shaders/TerrainTools.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height

            sampler2D _BrushTex;
            sampler2D _FilterTex;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 pcUV : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 pcUV : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pcUV = v.pcUV;
                return o;
            }

        ENDCG

        Pass
        {
            Name "Detail Scatter"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment DetailScatter

            float4 DetailScatter(v2f i) : SV_Target
            {
                float2 brushUV = PaintContextUVToBrushUV(i.pcUV);

                // out of bounds multiplier
                float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;
                float brushShape = oob * UnpackHeightmap(tex2D(_BrushTex, brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.pcUV));

                return PackHeightmap(brushShape);
            }
            ENDCG
        }
    }
}
