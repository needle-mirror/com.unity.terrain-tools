    Shader "Hidden/TerrainEngine/PaintHeightTool" {

    Properties { _MainTex ("Texture", any) = "" {} }

    SubShader {

        ZTest Always Cull Off ZWrite Off

        CGINCLUDE

            #include "UnityCG.cginc"
            #include "Packages/com.unity.terrain-tools/Shaders/TerrainTools.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height

            sampler2D _BrushTex;
            sampler2D _FilterTex;

            float4 _BrushParams;
            #define BRUSH_STRENGTH      (_BrushParams[0])
            #define BRUSH_TARGETHEIGHT  (_BrushParams[1])
            #define kMaxHeight          (32766.0f/65535.0f)

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

            float ApplyBrush(float height, float brushStrength)
            {
                float targetHeight = BRUSH_TARGETHEIGHT;
                if (targetHeight > height)
                {
                    height += brushStrength;
                    height = height < targetHeight ? height : targetHeight;
                }
                else
                {
                    height -= brushStrength;
                    height = height > targetHeight ? height : targetHeight;
                }
                return height;
            }

        ENDCG

        Pass    // 0 raise/lower heights
        {
            Name "Raise/Lower Heights"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment RaiseHeight

            float4 RaiseHeight(v2f i) : SV_Target
            {
                float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
                float2 heightmapUV = i.pcUV;

                // out of bounds multiplier
                float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

                float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));
                float brushShape = oob * UnpackHeightmap(tex2D(_BrushTex, brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.pcUV));

                return PackHeightmap(clamp(height + BRUSH_STRENGTH * brushShape, 0, kMaxHeight));
            }
            ENDCG
        }

		Pass    // 1 stamp heights
        {
            Name "Stamp Heights"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment StampHeight

            #define STAMP_TOOL_MODE     (_BrushParams[0]) // Min=0 | Set=1 | Max=2  
            #define HEIGHT_UNDER_CURSOR (_BrushParams[1])   
            #define BRUSH_STAMPHEIGHT   (_BrushParams[2])
            #define BLEND_AMOUNT        (_BrushParams[3])

            float SmoothMax(float a, float b, float p)
            {
                // calculates a smooth maximum of a and b, using an intersection power p
                // higher powers produce sharper intersections, approaching max()
                return log2(exp2(a * p) + exp2(b * p) - 1.0f) / p;
            }

            float4 StampHeight(v2f i) : SV_Target
            {
                float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
                float2 heightmapUV = i.pcUV;

                // out of bounds multiplier
                float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

                float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));
                float brushShape = oob * UnpackHeightmap(tex2D(_BrushTex, brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.pcUV));
                float brushHeight = brushShape * BRUSH_STAMPHEIGHT;

                // smoothmax behavior
                float targetHeight;
                float brushIntersection = saturate(1.0f - brushShape);
                float brushSmooth = exp2(brushIntersection * 8.0f);
                targetHeight = SmoothMax(height, brushHeight, brushSmooth);
					
                // "preserve details = 0" stamp is an offset from the height under the cursor
                float flatHeight = lerp(height, HEIGHT_UNDER_CURSOR + BRUSH_STAMPHEIGHT, brushShape);
                
                // composite results
                float outheight = lerp(flatHeight, targetHeight, BLEND_AMOUNT);	
                if (STAMP_TOOL_MODE != 2)
                {
                    outheight = lerp(min(height, outheight), max(height, outheight), STAMP_TOOL_MODE);
                }
                return PackHeightmap(clamp(outheight, 0.0f, kMaxHeight));
            }
            ENDCG
        }

        Pass    // 2 set height (flatten)
        {
            Name "Set Heights"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment SetHeight

            /*============================================================================

                NOTE(wyatt): use SetExactHeight.shader instead

            ============================================================================*/

            float4 SetHeight(v2f i) : SV_Target
            {
                float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
                float2 heightmapUV = i.pcUV;

                // out of bounds multiplier
                float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

                float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));
                float brushStrength = BRUSH_STRENGTH * oob * UnpackHeightmap(tex2D(_BrushTex, brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.pcUV));

                // smooth set
                float targetHeight = BRUSH_TARGETHEIGHT;

                // have to do this check to ensure strength 0 == no change (code below makes a super tiny change even with strength 0)
                if (brushStrength > 0.0f)
                {
                    float deltaHeight = height - targetHeight;

                    // see https://www.desmos.com/calculator/880ka3lfkl
                    float p = saturate(brushStrength);
                    float w = (1.0f - p) / (p + 0.000001f);
//                  float w = (1.0f - p*p) / (p + 0.000001f);       // alternative TODO test and compare
                    float fx = clamp(w * deltaHeight, -1.0f, 1.0f);
                    float g = fx * (0.5f * fx * sign(fx) - 1.0f);

                    deltaHeight = deltaHeight + g / w;

                    height = targetHeight + deltaHeight;
                }

                return PackHeightmap(height);
            }
            ENDCG
        }

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
                float2 heightmapUV = i.pcUV;

                // out of bounds multiplier
                float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

                float height = UnpackHeightmap(tex2D(_MainTex, heightmapUV));
                float brushStrength = BRUSH_STRENGTH * oob * UnpackHeightmap(tex2D(_BrushTex, brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.pcUV));

                float h = 0.0F;
                float xoffset = _MainTex_TexelSize.x;
                float yoffset = _MainTex_TexelSize.y;

                // 3*3 filter
                h += height;
                h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2( xoffset,  0      )));
                h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(-xoffset,  0      )));
                h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2( xoffset,  yoffset))) * 0.75F;
                h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(-xoffset,  yoffset))) * 0.75F;
                h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2( xoffset, -yoffset))) * 0.75F;
                h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2(-xoffset, -yoffset))) * 0.75F;
                h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2( 0,        yoffset)));
                h += UnpackHeightmap(tex2D(_MainTex, heightmapUV + float2( 0,       -yoffset)));
                h /= 8.0F;

                float3 new_height = float3(h, min(h, height), max(h, height));
                h = dot(new_height, _SmoothWeights.xyz);
                return PackHeightmap(lerp(height, h, brushStrength));
            }
            ENDCG
        }

        Pass    // 4 paint splat alphamap
        {
            Name "Paint Texture"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment PaintSplatAlphamap

            float4 PaintSplatAlphamap(v2f i) : SV_Target
            {
                float2 brushUV = PaintContextUVToBrushUV(i.pcUV);

                // out of bounds multiplier
                float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

                float brushStrength = BRUSH_STRENGTH * oob * UnpackHeightmap(tex2D(_BrushTex, brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.pcUV));
                float alphaMap = tex2D(_MainTex, i.pcUV).r;
                return ApplyBrush(alphaMap, brushStrength);
            }

            ENDCG
        }

        Pass    // 5 paint holes
        {
            Name "Paint Holes"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment PaintHoles
            float4 PaintHoles(v2f i) : SV_Target
            {
                float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
                float holes = tex2D(_MainTex, i.pcUV).r;
                float brush = UnpackHeightmap(tex2D(_BrushTex, brushUV));
                float filter = UnpackHeightmap(tex2D(_FilterTex, i.pcUV));
                
                // out of bounds multiplier
                float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;
                float brushStrength = BRUSH_STRENGTH * oob;

                float val = brush * filter;
                // filter could be negative. need to account for this
                val = abs(val) > (1 - abs(brushStrength)) && abs(val) > .0001f ? sign(brushStrength) * sign(val) : 0.0f;

                holes += val;
                return holes;
            }
            ENDCG
        }
    }
    Fallback Off
}
