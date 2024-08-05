using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.TerrainTools
{
    internal class SlopeFlattenTool : TerrainPaintTool<SlopeFlattenTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Slope Flatten Tool", typeof(TerrainToolShortcutContext))]
        static void SelectShortcut(ShortcutArguments args)
        {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintTool<SlopeFlattenTool>();
            TerrainToolsAnalytics.OnShortcutKeyRelease("Select Slope Flatten Tool");
        }
#endif

        [SerializeField]
        IBrushUIGroup m_commonUI;
        private IBrushUIGroup commonUI {
            get
            {
                if (m_commonUI == null)
                {
                    m_commonUI = new DefaultBrushUIGroup(
                        "SlopeFlattenTool",
                        null,
                        DefaultBrushUIGroup.Feature.All,
                        new DefaultBrushUIGroup.FeatureDefaults { Strength = 0.36f }
                        );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }


        Material m_Material = null;
        Material GetPaintMaterial()
        {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/SlopeFlatten"));
            return m_Material;
        }

        public override string GetName()
        {
            return "Effects/Slope Flatten";
        }

        public override string GetDescription()
        {
            return "Flattens areas while maintaining the average slope of the Terrain.";
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
                using (IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "SlopeFlatten", editContext.brushTexture))
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
            EditorGUI.BeginChangeCheck();

            commonUI.OnInspectorGUI(terrain, editContext);

            if (EditorGUI.EndChangeCheck())
                Save(true);
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            if (commonUI.allowPaint)
            {
                using (IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "SlopeFlatten", editContext.brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds(), 1);
                        Material mat = GetPaintMaterial();
                        var brushMask = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
                        Utility.GenerateAndSetFilterRT(commonUI, paintContext.sourceRenderTexture, brushMask, mat);
                        paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;
                        Vector4 brushParams = new Vector4(commonUI.brushStrength, 0.0f, commonUI.brushSize, 0);
                        mat.SetTexture("_BrushTex", editContext.brushTexture);
                        mat.SetVector("_BrushParams", brushParams);
                        brushRender.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                        brushRender.RenderBrush(paintContext, mat, 0);
                        RTUtils.Release(brushMask);
                    }
                }
            }
            return false;
        }
    }
}
