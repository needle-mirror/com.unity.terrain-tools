using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor.TerrainTools
{
    internal interface ITerrainEroder
    {
        void OnEnable();

        void ErodeHeightmap(RenderTexture dest, Vector3 terrainDimensions, Rect domainRect, Vector2 texelSize, bool invertEffect = false);

        Dictionary<string, RenderTexture> inputTextures { get; set; }
    }
}
