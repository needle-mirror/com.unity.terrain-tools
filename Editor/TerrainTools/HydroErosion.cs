using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEditor.ShortcutManagement;
using UnityEditor.TerrainTools.Erosion;

namespace UnityEditor.TerrainTools
{
    internal class HydroErosionTool : TerrainPaintTool<HydroErosionTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Hydraulic Erosion Brush", typeof(TerrainToolShortcutContext), KeyCode.F4)]               // tells shortcut manager what to call the shortcut and what to pass as args
        static void SelectShortcut(ShortcutArguments args)
        {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;          // gets interface to modify state of TerrainTools
            context.SelectPaintTool<HydroErosionTool>();                                                                        // set active tool
            TerrainToolsAnalytics.OnShortcutKeyRelease("Select Hydraulic Erosion Brush");
        }
#endif

        [SerializeField]
        IBrushUIGroup m_commonUI;
        private IBrushUIGroup commonUI {
            get
            {
                if (m_commonUI == null)
                {
                    m_commonUI = new ErosionBrushUIGroup(
                        "HydroErosion",
                        UpdateAnalyticParameters,
                        DefaultBrushUIGroup.Feature.All,
                        new DefaultBrushUIGroup.FeatureDefaults { Strength = 0.52f }
                        );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }


        NoiseSettings m_HardnessNoiseSettings = null;
        Erosion.HydraulicEroder m_Eroder = null;// = new Erosion.HydraulicEroder();

        Material m_Material = null;
        Material GetPaintMaterial()
        {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/SimpleHeightBlend"));
            return m_Material;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            m_Eroder = new Erosion.HydraulicEroder();
            m_Eroder.OnEnable();
        }

        public override string GetName()
        {
            return "Erosion/Hydraulic";
        }

        public override string GetDescription()
        {
            return "Simulates the effect of water transporting and redistributing sediment.";
        }

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
                using (IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "HydroErosion", editContext.brushTexture))
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

        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            if (m_HardnessNoiseSettings == null)
            {
                m_HardnessNoiseSettings = ScriptableObject.CreateInstance<NoiseSettings>();
                m_HardnessNoiseSettings.Reset();
            }



            Erosion.HydraulicErosionSettings erosionSettings = ((Erosion.HydraulicEroder)m_Eroder).m_ErosionSettings;

            EditorGUI.BeginChangeCheck();

            commonUI.OnInspectorGUI(terrain, editContext);

            m_Eroder.OnInspectorGUI(terrain, editContext);

            commonUI.validationMessage = TerrainToolGUIHelper.ValidateAndGenerateSceneGUIMessage(terrain);

            if (EditorGUI.EndChangeCheck())
            {
                Save(true);
                TerrainToolsAnalytics.OnParameterChange();
            }
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

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            if (!commonUI.allowPaint)
            { return true; }

            using (IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "HydroErosion", editContext.brushTexture))
            {
                if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    var brushBounds = brushXform.GetBrushXYBounds();
                    PaintContext paintContext = brushRender.AcquireHeightmap(true, brushBounds, 1);
                    paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;

                    m_Eroder.inputTextures["Height"] = paintContext.sourceRenderTexture;

                    var heightRT = RTUtils.GetTempHandle(RTUtils.GetDescriptorRW((int)brushBounds.width, (int)brushBounds.height, 0, RenderTextureFormat.RFloat));
                    Vector2 texelSize = new Vector2(terrain.terrainData.size.x / terrain.terrainData.heightmapResolution,
                                                    terrain.terrainData.size.z / terrain.terrainData.heightmapResolution);
                    m_Eroder.ErodeHeightmap(heightRT, terrain.terrainData.size, brushXform.GetBrushXYBounds(), texelSize, Event.current.control); // TODO(wyatt): commonUI.ModifierActive(BrushModifierKey.BRUSH_MOD_INVERT)

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

        private TerrainToolsAnalytics.IBrushParameter[] UpdateAnalyticParameters()
        {
            HydraulicErosionSettings settings = m_Eroder.m_ErosionSettings;
            return new TerrainToolsAnalytics.IBrushParameter[]{
            //Advanced Section
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_SimulationScale.text, Value = settings.m_SimScale.value},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_TimeDelta.text, Value = settings.m_HydroTimeDelta.value},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_NumIterations.text, Value = settings.m_HydroIterations.value},
            
            //Thermal Smoothing
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_ThermalDTScalar.text, Value = settings.m_ThermalTimeDelta},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_NumIterations.text, Value = settings.m_ThermalIterations},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_AngleOfRepose.text, Value = settings.m_ThermalReposeAngle},

            //Water Transport
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_PrecipitationRate.text, Value = settings.m_PrecipRate.value},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_EvaporationRate.text, Value = settings.m_EvaporationRate.value},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_FlowRate.text, Value = settings.m_FlowRate.value},

            //Sediment Transport
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_SedimentCap.text, Value = settings.m_SedimentCapacity.value},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_SedimentDeposit.text, Value = settings.m_SedimentDepositRate.value},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_SedimentDissolve.text, Value = settings.m_SedimentDissolveRate.value},

            //Riverbank
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_RiverbankDeposit.text, Value = settings.m_RiverBankDepositRate.value},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_RiverbankDissolve.text, Value = settings.m_RiverBankDissolveRate.value},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_RiverbedDeposit.text, Value = settings.m_RiverBedDepositRate.value},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Erosion.Styles.m_RiverbedDissolve.text, Value = settings.m_RiverBedDissolveRate.value},

            };
        }
    }
}
