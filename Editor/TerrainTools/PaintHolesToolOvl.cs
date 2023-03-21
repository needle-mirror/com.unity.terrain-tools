using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEditor.ShortcutManagement;

#if UNITY_2019_3_OR_NEWER
namespace UnityEditor.TerrainTools
{
    internal class PaintHolesToolOvl : TerrainToolsPaintTool<PaintHolesToolOvl>
    {

        [Shortcut("Terrain/Select Paint Holes Tool", typeof(TerrainToolShortcutContext), KeyCode.F8)]
        static void SelectShortcut(ShortcutArguments args)
        {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintToolWithOverlays<PaintHolesToolOvl>();
            TerrainToolsAnalytics.OnShortcutKeyRelease("Select Paint Holes Tool");
        }

        public override string OnIcon => "TerrainOverlays/Holes_On.png";
        public override string OffIcon => "TerrainOverlays/Holes.png";

        public override bool HasBrushMask => true;

        public override bool HasBrushAttributes => true;
        public override bool HasBrushFilters => true;
        
        IBrushUIGroup commonUI {
            get
            {
                if (m_commonUI == null)
                {
                    m_commonUI = new DefaultBrushUIGroup(
                        "PaintHoles",
                        null,
                        DefaultBrushUIGroup.Feature.All,
                        new DefaultBrushUIGroup.FeatureDefaults { Strength = 0.99f }
                        );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }
        
        public override int IconIndex
        {
            get { return (int) SculptIndex.Holes; }
        }

        public override TerrainCategory Category
        {
            get { return TerrainCategory.Sculpt; }
        }

        public override string GetName()
        { 
            return "Paint Holes"; 
        }

        public override string GetDescription()
        {
            return "Masks out areas on the Terrain.\n\n" +
                   "Hold Ctrl + Click to erase masked areas.";
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
        

        //Returns a negative value if ctrl is not held down (which will add holes), returns a positive value if ctrl is held down (which will remove holes)
        float GetBrushStrength()
        {
            //Due to the way the brush preview tool renders its threshold stripe, it's best to clamp the brushStrength to avoid ugly aliasing
            float brushStrength = Mathf.Clamp(commonUI.brushStrength, -254.0f/255.0f, 254.0f/255.0f);
            brushStrength = Event.current.control ? brushStrength : -brushStrength;
            return brushStrength;
        }

        public override void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext)
        {
            commonUI.OnSceneGUI2D(terrain, editContext);

            if (!editContext.hitValidTerrain && !commonUI.isInUse)
            {
                return;
            }
            
            // Only render preview if this is a repaint. losing performance if we do
            if (commonUI.isRaycastHitUnderCursorValid && Event.current.type == EventType.Repaint)
            {
                using (IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "PaintHoles", editContext.brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        PaintContext paintContext = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                        Material previewMaterial = Utility.GetDefaultPreviewMaterial(commonUI.hasEnabledFilters);
                        float brushStrength = GetBrushStrength();
                        previewMaterial.SetFloat("_HoleStripeThreshold", 1.0f - Mathf.Abs(brushStrength));
                        previewMaterial.SetFloat("_UseAltColor", brushStrength > 0.0f ? 1.0f : 0.0f);
                        previewMaterial.SetFloat("_IsPaintHolesTool", 1.0f);

                        var texelCtx = Utility.CollectTexelValidity(paintContext.originTerrain, brushXform.GetBrushXYBounds());

                        Utility.SetupMaterialForPaintingWithTexelValidityContext(paintContext, texelCtx, brushXform, previewMaterial);
                        var filterRT = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width,
                            paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
                        Utility.GenerateAndSetFilterRT(commonUI, paintContext.sourceRenderTexture, filterRT, previewMaterial);
                        TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, TerrainBrushPreviewMode.SourceRenderTexture,
                            editContext.brushTexture, brushXform, previewMaterial, 0);

                        texelCtx.Cleanup();
                        RTUtils.Release(filterRT);
                        brushRender.Release(paintContext);
                    }
                }
            }
            // update brush UI group
            commonUI.OnSceneGUI(terrain, editContext);
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            if (commonUI.allowPaint)
            {
                Vector2 uv = editContext.uv;

                if (commonUI.ScatterBrushStamp(ref terrain, ref uv))
                {
                    Texture brushTexture = editContext.brushTexture;

                    using (IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "PaintHoles", brushTexture))
                    {
                        Vector2 halfTexelOffset = new Vector2(0.5f / terrain.terrainData.holesResolution, 0.5f / terrain.terrainData.holesResolution);
                        BrushTransform brushXform = TerrainPaintUtility.CalculateBrushTransform(terrain, uv - halfTexelOffset, commonUI.brushSize, commonUI.brushRotation);
                        PaintContext paintContext = brushRender.AquireHolesTexture(true, brushXform.GetBrushXYBounds());
                        PaintContext paintContextHeight = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds());

                        // filter stack
                        Material mat = Utility.GetPaintHeightMaterial();
                        var brushMask = RTUtils.GetTempHandle(paintContextHeight.sourceRenderTexture.width, paintContextHeight.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
                        Utility.GenerateAndSetFilterRT(commonUI, paintContextHeight.sourceRenderTexture, brushMask, mat);

                        float brushStrength = GetBrushStrength();
                        Vector4 brushParams = new Vector4(brushStrength, 0.0f, 0.0f, 0.0f);
                        mat.SetTexture("_BrushTex", editContext.brushTexture);
                        mat.SetVector("_BrushParams", brushParams);

                        brushRender.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                        brushRender.RenderBrush(paintContext, mat, (int)TerrainBuiltinPaintMaterialPasses.PaintHoles);

                        TerrainPaintUtility.EndPaintHoles(paintContext, "Terrain Paint - Paint Holes");
                        RTUtils.Release(brushMask);
                    }
                }
            }
            return true;
        }
    }
}
#endif
