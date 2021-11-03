using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.TestTools;

namespace UnityEditor.TerrainTools
{
    [TestFixture]
    internal class BrushPlaybackTests
    {
        public class DefaultBrushUIGroupMock : DefaultBrushUIGroup
        {
            public DefaultBrushUIGroupMock(string name,
                Func<TerrainToolsAnalytics.IBrushParameter[]> analyticsCall = null, Feature feature = Feature.All) :
                base(name, analyticsCall, feature)
            {
                
            }
            public override bool allowPaint => true;
            public void SetTerrain(Terrain terrain)
            {
                terrainUnderCursor = terrain;
            }
            public void SetRaycastHit(RaycastHit raycastHit)
            {
                raycastHitUnderCursor = raycastHit;
            }
        }
        public class OnPaintContextMock : IOnPaint
        {
            public void RepaintAllInspectors(){}
            public void Repaint(RepaintFlags flags = RepaintFlags.UI){}
            
            public Texture brushTexture { get; }
            public Vector2 uv { get; }
            public float brushStrength { get; }
            public float brushSize { get; }
            public bool hitValidTerrain { get; }
            public RaycastHit raycastHit { get; }

            public OnPaintContextMock(Texture brushTexture, Vector2 uv, float brushStrength, float brushSize,
                bool hitValidTerrain, RaycastHit raycastHit)
            {
                this.brushTexture = brushTexture;
                this.uv = uv;
                this.brushStrength = brushStrength;
                this.brushSize = brushSize;
                this.hitValidTerrain = hitValidTerrain;
                this.raycastHit = raycastHit;
            }
        }
        private Terrain terrainObj;
        private Bounds terrainBounds;
        private int m_PrevRTHandlesCount;
        private ulong m_PrevTextureMemory;
        private DefaultBrushUIGroupMock defaultBrushUiGroupMock;
        [SetUp]
        public void Setup()
        {
            m_PrevTextureMemory = Texture.totalTextureMemory;
            m_PrevRTHandlesCount = RTUtils.GetHandleCount();
            defaultBrushUiGroupMock = new DefaultBrushUIGroupMock("PaintHeight");

        }
        
        [TearDown]
        public void Cleanup()
        {
            // delete test resources
            defaultBrushUiGroupMock?.brushMaskFilterStack?.Clear(true);
            PaintContext.ApplyDelayedActions();
            if (terrainObj != null)
            {
                UnityEngine.Object.DestroyImmediate(terrainObj.terrainData);
                UnityEngine.Object.DestroyImmediate(terrainObj.gameObject);
            }

            // check Texture memory and RTHandle count
            // var currentTextureMemory = Texture.totalTextureMemory;
            // Assert.True(m_PrevTextureMemory == currentTextureMemory, $"Texture memory leak. Was {m_PrevTextureMemory} but is now {currentTextureMemory}. Diff = {currentTextureMemory - m_PrevTextureMemory}");
            var currentRTHandlesCount = RTUtils.GetHandleCount();
            Assert.True(m_PrevRTHandlesCount == RTUtils.GetHandleCount(), $"RTHandle leak. Was {m_PrevRTHandlesCount} but is now {currentRTHandlesCount}. Diff = {currentRTHandlesCount - m_PrevRTHandlesCount}");
        }

        private Queue<BaseBrushUIGroup.OnPaintOccurrence> LoadDataFile(string recordingFileName, bool expectNull = false) {
            // Discover path to data file
            string[] assets = AssetDatabase.FindAssets(recordingFileName);
            if (assets.Length == 0) {
                Debug.LogError("No asset with name " + recordingFileName + " found");
            }
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

            // Load data file as a List<paintHistory>
            FileStream file = File.OpenRead(assetPath);
            BinaryFormatter bf = new BinaryFormatter();
            Queue<BaseBrushUIGroup.OnPaintOccurrence> paintHistory = new Queue<BaseBrushUIGroup.OnPaintOccurrence>(bf.Deserialize(file) as List<BaseBrushUIGroup.OnPaintOccurrence>);

            file.Close();

            if (paintHistory.Count == 0 && !expectNull)
            {
                throw new InconclusiveException("The loaded file contains no recordings");
            }

            return paintHistory;
        }

        private void SetupTerrain()
        {
            TerrainData td = new TerrainData();
            td.size = new Vector3(1000, 600, 1000);
            td.heightmapResolution = 513;
            td.baseMapResolution = 1024;
            td.SetDetailResolution(1024, 32);
            // Generate terrain
            GameObject terrainGo = Terrain.CreateTerrainGameObject(td);
            terrainObj = terrainGo.GetComponent<Terrain>();
            terrainBounds = td.bounds;
        }

