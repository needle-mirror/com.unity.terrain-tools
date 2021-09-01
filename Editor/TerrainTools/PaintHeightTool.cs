#pragma warning disable 0436

using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.TerrainTools
{
    internal class PaintHeightTool : TerrainPaintTool<PaintHeightTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Sculpt Tool", typeof(TerrainToolShortcutContext), KeyCode.F1)]                     // tells shortcut manager what to call the shortcut and what to pass as args
        static void SelectShortcut(ShortcutArguments args)
        {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;               // gets interface to modify state of TerrainTools
            context.SelectPaintTool<PaintHeightTool>();                                                  // set active tool
            TerrainToolsAnalytics.OnShortcutKeyRelease("Select Sculpt Tool");
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
                        "PaintHeight",
                        null,
                        DefaultBrushUIGroup.Feature.All,
                        new DefaultBrushUIGroup.FeatureDefaults { Strength = 0.26f }
                        );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }

        /// <summary>
        /// Allows overriding for unit testing purposes
        /// </summary>
        /// <param name="uiGroup"></param>
        internal void ChangeCommonUI(IBrushUIGroup uiGroup)
        {
            m_commonUI = uiGroup;
        }

        public override string GetName()
        {
            return "Raise or Lower Terrain";
        }

        public override string GetDescription()
        {
            return "Increases or decreases the Terrain height.\n\n" +
                "Hold Ctrl + Click to decrease the height.";
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

        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            commonUI.OnInspectorGUI(terrain, editContext);
        }

        private void ApplyBrushInternal(Terrain terrain, IPaintContextRender renderer, PaintContext paintContext, float brushStrength, Texture brushTexture, BrushTransform brushTransform)
        {
            Material mat = Utility.GetPaintHeightMaterial();
            var brushMask = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
            Utility.GenerateAndSetFilterRT(commonUI, paintContext.sourceRenderTexture, brushMask, mat);
            Vector4 brushParams = new Vector4(0.0135f * brushStrength, 0.0f, 0.0f, 0.0f);
            mat.SetTexture("_BrushTex", brushTexture);
            mat.SetVector("_BrushParams", brushParams);

            renderer.SetupTerrainToolMaterialProperties(paintContext, brushTransform, mat);
            renderer.RenderBrush(paintContext, mat, (int)TerrainBuiltinPaintMaterialPasses.RaiseLowerHeight);
            RTUtils.Release(brushMask);
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

            // update brush UI group
            commonUI.OnSceneGUI(terrain, editContext);

            // dont render preview if this isnt a repaint. losing performance if we do
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            if (commonUI.isRaycastHitUnderCursorValid)
            {
                using (IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "PaintHeight", editContext.brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        PaintContext paintContext = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                        Material previewMaterial = Utility.GetDefaultPreviewMaterial(commonUI.hasEnabledFilters);
                        var filterRT = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width,
                                                             paintContext.sourceRenderTexture.height,
                                                             0, FilterUtility.defaultFormat);
                        Utility.GenerateAndSetFilterRT(commonUI, paintContext.sourceRenderTexture, filterRT, previewMaterial);
                        var texelCtx = Utility.CollectTexelValidity(paintContext.originTerrain, brushXform.GetBrushXYBounds());
                        Utility.SetupMaterialForPaintingWithTexelValidityContext(paintContext, texelCtx, brushXform, previewMaterial);
                        TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, TerrainBrushPreviewMode.SourceRenderTexture,
                            editContext.brushTexture, brushXform, previewMaterial, 0);

                        // draw result preview
                        {
                            float s = Event.current.control ? -commonUI.brushStrength : commonUI.brushStrength;
                            ApplyBrushInternal(terrain, brushRender, paintContext, s, editContext.brushTexture, brushXform);

                            // restore old render target
                            RenderTexture.active = paintContext.oldRenderTexture;

                            previewMaterial.SetTexture("_HeightmapOrig", paintContext.sourceRenderTexture);
                            TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, TerrainBrushPreviewMode.DestinationRenderTexture,
                                editContext.brushTexture, brushXform, previewMaterial, 1);
                            texelCtx.Cleanup();
                        }
                        
                        RTUtils.Release(filterRT);
                    }
                }
            }
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            if (commonUI.allowPaint)
            {
                Texture brushTexture = editContext.brushTexture;

                using (IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "PaintHeight", brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushTransform))
                    {
                        PaintContext paintContext = brushRender.AcquireHeightmap(true, brushTransform.GetBrushXYBounds());
                        float s = commonUI.brushStrength;
                        if (Event.current != null && Event.current.control)
                        {
                            s = -commonUI.brushStrength;
                        }
                        ApplyBrushInternal(terrain, brushRender, paintContext, s, brushTexture, brushTransform);
                    }
                }
            }
            return true;
        }
    }
}

#pragma warning restore 0436