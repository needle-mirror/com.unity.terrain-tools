using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class SmudgeHeightTool : TerrainPaintTool<SmudgeHeightTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Smudge Tool", typeof(TerrainToolShortcutContext))]
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintTool<SmudgeHeightTool>();
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
                    m_commonUI = new DefaultBrushUIGroup( "SmudgeTool" );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }

        EventType m_PreviousEvent = EventType.Ignore;
        Vector2 m_PrevBrushPos = new Vector2(0.0f, 0.0f);

        [SerializeField]
        bool m_AffectMaterials = true;
        [SerializeField]
        bool m_AffectHeight = true;

        Material m_Material = null;
        Material GetPaintMaterial()
        {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/SmudgeHeight"));
            return m_Material;
        }

        public override string GetName()
        {
            return "Transform/Smudge";
        }

        public override string GetDesc()
        {
            return "Click to Smudge the terrain height in the direction of the brush stroke.";
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

            using(IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "SmudgeHeight", editContext.brushTexture))
            {
                if(brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                {
                    PaintContext ctx = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                
                    brushRender.RenderBrushPreview(ctx, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, brushXform, TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);
                }
            }
        }

        private bool m_ShowControls = true;
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
                EditorGUILayout.BeginHorizontal("GroupBox");
                {
                    EditorGUILayout.PrefixLabel(Styles.affect);
                    m_AffectMaterials = TerrainToolGUIHelper.ToggleButton(Styles.alphamap, m_AffectMaterials);
                    m_AffectHeight = TerrainToolGUIHelper.ToggleButton(Styles.heightmap, m_AffectHeight);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSetting();
                Save(true);
            }
        }

        private void Reset()
        {
            m_AffectMaterials = true;
            m_AffectHeight = true;
         }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);
            
            if(!commonUI.allowPaint) { return true; }

            if(Event.current.type == EventType.MouseDown)
            {
                m_PrevBrushPos = editContext.uv;
                return false;
            }
            
            if (Event.current.type == EventType.MouseDrag && m_PreviousEvent == EventType.MouseDrag)
            {
                Vector2 uv = editContext.uv;

                if(commonUI.ScatterBrushStamp(ref terrain, ref uv))
                {
                    Material mat = GetPaintMaterial();
                    Vector2 smudgeDir = uv - m_PrevBrushPos;

                    BrushTransform brushXform = TerrainPaintUtility.CalculateBrushTransform(terrain, uv, commonUI.brushSize, commonUI.brushRotation);

                    Vector4 brushParams = new Vector4(commonUI.brushStrength, smudgeDir.x, smudgeDir.y, 0);
                    mat.SetTexture("_BrushTex", editContext.brushTexture);
                    mat.SetVector("_BrushParams", brushParams);

                    //smudge splat map
                    if (m_AffectMaterials)
                    {
                        for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
                        {
                            TerrainLayer layer = terrain.terrainData.terrainLayers[i];

                            if (layer == null) continue; // nothing to paint if the layer is NULL

                            PaintContext sampleContext = TerrainPaintUtility.BeginPaintTexture(terrain, brushXform.GetBrushXYBounds(), layer);

                            Vector3 brushPos = new Vector3( commonUI.raycastHitUnderCursor.point.x, 0, commonUI.raycastHitUnderCursor.point.z );
                            FilterContext fc = new FilterContext( terrain, brushPos, commonUI.brushSize, commonUI.brushRotation );
                            fc.renderTextureCollection.GatherRenderTextures(sampleContext.sourceRenderTexture.width, sampleContext.sourceRenderTexture.height);
                            RenderTexture filterMaskRT = commonUI.GetBrushMask(fc, sampleContext.sourceRenderTexture);
                            mat.SetTexture("_FilterTex", filterMaskRT);

                            Graphics.Blit(sampleContext.sourceRenderTexture, sampleContext.destinationRenderTexture, mat, 0);
                            TerrainPaintUtility.EndPaintTexture(sampleContext, "Terrain Paint - Smudge Brush (Texture)");
                        }
                    }

                    //smudge the height map
                    if (m_AffectHeight)
                    {
                        PaintContext paintContext = TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds(), 1);

                        Vector3 brushPos = new Vector3( commonUI.raycastHitUnderCursor.point.x, 0, commonUI.raycastHitUnderCursor.point.z );
                        FilterContext fc = new FilterContext( terrain, brushPos, commonUI.brushSize, commonUI.brushRotation );
                        fc.renderTextureCollection.GatherRenderTextures(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height);
                        RenderTexture filterMaskRT = commonUI.GetBrushMask(fc, paintContext.sourceRenderTexture);
                        mat.SetTexture("_FilterTex", filterMaskRT);

                        TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                        Graphics.Blit(paintContext.sourceRenderTexture, paintContext.destinationRenderTexture, mat, 0);
                        TerrainPaintUtility.EndPaintHeightmap(paintContext, "Terrain Paint - Smudge Brush (Height)");
                    }

                    m_PrevBrushPos = uv;
                }
            }
            m_PreviousEvent = Event.current.type;
            return false;
        }

        private static class Styles
        {
            public static readonly GUIContent controls = EditorGUIUtility.TrTextContent("Smudge Height Controls");
            public static readonly GUIContent alphamap = EditorGUIUtility.TrTextContent("Materials");
            public static readonly GUIContent heightmap = EditorGUIUtility.TrTextContent("Heightmap");
            public static readonly GUIContent affect = EditorGUIUtility.TrTextContent("Targets", "Determines which textures the smudge operations will be applied to");
        }

        private void SaveSetting()
        {
            EditorPrefs.SetBool("Unity.TerrainTools.Smudge.Heightmap", m_AffectHeight);
            EditorPrefs.SetBool("Unity.TerrainTools.Smudge.Materials", m_AffectMaterials);

        }

        private void LoadSettings()
        {
            m_AffectHeight = EditorPrefs.GetBool("Unity.TerrainTools.Smudge.Heightmap", true);
            m_AffectMaterials = EditorPrefs.GetBool("Unity.TerrainTools.Smudge.Materials", true);

        }
    }
}
