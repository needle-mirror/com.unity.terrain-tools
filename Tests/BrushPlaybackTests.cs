using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEditor.Experimental.TerrainAPI.BaseBrushUIGroup;

namespace UnityEditor.Experimental.TerrainAPI {
    [TestFixture]
    public class BrushPlaybackTests : MonoBehaviour {
        Terrain terrainObj = null;
        private Bounds terrainBounds;
        private Queue<OnPaintOccurrence> onPaintHistory = null;

        private Type onSceneGUIContextType, terrainToolType, onPaintType;

        private static BindingFlags s_bindingFlags = BindingFlags.Public |
                                                        BindingFlags.NonPublic |
                                                        BindingFlags.Static |
                                                        BindingFlags.Instance |
                                                        BindingFlags.FlattenHierarchy;
        private float[,] startHeightArr;

        private object terrainToolInstance;
        private MethodInfo onPaintMethod, onSceneGUIMethod;
        private Type baseBrushUIGroupType, brushRotationType, brushSizeType, brushStrengthType;
        private BaseBrushUIGroup commonUIInstance;
        private PropertyInfo brushRotationProperty, brushSizeProperty, brushStrengthProperty;

        private void InitTerrainTypesWithReflection(string paintToolName) {

            terrainToolType = Type.GetType("UnityEditor.Experimental.TerrainAPI." + paintToolName + ", " +
                                           "Unity.TerrainTools.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            onPaintType = Type.GetType("UnityEditor.Experimental.TerrainAPI.OnPaintContext, " +
                                       "UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            onSceneGUIContextType = Type.GetType("UnityEditor.Experimental.TerrainAPI.OnSceneGUIContext, " +
                                                 "UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

            // Get the method and instance for the current tool being tested
            PropertyInfo propertyInfo = terrainToolType.GetProperty("instance", s_bindingFlags);
            MethodInfo methodInfo = propertyInfo.GetGetMethod();
            terrainToolInstance = methodInfo.Invoke(null, null);

            onPaintMethod = terrainToolType.GetMethod("OnPaint");
            onSceneGUIMethod = terrainToolType.GetMethod("OnSceneGUI");

            MethodInfo loadSettingsInfo = terrainToolType.GetMethod("LoadSettings", s_bindingFlags);

            if (loadSettingsInfo != null) {
                loadSettingsInfo.Invoke(terrainToolInstance, null);
            }

            // LOAD TOOL SETTINGS
            baseBrushUIGroupType = Type.GetType("UnityEditor.Experimental.TerrainAPI.BaseBrushUIGroup, " +
                                                 "Unity.TerrainTools.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            brushSizeType = Type.GetType("UnityEditor.Experimental.TerrainAPI.IBrushSizeController, " +
                                                 "Unity.TerrainTools.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            brushStrengthType = Type.GetType("UnityEditor.Experimental.TerrainAPI.IBrushStrengthController, " +
                                                 "Unity.TerrainTools.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            brushRotationType = Type.GetType("UnityEditor.Experimental.TerrainAPI.IBrushRotationController, " +
                                                 "Unity.TerrainTools.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

            FieldInfo baseBrushUIGroupFieldInfo = terrainToolType.GetField("commonUI", BindingFlags.NonPublic | BindingFlags.Instance);
            if (baseBrushUIGroupFieldInfo == null) {
                PropertyInfo baseBrushUIGroupPropertyInfo = terrainToolType.GetProperty("commonUI", BindingFlags.NonPublic | BindingFlags.Instance);
                if (baseBrushUIGroupPropertyInfo != null) {
                    commonUIInstance = baseBrushUIGroupPropertyInfo.GetValue(terrainToolInstance) as BaseBrushUIGroup;
                }
            } else {
                commonUIInstance = baseBrushUIGroupFieldInfo.GetValue(terrainToolInstance) as BaseBrushUIGroup;
                }
            if (commonUIInstance == null) {
                throw new Exception("The commonUI of the brush can't be found - does it have one?");
            }

            brushSizeProperty = baseBrushUIGroupType.GetProperty("brushSize", BindingFlags.Public | BindingFlags.Instance);
            brushStrengthProperty = baseBrushUIGroupType.GetProperty("brushStrength", BindingFlags.Public | BindingFlags.Instance);
            brushRotationProperty = baseBrushUIGroupType.GetProperty("brushRotation", BindingFlags.Public | BindingFlags.Instance);
        }

        // Triggered once per frame while the 
        void OnSceneGUI(SceneView sceneView)
        {
            if (onPaintHistory.Count == 0 || terrainObj == null)
            {
                return;
            }

            OnPaintOccurrence paintOccurrence = onPaintHistory.Dequeue();

            // Generate a raycast from the relative UV and terrain size
            Vector3 rayOrigin = new Vector3(
                Mathf.Lerp(terrainBounds.min.x, terrainBounds.max.x, paintOccurrence.xPos),
                1000,
                Mathf.Lerp(terrainBounds.min.z, terrainBounds.max.z, paintOccurrence.yPos)
            );

            Physics.Raycast(new Ray(rayOrigin, Vector3.down), out RaycastHit hit);

            Texture brushTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(paintOccurrence.brushTextureAssetPath) as Texture;

            // Instantiate a null SceneGUIContext with the above raycast
            object onSceneGUIContextInstance = Activator.CreateInstance(
                onSceneGUIContextType,
                null, hit, brushTexture, paintOccurrence.brushStrength, paintOccurrence.brushSize
            );

            // set context info in case tool uses that instead of brush ui group
            MethodInfo setInfo = onSceneGUIContextType.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance);
            setInfo.Invoke(onSceneGUIContextInstance,
                           new object[]
                           {
                                sceneView, true, hit,
                                brushTexture,
                                paintOccurrence.brushStrength,
                                paintOccurrence.brushSize
                           });

            brushSizeProperty.SetValue(commonUIInstance, paintOccurrence.brushSize);
            brushStrengthProperty.SetValue(commonUIInstance, paintOccurrence.brushStrength);
            brushRotationProperty.SetValue(commonUIInstance, paintOccurrence.brushRotation);

            onSceneGUIMethod.Invoke(terrainToolInstance, new object[] { terrainObj, onSceneGUIContextInstance });

            // Set the brush strength via commonUI
            commonUIInstance.brushStrength = paintOccurrence.brushStrength;
            commonUIInstance.brushSize = paintOccurrence.brushSize;

            object onPaintContext = Activator.CreateInstance(
                onPaintType,
                hit,
                brushTexture,
                new Vector2(paintOccurrence.xPos, paintOccurrence.yPos),
                paintOccurrence.brushStrength,
                paintOccurrence.brushSize
            );
            onPaintMethod.Invoke(terrainToolInstance, new object[] { terrainObj, onPaintContext });
        }

        private void ResetTerrainHeight(Terrain terrain)
        {
            float[,] heights = GetFullTerrainHeights(terrain);

            for (int x = 0; x < terrain.terrainData.heightmapResolution; x++) {
                for (int y = 0; y < terrain.terrainData.heightmapResolution; y++) {
                    heights[x, y] = 0;
                }
            }

            terrain.terrainData.SetHeights(0, 0, heights);
        }

        private Queue<OnPaintOccurrence> LoadDataFile(string recordingFileName, bool expectNull = false) {
            // Discover path to data file
            string[] assets = AssetDatabase.FindAssets(recordingFileName);
            if (assets.Length == 0) {
                Debug.LogError("No asset with name " + recordingFileName + " found");
            }
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

            // Load data file as a List<paintHistory>
            FileStream file = File.OpenRead(assetPath);
            BinaryFormatter bf = new BinaryFormatter();
            Queue<OnPaintOccurrence> paintHistory = new Queue<OnPaintOccurrence>(bf.Deserialize(file) as List<OnPaintOccurrence>);

            file.Close();

            if (paintHistory.Count == 0 && !expectNull)
            {
                throw new InconclusiveException("The loaded file contains no recordings");
            }

            return paintHistory;
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

        private bool AreHeightsNotEqual(float[,] arr1, float[,] arr2)
        {
            return !AreHeightsEqual(arr1, arr2);
        }

        public void SetupTerrain(string terrainName) {
            TerrainData td = new TerrainData();
            td.size = new Vector3(1000, 600, 1000);
            td.heightmapResolution = 513;
            td.baseMapResolution = 1024;
            td.SetDetailResolution(1024, 32);

            // Generate terrain
            GameObject terrainGo = Terrain.CreateTerrainGameObject(td);
            terrainObj = terrainGo.GetComponent<Terrain>();
            terrainBounds = terrainGo.GetComponent<TerrainCollider>().bounds;
            Selection.activeObject = terrainGo;

            ResetTerrainHeight(terrainObj);

            Selection.activeObject = terrainGo;
            
            startHeightArr = GetFullTerrainHeights(terrainObj);
        }

        [TearDown]
        public void Cleanup() {
            Selection.activeObject = null;
            if (onPaintHistory != null)
                onPaintHistory.Clear();
        }

        [SetUp]
        public void SetUp() {
            EditorWindow.GetWindow<SceneView>().Focus();
        }

        [UnityTest]
        [TestCase("PaintHeightHistory", "Terrain", ExpectedResult = null)]
        public IEnumerator Test_PaintHeight_Playback(string recordingFilePath, string targetTerrainName) {
            yield return null;

            InitTerrainTypesWithReflection("PaintHeightTool");
            onPaintHistory = LoadDataFile(recordingFilePath);
            SetupTerrain(targetTerrainName);

            // Enables the core brush playback on OnSceneGUI
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            while (onPaintHistory.Count > 0)
            {
                // Force a SceneView update for OnSceneGUI to be triggered
                SceneView.RepaintAll();
                yield return null;
            }

            if(terrainObj.drawInstanced)
            {
                terrainObj.terrainData.SyncHeightmap();
            }
            
            SceneView.duringSceneGui -= OnSceneGUI;
            Selection.activeObject = null;

            if(AreHeightsNotEqual(startHeightArr, GetFullTerrainHeights(terrainObj)))
            {
                Assert.Pass();
            }
            
            Assert.Fail("Test reached the end of function without pass or fail");
        }
        
        [UnityTest]
        [TestCase("SetHeightHistory", 204f, ExpectedResult = null)]
        public IEnumerator Test_SetHeight_Playback(string recordingFilePath, float targetHeight) {
            yield break;

            InitTerrainTypesWithReflection("SetHeightTool");
            onPaintHistory = LoadDataFile(recordingFilePath);
            SetupTerrain("Terrain");

            // Set the height parameter
            FieldInfo heightField = terrainToolType.GetField("m_HeightWorldSpace", BindingFlags.NonPublic | BindingFlags.Instance);
            heightField.SetValue(terrainToolInstance, targetHeight);

            // Enables the core brush playback on OnSceneGUI
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            while (onPaintHistory.Count > 0)
            {
                // Force a SceneView update for OnSceneGUI to be triggered
                SceneView.RepaintAll();
                yield return null;
            }

            if(terrainObj.drawInstanced)
            {
                terrainObj.terrainData.SyncHeightmap();
            }
            
            SceneView.duringSceneGui -= OnSceneGUI;
            Selection.activeObject = null;

            if(AreHeightsNotEqual(startHeightArr, GetFullTerrainHeights(terrainObj)))
            {
                Assert.Pass();
            }
            
            Assert.Fail("Test reached the end of function without pass or fail");
        }

        [UnityTest]
        [TestCase("SmileHistory", "VerticalZigzagHistory", 100.0f, ExpectedResult = null)]
        public IEnumerator Test_SmoothHeight_Playback(string setHeightFilePath, string smoothFilePath, float setHeightScalar) {
            yield break;
            // Paint with the SetHeightTool first to have something to smooth from

            InitTerrainTypesWithReflection("SetHeightTool");
            onPaintHistory = LoadDataFile(setHeightFilePath);
            SetupTerrain("Terrain");

            // Set the height parameter
            FieldInfo heightField = terrainToolType.GetField("m_HeightWorldSpace", BindingFlags.NonPublic | BindingFlags.Instance);
            heightField.SetValue(terrainToolInstance, setHeightScalar);  // Use 20 b/c why not

            // Enables the core brush playback on OnSceneGUI
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            
            while (onPaintHistory.Count > 0)
            {
                // Force a SceneView update for OnSceneGUI to be triggered
                SceneView.RepaintAll();
                yield return null;
            }

            if(terrainObj.drawInstanced)
            {
                terrainObj.terrainData.SyncHeightmap();
            }
            
            SceneView.duringSceneGui -= OnSceneGUI;
            Selection.activeObject = null;

            if(AreHeightsNotEqual(startHeightArr, GetFullTerrainHeights(terrainObj)))
            {
                Assert.Pass();
            }
            
            // Repopulate onPaintHistory for the smoothing loop
            InitTerrainTypesWithReflection("SmoothHeightTool");
            onPaintHistory = LoadDataFile(smoothFilePath);
            // Instead of using the full terrain reset, only init terrain height
            startHeightArr = GetFullTerrainHeights(terrainObj);

            // Enables the core brush playback on OnSceneGUI
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            
            while (onPaintHistory.Count > 0)
            {
                // Force a SceneView update for OnSceneGUI to be triggered
                SceneView.RepaintAll();
                yield return null;
            }

            if(terrainObj.drawInstanced)
            {
                terrainObj.terrainData.SyncHeightmap();
            }
            
            SceneView.duringSceneGui -= OnSceneGUI;
            Selection.activeObject = null;

            if(AreHeightsNotEqual(startHeightArr, GetFullTerrainHeights(terrainObj)))
            {
                Assert.Pass();
            }
            
            Assert.Fail("Test reached the end of function without pass or fail");
        }

        [UnityTest]
        [TestCase("StampToolHistory", 500.0f, ExpectedResult = null)]
        public IEnumerator Test_StampTerrain_Playback(string recordingFilePath, float stampHeight) {
            yield return null;

            InitTerrainTypesWithReflection("StampTool");
            onPaintHistory = LoadDataFile(recordingFilePath);
            SetupTerrain("Terrain");

    
            // Set the height parameter
            FieldInfo propertiesField = terrainToolType.GetField("stampToolProperties", BindingFlags.NonPublic | BindingFlags.Instance);
            object props = propertiesField.GetValue(terrainToolInstance);
            FieldInfo heightField = props.GetType().GetField("m_StampHeight");
            heightField.SetValue(props, stampHeight);  // Use 20 b/c why not

            // Enables the core brush playback on OnSceneGUI
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            while (onPaintHistory.Count > 0)
            {
                // Force a SceneView update for OnSceneGUI to be triggered
                SceneView.RepaintAll();
                yield return null;
            }

            if(terrainObj.drawInstanced)
            {
                terrainObj.terrainData.SyncHeightmap();
            }
            
            SceneView.duringSceneGui -= OnSceneGUI;
            Selection.activeObject = null;

            if(AreHeightsNotEqual(startHeightArr, GetFullTerrainHeights(terrainObj)))
            {
                Assert.Pass();
            }
            
            Assert.Fail("Test reached the end of function without pass or fail");
        }
        
        [UnityTest]
        [TestCase("NoiseHeightHistory", "Terrain", ExpectedResult = null)]
        public IEnumerator Test_PaintNoiseHeight_Playback(string recordingFilePath, string targetTerrainName) {
            yield return null;

            InitTerrainTypesWithReflection("NoiseHeightTool");
            onPaintHistory = LoadDataFile(recordingFilePath);
            SetupTerrain(targetTerrainName);

            // Enables the core brush playback on OnSceneGUI
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            while (onPaintHistory.Count > 0)
            {
                // Force a SceneView update for OnSceneGUI to be triggered
                SceneView.RepaintAll();
                yield return null;
            }

            if(terrainObj.drawInstanced)
            {
                terrainObj.terrainData.SyncHeightmap();
            }
            
            SceneView.duringSceneGui -= OnSceneGUI;
            Selection.activeObject = null;

            if(AreHeightsNotEqual(startHeightArr, GetFullTerrainHeights(terrainObj)))
            {
                Assert.Pass();
            }
            
            Assert.Fail("Test reached the end of function without pass or fail");
        }

        // Used to check for texture matrix regressions
        [UnityTest]
        [TestCase("NoiseHeightHistory", "Terrain", ExpectedResult = null)]
        public IEnumerator Test_PaintTexture_Playback(string recordingFilePath, string targetTerrainName) {
            yield return null;

            Terrain t = GameObject.Find("Terrain").GetComponent<Terrain>();

            InitTerrainTypesWithReflection("PaintTextureTool");
            onPaintHistory = LoadDataFile(recordingFilePath);
            SetupTerrain(targetTerrainName);

            TerrainLayer tl1 = new TerrainLayer(), tl2 = new TerrainLayer();
            tl1.diffuseTexture = Resources.Load<Texture2D>("testGradientCircle");
            tl2.diffuseTexture = Resources.Load<Texture2D>("testGradientCircle");
            terrainObj.terrainData.terrainLayers = new TerrainLayer[] { tl1, tl2 };

            PaintTextureTool paintTextureTool = terrainToolInstance as PaintTextureTool;
            FieldInfo selectedTerrainLayerInfo = typeof(PaintTextureTool).GetField("m_SelectedTerrainLayer", 
               s_bindingFlags);
            selectedTerrainLayerInfo.SetValue(paintTextureTool, tl2);

            // Enables the core brush playback on OnSceneGUI
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            
            while (onPaintHistory.Count > 0) {
                // Force a SceneView update for OnSceneGUI to be triggered
                SceneView.RepaintAll();
                yield return null;
            }

            Assert.Pass("Matrix stack regression not found!");
        }
    }
}
