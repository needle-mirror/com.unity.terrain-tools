using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class SharpenPeaksTool : TerrainPaintTool<SharpenPeaksTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Sharpen Peaks Tool", typeof(TerrainToolShortcutContext))]                     // tells shortcut manager what to call the shortcut and what to pass as args
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;              // gets interface to modify state of TerrainTools
            context.SelectPaintTool<SharpenPeaksTool>();                                                  // set active tool
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
                    m_commonUI = new DefaultBrushUIGroup( "SharpenPeaksTool" );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }

        [SerializeField]
        float m_ErosionStrength = 16.0f;

        [SerializeField]
        float m_MixStrength = 0.7f;

        Material m_Material = null;

        Material GetPaintMaterial()
        {
            if (m_Material == null)
				m_Material = new Material(Shader.Find("Hidden/TerrainTools/SharpenPeaks"));
            return m_Material;
        }

        public override string GetName()
        {
			return "Effects/Sharpen Peaks";
        }

        public override string GetDesc()
        {
            return "Sharpens peak features.";
        }

        public override void OnEnterToolMode() {
            base.OnEnterToolMode();
            commonUI.OnEnterToolMode();
        }

        public override void OnExitToolMode() {
            base.OnExitToolMode();
            commonUI.OnExitToolMode();
        }

        bool m_ShowControls = true;
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

            using(IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "SharpenPeak", editContext.brushTexture))
            {
                if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext ctx = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                
                    brushRender.RenderBrushPreview(ctx, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, brushXform, TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);
                }
            }
        }
        bool m_initialized = false;
        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {

            if (!m_initialized)
            {
                LoadSettings();
                m_initialized = true;
            }
            EditorGUI.BeginChangeCheck();
            
            commonUI.OnInspectorGUI(terrain, editContext);

            m_ShowControls = TerrainToolGUIHelper.DrawHeaderFoldoutForBrush(Styles.controls, m_ShowControls, () => { m_MixStrength = 0.7f; });
            if (m_ShowControls) {
                EditorGUILayout.BeginVertical("GroupBox");
                    m_MixStrength = EditorGUILayout.Slider(Styles.featureSharpness, m_MixStrength, 0, 1);
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSetting();
                Save(true);
            }
		}
		
		public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);
            if(!commonUI.allowPaint) { return true; }

            using(IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "SharpenPeak", editContext.brushTexture))
            {
                if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds(), 1);

                    Vector3 brushPos = new Vector3( commonUI.raycastHitUnderCursor.point.x, 0, commonUI.raycastHitUnderCursor.point.z );
                    FilterContext fc = new FilterContext( terrain, brushPos, commonUI.brushSize, commonUI.brushRotation );
                    fc.renderTextureCollection.GatherRenderTextures(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height);
                    RenderTexture filterMaskRT = commonUI.GetBrushMask(fc, paintContext.sourceRenderTexture);

                    Material mat = GetPaintMaterial();

                    // apply brush
                    Vector4 brushParams = new Vector4(
                        commonUI.brushStrength,
                        m_ErosionStrength,
                        m_MixStrength,
                        0.0f);

                    mat.SetTexture("_BrushTex", editContext.brushTexture);
                    mat.SetTexture("_FilterTex", filterMaskRT);
                    mat.SetVector("_BrushParams", brushParams);
                
                    brushRender.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                    brushRender.RenderBrush(paintContext, mat, 0);
                }
            }

            return false;
        }

        private static class Styles
        {
            public static readonly GUIContent controls = EditorGUIUtility.TrTextContent("Sharpen Peaks Tool Controls");
            public static readonly GUIContent featureSharpness = EditorGUIUtility.TrTextContent("Sharpness", "Values close to 1 make peaks sharper, and values closer to 0 flatten areas.");
        }

        private void SaveSetting()
        {
            EditorPrefs.SetFloat("Unity.TerrainTools.SharpenPeaks.FeatureSharpness", m_MixStrength);

        }

        private void LoadSettings()
        {
            m_MixStrength = EditorPrefs.GetFloat("Unity.TerrainTools.SharpenPeaks.FeatureSharpness", 0.7f);

        }
    }
}
