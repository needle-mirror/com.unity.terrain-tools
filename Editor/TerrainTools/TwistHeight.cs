using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class TwistHeightTool : TerrainPaintTool<TwistHeightTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Twist Tool", typeof(TerrainToolShortcutContext))]
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintTool<TwistHeightTool>();
        }
#endif

        private bool m_ShowControls = true;

        [SerializeField]
        IBrushUIGroup m_commonUI;
        private IBrushUIGroup commonUI
        {
            get
            {
                if( m_commonUI == null )
                {
                    m_commonUI = new DefaultBrushUIGroup( "TwistTool" );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }

        [SerializeField]
        float m_TwistAmount = 100.0f;

        [SerializeField]
        bool m_AffectMaterials = true;
        [SerializeField]
        bool m_AffectHeight = true;

        Material m_Material = null;
        Material GetPaintMaterial()
        {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/TwistHeight"));
            return m_Material;
        }

        public override string GetName()
        {
            return "Transform/Twist";
        }

        public override string GetDesc()
        {
            return "Click to Twist the terrain height.";
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

            if(commonUI.isRaycastHitUnderCursorValid)
            {
                Texture brushTexture = editContext.brushTexture;

                using(IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "Twist", editContext.brushTexture))
                {
                    //draw brush circle
                    if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        Material material = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();
                        PaintContext ctx = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                
                        brushRender.RenderBrushPreview(ctx, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, brushXform, TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);
            
                        // draw result preview
                        {
                            float finalTwistAmount = m_TwistAmount * -0.002f; //scale to a reasonable value and negate so default mode is clockwise
                            if (Event.current.shift) {
                                finalTwistAmount *= -1.0f;
                            }

                            ApplyBrushInternal(brushRender, ctx, commonUI.brushStrength, finalTwistAmount, brushTexture, brushXform);

                            // restore old render target
                            RenderTexture.active = ctx.oldRenderTexture;
                
                            material.SetTexture("_HeightmapOrig", ctx.sourceRenderTexture);
                        
                            brushRender.RenderBrushPreview(ctx, TerrainPaintUtilityEditor.BrushPreview.DestinationRenderTexture, brushXform, material, 1);
                        }
                    }
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

            m_ShowControls = TerrainToolGUIHelper.DrawHeaderFoldoutForBrush(Styles.controlHeader, m_ShowControls, Reset);

            if(m_ShowControls)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PrefixLabel(Styles.targets);
                        m_AffectMaterials = TerrainToolGUIHelper.ToggleButton(Styles.materials, m_AffectMaterials);
                        m_AffectHeight = TerrainToolGUIHelper.ToggleButton(Styles.heightmap, m_AffectHeight);
                    }
                    EditorGUILayout.EndHorizontal();

                    m_TwistAmount = EditorGUILayout.Slider(Styles.twistAmount, m_TwistAmount, -100.0f, 100.0f);
                }
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
            m_TwistAmount = 100.0f;

            m_AffectMaterials = true;
            m_AffectHeight = true;
    }

        void ApplyBrushInternal(IPaintContextRender renderer, PaintContext paintContext, float brushStrength, float finalTwistAmount, Texture brushTexture, BrushTransform brushXform) {
            Material mat = GetPaintMaterial();
            if(Event.current.control) { finalTwistAmount *= -1.0f; }
            Vector4 brushParams = new Vector4(commonUI.brushStrength, 0.0f, finalTwistAmount, 0.0f);
            
            mat.SetTexture("_BrushTex", brushTexture);
            mat.SetVector("_BrushParams", brushParams);

            renderer.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
            renderer.RenderBrush(paintContext, mat, 0);
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            if(commonUI.allowPaint)
            {
                Texture brushTexture = editContext.brushTexture;
                
                using(IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "Twist", brushTexture))
                {
                    if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        float finalTwistAmount = m_TwistAmount * -0.001f; //scale to a reasonable value and negate so default mode is clockwise
                        if(Event.current.shift)
                        {
                            finalTwistAmount *= -1.0f;
                        }
    
                        Material mat = GetPaintMaterial();
                        Vector4 brushParams = new Vector4(commonUI.brushStrength, 0.0f, finalTwistAmount, 0.0f);
                        mat.SetTexture("_BrushTex", editContext.brushTexture);
                        mat.SetVector("_BrushParams", brushParams);

                        //twist splat map
                        if (m_AffectMaterials)
                        {
                            for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
                            {
                                TerrainLayer layer = terrain.terrainData.terrainLayers[i];
                                PaintContext paintContext = brushRender.AcquireTexture(true, brushXform.GetBrushXYBounds(), layer);

                                Vector3 brushPos = new Vector3( commonUI.raycastHitUnderCursor.point.x, 0, commonUI.raycastHitUnderCursor.point.z );
                                FilterContext fc = new FilterContext( terrain, brushPos, commonUI.brushSize, commonUI.brushRotation );
                                fc.renderTextureCollection.GatherRenderTextures(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height);
                                RenderTexture filterMaskRT = commonUI.GetBrushMask(fc, paintContext.sourceRenderTexture);
                                mat.SetTexture("_FilterTex", filterMaskRT);

                                paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;

                                brushRender.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                                brushRender.RenderBrush(paintContext, mat, 0);
                                brushRender.Release(paintContext);
                            }
                        }
    
                        //twist height map
                        if(m_AffectHeight)
                        {
                            PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds(), 1);

                            Vector3 brushPos = new Vector3( commonUI.raycastHitUnderCursor.point.x, 0, commonUI.raycastHitUnderCursor.point.z );
                            FilterContext fc = new FilterContext( terrain, brushPos, commonUI.brushSize, commonUI.brushRotation );
                            fc.renderTextureCollection.GatherRenderTextures(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height);
                            RenderTexture filterMaskRT = commonUI.GetBrushMask(fc, paintContext.sourceRenderTexture);
                            mat.SetTexture("_FilterTex", filterMaskRT);

                            paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;
 
                            ApplyBrushInternal(brushRender, paintContext, commonUI.brushStrength, finalTwistAmount, brushTexture, brushXform);
                            brushRender.Release(paintContext);
                        }
                    }    
                }
            }

            return false;
        }

        private static class Styles
        {
            public static readonly GUIContent controlHeader = EditorGUIUtility.TrTextContent("Twist Height Controls");
            public static readonly GUIContent targets = EditorGUIUtility.TrTextContent("Targets", "Determines which textures the twist operations target");
            public static readonly GUIContent twistAmount = EditorGUIUtility.TrTextContent("Twist Amount", "Negative values twist clockwise, Positive values twist counter clockwise");
            public static readonly GUIContent materials = EditorGUIUtility.TrTextContent("Materials");
            public static readonly GUIContent heightmap = EditorGUIUtility.TrTextContent("Heightmap");
        }

        private void SaveSetting()
        {
            EditorPrefs.SetFloat("Unity.TerrainTools.Twist.TwistAmount", m_TwistAmount);
            EditorPrefs.SetBool("Unity.TerrainTools.Twist.Heightmap", m_AffectHeight);
            EditorPrefs.SetBool("Unity.TerrainTools.Twist.Materials", m_AffectMaterials);

        }

        private void LoadSettings()
        {
            m_TwistAmount = EditorPrefs.GetFloat("Unity.TerrainTools.Twist.TwistAmount", 100.0f);
            m_AffectHeight = EditorPrefs.GetBool("Unity.TerrainTools.Twist.Heightmap", true);
            m_AffectMaterials = EditorPrefs.GetBool("Unity.TerrainTools.Twist.Materials", true);

        }
    }
}
