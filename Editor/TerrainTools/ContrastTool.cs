using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class ContrastTool : TerrainPaintTool<ContrastTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Constrast Tool", typeof(TerrainToolShortcutContext))]
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintTool<ContrastTool>();
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
                    m_commonUI = new DefaultBrushUIGroup( "ContrastTool" );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }


        [SerializeField]
        float m_FeatureSize = 25.0f;

        Material m_Material = null;
        Material GetPaintMaterial()
        {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/ContrastTool"));
            return m_Material;
        }

        public override string GetName()
        {
            return "Effects/Contrast";
        }

        public override string GetDesc()
        {
            return "Click to sharpen the terrain height.";
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

            Texture brushTexture = editContext.brushTexture;
            
            using(IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "ContrastTool", brushTexture))
            {
                if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext paintContext = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                    Material material = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();
                
                    brushRender.RenderBrushPreview(paintContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, brushXform, material, 0);

                    // draw result preview
                    {
                        ApplyBrushInternal(brushRender, paintContext, commonUI.brushStrength, brushTexture, brushXform);

                        // restore old render target
                        RenderTexture.active = paintContext.oldRenderTexture;

                        material.SetTexture("_HeightmapOrig", paintContext.sourceRenderTexture);
                        brushRender.RenderBrushPreview(paintContext, TerrainPaintUtilityEditor.BrushPreview.DestinationRenderTexture, brushXform, material, 1);
                    }
                }
            }
        }

        bool m_ShowControls = true;
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

            m_ShowControls = TerrainToolGUIHelper.DrawHeaderFoldoutForBrush(Styles.controls, m_ShowControls, () => { m_FeatureSize = 25.0f; });
            if (m_ShowControls) {
                EditorGUILayout.BeginVertical("GroupBox");
                    m_FeatureSize = EditorGUILayout.Slider(Styles.featureSize, m_FeatureSize, 1.0f, 100.0f);
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSetting();
                Save(true);
            }
        }

        public void ApplyBrushInternal(IPaintContextRender renderer, PaintContext paintContext, float brushStrength, Texture brushTexture, BrushTransform brushXform) {
            Material mat = GetPaintMaterial();
            Vector4 brushParams = new Vector4(commonUI.brushStrength, 0.0f, m_FeatureSize, 0);
            
            mat.SetTexture("_BrushTex", brushTexture);
            mat.SetVector("_BrushParams", brushParams);
            
            renderer.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
            renderer.RenderBrush(paintContext, mat, 0);
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            Texture brushTexture = editContext.brushTexture;
            
            commonUI.OnPaint(terrain, editContext);

            if(!commonUI.allowPaint) { return true; }

            using(IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "ContrastTool", brushTexture))
            {
                if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds(), 1);

                    Material mat = GetPaintMaterial();
                    Vector3 brushPos = new Vector3( commonUI.raycastHitUnderCursor.point.x, 0, commonUI.raycastHitUnderCursor.point.z );
                    FilterContext fc = new FilterContext( terrain, brushPos, commonUI.brushSize, commonUI.brushRotation );
                    fc.renderTextureCollection.GatherRenderTextures(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height);
                    RenderTexture filterMaskRT = commonUI.GetBrushMask(fc, paintContext.sourceRenderTexture);
                    mat.SetTexture("_FilterTex", filterMaskRT);

                    paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;

                    ApplyBrushInternal(brushRender, paintContext, commonUI.brushStrength, editContext.brushTexture, brushXform);
                }
            }
            return false;
        }

        private static class Styles
        {
            public static readonly GUIContent controls = EditorGUIUtility.TrTextContent("Contrast Tool Controls");
            public static readonly GUIContent featureSize = EditorGUIUtility.TrTextContent("Feature Size", "Larger value will affect larger features, smaller values will affect smaller features");
        }

        private void SaveSetting()
        {
            EditorPrefs.SetFloat("Unity.TerrainTools.Contrast.FeatureSize", m_FeatureSize);

        }

        private void LoadSettings()
        {
            m_FeatureSize = EditorPrefs.GetFloat("Unity.TerrainTools.Contrast.FeatureSize", 25.0f);

        }
    }
}
