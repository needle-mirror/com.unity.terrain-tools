using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Experimental.TerrainAPI {
    public class FilterContext {
        public Terrain terrain { get; set; }
        public Vector3 brushPos { get; set; }
        public float brushSize { get; set; }
        public float brushRotation { get; set; }
        public RenderTexture sourceRenderTexture { get; set; }
        public RenderTexture destinationRenderTexture { get; set; }
        public RenderTextureCollection renderTextureCollection { get; set; }
        public Dictionary<string, float> properties { get; set; }

        public FilterContext(Terrain t, Vector3 brushPos, float brushSize, float brushRotation)
        {
            terrain = t;
            sourceRenderTexture = null;
            destinationRenderTexture = null;
            renderTextureCollection = new RenderTextureCollection();
            properties = new Dictionary<string, float>();

            this.brushPos = brushPos;
            this.brushSize = brushSize;
            this.brushRotation = brushRotation;
        }

        public static class Keywords
        {
            public static readonly string Heightmap = "heightMap";
        }
    }
}