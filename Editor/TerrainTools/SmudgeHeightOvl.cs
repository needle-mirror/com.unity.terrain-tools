using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.TerrainTools
{
    internal class SmudgeHeightToolOvl : TerrainToolsPaintTool<SmudgeHeightToolOvl>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Smudge Tool", typeof(TerrainToolShortcutContext))]
        static void SelectShortcut(ShortcutArguments args)
        {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintToolWithOverlays<SmudgeHeightToolOvl>();
            TerrainToolsAnalytics.OnShortcutKeyRelease("Select Smudge Tool");
        }
#endif

        public override string OnIcon => "Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/Smudge_On.png";
        public override string OffIcon => "Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/Smudge.png";
        
        IBrushUIGroup commonUI {
            get
            {
                if (m_commonUI == null)
                {
                    LoadSettings();
                    m_commonUI = new DefaultBrushUIGroup(
                        "SmudgeTool",
                        UpdateAnalyticParameters,
                        DefaultBrushUIGroup.Feature.All,
                        new DefaultBrushUIGroup.FeatureDefaults { Strength = 0.99f }
                        );
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

        public override int IconIndex
        {
            get { return (int) ToolIndex.SculptIndex.Smudge; }
        }

        public override TerrainCategory Category
        {
           get { return TerrainCategory.Sculpt; }
        }

        public override string GetName()
        {
            return "Transform/Smudge";
            
        }

        public override string GetDescription()
        {
            return "Smears Terrain features and layers.";
            
        }
        
        public override bool HasToolSettings => true;
        public override bool HasBrushFilters => true;
        public override bool HasBrushMask => true;
        public override bool HasBrushAttributes => true;

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
                using (IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "SmudgeHeight", editContext.brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        PaintContext ctx = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                        Material previewMaterial = Utility.GetDefaultPreviewMaterial(commonUI.hasEnabledFilters);

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

        private bool m_ShowControls = true;

        public override void OnToolSettingsGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            m_ShowControls = TerrainToolGUIHelper.DrawHeaderFoldoutForBrush(Styles.controls, m_ShowControls, Reset);
            if (m_ShowControls)
            {
                EditorGUILayout.BeginHorizontal("GroupBox");
                {
                    EditorGUILayout.PrefixLabel(Styles.affect);
                    m_AffectMaterials = GUILayout.Toggle(m_AffectMaterials || !m_AffectHeight, Styles.alphamap, GUI.skin.button);
                    m_AffectHeight = GUILayout.Toggle(m_AffectHeight || !m_AffectMaterials, Styles.heightmap, GUI.skin.button);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            EditorGUI.BeginChangeCheck();

            commonUI.OnInspectorGUI(terrain, editContext);

            OnToolSettingsGUI(terrain, editContext);

            if (EditorGUI.EndChangeCheck())
            {
                SaveSetting();
                TerrainToolsAnalytics.OnParameterChange();
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

            if (!commonUI.allowPaint)
            { return true; }

            if (Event.current.type == EventType.MouseDown)
            {
                m_PrevBrushPos = editContext.uv;
                return false;
            }

            if (Event.current.type == EventType.MouseDrag && m_PreviousEvent == EventType.MouseDrag)
            {
                Vector2 uv = editContext.uv;

                if (commonUI.ScatterBrushStamp(ref terrain, ref uv))
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

                            if (layer == null)
                                continue; // nothing to paint if the layer is NULL

                            PaintContext sampleContext = TerrainPaintUtility.BeginPaintTexture(terrain, brushXform.GetBrushXYBounds(), layer);
                            var brushMask = RTUtils.GetTempHandle(sampleContext.sourceRenderTexture.width, sampleContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
                            Utility.GenerateAndSetFilterRT(commonUI, sampleContext.sourceRenderTexture, brushMask, mat);
                            TerrainPaintUtility.SetupTerrainToolMaterialProperties(sampleContext, brushXform, mat);
                            Graphics.Blit(sampleContext.sourceRenderTexture, sampleContext.destinationRenderTexture, mat, 0);
                            TerrainPaintUtility.EndPaintTexture(sampleContext, "Terrain Paint - Smudge Brush (Texture)");
                            RTUtils.Release(brushMask);
                        }
                    }

                    //smudge the height map
                    if (m_AffectHeight)
                    {
                        PaintContext paintContext = TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds(), 1);
                        var brushMask = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
                        Utility.GenerateAndSetFilterRT(commonUI, paintContext.sourceRenderTexture, brushMask, mat);
                        TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
                        Graphics.Blit(paintContext.sourceRenderTexture, paintContext.destinationRenderTexture, mat, 0);
                        TerrainPaintUtility.EndPaintHeightmap(paintContext, "Terrain Paint - Smudge Brush (Height)");
                        RTUtils.Release(brushMask);
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

        // Analytics Setup
        private TerrainToolsAnalytics.IBrushParameter[] UpdateAnalyticParameters() => new TerrainToolsAnalytics.IBrushParameter[]{
            new TerrainToolsAnalytics.BrushParameter<bool>{Name = Styles.heightmap.text, Value = m_AffectHeight},
            new TerrainToolsAnalytics.BrushParameter<bool>{Name = Styles.alphamap.text, Value = m_AffectMaterials},
        };
    }
}
