#pragma warning disable 0436

using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.Experimental.TerrainAPI
{
    internal class PaintHeightTool : TerrainPaintTool<PaintHeightTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Sculpt Tool", typeof(TerrainToolShortcutContext), KeyCode.F1)]                     // tells shortcut manager what to call the shortcut and what to pass as args
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;               // gets interface to modify state of TerrainTools
            context.SelectPaintTool<PaintHeightTool>();                                                  // set active tool
        }
#endif

        [SerializeField]
        IBrushUIGroup commonUI = new DefaultBrushUIGroup("PaintHeight");

        public override string GetName()
        {
            return "Raise or Lower Terrain";
        }

        public override string GetDesc()
        {
            return "Left Click to Raise Terrain, Hold Control + Left Click to Lower Terrain";
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

        private void ApplyBrushInternal(IPaintContextRender renderer, PaintContext paintContext, float brushStrength, Texture brushTexture, BrushTransform brushTransform)
        {
            Material mat = TerrainPaintUtility.GetBuiltinPaintMaterial();
            
            Vector4 brushParams = new Vector4(0.05f * brushStrength, 0.0f, 0.0f, 0.0f);
            mat.SetTexture("_BrushTex", brushTexture);
            mat.SetVector("_BrushParams", brushParams);

            renderer.SetupTerrainToolMaterialProperties(paintContext, brushTransform, mat);
            renderer.RenderBrush(paintContext, mat, (int)TerrainPaintUtility.BuiltinPaintMaterialPasses.RaiseLowerHeight);
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
                using(IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "PaintHeight", editContext.brushTexture))
                {
                    if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        PaintContext paintContext = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                        Material material = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();
                    
                        brushRender.RenderBrushPreview(paintContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, brushXform, material, 0);

                        // draw result preview
                        {
                            float s = Event.current.control ? -commonUI.brushStrength : commonUI.brushStrength;
                            ApplyBrushInternal(brushRender, paintContext, s, editContext.brushTexture, brushXform);

                            // restore old render target
                            RenderTexture.active = paintContext.oldRenderTexture;

                            material.SetTexture("_HeightmapOrig", paintContext.sourceRenderTexture);
                            brushRender.RenderBrushPreview(paintContext, TerrainPaintUtilityEditor.BrushPreview.DestinationRenderTexture, brushXform, material, 1);
                        }
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
                
                using(IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "PaintHeight", brushTexture))
                {
                    if(brushRender.CalculateBrushTransform(out BrushTransform brushTransform))
                    {
                        PaintContext paintContext = brushRender.AcquireHeightmap(true, brushTransform.GetBrushXYBounds());
                        float s = Event.current.control ? -commonUI.brushStrength : commonUI.brushStrength;
                    
                        ApplyBrushInternal(brushRender, paintContext, s, brushTexture, brushTransform);
                    }
                }
            }
            return true;
        }
    }
}

#pragma warning restore 0436