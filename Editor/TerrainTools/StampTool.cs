using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.TerrainTools
{
    //[FilePathAttribute("Library/TerrainTools/Stamp", FilePathAttribute.Location.ProjectFolder)]
    internal class StampTool : TerrainPaintTool<StampTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Stamp Tool", typeof(TerrainToolShortcutContext))]
        static void SelectShortcut(ShortcutArguments args)
        {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintTool<StampTool>();
            TerrainToolsAnalytics.OnShortcutKeyRelease("Select Stamp Tool");
        }
#endif

        static class Styles
        {
            public static readonly GUIContent controls = EditorGUIUtility.TrTextContent("Stamp Tool Controls");
            public static readonly GUIContent description = EditorGUIUtility.TrTextContent("Left click to stamp the brush onto the terrain.\n\nHold control and mousewheel to adjust height.");
            public static readonly GUIContent height = EditorGUIUtility.TrTextContent("Stamp Height", "You can set the Stamp Height manually or you can hold control and mouse wheel on the terrain to adjust it.");
            public static readonly GUIContent preserveDetails = EditorGUIUtility.TrTextContent("Preserve Details", "Blends between replacing and offsetting the existing heights under the stamp.");
            public static readonly GUIContent stampToolBehavior = EditorGUIUtility.TrTextContent("Behavior", "Stamping behavior.");
            public static readonly GUIContent minBehavior = EditorGUIUtility.TrTextContent("Min", "Stamp where the terrain's height is greater than the input height.");
            public static readonly GUIContent maxBehavior = EditorGUIUtility.TrTextContent("Max", "Stamp where the terrain's height is less than the input height.");
            public static readonly GUIContent setBehavior = EditorGUIUtility.TrTextContent("Set", "Stamp the terrain setting it's height to the input height");
        }

        enum StampToolBehavior
        {
            Min,
            Max,
            Set,
        }

        [System.Serializable]
        class StampToolSerializedProperties
        {
            public float stampHeight;
            public float preserveDetails;
            public StampToolBehavior behavior;
            public void SetDefaults()
            {
                stampHeight = 100.0f;
                preserveDetails = 0.0f;
                behavior = StampToolBehavior.Set;
            }
        }

        StampToolSerializedProperties stampToolProperties = new StampToolSerializedProperties();

        [SerializeField]
        IBrushUIGroup m_commonUI;
        private IBrushUIGroup commonUI {
            get
            {
                if (m_commonUI == null)
                {
                    LoadSettings();
                    m_commonUI = new DefaultBrushUIGroup("StampTool", UpdateAnalyticParameters);
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
            return "Stamp Terrain";
        }

        public override string GetDescription()
        {
            return "Left click to stamp the brush onto the terrain.\n\nHold control and scroll the mouse wheel to adjust stamp height.";
        }

        internal void SetStampHeight(float height)
        {
            stampToolProperties.SetDefaults();
            stampToolProperties.stampHeight = height;
        }

        private void ApplyBrushInternal(IPaintContextRender renderer, PaintContext paintContext, float brushStrength, Texture brushTexture, BrushTransform brushXform, Terrain terrain)
        {
            Vector3 prevHandlePosWS = commonUI.raycastHitUnderCursor.point;
            float HeightUnderCursor = terrain.SampleHeight(prevHandlePosWS)  / (terrain.terrainData.size.y * 2.0f);
            float height = stampToolProperties.stampHeight * brushStrength / (terrain.terrainData.size.y * 2.0f);

            Material mat = Utility.GetPaintHeightMaterial();
            Vector4 brushParams = new Vector4((int) this.stampToolProperties.behavior, HeightUnderCursor, height, stampToolProperties.preserveDetails);
            mat.SetTexture("_BrushTex", brushTexture);
            mat.SetVector("_BrushParams", brushParams);

            var brushMask = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
            Utility.SetFilterRT(commonUI, paintContext.sourceRenderTexture, brushMask, mat);
            renderer.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
            renderer.RenderBrush(paintContext, mat, (int)TerrainBuiltinPaintMaterialPasses.StampHeight);
            RTUtils.Release(brushMask);
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

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            // ignore mouse drags
            if (Event.current == null || Event.current.type != EventType.MouseDrag && !Event.current.shift)
            {
                Texture brushTexture = editContext.brushTexture;

                using (IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "Stamp", brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        PaintContext paintContext = brushRender.AcquireHeightmap(true, brushXform.GetBrushXYBounds());

                        ApplyBrushInternal(brushRender, paintContext, commonUI.brushStrength, brushTexture, brushXform, terrain);
                    }
                }
            }
            return true;
        }

        public override void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext)
        {
            commonUI.OnSceneGUI2D(terrain, editContext);

            if (!editContext.hitValidTerrain && !commonUI.isInUse)
            {
                return;
            }

            commonUI.OnSceneGUI(terrain, editContext);

            Event evt = Event.current;
            if (evt.control && (evt.type == EventType.ScrollWheel))
            {
                const float k_mouseWheelToHeightRatio = -0.004f;
                stampToolProperties.stampHeight += Event.current.delta.y * k_mouseWheelToHeightRatio * editContext.raycastHit.distance;
                evt.Use();
                editContext.Repaint();
                SaveSetting();
            }

            // We're only doing painting operations, early out if it's not a repaint
            if (evt.type != EventType.Repaint)
            {
                return;
            }

            if (commonUI.isRaycastHitUnderCursorValid)
            {
                Texture brushTexture = editContext.brushTexture;
                using (IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "Stamp", brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushXform))
                    {
                        PaintContext paintContext = brushRender.AcquireHeightmap(false, brushXform.GetBrushXYBounds(), 1);
                        Material previewMaterial = Utility.GetDefaultPreviewMaterial();

                        var texelCtx = Utility.CollectTexelValidity(paintContext.originTerrain, brushXform.GetBrushXYBounds());
                        Utility.SetupMaterialForPaintingWithTexelValidityContext(paintContext, texelCtx, brushXform, previewMaterial);
                        TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, TerrainBrushPreviewMode.SourceRenderTexture,
                            editContext.brushTexture, brushXform, previewMaterial, 0);

                        // draw result preview
                        {
                            ApplyBrushInternal(brushRender, paintContext, commonUI.brushStrength, brushTexture, brushXform, terrain);

                            // restore old render target
                            RenderTexture.active = paintContext.oldRenderTexture;

                            previewMaterial.SetTexture("_HeightmapOrig", paintContext.sourceRenderTexture);
                            TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, TerrainBrushPreviewMode.DestinationRenderTexture,
                                editContext.brushTexture, brushXform, previewMaterial, 1);
                        }
                        TerrainPaintUtility.ReleaseContextResources(paintContext);
                        texelCtx.Cleanup();
                    }
                }
            }
        }

        bool m_ShowControls = true;
        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            EditorGUI.BeginChangeCheck();
            commonUI.OnInspectorGUI(terrain, editContext);

            m_ShowControls = TerrainToolGUIHelper.DrawHeaderFoldoutForBrush(Styles.controls, m_ShowControls, stampToolProperties.SetDefaults);

            if (!m_ShowControls)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginVertical("GroupBox");
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(Styles.stampToolBehavior);
                    if (GUILayout.Toggle(stampToolProperties.behavior == StampToolBehavior.Min, Styles.minBehavior, GUI.skin.button))
                    {
                        stampToolProperties.behavior = StampToolBehavior.Min;
                    }

                    if (GUILayout.Toggle(stampToolProperties.behavior == StampToolBehavior.Set, Styles.setBehavior, GUI.skin.button))
                    {
                        stampToolProperties.behavior = StampToolBehavior.Set;
                    }

                    if (GUILayout.Toggle(stampToolProperties.behavior == StampToolBehavior.Max, Styles.maxBehavior, GUI.skin.button))
                    {
                        stampToolProperties.behavior = StampToolBehavior.Max;
                    }
                    EditorGUILayout.EndHorizontal();

                    stampToolProperties.stampHeight = EditorGUILayout.Slider(Styles.height, stampToolProperties.stampHeight, -terrain.terrainData.size.y, terrain.terrainData.size.y);
                    stampToolProperties.preserveDetails = EditorGUILayout.Slider(Styles.preserveDetails, stampToolProperties.preserveDetails, 0.0f, 1.0f);
                }
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSetting();
                Save(true);
                TerrainToolsAnalytics.OnParameterChange();
            }

            base.OnInspectorGUI(terrain, editContext);
        }

        private void SaveSetting()
        {
            string stampToolData = JsonUtility.ToJson(stampToolProperties);
            EditorPrefs.SetString("Unity.TerrainTools.Stamp", stampToolData);
        }

        private void LoadSettings()
        {
            string stampToolData = EditorPrefs.GetString("Unity.TerrainTools.Stamp");
            stampToolProperties.SetDefaults();
            JsonUtility.FromJsonOverwrite(stampToolData, stampToolProperties);
        }

        //Analytics Setup
        private TerrainToolsAnalytics.IBrushParameter[] UpdateAnalyticParameters() => new TerrainToolsAnalytics.IBrushParameter[]{
            new TerrainToolsAnalytics.BrushParameter<string>{Name = Styles.stampToolBehavior.text, Value = stampToolProperties.behavior.ToString()},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Styles.height.text, Value = stampToolProperties.stampHeight},
            new TerrainToolsAnalytics.BrushParameter<float>{Name = Styles.preserveDetails.text, Value = stampToolProperties.preserveDetails},
            };
    }
}
