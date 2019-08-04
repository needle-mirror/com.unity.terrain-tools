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
			sampler2D _FilterTex;

            float4 _BrushParams;
            #define BRUSH_STRENGTH      (_BrushParams[0])
            #define BRUSH_TARGETHEIGHT  (_BrushParams[1])

			float4 _SmoothWeights; // centered, min, max, unused

			static int KernelWeightCount = 7;
			static float KernelWeights[] = { 0.95f, 0.85f, 0.7f, 0.4f, 0.2f, 0.15f, 0.05f };


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
			Name "Smooth Horizontal"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment SmoothHorizontal

			float4 SmoothHorizontal(v2f i) : SV_Target
			{
				float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
				float2 heightmapUV = PaintContextUVToHeightmapUV(i.pcUV);

				// out of bounds multiplier
				float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

				float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));
				float brushStrength = BRUSH_STRENGTH * oob * UnpackHeightmap(tex2D(_BrushTex, brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.pcUV));

				float divisor = 1.0f;
				float offset = 0.0f;
				float h = height;
			
				for (int i = 0; i < KernelWeightCount; i++) {
					offset += _MainTex_TexelSize.x * _SmoothWeights.w;
					divisor += 2.0f * KernelWeights[i];

					float2 rightUV = heightmapUV + float2(offset, 0.0f);
					float2 leftUV = heightmapUV - float2(offset, 0.0f);

					h += KernelWeights[i] * UnpackHeightmap(tex2D(_MainTex, rightUV));
					h += KernelWeights[i] * UnpackHeightmap(tex2D(_MainTex, leftUV));
				}

				h /= divisor;

				float3 new_height = float3(h, min(h, height), max(h, height));
				h = dot(new_height, _SmoothWeights.xyz);
				return PackHeightmap(lerp(height, h, brushStrength));
			}
			ENDCG
		}

		Pass
		{
			Name "Smooth Vertical"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment SmoothVertical

			float4 SmoothVertical(v2f i) : SV_Target
			{
				float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
				float2 heightmapUV = PaintContextUVToHeightmapUV(i.pcUV);

				// out of bounds multiplier
				float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

				float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));
				float brushStrength = BRUSH_STRENGTH * oob * UnpackHeightmap(tex2D(_BrushTex, brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.pcUV));

				float divisor = 1.0f;
				float offset = 0.0f;
				float h = height;

				for (int i = 0; i < KernelWeightCount; i++) {
					offset += _MainTex_TexelSize.x * _SmoothWeights.w;
					divisor += 2.0f * KernelWeights[i];

					float2 upUV = heightmapUV + float2(0.0f, offset);
					float2 downUV = heightmapUV - float2(0.0f, offset);

					h += KernelWeights[i] * UnpackHeightmap(tex2D(_MainTex, upUV));
					h += KernelWeights[i] * UnpackHeightmap(tex2D(_MainTex, downUV));
				}

				h /= divisor;

				float3 new_height = float3(h, min(h, height), max(h, height));
				h = dot(new_height, _SmoothWeights.xyz);
				return PackHeightmap(lerp(height, h, brushStrength));
			}
			ENDCG
		}

		/*
		Pass    // 3 smooth terrain
		{
			Name "Smooth Heights"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment SmoothHeight

			float4 _SmoothWeights;      // centered, min, max, unused

			float aggregateHeight(float2 uv, out float oobWeight)
			{
				oobWeight = all(saturate(uv) == uv) ? 1.0f : 0.0f;
				return oobWeight * UnpackHeightmap(tex2D(_MainTex, uv));
			}

			float4 SmoothHeight(v2f i) : SV_Target
			{
				float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
				float2 heightmapUV = PaintContextUVToHeightmapUV(i.pcUV);

				// out of bounds multiplier
				float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

				float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));
				float brushStrength = BRUSH_STRENGTH * oob * UnpackHeightmap(tex2D(_BrushTex, brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.pcUV));

				float h = 0.0F;
				float xoffset = _MainTex_TexelSize.x * _SmoothWeights.w;
				float yoffset = _MainTex_TexelSize.y * _SmoothWeights.w;

				// 3*3 filter
				h += height;

				float sum = 1.0f;
				float weight = 1.0f;
				h += aggregateHeight(heightmapUV + float2( xoffset, 0), weight);
				sum += weight;
				h += aggregateHeight(heightmapUV + float2(-xoffset, 0), weight);
				sum += weight;
				h += aggregateHeight(heightmapUV + float2( xoffset, yoffset), weight) * 0.75f;
				sum += 0.75f * weight;
				h += aggregateHeight(heightmapUV + float2(-xoffset, yoffset), weight) * 0.75f;
				sum += 0.75f * weight;
				h += aggregateHeight(heightmapUV + float2( xoffset, -yoffset), weight) * 0.75f;
				sum += 0.75f * weight;
				h += aggregateHeight(heightmapUV + float2(-xoffset, -yoffset), weight) * 0.75f;
				sum += 0.75f * weight;
				h += aggregateHeight(heightmapUV + float2( 0, yoffset), weight);
				sum += weight;
				h += aggregateHeight(heightmapUV + float2( 0, -yoffset), weight);
				sum += weight;
				
				h /= sum;
				

				float3 new_height = float3(h, min(h, height), max(h, height));
				h = dot(new_height, _SmoothWeights.xyz);
				return PackHeightmap(lerp(height, h, brushStrength));
			}
			ENDCG
		}
		*/
    }
    Fallback Off
}