        private Texture2D GetBrushTexture()
        {
            var textureDimensions = 32;
            var brushTexture = new Texture2D(textureDimensions, textureDimensions);
            
            var colors = new Color32[textureDimensions*textureDimensions];
            for (int i = 0; i < textureDimensions*textureDimensions; i++)
            {
                colors[i] = Color.white;
            }
            brushTexture.SetPixels32(colors);
            brushTexture.Apply();
            return brushTexture;
        }

        private void RunPainting<T>(string recordingFilePath, TerrainPaintTool<T> paintTool) where T : TerrainPaintTool<T>
        {
            var onPaintHistory = LoadDataFile(recordingFilePath);
            while (onPaintHistory.Count > 0)
            {
                var paintOccurrence = onPaintHistory.Dequeue();
                var paintPosition = new Vector2(paintOccurrence.xPos, paintOccurrence.yPos);
                Vector3 rayOrigin = new Vector3(
                    Mathf.Lerp(terrainBounds.min.x, terrainBounds.max.x, paintOccurrence.xPos),
                    1000,
                    Mathf.Lerp(terrainBounds.min.z, terrainBounds.max.z, paintOccurrence.yPos)
                );
                Physics.Raycast(new Ray(rayOrigin, Vector3.down), out RaycastHit raycastHit);
                defaultBrushUiGroupMock.SetRaycastHit(raycastHit);
                OnPaintContextMock paintContext = new OnPaintContextMock(GetBrushTexture(), paintPosition, paintOccurrence.brushStrength, paintOccurrence.brushSize, true, raycastHit);
                paintTool.OnPaint(terrainObj, paintContext);
            }
        }
        private float[,] GetFullTerrainHeights(Terrain terrain)
        {
            int terrainWidth = terrain.terrainData.heightmapResolution;
            int terrainHeight = terrain.terrainData.heightmapResolution;
            return terrain.terrainData.GetHeights(
                0, 0,
                terrainWidth,
                terrainHeight
            );
        }

