using UnityEngine;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    internal class BrushRenderWithTerrainUIGroup : IBrushRenderWithTerrain
    {
        private readonly IBrushUIGroup m_UiGroup;
        private readonly string m_Name;
        private readonly Texture m_BrushTexture;

        private PaintContext m_HeightmapContext;
        private bool m_WriteToHeightmap;
        private PaintContext m_TextureContext;
        private bool m_WriteToTexture;
        private PaintContext m_NormalmapContext;
        private bool m_WriteToHoles;
        private PaintContext m_HolesContext;

        private float brushSize => m_UiGroup.brushSize;
        private float brushRotation => m_UiGroup.brushRotation;
        private float brushStrength => m_UiGroup.brushStrength;

        public IBrushUIGroup uiGroup => m_UiGroup;
        public Texture brushTexture => m_BrushTexture;

        public BrushRenderWithTerrainUIGroup(IBrushUIGroup uiGroup, string name, Texture brushTexture)
        {
            m_UiGroup = uiGroup;
            m_Name = name;
            m_BrushTexture = brushTexture;
        }

        public virtual bool CalculateBrushTransform(Terrain terrain, Vector2 uv, float size, float rotation, out BrushTransform brushTransform)
        {
            if (m_UiGroup.ScatterBrushStamp(ref terrain, ref uv))
            {
                brushTransform = TerrainPaintUtility.CalculateBrushTransform(terrain, uv, size, rotation);
                return true;
            }
            else
            {
                brushTransform = new BrushTransform();
                return false;
            }
        }

        public bool CalculateBrushTransform(Terrain terrain, Vector2 uv, float size, out BrushTransform brushTransform)
        {
            return CalculateBrushTransform(terrain, uv, size, brushRotation, out brushTransform);
        }

        public bool CalculateBrushTransform(Terrain terrain, Vector2 uv, out BrushTransform brushTransform)
        {
            return CalculateBrushTransform(terrain, uv, brushSize, brushRotation, out brushTransform);
        }

        public void SetupTerrainToolMaterialProperties(PaintContext paintContext, BrushTransform brushTransform, Material material)
        {
            Utility.SetupMaterialForPainting(paintContext, brushTransform, material);
        }

        public void RenderBrush(PaintContext paintContext, Material material, int pass)
        {
            Texture sourceTexture = paintContext.sourceRenderTexture;
            RenderTexture destinationTexture = paintContext.destinationRenderTexture;

            Graphics.Blit(sourceTexture, destinationTexture, material, pass);
        }

        public PaintContext AcquireHeightmap(bool writable, Terrain terrain, Rect boundsInTerrainSpace, int extraBorderPixels = 0)
        {
            if (m_HeightmapContext != null) return m_HeightmapContext;
            
            m_WriteToHeightmap = writable;
            m_HeightmapContext = TerrainPaintUtility.BeginPaintHeightmap(terrain, boundsInTerrainSpace, extraBorderPixels);

            if (m_HeightmapContext == null)
                return null;

            m_HeightmapContext.sourceRenderTexture.name = $"{m_Name}::HeightmapContext::SourceRT";
            m_HeightmapContext.destinationRenderTexture.name = $"{m_Name}::HeightmapContext::DestRT";
            return m_HeightmapContext;
        }

        public PaintContext AcquireTexture(bool writable, Terrain terrain, Rect boundsInTerrainSpace, TerrainLayer layer, int extraBorderPixels = 0)
        {
            if (m_TextureContext != null) return m_TextureContext;
            
            m_WriteToTexture = writable;
            m_TextureContext = TerrainPaintUtility.BeginPaintTexture(terrain, boundsInTerrainSpace, layer, extraBorderPixels);

            if (m_TextureContext == null)
                return null;

            m_TextureContext.sourceRenderTexture.name = $"{m_Name}::TextureContext::SourceRT";
            m_TextureContext.destinationRenderTexture.name = $"{m_Name}::TextureContext::DestRT";
            return m_TextureContext;
        }

        public PaintContext AcquireNormalmap(bool writable, Terrain terrain, Rect boundsInTerrainSpace, int extraBorderPixels = 0)
        {
            if (m_NormalmapContext != null) return m_NormalmapContext;
            
            m_NormalmapContext = TerrainPaintUtility.CollectNormals(terrain, boundsInTerrainSpace, extraBorderPixels);

            if (m_NormalmapContext == null)
                return null;

            m_NormalmapContext.sourceRenderTexture.name = $"{m_Name}::NormalContext::SourceRT";
            m_NormalmapContext.destinationRenderTexture.name = $"{m_Name}::NormalContext::DestRT";
            return m_NormalmapContext;
        }

        public PaintContext AcquireHolesTexture(bool writable, Terrain terrain, Rect boundsInTerrainSpace, int extraBorderPixels = 0)
        {
            if (m_HolesContext != null) return m_HolesContext;
            
            m_WriteToHoles = writable;
#if UNITY_2019_3_OR_NEWER
            m_HolesContext = TerrainPaintUtility.BeginPaintHoles(terrain, boundsInTerrainSpace, extraBorderPixels);

            if (m_HolesContext == null)
                return null;

            m_HolesContext.sourceRenderTexture.name = $"{m_Name}::HolesContext::SourceRT";
            m_HolesContext.destinationRenderTexture.name = $"{m_Name}::HolesContext::DestRT";
#endif

            return m_HolesContext;
        }

        public void Release(PaintContext paintContext)
        {
            if (ReferenceEquals(paintContext, m_HeightmapContext))
            {
                if (m_WriteToHeightmap)
                {
                    TerrainPaintUtility.EndPaintHeightmap(m_HeightmapContext, $"{m_Name} - Heightmap");
                }
                else
                {
                    TerrainPaintUtility.ReleaseContextResources(m_HeightmapContext);
                }

                m_HeightmapContext = null;
            }
            else if (ReferenceEquals(paintContext, m_NormalmapContext))
            {
                TerrainPaintUtility.ReleaseContextResources(m_NormalmapContext);
                m_NormalmapContext = null;
            }
            else if (ReferenceEquals(paintContext, m_TextureContext))
            {
                if (m_WriteToTexture)
                {
                    TerrainPaintUtility.EndPaintTexture(m_TextureContext, $"{m_Name} - Texture");
                }
                else
                {
                    TerrainPaintUtility.ReleaseContextResources(m_TextureContext);
                }

                m_TextureContext = null;
            }
#if UNITY_2019_3_OR_NEWER
            else if (ReferenceEquals(paintContext, m_HolesContext))
            {
                if (m_WriteToHoles)
                {
                    TerrainPaintUtility.EndPaintHoles(m_HolesContext, "Terrain Paint - Paint Holes");
                }
                else
                {
                    TerrainPaintUtility.ReleaseContextResources(m_HolesContext);
                }

                m_HolesContext = null;
            }
#endif
        }

        public void Dispose()
        {
            if (m_HeightmapContext != null)
            {
                Release(m_HeightmapContext);
            }

            if (m_NormalmapContext != null)
            {
                Release(m_NormalmapContext);
            }

            if (m_TextureContext != null)
            {
                Release(m_TextureContext);
            }
        }
    }

    internal class BrushRenderPreviewWithTerrainUiGroup : BrushRenderWithTerrainUIGroup, IBrushRenderPreviewWithTerrain
    {
        public BrushRenderPreviewWithTerrainUiGroup(IBrushUIGroup uiGroup, string name, Texture brushTexture) : base(uiGroup, name, brushTexture)
        {
        }

        public override bool CalculateBrushTransform(Terrain terrain, Vector2 uv, float size, float rotation, out BrushTransform brushTransform)
        {
            // TODO: Remove this method and replace the preview with a radius effect and scatter at the correct position...
            brushTransform = TerrainPaintUtility.CalculateBrushTransform(terrain, uv, size, rotation);
            return true;
        }

        public void RenderBrushPreview(PaintContext paintContext, TerrainBrushPreviewMode previewTexture, BrushTransform brushTransform, Material material, int pass)
        {
            TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, previewTexture, brushTexture, brushTransform, material, pass);
        }
    }
}
