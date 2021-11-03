using NUnit.Framework;
using UnityEngine;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    [TestFixture]
    public class PaintTextureTest
    {
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void RemoveTerrainLayer_UndoShouldCorrectSplatTextures()
        {
            var terrainData = new TerrainData();
            terrainData.alphamapResolution = 16;
            terrainData.terrainLayers = new[]
            {
                new UnityEngine.TerrainLayer(), new UnityEngine.TerrainLayer(), new UnityEngine.TerrainLayer()
            };
            int layerCount = 3;
            float[,,] map = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                for (int y = 0; y < terrainData.alphamapHeight; y++)
                {
                    for (int x = 0; x < terrainData.alphamapWidth; x++)
                    {
                        map[x, y, 0] = 0;
                    }
                }
            }
            // set magic pixels on all the layers so we can track what should be restored
            map[0, 0, 0] = 1.0f;
            map[0, 1, 1] = 1.0f;
            map[0, 2, 2] = 1.0f;

            terrainData.SetAlphamaps(0, 0, map);
            var terrain = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
            // the package uses this function because it performs some prepass prior to deletion
            TerrainToolboxLayer.RemoveLayerFromTerrain(terrainData, 1);
            Undo.PerformUndo();
            terrainData.SyncTexture(TerrainData.AlphamapTextureName);
            var newMap = terrainData.GetAlphamaps(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution);
            for (int i = 0; i < layerCount; i++)
            {
                Assert.That(newMap[0, i, i], Is.EqualTo(1.0f));
            }
            GameObject.DestroyImmediate(terrain.gameObject);
        }
    }
}