        private bool AreHeightsEqual(float[,] arr1, float[,] arr2)
        {
            if(arr1.Rank != arr2.Rank)
            {
                return false;
            }

            if(arr1.Rank > 1 && arr2.Rank > 1)
            {
                if(arr1.GetLength(0) != arr2.GetLength(0) ||
                   arr1.GetLength(1) != arr2.GetLength(1))
                {
                    return false;
                }
            }

            int xlen = arr1.GetLength(0);
            int ylen = arr1.GetLength(1);

            for(int x = 0; x < xlen; ++x)
            {
                for(int y = 0; y < ylen; ++y)
                {
                    if(arr1[x,y] != arr2[x,y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        [UnityTest]
        [TestCase("PaintHeightHistory", ExpectedResult = null)]
        public IEnumerator Test_PaintHeight_Playback(string recordingFilePath)
        {
            yield return null;
            SetupTerrain();
            var startHeightArr = GetFullTerrainHeights(terrainObj);

            // test load up the terrain paint height brush
            var paintHeightTool = PaintHeightTool.instance;

            // need a way to override the common UI on the different tools so
            // that I can bypass some of their behaviors
            
            paintHeightTool.ChangeCommonUI(defaultBrushUiGroupMock);
            defaultBrushUiGroupMock.SetTerrain(terrainObj);
            
            RunPainting(recordingFilePath, paintHeightTool);
            // check to see if the terrain changed
            Assert.That(AreHeightsEqual(startHeightArr, GetFullTerrainHeights(terrainObj)), Is.False, "Brush didn't make changes to terrain heightmap");
        }

        [UnityTest]
        [TestCase("SetHeightHistory", 204f, ExpectedResult = null)]
        public IEnumerator Test_SetHeight_Playback(string recordingFilePath, float targetHeight)
        {
            yield return null;
            SetupTerrain();
            var startHeightArr = GetFullTerrainHeights(terrainObj);

            // test load up the terrain paint height brush
            var setHeightTool = SetHeightTool.instance;
            setHeightTool.SetTargetHeight(targetHeight);
            // need a way to override the common UI on the different tools so
            // that I can bypass some of their behaviors
            
            setHeightTool.ChangeCommonUI(defaultBrushUiGroupMock);
            defaultBrushUiGroupMock.SetTerrain(terrainObj);
            
            RunPainting(recordingFilePath, setHeightTool);
            // check to see if the terrain changed
            Assert.That(AreHeightsEqual(startHeightArr, GetFullTerrainHeights(terrainObj)), Is.False, "Brush didn't make changes to terrain heightmap");
        }

        [UnityTest]
        [TestCase("StampToolHistory", 500.0f, ExpectedResult = null)]
        public IEnumerator Test_StampTerrain_Playback(string recordingFilePath, float stampHeight) {
            yield return null;
            SetupTerrain();
            var startHeightArr = GetFullTerrainHeights(terrainObj);

            // test load up the terrain paint height brush
            var stampTool = StampTool.instance;
            stampTool.ChangeCommonUI(defaultBrushUiGroupMock);
            stampTool.SetStampHeight(stampHeight);
            defaultBrushUiGroupMock.SetTerrain(terrainObj);
            
            RunPainting(recordingFilePath, stampTool);
            // check to see if the terrain changed
            Assert.That(AreHeightsEqual(startHeightArr, GetFullTerrainHeights(terrainObj)), Is.False, "Brush didn't make changes to terrain heightmap");
        }

        [UnityTest]
        [TestCase("NoiseHeightHistory", "Terrain", ExpectedResult = null)]
        public IEnumerator Test_PaintNoiseHeight_Playback(string recordingFilePath, string targetTerrainName)
        {
            yield return null;
            SetupTerrain();
            var startHeightArr = GetFullTerrainHeights(terrainObj);

            // test load up the terrain paint height brush
            var stampTool = NoiseHeightTool.instance;
            stampTool.ChangeCommonUI(defaultBrushUiGroupMock);
            defaultBrushUiGroupMock.SetTerrain(terrainObj);

            RunPainting(recordingFilePath, stampTool);
            // check to see if the terrain changed
            Assert.That(AreHeightsEqual(startHeightArr, GetFullTerrainHeights(terrainObj)), Is.False,
                "Brush didn't make changes to terrain heightmap");

            yield return null;
        }
        
        // Used to check for texture matrix regressions
        [UnityTest]
        [TestCase("NoiseHeightHistory", "Terrain", ExpectedResult = null)]
        public IEnumerator Test_PaintTexture_Playback(string recordingFilePath, string targetTerrainName) {
            yield return null;
            SetupTerrain();
            var textureTool = PaintTextureTool.instance;
            textureTool.ChangeCommonUI(defaultBrushUiGroupMock);
            defaultBrushUiGroupMock.SetTerrain(terrainObj);

            TerrainLayer tl1 = new TerrainLayer(), tl2 = new TerrainLayer();
            string[] assets = AssetDatabase.FindAssets("testGradientCircle");
            if (assets.Length == 0) {
                Debug.LogError("testGradientCircle could not be found");
            }
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            tl1.diffuseTexture = tl2.diffuseTexture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            terrainObj.terrainData.terrainLayers = new TerrainLayer[] { tl1, tl2 };

            textureTool.SetSelectedTerrainLayer(tl2);
            RunPainting(recordingFilePath, textureTool);


            Assert.Pass("Matrix stack regression not found!");
        }
        
        [UnityTest]
        [TestCase("PaintHeightHistory", "Terrain", ExpectedResult = null)]
        public IEnumerator Test_PaintHeight_With_BrushMaskFilters_Playback(string recordingFilePath, string targetTerrainName)
        {
            yield return null;
            SetupTerrain();
            var startHeightArr = GetFullTerrainHeights(terrainObj);

            // test load up the terrain paint height brush
            var paintHeightTool = PaintHeightTool.instance;

            // need a way to override the common UI on the different tools so
            // that I can bypass some of their behaviors
            
            paintHeightTool.ChangeCommonUI(defaultBrushUiGroupMock);
            defaultBrushUiGroupMock.SetTerrain(terrainObj);

            defaultBrushUiGroupMock.brushMaskFilterStack.Clear(true);

            var filterCount = FilterUtility.GetFilterTypeCount();
            for(int i = 0; i < filterCount; ++i)
            {
                defaultBrushUiGroupMock.brushMaskFilterStack.Add(FilterUtility.CreateInstance(FilterUtility.GetFilterType(i)));
            }
            
            RunPainting(recordingFilePath, paintHeightTool);
        }
        
        [UnityTest]
        public IEnumerator Test_SetHeight_FlattenTile()
        {
            yield return null;
            var setHeightTool = SetHeightTool.instance;
            setHeightTool.ChangeCommonUI(defaultBrushUiGroupMock);
            defaultBrushUiGroupMock.SetTerrain(terrainObj);

            SetupTerrain();
            yield return null;
            setHeightTool.Flatten(terrainObj);
            yield return null;
        }
    }
}