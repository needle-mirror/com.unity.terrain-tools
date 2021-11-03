Shader "Hidden/TerrainEngine/PaintMaterialBrushPreview"
{
    SubShader
    {
        ZTest Always Cull Back ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGINCLUDE
            // Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
            #pragma exclude_renderers gles

            #include "UnityCG.cginc"
            #include "TerrainPreview.cginc"
            #include "Packages/com.unity.terrain-tools/Shaders/TerrainTools.hlsl"

            sampler2D _BrushTex;
            sampler2D _FilterTex;

            float _BrushStrength;

        ENDCG

        Pass    // 0
        {
            Name "PaintMaterialTerrainPreview"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct v2f {
                float4 clipPosition : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 pcPixels : TEXCOORD1;
                float2 brushUV : TEXCOORD2;
            };

            v2f vert(uint vid : SV_VertexID)
            {
                // build a quad mesh, with one vertex per paint context pixel (pcPixel)
                float2 pcPixels = BuildProceduralQuadMeshVertex(vid);

                // compute heightmap UV and sample heightmap
                float2 heightmapUV = PaintContextPixelsToHeightmapUV(pcPixels);
                float heightmapSample = UnpackHeightmap(tex2Dlod(_Heightmap, float4(heightmapUV, 0, 0)));

                // compute brush UV
                float2 brushUV = PaintContextPixelsToBrushUV(pcPixels);

                // compute object position (in terrain space) and world position
                float3 positionObject = PaintContextPixelsToObjectPosition(pcPixels, heightmapSample);
                float3 positionWorld = TerrainObjectToWorldPosition(positionObject);

                v2f o;
                o.uv = heightmapUV;
                o.pcPixels = pcPixels;
                o.clipPosition = UnityWorldToClipPos(positionWorld);
                o.brushUV = brushUV;
                return o;
            }


            float4 frag(v2f i) : SV_Target
            {
                float brushSample = _BrushStrength * UnpackHeightmap(tex2D(_BrushTex, i.brushUV)) * UnpackHeightmap(tex2D(_FilterTex, i.uv));
                
                float iib = IsPcUvPartOfValidTerrainTileTexel(i.pcPixels / _PcPixelRect.zw);
                clip(iib - .01);

                // out of bounds multiplier
                float oob = all(saturate(i.brushUV) == i.brushUV) ? 1.0f : 0.0f;

                // brush outline stripe
                float stripeWidth = 1.5f;       // pixels
                float stripeLocation = 0.0025f;
                float brushStripe = Stripe(brushSample, stripeLocation, stripeWidth);

                float4 color = float4(1.0f, 0.6f, 0.05f, 1.0f);
                color.a = lerp(.75f * saturate(brushSample * 10), 1, saturate(brushStripe));
                return color * oob;
            }
            ENDCG
        }
    }
    Fallback Off
}
