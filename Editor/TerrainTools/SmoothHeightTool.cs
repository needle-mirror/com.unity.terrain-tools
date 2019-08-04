using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.Experimental.TerrainAPI
{
    internal class SmoothHeightTool : TerrainPaintTool<SmoothHeightTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Smooth Tool", typeof(TerrainToolShortcutContext), KeyCode.F3)]
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintTool<SmoothHeightTool>();
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
                    m_commonUI = new DefaultBrushUIGroup( "SmoothTool" );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }

        const string toolName = "Smooth Height";

        [SerializeField]
        public float m_direction = 0.0f;     // -1 to 1
        [SerializeField]
        public float m_KernelSize = 1.0f; //blur kernel size

        Material m_Material = null;
        Material GetMaterial() {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/SmoothHeight"));
            return m_Material;
        }

        ComputeShader m_DiffusionCS = null;
        ComputeShader GetDiffusionShader() {
            if(m_DiffusionCS == null) {
                m_DiffusionCS = (ComputeShader)Resources.Load("Diffusion");
            }
            return m_DiffusionCS;
        }

        public override string GetName()
        {
            return toolName;
        }

        public override string GetDesc()
        {
            return Styles.description.text;
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

            m_ShowControls = TerrainToolGUIHelper.DrawHeaderFoldoutForBrush(Styles.controls, m_ShowControls, Reset);
            if (m_ShowControls)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                    m_direction = EditorGUILayout.Slider(Styles.direction, m_direction, -1.0f, 1.0f);
                    m_KernelSize = EditorGUILayout.Slider(Styles.kernelSize, m_KernelSize, 0.0f, 10.0f);
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSetting();
                Save(true);
            }
        }


        private void Reset()
        {
            m_direction = 0.0f;     // -1 to 1
            m_KernelSize = 1.0f; //blur kernel size
        }

        private void ApplyBrushInternal(Terrain terrain, IPaintContextRender renderer, PaintContext paintContext, float brushStrength, Texture brushTexture, BrushTransform brushXform)
        {
            Vector3 brushPos = new Vector3( commonUI.raycastHitUnderCursor.point.x, 0, commonUI.raycastHitUnderCursor.point.z );
            FilterContext fc = new FilterContext( terrain, brushPos, commonUI.brushSize, commonUI.brushRotation );
            fc.renderTextureCollection.GatherRenderTextures(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height);
            RenderTexture filterMaskRT = commonUI.GetBrushMask(fc, paintContext.sourceRenderTexture);

            /*
            ComputeShader cs = GetDiffusionShader();

            int kernel = cs.FindKernel("Diffuse");
            cs.SetFloat("dt", 0.1f);
            cs.SetFloat("diff", 0.01f);
            cs.SetVector("texDim", new Vector4(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0.0f, 0.0f));
            cs.SetTexture(kernel, "InputTex", paintContext.sourceRenderTexture);
            cs.SetTexture(kernel, "OutputTex", paintContext.destinationRenderTexture);
            cs.Dispatch(kernel, paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 1);
            */

            
            Material mat = GetMaterial();
            Vector4 brushParams = new Vector4(Mathf.Clamp(brushStrength, 0.0f, 1.0f), 0.0f, 0.0f, 0.0f);
            
            mat.SetTexture("_BrushTex", brushTexture);
            mat.SetTexture("_FilterTex", filterMaskRT);
            mat.SetVector("_BrushParams", brushParams);
            Vector4 smoothWeights = new Vector4(
                Mathf.Clamp01(1.0f - Mathf.Abs(m_direction)),   // centered
                Mathf.Clamp01(-m_direction),                    // min
                Mathf.Clamp01(m_direction),                     // max
                m_KernelSize);                                  // kernel size
            mat.SetVector("_SmoothWeights", smoothWeights);
            
            renderer.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);

            // Two pass blur (first horizontal, then vertical)
            RenderTexture tmpRT = RenderTexture.GetTemporary(paintContext.destinationRenderTexture.descriptor);
            Graphics.Blit(paintContext.sourceRenderTexture, tmpRT, mat, 0);
            Graphics.Blit(tmpRT, paintContext.destinationRenderTexture, mat, 1);

            RenderTexture.ReleaseTemporary(tmpRT);
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

            using(IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "SmoothHeight", editContext.brushTexture))
            {
                if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    Material material = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();
                    PaintContext ctx = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                
                    brushRender.RenderBrushPreview(ctx, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, brushXform, material, 0);

                    // draw result preview
                    {
                        ApplyBrushInternal(terrain, brushRender, ctx, commonUI.brushStrength, editContext.brushTexture, brushXform);

                        // restore old render target
                        RenderTexture.active = ctx.oldRenderTexture;

                        material.SetTexture("_HeightmapOrig", ctx.sourceRenderTexture);
                        brushRender.RenderBrushPreview(ctx, TerrainPaintUtilityEditor.BrushPreview.DestinationRenderTexture,brushXform, material, 1);
                    }
                }
            }
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);
            
            if(!commonUI.allowPaint) { return true; }

            using(IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "SmoothHeight", editContext.brushTexture))
            {
                if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds());
                
                    ApplyBrushInternal(terrain, brushRender, paintContext, commonUI.brushStrength, editContext.brushTexture, brushXform);
                }
            }
            return true;
        }

        private static class Styles
        {
            public static readonly GUIContent controls = EditorGUIUtility.TrTextContent("Smooth Controls");
            public static readonly GUIContent description = EditorGUIUtility.TrTextContent("Click to smooth the terrain height.");
            public static readonly GUIContent direction = EditorGUIUtility.TrTextContent("Verticality", "Blur only up (1.0), only down (-1.0) or both (0.0)");
            public static readonly GUIContent kernelSize = EditorGUIUtility.TrTextContent("Blur Radius", "Specifies the size of the blur kernel");
        }

        private void SaveSetting()
        {
            EditorPrefs.SetFloat("Unity.TerrainTools.SmoothHeight.Verticality", m_direction);

        }

        private void LoadSettings()
        {
            m_direction = EditorPrefs.GetFloat("Unity.TerrainTools.SmoothHeight.Verticality", 0.0f);
        }
    }
}
