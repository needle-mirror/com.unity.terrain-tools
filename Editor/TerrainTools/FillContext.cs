using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class FillContext
    {
        private Terrain m_terrain;

        private RenderTexture m_sourceRenderTexture;
        private RenderTexture m_destinationRenderTexture;
        private RenderTexture m_oldRenderTexture;

        public RenderTexture sourceRenderTexture { get { return m_sourceRenderTexture; } }
        public RenderTexture destinationRenderTexture { get { return m_destinationRenderTexture; } }
        public RenderTexture oldRenderTexture { get { return m_oldRenderTexture; } }

        public RectInt pixelRect { get; private set; }
        public int targetTextureWidth { get; private set; }
        public int targetTextureHeight { get; private set; }
        public Vector2 pixelSize { get; private set; }

        public FillContext(Terrain terrain, int targetTextureWidth, int targetTextureHeight)
        {
            m_terrain = terrain;
            this.targetTextureWidth = targetTextureWidth;
            this.targetTextureHeight = targetTextureHeight;
            
            TerrainData terrainData = terrain.terrainData;
            this.pixelSize = new Vector2( terrainData.size.x / (targetTextureWidth - 1.0f),
                                          terrainData.size.z / (targetTextureHeight - 1.0f) );
        }

        public void CreateRenderTargets(RenderTextureFormat format)
        {
            m_sourceRenderTexture = RenderTexture.GetTemporary(pixelRect.width, pixelRect.height, 0, format, RenderTextureReadWrite.Linear);
            m_destinationRenderTexture = RenderTexture.GetTemporary(pixelRect.width, pixelRect.height, 0, format, RenderTextureReadWrite.Linear);
            m_sourceRenderTexture.wrapMode = TextureWrapMode.Clamp;
            m_sourceRenderTexture.filterMode = FilterMode.Point;
            m_oldRenderTexture = RenderTexture.active;
        }

        public void GatherHeightmap()
        {

        }

        public void ScatterHeightmap(string editorUndoName)
        {
            
        }

        public void GatherAlphamap(TerrainLayer inputLayer)
        {

        }
    }
}