using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class SlopeFlattenTool : TerrainPaintTool<SlopeFlattenTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Slope Flatten Tool", typeof(TerrainToolShortcutContext))]
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintTool<SlopeFlattenTool>();
        }
#endif

        [SerializeField]
        IBrushUIGroup commonUI = new DefaultBrushUIGroup("SlopeFlatten");

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

        public override string GetDesc()
        {
            return "Flattens terrain while maintaining average slope.";
        }

        public override void OnEnterToolMode() {
            base.OnEnterToolMode();
            commonUI.OnEnterToolMode();
        }

        public override void OnExitToolMode() {
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

            // update brush UI group
            commonUI.OnSceneGUI(terrain, editContext);

            // dont render preview if this isnt a repaint. losing performance if we do
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            using(IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "SlopeFlatten", editContext.brushTexture))
            {
                if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext ctx = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                
                    brushRender.RenderBrushPreview(ctx, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, brushXform, TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);
                }
            }
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

            if (commonUI.allowPaint) {
                using(IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "SlopeFlatten", editContext.brushTexture))
                {
                    if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds(), 1);

                        paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;

                        Material mat = GetPaintMaterial();
                        Vector4 brushParams = new Vector4(commonUI.brushStrength, 0.0f, commonUI.brushSize, 0);
                        mat.SetTexture("_BrushTex", editContext.brushTexture);
                        mat.SetVector("_BrushParams", brushParams);
                    
                        brushRender.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                        brushRender.RenderBrush(paintContext, mat, 0);
                    }
                }
            }
            return false;
        }
    }
}
