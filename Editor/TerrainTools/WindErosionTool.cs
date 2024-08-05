using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.TerrainTools
{
    internal class WindErosionTool : TerrainPaintTool<WindErosionTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Wind Erosion Tool", typeof(TerrainToolShortcutContext))]                     // tells shortcut manager what to call the shortcut and what to pass as args
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;              // gets interface to modify state of TerrainTools
            context.SelectPaintTool<WindErosionTool>();                                                  // set active tool
            TerrainToolsAnalytics.OnShortcutKeyRelease("Select Wind Erosion Tool");
        }
#endif

        [SerializeField]
        IBrushUIGroup m_commonUI;
        private IBrushUIGroup commonUI
        {
            get
            {
                if( m_commonUI == null )
                {
                    m_commonUI = new Erosion.ErosionBrushUIGroup( "WindErosion",
                        UpdateAnalyticParameters,
                        DefaultBrushUIGroup.Feature.All,
                        new DefaultBrushUIGroup.FeatureDefaults { Strength = 0.64f }
                        );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }

        Erosion.WindEroder m_Eroder = null;

        public override void OnEnterToolMode() {
            base.OnEnterToolMode();
            commonUI.OnEnterToolMode();
        }

        public override void OnExitToolMode() {
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

        public override string GetName()
        {
            return "Erosion/Wind";
        }

        public override string GetDescription()
        {
            return "Simulates the effect of wind transporting and redistributing sediment.\n\n" + 
                "Hold D + Drag to change the wind direction";
        }

        private void RepaintInspector() {
            Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; ++i) {
                if (ed[i].GetType() == this.GetType()) {
                    ed[i].Repaint();
                    return;
                }
            }
        }

        public override void OnEnable() {
            base.OnEnable();
            m_Eroder = new Erosion.WindEroder();
            m_Eroder.OnEnable();
        }

        Vector3 m_SceneRaycastHitPoint;
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
                using(IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "WindErosion", editContext.brushTexture))
                {
                
                    if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
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
                        brushRender.Release(ctx); // release the context 
            
                        Quaternion windRot = Quaternion.AngleAxis(commonUI.brushRotation, new Vector3(0.0f, 1.0f, 0.0f));
                        Handles.ArrowHandleCap(0, commonUI.raycastHitUnderCursor.point, windRot, 0.5f * commonUI.brushSize, EventType.Repaint);
                    }
                }
            }

            // update brush UI group
            commonUI.OnSceneGUI(terrain, editContext);

        }

        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            EditorGUI.BeginChangeCheck();

            commonUI.OnInspectorGUI(terrain, editContext);
            m_Eroder.OnInspectorGUI();

            commonUI.validationMessage = TerrainToolGUIHelper.ValidateAndGenerateSceneGUIMessage(terrain);

            if (EditorGUI.EndChangeCheck()) {
                Save(true);
                TerrainToolsAnalytics.OnParameterChange();
            }

        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            if(!commonUI.allowPaint) { return true; }

            Vector2 uv = editContext.uv;

            if(commonUI.ScatterBrushStamp(ref terrain, ref uv))
            {
                using(IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "WindErosion", editContext.brushTexture))
                {
                    if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        var brushBounds = brushXform.GetBrushXYBounds();
                        PaintContext paintContext = brushRender.AcquireHeightmap(true, brushBounds, 4);
                        paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;

                        //paintContext.sourceRenderTexture = input heightmap
                        //Add Velocity (user wind direction and strength, or texture input, noise, forces, drag etc...)
                        float angle = commonUI.brushRotation; //m_WindAngleDegrees + r;
                        float r = 0.5f * (2.0f * UnityEngine.Random.value - 1.0f) * 0.01f * m_Eroder.m_WindSpeedJitter;
                        float speed = m_Eroder.m_WindSpeed.value + r;

                        float rad = angle * Mathf.Deg2Rad;
                        m_Eroder.m_WindVel = speed * (new Vector4(-Mathf.Sin(rad), Mathf.Cos(rad), 0.0f, 0.0f));
                        m_Eroder.inputTextures["Height"] = paintContext.sourceRenderTexture;

                        var heightRT = RTUtils.GetTempHandle(RTUtils.GetDescriptorRW((int) brushBounds.width, (int) brushBounds.height, 0, RenderTextureFormat.RFloat));
                        Vector2 texelSize = new Vector2(terrain.terrainData.size.x / terrain.terrainData.heightmapResolution,
                                                    terrain.terrainData.size.z / terrain.terrainData.heightmapResolution);
                        m_Eroder.ErodeHeightmap(heightRT, terrain.terrainData.size, brushBounds, texelSize);
    
                        //Blit the result onto the new height map
                        Material mat = GetPaintMaterial();
                        var brushMask = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
                        Utility.GenerateAndSetFilterRT(commonUI, paintContext.sourceRenderTexture, brushMask, mat);
                        Vector4 brushParams = new Vector4(commonUI.brushStrength, 0.0f, 0.0f, 0.0f);
                        mat.SetTexture("_BrushTex", editContext.brushTexture);
                        mat.SetTexture("_NewHeightTex", heightRT);
                        mat.SetVector("_BrushParams", brushParams);
                
                        brushRender.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                        brushRender.RenderBrush(paintContext, mat, 0);
                        brushRender.Release(paintContext);
                        RTUtils.Release(brushMask);
                        RTUtils.Release(heightRT);
                    }
                }
            }

            return true;
        }

        //Analytics Setup
        private TerrainToolsAnalytics.IBrushParameter[] UpdateAnalyticParameters() => new TerrainToolsAnalytics.IBrushParameter[] {
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_SimulationScale.text, Value = m_Eroder.SimulationScale.value},
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_WindSpeed.text, Value = m_Eroder.m_WindSpeed.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_TimeDelta.text, Value = m_Eroder.TimeInterval.value },
            new TerrainToolsAnalytics.BrushParameter<int> { Name = Erosion.Styles.m_NumIterations.text, Value = m_Eroder.Iterations.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_SuspensionRate.text, Value = m_Eroder.SuspensionRate.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_DepositionRate.text, Value = m_Eroder.DepositionRate.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_SlopeFactor.text, Value = m_Eroder.SlopeFactor.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_FlowRate.text, Value = m_Eroder.AdvectionVelScale.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_DragCoefficient.text, Value = m_Eroder.DragCoefficient.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_ReflectionCoefficient.text, Value = m_Eroder.ReflectionCoefficient.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_DiffusionRate.text, Value = m_Eroder.DiffusionRate.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_AbrasivenessCoefficient.text, Value = m_Eroder.AbrasivenessCoefficient.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_Viscosity.text, Value = m_Eroder.Viscosity.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = "# Iterations", Value = m_Eroder.ThermalIterations },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_ThermalDTScalar.text, Value = m_Eroder.ThermalTimeDelta.value },
            new TerrainToolsAnalytics.BrushParameter<float> { Name = Erosion.Styles.m_AngleOfRepose.text, Value = m_Eroder.AngleOfRepose },

        };
    }
}
