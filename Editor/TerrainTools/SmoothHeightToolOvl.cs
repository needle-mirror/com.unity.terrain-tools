using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.TerrainTools
{
    internal class SmoothHeightToolOvl : TerrainToolsPaintTool<SmoothHeightToolOvl>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Smooth Tool", typeof(TerrainToolShortcutContext), KeyCode.F3)]
        static void SelectShortcut(ShortcutArguments args)
        {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintToolWithOverlays<SmoothHeightToolOvl>();
            TerrainToolsAnalytics.OnShortcutKeyRelease("Select Smooth Tool");
        }
#endif
        
        IBrushUIGroup commonUI {
            get
            {
                if (m_commonUI == null)
                {
                    LoadSettings();
                    m_commonUI = new DefaultBrushUIGroup(
                        "SmoothTool",
                        UpdateAnalyticParameters,
                        DefaultBrushUIGroup.Feature.All,
                        new DefaultBrushUIGroup.FeatureDefaults { Strength = 0.98f }
                    );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }

        const string k_ToolName = "Smooth Height";
        public override string OnIcon => "TerrainOverlays/Smooth_On.png";
        public override string OffIcon => "TerrainOverlays/Smooth.png";

        [SerializeField]
        public float m_direction = 0.0f;     // -1 to 1
        [SerializeField]
        public int m_KernelSize = 1; //blur kernel size

        Material m_Material = null;
        Material GetMaterial()
        {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/SmoothHeight"));
            return m_Material;
        }

        ComputeShader m_DiffusionCS = null;
        ComputeShader GetDiffusionShader()
        {
            if (m_DiffusionCS == null)
            {
                m_DiffusionCS = ComputeUtility.GetShader("Diffusion");
            }
            return m_DiffusionCS;
        }        

        public override int IconIndex
        {
            get { return (int) SculptIndex.Smooth; }
        }

        public override TerrainCategory Category
        {
            get { return TerrainCategory.Sculpt; }
        }

        public override string GetName()
        {
            return k_ToolName;
            
        }

        public override string GetDescription()
        {
            return Styles.description.text;
            
        }
        
        public override bool HasToolSettings => true;
        public override bool HasBrushFilters => true;
        public override bool HasBrushMask => true;
        public override bool HasBrushAttributes => true;

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

        bool m_ShowControls = true;

        public override void OnToolSettingsGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            m_ShowControls = TerrainToolGUIHelper.DrawHeaderFoldoutForBrush(Styles.controls, m_ShowControls, Reset);
            if (m_ShowControls)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                m_direction = EditorGUILayout.Slider(Styles.direction, m_direction, -1.0f, 1.0f);
                m_KernelSize = EditorGUILayout.IntSlider(Styles.kernelSize, m_KernelSize, 1, terrain.terrainData.heightmapResolution / 2 - 1);
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
                SaveSetting();
                TerrainToolsAnalytics.OnParameterChange();
            }
        }
        
        private void Reset()
        {
            m_direction = 0.0f;     // -1 to 1
            m_KernelSize = 1; //blur kernel size
        }

        private void ApplyBrushInternal(Terrain terrain, PaintContext paintContext, float brushStrength, Texture brushTexture, BrushTransform brushXform)
        {
            RenderTexture prev = RenderTexture.active;

            Material mat = GetMaterial();
            var brushMask = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
            Utility.GenerateAndSetFilterRT(commonUI, paintContext.sourceRenderTexture, brushMask, mat);
            Vector4 brushParams = new Vector4(Mathf.Clamp(brushStrength, 0.0f, 1.0f), 0.0f, 0.0f, 0.0f);

            mat.SetTexture("_BrushTex", brushTexture);
            mat.SetVector("_BrushParams", brushParams);
            Vector4 smoothWeights = new Vector4(
                Mathf.Clamp01(1.0f - Mathf.Abs(m_direction)),   // centered
                Mathf.Clamp01(-m_direction),                    // min
                Mathf.Clamp01(m_direction),                     // max
                0);
            mat.SetInt("_KernelSize", (int)Mathf.Max(1, m_KernelSize)); // kernel size
            mat.SetVector("_SmoothWeights", smoothWeights);

            var texelCtx = Utility.CollectTexelValidity(terrain, brushXform.GetBrushXYBounds());
            Utility.SetupMaterialForPaintingWithTexelValidityContext(paintContext, texelCtx, brushXform, mat);

            paintContext.sourceRenderTexture.wrapMode = TextureWrapMode.Clamp;
            paintContext.destinationRenderTexture.wrapMode = TextureWrapMode.Clamp;

            // Two pass blur (first horizontal, then vertical)
            var tmpRT = RTUtils.GetTempHandle(paintContext.destinationRenderTexture.descriptor);
            tmpRT.RT.wrapMode = TextureWrapMode.Clamp;
            mat.SetVector("_BlurDirection", Vector2.right);
            Graphics.Blit(paintContext.sourceRenderTexture, tmpRT, mat);
            mat.SetVector("_BlurDirection", Vector2.up);
            Graphics.Blit(tmpRT, paintContext.destinationRenderTexture, mat);

            RTUtils.Release(tmpRT);
            RTUtils.Release(brushMask);
            texelCtx.Cleanup();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(brushMask);
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
                using (IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "SmoothHeight", editContext.brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        PaintContext ctx = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                        Material previewMaterial = Utility.GetDefaultPreviewMaterial(commonUI.hasEnabledFilters);
                        previewMaterial.SetVector("_JitterOffset", Vector3.zero);

                        var texelCtx = Utility.CollectTexelValidity(ctx.originTerrain, brushXform.GetBrushXYBounds());
                        Utility.SetupMaterialForPaintingWithTexelValidityContext(ctx, texelCtx, brushXform, previewMaterial);
                        var filterRT = RTUtils.GetTempHandle(ctx.sourceRenderTexture.width, ctx.sourceRenderTexture.height,
                            0, FilterUtility.defaultFormat);
                        Utility.GenerateAndSetFilterRT(commonUI, ctx.sourceRenderTexture, filterRT, previewMaterial);
                        TerrainPaintUtilityEditor.DrawBrushPreview(ctx, TerrainBrushPreviewMode.SourceRenderTexture,
                            editContext.brushTexture, brushXform, previewMaterial, 0);

                        // draw result preview
                        {
                            ApplyBrushInternal(terrain, ctx, commonUI.brushStrength, editContext.brushTexture, brushXform);

                            // restore old render target
                            RenderTexture.active = ctx.oldRenderTexture;

                            previewMaterial.SetTexture("_HeightmapOrig", ctx.sourceRenderTexture);
                            TerrainPaintUtilityEditor.DrawBrushPreview(ctx, TerrainBrushPreviewMode.DestinationRenderTexture,
                                editContext.brushTexture, brushXform, previewMaterial, 1);
                        }

                        texelCtx.Cleanup();
                        RTUtils.Release(filterRT);
                        brushRender.Release(ctx);
                    }
                }
            }

            
            // update brush UI group
            commonUI.OnSceneGUI(terrain, editContext);

        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            if (!commonUI.allowPaint)
            { return true; }

            using (IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "SmoothHeight", editContext.brushTexture))
            {
                if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds());

                    ApplyBrushInternal(terrain, paintContext, commonUI.brushStrength, editContext.brushTexture, brushXform);
                }
            }
            return true;
        }

        private static class Styles
        {
            public static readonly GUIContent controls = EditorGUIUtility.TrTextContent("Smooth Controls");
            public static readonly GUIContent description = EditorGUIUtility.TrTextContent("Soften terrain features.");
            public static readonly GUIContent direction = EditorGUIUtility.TrTextContent("Verticality", "Blur only up (1.0), only down (-1.0) or both (0.0)");
            public static readonly GUIContent kernelSize = EditorGUIUtility.TrTextContent("Blur Radius", "Specifies the size of the blurring operation in texture space. This is used to determine the number of texels to include in the blur sample average");
        }

        private void SaveSetting()
        {
            EditorPrefs.SetFloat("Unity.TerrainTools.SmoothHeight.Verticality", m_direction);
        }

        private void LoadSettings()
        {
            m_direction = EditorPrefs.GetFloat("Unity.TerrainTools.SmoothHeight.Verticality", 0.0f);
        }

        //Analytics Setup
        private TerrainToolsAnalytics.IBrushParameter[] UpdateAnalyticParameters() => new TerrainToolsAnalytics.IBrushParameter[]{
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Styles.direction.text, Value = m_direction},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Styles.kernelSize.text, Value = m_KernelSize},
        };
    }
}
