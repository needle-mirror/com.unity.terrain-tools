using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.TerrainTools
{
    internal class ThermalErosionToolOvl : TerrainToolsPaintTool<ThermalErosionToolOvl>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Thermal Erosion Tool", typeof(TerrainToolShortcutContext))]               // tells shortcut manager what to call the shortcut and what to pass as args
        static void SelectShortcut(ShortcutArguments args)
        {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;          // gets interface to modify state of TerrainTools
            context.SelectPaintToolWithOverlays<ThermalErosionToolOvl>();                                                                        // set active tool
            TerrainToolsAnalytics.OnShortcutKeyRelease("Select Thermal Erosion Tool");
        }
#endif
        public override string OnIcon => "Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/ThermalErosion_On.png";
        public override string OffIcon => "Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/ThermalErosion.png";
        
        IBrushUIGroup commonUI {
            get
            {
                if (m_commonUI == null)
                {
                    m_commonUI = new Erosion.ErosionBrushUIGroup(
                        "ThermalErosion",
                        UpdateAnalyticParameters,
                        DefaultBrushUIGroup.Feature.All,
                        new DefaultBrushUIGroup.FeatureDefaults { Strength = 0.95f }
                        );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }

        Erosion.ThermalEroder m_Eroder = null;

        public override void OnEnable()
        {
            base.OnEnable();
            m_Eroder = new Erosion.ThermalEroder();
            m_Eroder.OnEnable();
        }

        public override void OnEnterToolMode()
        {
            base.OnEnterToolMode();
            commonUI.OnEnterToolMode();
        }

        public override void OnExitToolMode()
        {
            base.OnExitToolMode();
            commonUI.OnExitToolMode();
        }

        Material m_Material = null;
        Material GetPaintMaterial()
        {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/SimpleHeightBlend"));
            return m_Material;
        }

        Material m_SplatMaterial = null;
        Material GetSplatMaterial()
        {
            if (m_SplatMaterial == null)
                m_SplatMaterial = new Material(Shader.Find("Hidden/TerrainTools/SedimentSplat"));
            return m_SplatMaterial;
        }

        public override int IconIndex
        {
            get { return (int) ToolIndex.SculptIndex.ThermalErosion; }
        }

        public override TerrainCategory Category
        {
            get { return TerrainCategory.Sculpt; }
        }

        public override string GetName()
        {
            return "Erosion/Thermal";
            
        }

        public override string GetDescription()
        {
            return "Simulates the effect of temperature changes, weathering and redistributing sediment.";
            
        }
        
        public override bool HasToolSettings => true;
        public override bool HasBrushFilters => true;
        public override bool HasBrushMask => true;
        public override bool HasBrushAttributes => true;

        public override void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext)
        {
            commonUI.OnSceneGUI2D(terrain, editContext);

            // only do the rest if user mouse hits valid terrain or they are using the
            // brush parameter hotkeys to resize, etc
            if (!editContext.hitValidTerrain && !commonUI.isInUse)
            {
                return;
            }
            
            // Only render preview if this is a repaint. losing performance if we do
            if (Event.current.type == EventType.Repaint)
            {
                using (IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "ThermalErosion", editContext.brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        Material previewMaterial = Utility.GetDefaultPreviewMaterial(commonUI.hasEnabledFilters);
                        PaintContext ctx = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                        var texelCtx = Utility.CollectTexelValidity(ctx.originTerrain, brushXform.GetBrushXYBounds());
                        Utility.SetupMaterialForPaintingWithTexelValidityContext(ctx, texelCtx, brushXform, previewMaterial);
                        var filterRT = RTUtils.GetTempHandle(ctx.sourceRenderTexture.width, ctx.sourceRenderTexture.height,
                            0, FilterUtility.defaultFormat);
                        Utility.GenerateAndSetFilterRT(commonUI, ctx.sourceRenderTexture, filterRT, previewMaterial);
                        TerrainPaintUtilityEditor.DrawBrushPreview(ctx, TerrainBrushPreviewMode.SourceRenderTexture,
                            editContext.brushTexture, brushXform, previewMaterial, 0);
                        texelCtx.Cleanup();
                        RTUtils.Release(filterRT);
                        brushRender.Release(ctx);
                    }
                }
            }
            
            // update brush UI group
            commonUI.OnSceneGUI(terrain, editContext);

        }

        bool m_ShowControls = true;
        bool m_ShowAdvancedUI = false;
        string[] m_MatNames = {
                    "Custom",
                    "Dry Ash",    // 40
                    "Chalk",      // 45
                    "Dry Clay",   // 25 - 40
                    "Wet Clay",   // 15
                    "Soil",       // 30-45
                    "Granite Scree", // 35 - 40
                    "Gravel", //45 - 45
                    "Dry Sand",
                    "Wet Sand",
                    "Quick Sand",
                    "Snow"
                };

        float[,] m_TauValues = new float[,] {
                    { -1.0f, -1.0f },  //custom
                    { 38.0f, 42.0f },  //dry ash
                    { 45.0f, 45.0f },  //chalk
                    { 25.0f, 40.0f },  //dry clay (25 - 40)
                    { 15.0f, 15.0f },  //wet clay 
                    { 30.0f, 45.0f },  //soil (30-45)
                    { 35.0f, 40.0f },  //crushed granite (35 - 40) 
                    { 45.0f, 45.0f },  //gravel
                    { 32.0f, 36.0f },  //dry sand
                    { 45.0f, 45.0f },  //wet sand
                    { 15.0f, 30.0f },  //quicksand (15-30)
                    { 38.0f, 38.0f }   //snow
                };

        public override void OnToolSettingsGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            commonUI.validationMessage = TerrainToolGUIHelper.ValidateAndGenerateSceneGUIMessage(terrain);

            m_ShowControls = TerrainToolGUIHelper.DrawHeaderFoldoutForErosion(Erosion.Styles.m_ThermalErosionControls, m_ShowControls, m_Eroder.ResetTool);

            if (m_ShowControls)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                EditorGUI.BeginChangeCheck();
                m_Eroder.m_MatPreset = EditorGUILayout.Popup(Erosion.Styles.m_MatPreset, m_Eroder.m_MatPreset, m_MatNames);
                if (EditorGUI.EndChangeCheck() && m_Eroder.m_MatPreset != 0)
                {
                    m_Eroder.m_AngleOfRepose.x = m_TauValues[m_Eroder.m_MatPreset, 0];
                    m_Eroder.m_AngleOfRepose.y = m_TauValues[m_Eroder.m_MatPreset, 1];
                }

                m_ShowAdvancedUI = TerrainToolGUIHelper.DrawSimpleFoldout(new GUIContent("Advanced"), m_ShowAdvancedUI);
                if (m_ShowAdvancedUI)
                {
                    EditorGUI.indentLevel++;
                    m_Eroder.m_ThermalIterations = EditorGUILayout.IntSlider(Erosion.Styles.m_NumIterations, m_Eroder.m_ThermalIterations, 1, 1000);
                    m_Eroder.m_dt = EditorGUILayout.Slider(Erosion.Styles.m_TimeDelta, m_Eroder.m_dt, 0.00001f, 0.05f);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.MinMaxSlider(Erosion.Styles.m_AngleOfRepose, ref m_Eroder.m_AngleOfRepose.x, ref m_Eroder.m_AngleOfRepose.y, 0.0f, 90.0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_Eroder.m_MatPreset = 0; //we changed the angle of repose, so now we should switch the UI to "Custom"
                    }

                    m_Eroder.m_ReposeJitter = EditorGUILayout.IntSlider(Erosion.Styles.m_AngleOfReposeJitter, (int)m_Eroder.m_ReposeJitter, 0, 100);
                    EditorGUI.indentLevel--; 
                }

                EditorGUILayout.EndVertical();
            }
        }

        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            EditorGUI.BeginChangeCheck();

            commonUI.OnInspectorGUI(terrain, editContext);
            OnToolSettingsGUI(terrain, editContext);

            if (EditorGUI.EndChangeCheck())
            {
                TerrainToolsAnalytics.OnParameterChange();
            }
        }

        private void AddHeight(Terrain terrain, IOnPaint editContext)
        {
            using (IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "ThermalErosion", editContext.brushTexture))
            {
                if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds(), 1);

                    paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;
                    Material mat = TerrainPaintUtility.GetBuiltinPaintMaterial();
                    float brushStrength = Event.current.shift ? -m_Eroder.m_AddHeightAmt : m_Eroder.m_AddHeightAmt;
                    brushStrength *= (commonUI.brushStrength);

                    if (Event.current.control)
                    { brushStrength = 0.0f; }

                    Vector4 brushParams = new Vector4(0.0001f * brushStrength, 0.0f, 0.0f, 0.0f);
                    mat.SetTexture("_BrushTex", editContext.brushTexture);
                    mat.SetVector("_BrushParams", brushParams);

                    brushRender.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                    brushRender.RenderBrush(paintContext, mat, (int)TerrainBuiltinPaintMaterialPasses.RaiseLowerHeight);
                }
            }
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            if (!commonUI.allowPaint)
            { return false; }

            int[] numWorkGroups = { 8, 8, 1 };

            using (IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "ThermalErosion", editContext.brushTexture))
            {
                if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds(), 1);
                    paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;

                    //figure out what size we need our render targets to be
                    Rect brushRect = brushXform.GetBrushXYBounds();

                    m_Eroder.inputTextures["Height"] = paintContext.sourceRenderTexture;

                    var heightRT = RTUtils.GetTempHandle(RTUtils.GetDescriptorRW((int)brushRect.width, (int)brushRect.height, 0, RenderTextureFormat.RFloat));
                    Vector2 texelSize = new Vector2(terrain.terrainData.size.x / terrain.terrainData.heightmapResolution,
                                                    terrain.terrainData.size.z / terrain.terrainData.heightmapResolution);
                    m_Eroder.ErodeHeightmap(heightRT, terrain.terrainData.size, brushRect, texelSize);

                    Material mat = GetPaintMaterial();
                    var brushMask = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
                    Utility.GenerateAndSetFilterRT(commonUI, paintContext.sourceRenderTexture, brushMask, mat);
                    Vector4 brushParams = new Vector4(commonUI.brushStrength, 0.0f, 0.0f, 0.0f);
                    mat.SetTexture("_BrushTex", editContext.brushTexture);
                    mat.SetTexture("_NewHeightTex", heightRT);
                    mat.SetVector("_BrushParams", brushParams);

                    brushRender.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                    brushRender.RenderBrush(paintContext, mat, 0);
                    RTUtils.Release(brushMask);
                    RTUtils.Release(heightRT);
                }
            }

            return true;
        }

        //Analytics Setup
        private TerrainToolsAnalytics.IBrushParameter[] UpdateAnalyticParameters() => new TerrainToolsAnalytics.IBrushParameter[]{
            new TerrainToolsAnalytics.BrushParameter<string>{Name = Erosion.Styles.m_MatPreset.text, Value = m_MatNames[m_Eroder.m_MatPreset]},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_NumIterations.text, Value = m_Eroder.m_ThermalIterations},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_TimeDelta.text, Value = m_Eroder.m_dt},
            new TerrainToolsAnalytics.BrushParameter<Vector2>{Name = Erosion.Styles.m_AngleOfRepose.text, Value = m_Eroder.m_AngleOfRepose},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_AngleOfReposeJitter.text, Value = m_Eroder.m_ReposeJitter},
        };
    }
}
