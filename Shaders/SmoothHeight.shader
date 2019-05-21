    Shader "Hidden/TerrainTools/SmoothHeight" {

    Properties { _MainTex ("Texture", any) = "" {} }

    SubShader {

        ZTest Always Cull Off ZWrite Off

        CGINCLUDE

            #include "UnityCG.cginc"
            #include "TerrainTool.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height

            sampler2D _BrushTex;

            float4 _BrushParams;
            #define BRUSH_STRENGTH      (_BrushParams[0])
            #define BRUSH_TARGETHEIGHT  (_BrushParams[1])

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


		Pass    // 3 smooth terrain
		{
			Name "Smooth Heights"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment SmoothHeight

			float4 _SmoothWeights;      // centered, min, max, unused

			float4 SmoothHeight(v2f i) : SV_Target
			{
				float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
				float2 heightmapUV = PaintContextUVToHeightmapUV(i.pcUV);

				// out of bounds multiplier
				float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

				float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));
				float brushStrength = BRUSH_STRENGTH * oob * UnpackHeightmap(tex2D(_BrushTex, brushUV));

				float h = 0.0F;
				float xoffset = _MainTex_TexelSize.x * _SmoothWeights.w;
				float yoffset = _MainTex_TexelSize.y * _SmoothWeights.w;

				// 3*3 filter
				h += height;
				h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(xoffset,  0)));
				h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(-xoffset,  0)));
				h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(xoffset,  yoffset))) * 0.75F;
				h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(-xoffset,  yoffset))) * 0.75F;
				h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(xoffset, -yoffset))) * 0.75F;
				h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(-xoffset, -yoffset))) * 0.75F;
				h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(0,        yoffset)));
				h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(0,       -yoffset)));
				h /= 8.0F;

				float3 new_height = float3(h, min(h, height), max(h, height));
				h = dot(new_height, _SmoothWeights.xyz);
				return PackHeightmap(lerp(height, h, brushStrength));
			}
			ENDCG
		}
    }
    Fallback Off
}
