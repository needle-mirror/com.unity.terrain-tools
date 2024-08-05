using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TerrainTools;
using UnityEditorInternal;

namespace UnityEditor.TerrainTools
{
    internal class DetailScatterTool : TerrainPaintTool<DetailScatterTool>
    {
        Terrain m_SelectedTerrain;
        DetailBrushRepresentation m_BrushRep;
        [NonReorderable]
        ReorderableList m_ReordableDetailsList;

        bool m_ShowDetailControls = true;
        int m_PreviouslySelectedIndex;
        int m_MouseOnPatchIndex = -1;
        
        List<DetailUIData> m_DetailDataList = new List<DetailUIData>();
        internal List<DetailUIData> detailDataList => m_DetailDataList;

        private const string k_MissingDetailPrototype = "Detail Prototype is missing.";
        private const string k_MissingDetailTexture = "Detail Texture is missing.";
        
        Material m_Material = null;
        Material scatterMaterial
        {
            get
            {
                if (m_Material == null)
                    m_Material = new Material(Shader.Find("Hidden/TerrainEngine/DetailScatter"));
                return m_Material;
            }
        }

        [SerializeField]
        IBrushUIGroup m_CommonUI;
        IBrushUIGroup commonUI
        {
            get
            {
                if (m_CommonUI == null)
                {
                    m_CommonUI = new DefaultBrushUIGroup("DetailsScatterTool", UpdateAnalyticParameters, DefaultBrushUIGroup.Feature.NoSmoothing);
                    m_CommonUI.OnEnterToolMode();
                }

                return m_CommonUI;
            }
        }

        /// <summary>
        /// Allows overriding for unit testing purposes
        /// </summary>
        /// <param name="uiGroup"></param>
        internal void ChangeCommonUI(IBrushUIGroup uiGroup)
        {
            m_CommonUI = uiGroup;
        }
        
        private class Styles
        {
            public readonly GUIContent editDetails = EditorGUIUtility.TrTextContent("Edit Details...", "Add or remove detail meshes");
            public readonly GUIContent detailControlHeader = EditorGUIUtility.TrTextContent("Paint Details Control");

            public Texture settingsIcon = EditorGUIUtility.IconContent("SettingsIcon").image;
            public GUIStyle largeSquare = new GUIStyle("Button")
            {
                fixedHeight = 22
            };
            public GUIStyle buttonIcon = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,

                normal = new GUIStyleState()
                {
                    background = null
                },
                hover = new GUIStyleState()
                {
                    background = null
                },
                stretchWidth = true
            };
            public GUIStyle prototypeLabel = new GUIStyle(GUI.skin.box);
            public GUIStyle toggleColor = new GUIStyle(GUI.skin.button);

            public enum ViewType
            {
                List, 
                Grid
            }
            public ViewType viewType = ViewType.List;
            public readonly GUIContent viewTypesLabel = EditorGUIUtility.TrTextContent("View", "Choose between two different detail control views.");
            public readonly GUIContent listViewLabel = EditorGUIUtility.TrTextContent("List", "View the detail controls in a list. Allows for greater access to parameters.");
            public readonly GUIContent gridViewLabel = EditorGUIUtility.TrTextContent("Grid", "View the detail controls in a grid. Allows for a simpler UI.");
            public readonly GUIContent detailSelectionWarning = EditorGUIUtility.TrTextContentWithIcon("Select the \"+\" button in order to add a Detail to scatter with.", MessageType.Info);
            
            //List View
            public readonly GUIContent previewLabel = EditorGUIUtility.TrTextContent("Preview", "Detail preview image");
            public readonly GUIContent targetDensityLabel = EditorGUIUtility.TrTextContent("Target Density", "Clamps the scattered density to a percentage of the Detail Density.");
            public readonly GUIContent elementPrototypeLabel = EditorGUIUtility.TrTextContent("Detail Prefab", "");
            public readonly GUIContent elementMinWidthLabel = EditorGUIUtility.TrTextContent("Min Width", "");
            public readonly GUIContent elementMaxWidthLabel = EditorGUIUtility.TrTextContent("Max Width", "");
            public readonly GUIContent elementMinHeightLabel = EditorGUIUtility.TrTextContent("Min Height", "");
            public readonly GUIContent elementMaxHeightLabel = EditorGUIUtility.TrTextContent("Max Height", "");
            public readonly GUIContent elementNoiseSeedLabel = EditorGUIUtility.TrTextContent("Noise Seed", "Specifies the random seed value for detail object placement.");
            public readonly GUIContent elementNoiseSpreadLabel = EditorGUIUtility.TrTextContent("Noise Spread", "Controls the spatial frequency of the noise pattern used to vary the scale and color of the detail objects.");
            public readonly GUIContent elementDetailDensityLabel = EditorGUIUtility.TrTextContent("Detail Density", "Controls detail density for this detail prototype, relative to it's size. Only enabled in \"Coverage\" detail scatter mode.");
            public readonly GUIContent elementAlignToGround = EditorGUIUtility.TrTextContent("Align To Ground (%)", "Rotate detail axis to ground normal direction.");
            public readonly GUIContent elementPositionJitter = EditorGUIUtility.TrTextContent("Position Jitter (%)", "Controls the randomness of the detail distribution, from ordered to random. Only available when legacy distribution in Quality Settings is turned off.");
            public readonly GUIContent elementHolePaddingLabel = EditorGUIUtility.TrTextContent("Hole Edge Padding (%)", "Controls how far away detail objects are from the edge of the hole area.\n\nSpecify this value as a percentage of the detail width, which determines the radius of the circular area around the detail object used for hole testing.");

            public readonly Color prototypeColor = new Color(0.82745f, 1.07450f, 1.23333f);

            //Distribution Slider
            public readonly GUIContent distributionLabel = EditorGUIUtility.TrTextContent("Target Coverage Distribution", "Visualizes the ratio of multiple detail's scatter target coverage.");
        }
        private static Styles s_Styles;

        internal class DetailUIData
        {
            public bool isSelected;
            public bool isSettingsExpanded;
        }

        public override string GetName()
        {
            return "Paint Details";
        }

        public override string GetDescription()
        {
            return "Paints the selected detail prototype onto the terrain";
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

            PaintDetailsToolUtility.ResetDetailsUtilityData();
        }

        public override void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext)
        {
            commonUI.OnSceneGUI(terrain, editContext);

            //Grab m_MouseOnPatchIndex here to avoid calling again in OnRenderBrushPreview
            m_MouseOnPatchIndex = PaintDetailsToolUtility.ClampedDetailPatchesGUI(terrain, out var detailMinMaxHeight, out var clampedDetailPatchIconScreenPositions);

            //Don't render the brush preview the scene isn't being repainted. There's a performance loss.
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            Texture brushTexture = editContext.brushTexture;

            using (IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "DetailScatterTool", brushTexture))
            {
                if (brushRender.CalculateBrushTransform(out BrushTransform brushTransform))
                {
                    RenderTexture tmpRT = RenderTexture.active;
                    Rect brushTransformBounds = brushTransform.GetBrushXYBounds();
                    PaintContext heightmapContext = brushRender.AcquireHeightmap(false, brushTransformBounds, 1);
                    var previewMaterial = Utility.GetDefaultPreviewMaterial(commonUI.hasEnabledFilters);

                    var texelCtx = Utility.CollectTexelValidity(heightmapContext.originTerrain, brushTransform.GetBrushXYBounds());
                    Utility.SetupMaterialForPaintingWithTexelValidityContext(heightmapContext, texelCtx, brushTransform, previewMaterial);

                    var filterRT = RTUtils.GetTempHandle(heightmapContext.sourceRenderTexture.width,
                        heightmapContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
                    Utility.GenerateAndSetFilterRT(commonUI, brushRender, filterRT, previewMaterial);

                    brushRender.RenderBrushPreview(heightmapContext, TerrainBrushPreviewMode.SourceRenderTexture, brushTransform, previewMaterial, 0);
                    texelCtx.Cleanup();
                    RTUtils.Release(filterRT);
                }
            }

            PaintDetailsToolUtility.DrawClampedDetailPatchGUI(m_MouseOnPatchIndex, clampedDetailPatchIconScreenPositions, detailMinMaxHeight, terrain, editContext);
        }

        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            if (m_CommonUI == null)
                return;

            if (s_Styles == null)
                s_Styles = new Styles();

            //Brush selector
            m_CommonUI.OnInspectorGUI(terrain, editContext);

            m_ShowDetailControls = TerrainToolGUIHelper.DrawHeaderFoldout(s_Styles.detailControlHeader, m_ShowDetailControls);
            if (m_ShowDetailControls)
            {
                DetailPrototype[] prototypes = terrain.terrainData.detailPrototypes;

                //Reset toggle list if it's not synced with the terrain
                if (m_SelectedTerrain != terrain || prototypes.Length != m_DetailDataList.Count)
                {
                    UpdateDetailUIData(terrain);
                }

                if (m_ReordableDetailsList == null)
                {
                    InitReordableLayerSelection();
                }

                DetailsControlGUI(terrain);
            }
            m_SelectedTerrain = terrain;
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);
            if(!m_CommonUI.allowPaint)
            {
                return true;
            }

            Texture2D brushTexture = editContext.brushTexture as Texture2D;
            if (brushTexture == null)
            {
                Debug.LogError("Brush texture is not a Texture2D.");
                return false;
            }

            if (m_BrushRep == null)
            {
                m_BrushRep = new DetailBrushRepresentation();
            }

            float eraseStrength = 1;
            if (Event.current != null && (Event.current.shift || Event.current.control))
            {
                eraseStrength = -eraseStrength;
            }

            //Create an array of indices that are selected
            int[] layers = m_DetailDataList.Select((v, i) => new { Value = v, Index = i })
                .Where(b => b.Value.isSelected == true)
                .Select(b => b.Index).ToArray();

            PaintTreesDetailsContext ctx = PaintTreesDetailsContext.Create(terrain, editContext.uv);
            for (int t = 0; t < ctx.neighborTerrains.Length; ++t)
            {
                TerrainData terrainData = ctx.neighborTerrains[t]?.terrainData;
                
                if (terrainData == null)
                {
                    continue;
                }

                using (IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "DetailsScatterTool", brushTexture))
                {
                    if (brushRender.CalculateBrushTransform(out BrushTransform brushTransform))
                    {
                        ApplyBrushInternal(brushTexture, brushRender);
                        PaintContext paintContext = brushRender.AcquireHeightmap(false, brushTransform.GetBrushXYBounds());
                    }
                }

                int size = (int)Mathf.Max(1.0f, m_CommonUI.brushSize * ((float)terrainData.detailResolution / terrainData.size.x));
                DetailBrushBounds brushBounds = new DetailBrushBounds(terrainData, ctx, size, t);
                if (brushBounds.bounds.height + brushBounds.bounds.width == 0)
                {
                    continue;
                }

                m_BrushRep.Update(brushTexture, size, true);

                if (eraseStrength < 0.0F && !Event.current.control) //If erasing a list of all the layers within the terrain
                {
                    layers = terrainData.GetSupportedLayers(brushBounds.min, brushBounds.bounds.size);
                }

                TerrainPaintUtilityEditor.UpdateTerrainDataUndo(terrainData, "Terrain - Detail Edit");
                
                for (int i = 0; i < layers.Length; i++)
                {
                    int layerIndex;
                    if (Event.current != null && !Event.current.shift && !Event.current.control)
                    {
                        layerIndex = PaintDetailsToolUtility.FindDetailPrototype(ctx.neighborTerrains[t], m_SelectedTerrain, layers[i]);
                        if (layerIndex == -1)
                        {
                            layerIndex = PaintDetailsToolUtility.CopyDetailPrototype(ctx.neighborTerrains[t], m_SelectedTerrain, layers[i]);
                        }
                    }
                    else
                    {
                        layerIndex = layers[i];
                    }

                    int[,] alphamap = terrainData.GetDetailLayer(brushBounds.min, brushBounds.bounds.size, layerIndex);
                    for (int y = 0; y < brushBounds.bounds.height; y++)
                    {
                        for (int x = 0; x < brushBounds.bounds.width; x++)
                        {
                            Vector2Int brushOffset = brushBounds.GetBrushOffset(x, y);
                            float opa = m_CommonUI.brushStrength * m_BrushRep.GetStrength(brushOffset.x, brushOffset.y);

                            float targetValue = Mathf.Lerp(alphamap[y, x], eraseStrength * terrainData.detailPrototypes[layerIndex].targetCoverage * terrainData.maxDetailScatterPerRes, opa);
                            alphamap[y, x] = Mathf.Min(Mathf.RoundToInt(targetValue - .5f + UnityEngine.Random.value), terrainData.maxDetailScatterPerRes);
                        }
                    }
                    terrainData.SetDetailLayer(brushBounds.min, layerIndex, alphamap);
                }
            }

            return false;
        }

        void ApplyBrushInternal(Texture2D brushTex, IBrushRenderUnderCursor brushRender)
        {
            Texture2D mask = brushTex;
            if (mask != null)
            {
                Texture2D readableTexture = null;
                if (!mask.isReadable)
                {
                    readableTexture = new Texture2D(mask.width, mask.height, mask.format, mask.mipmapCount > 1);
                    Graphics.CopyTexture(mask, readableTexture);
                    readableTexture.Apply();
                }
                else
                {
                    readableTexture = mask;
                }

                brushRender.CalculateBrushTransform(out BrushTransform brushTransform);
                PaintContext paintContext = brushRender.AcquireHeightmap(false, brushTransform.GetBrushXYBounds());

                RenderTexture renderTexture = RenderTexture.GetTemporary(readableTexture.width, readableTexture.height, 16, readableTexture.graphicsFormat);
                RenderTexture oldRT = RenderTexture.active;
                Material mat = scatterMaterial;

                var brushMask = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
                Utility.GenerateAndSetFilterRT(commonUI, brushRender, brushMask, mat);

                mat.SetTexture("_BrushTex", readableTexture);
                brushRender.SetupTerrainToolMaterialProperties(paintContext, brushTransform, mat);

                Graphics.Blit(readableTexture, renderTexture, mat, 0);
                RenderTexture.active = renderTexture;
                brushTex.ReadPixels(new Rect(0, 0, brushTex.width, brushTex.height), 0, 0);
                RenderTexture.active = oldRT;
                RenderTexture.ReleaseTemporary(renderTexture);
                RTUtils.Release(brushMask);
            }
        }

        void DetailsControlGUI(Terrain terrain)
        {
            DetailPrototype[] prototypes = terrain.terrainData.detailPrototypes;

            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Label(s_Styles.viewTypesLabel);
            if (GUILayout.Toggle(s_Styles.viewType == Styles.ViewType.List, s_Styles.listViewLabel, GUI.skin.button))
            {
                s_Styles.viewType = Styles.ViewType.List;
            }

            if (GUILayout.Toggle(s_Styles.viewType == Styles.ViewType.Grid, s_Styles.gridViewLabel, GUI.skin.button))
            {
                s_Styles.viewType = Styles.ViewType.Grid;
            }
            EditorGUILayout.EndHorizontal();

            //Show distribution slider limit warning
            if (m_DetailDataList.Count == 0)
            {
                EditorGUILayout.HelpBox(s_Styles.detailSelectionWarning);
            }

            //Multi-Detail Selection GUI
            if (s_Styles.viewType == Styles.ViewType.List)
            {
                m_ReordableDetailsList.DoLayoutList();
            }
            else
            {
                DrawGridSelection(terrain, prototypes);
            }

            //Distribution Slider
            EditorGUILayout.LabelField("Target Density Distribution");
            int[] layers = m_DetailDataList.Select((v, i) => new { Value = v, Index = i })
                        .Where(b => b.Value.isSelected == true)
                        .Select(b => b.Index).ToArray();

            var sliderBarPosition = GUILayoutUtility.GetRect(0, 30, GUILayout.ExpandWidth(true));
            var distributionElements = DistributionSliderGUI.CreateDistributionInfos(layers.Length, sliderBarPosition,
               i => GetPrototypeName(prototypes[layers[i]]),
               i => prototypes[layers[i]].targetCoverage,
               i => layers[i]);

            if (DistributionSliderGUI.DrawSlider(sliderBarPosition, distributionElements, prototypes))
            {
                m_SelectedTerrain.terrainData.detailPrototypes = prototypes; //Necessary to update the settings
                EditorUtility.SetDirty(m_SelectedTerrain);
            }

            //Show distribution slider limit warning
            if (layers.Length > DistributionSliderGUI.k_MaxDistributionSliderCount)
            {
                EditorGUILayout.HelpBox("The distribution slider only supports the first 8 prototypes ", MessageType.Info);
            }
        }

        readonly int m_DistributionPrototypeCardId = "PrototypeCardIDHash".GetHashCode();
        const int k_CardSize = 80;
        const int k_CardPreviewOffset = 5;
        void DrawGridSelection(Terrain terrain, DetailPrototype[] prototypes)
        {
            var detailIcons = PaintDetailsToolUtility.LoadDetailIcons(prototypes);

            float inspectorWidth = EditorGUIUtility.currentViewWidth;
            int prototypeCount = prototypes.Length + 1;
            int columns = (int)MathF.Floor(inspectorWidth / k_CardSize);
            int rows = (int)MathF.Ceiling((prototypeCount * k_CardSize) / inspectorWidth);
            rows = Mathf.Max(rows, rows + (prototypeCount) - (rows * columns)); //Make sure the column and row count is equivalent to the amount of prototypes being displayed

            bool finalindexReached = false;
            for (int y = 0; y < rows; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < columns; x++)
                {
                    int index = x + (y * columns);
                    if (index >= prototypes.Length)
                    {
                        finalindexReached = true;
                        break;
                    }

                    bool prevSelectedValue = m_DetailDataList[index].isSelected;
                    Rect prototypeCardRect = GUILayoutUtility.GetRect(k_CardSize, k_CardSize);
                    Rect prototypePreviewRect = new Rect(
                        prototypeCardRect.x + k_CardPreviewOffset,
                        prototypeCardRect.y + k_CardPreviewOffset,
                        k_CardSize - (k_CardPreviewOffset * 2),
                        k_CardSize - (k_CardPreviewOffset * 2));
                    Rect prototypeCheckBoxRect = new Rect(prototypePreviewRect.x + 2, prototypePreviewRect.y + 9, 0, 0);
                    Color tempColor = GUI.backgroundColor;
                    GUI.backgroundColor = m_DetailDataList[index].isSelected ? s_Styles.prototypeColor : tempColor; //new Color(0.479f, 0.708f, 0.983f, 1.0f)
                    GUI.Box(prototypeCardRect, "", GUI.skin.button);
                    GUI.backgroundColor = tempColor;
                    if(detailIcons[index]?.image != null)
                    {
                        EditorGUI.DrawPreviewTexture(prototypePreviewRect, detailIcons[index].image);
                    }
                    else
                    {
                        EditorGUI.DrawTextureAlpha(prototypePreviewRect, EditorGUIUtility.IconContent("SceneAsset Icon").image);
                    }
                    GUI.Toggle(prototypeCheckBoxRect, prevSelectedValue, String.Empty);

                    Event currentEvent = Event.current;
                    MouseClickOperations(currentEvent, terrain.terrainData, prototypeCardRect, prototypeCardRect, prototypes, detailIcons, index);

                    if (currentEvent.type == EventType.Repaint)
                    {
                        Rect toggleRect = GUILayoutUtility.GetLastRect();
                        string prototypeName = prototypes[index].usePrototypeMesh ? prototypes[index].prototype.name : prototypes[index].prototypeTexture.name;
                        GUIContent labelText = EditorGUIUtility.TrTextContent(prototypeName, prototypeName + " ");
                        EditorGUI.LabelField(new Rect(toggleRect.x, toggleRect.yMax - 22, toggleRect.width, 22), labelText, GUI.skin.box);
                    }
                }
                if(finalindexReached)
                {
                    Rect addDetailButtonRect = GUILayoutUtility.GetRect(k_CardSize, k_CardSize);
                    if (GUI.Button(addDetailButtonRect, EditorGUIUtility.IconContent("d_Toolbar Plus@2x")))
                    {
                        DisplayDetailAddMenu();
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        
        void InitReordableLayerSelection()
        {
            m_ReordableDetailsList = new ReorderableList(m_DetailDataList, typeof(DetailUIData), false, true, true, true);
            m_ReordableDetailsList.elementHeightCallback = GetElementHeight;
            m_ReordableDetailsList.drawHeaderCallback = DrawHeader;
            m_ReordableDetailsList.drawElementCallback = DrawElement;
            m_ReordableDetailsList.drawElementBackgroundCallback = DrawElementBackground;
            m_ReordableDetailsList.onAddCallback = DisplayDetailAddMenu;
            m_ReordableDetailsList.onRemoveCallback = RemoveElement;
        }

        const int k_ElementHeight = 85;
        const int k_GrassTextureElementHeight = 385;
        const int k_GPUEnabledElementHeight = 365;
        const int k_GPUDisabledElementHeight = 410;
        float GetElementHeight(int index)
        {
            if(m_SelectedTerrain == null || index >= m_SelectedTerrain.terrainData.detailPrototypes.Length)
                return 0;

            DetailPrototype prototype = m_SelectedTerrain.terrainData.detailPrototypes[index];
            if (m_DetailDataList[index].isSettingsExpanded)
            {
                if(prototype.usePrototypeMesh)
                {
                    return prototype.useInstancing ? k_GPUEnabledElementHeight : k_GPUDisabledElementHeight;
                }
                else
                {
                    return k_GrassTextureElementHeight;
                }
            }

            return k_ElementHeight;
        }

        void DisplayDetailAddMenu(ReorderableList list = null)
        {
            MenuCommand item = new MenuCommand(m_SelectedTerrain, m_PreviouslySelectedIndex);
            if (ValidateDetailTexture())
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Detail Mesh"), false, () =>
                {
                    TerrainWizard.DisplayTerrainWizard<TerrainDetailMeshWizard>("Add Detail Mesh", "Add").ResetDefaults((Terrain)item.context, -1);
                });
                menu.AddItem(new GUIContent("Grass Texture"), false, () =>
                {
                    TerrainWizard.DisplayTerrainWizard<TerrainDetailTextureWizard>("Add Grass Texture", "Add").ResetDefaults((Terrain)item.context, -1);
                });
                menu.ShowAsContext();
            }
            else
            {
                TerrainWizard.DisplayTerrainWizard<TerrainDetailMeshWizard>("Add Detail Mesh", "Add").ResetDefaults((Terrain)item.context, -1);
            }
        }

        void RemoveElement(ReorderableList list)
        {
            int[] selectedIndecies = m_DetailDataList.Select((v, i) => new { Value = v, Index = i })
                    .Where(b => b.Value.isSelected == true)
                    .Select(b => b.Index).ToArray();
            Array.Reverse(selectedIndecies);

            foreach(int index in selectedIndecies)
            {
                Undo.RegisterCompleteObjectUndo(m_SelectedTerrain.terrainData, "Remove detail object");
                m_SelectedTerrain.terrainData.RemoveDetailPrototype(index);
            }
        }

        const float k_PreviewLabelWidth = 100f;
        void DrawHeader(Rect rect)
        {
            Rect previewLabelRect = new Rect(
                rect.x,
                rect.y,
                k_PreviewLabelWidth,
                rect.height);
            GUI.Label(previewLabelRect, s_Styles.previewLabel);
        }

        const int k_ElementPadding = 4; 
        const int k_ElementPrototypeCardSize = 80;
        const int k_ElementPrototypePreviewPadding = 4;
        const int k_ElementOptionsMenuXOffest = 15;
        const int k_ElementOptionsMenuSize = 30;
        const int k_MinTargetCoverageValue = 0;
        const int k_MaxTargetCoverageValue = 100;
        Vector2 m_ElementPadding = new Vector2(8, 6);
        Texture originalPrototypeThumbnail;

        //The empty method is needed to properly draw detail elements without the selection highlight
        void DrawElement(Rect rect, int index, bool selected, bool focused) { }
        void DrawElementBackground(Rect rect, int index, bool selected, bool focused) 
        {
            if (m_SelectedTerrain == null || m_DetailDataList.Count == 0)
                return;

            TerrainData terrainData = m_SelectedTerrain.terrainData;

            //Prototype Selection
            float prototypeCardSize = k_ElementPrototypeCardSize - k_ElementPrototypePreviewPadding;
            Rect prototypeCardRect = new Rect(
                rect.x + 5,
                rect.y + 6,
                prototypeCardSize,
                prototypeCardSize);
            Rect prototypeCardPreviewRect = new Rect(
                prototypeCardRect.x + k_ElementPrototypePreviewPadding,
                prototypeCardRect.y + k_ElementPrototypePreviewPadding,
                prototypeCardRect.width - (k_ElementPrototypePreviewPadding * 2),
                prototypeCardRect.height - (k_ElementPrototypePreviewPadding * 2));
            Rect prototypeCardCheckBoxRect = new Rect(
                prototypeCardPreviewRect.x + 2,
                prototypeCardPreviewRect.y + 9,
                0,
                0);

            DetailPrototype[] prototypes = terrainData.detailPrototypes;
            if (index >= prototypes.Length)
            {
                return;
            }

            var detailIcons = PaintDetailsToolUtility.LoadDetailIcons(prototypes);

            Color tempColor = GUI.backgroundColor;
            GUI.backgroundColor = m_DetailDataList[index].isSelected ? s_Styles.prototypeColor : tempColor;
            GUI.Box(prototypeCardRect, String.Empty, GUI.skin.button);
            GUI.backgroundColor = tempColor;

            Texture thumbnail = prototypes[index].Validate() ? detailIcons[index]?.image : originalPrototypeThumbnail;
            if (thumbnail != null)
            {
                EditorGUI.DrawPreviewTexture(prototypeCardPreviewRect, thumbnail);
            }
            else
            {
                EditorGUI.DrawTextureAlpha(prototypeCardPreviewRect, EditorGUIUtility.IconContent("SceneAsset Icon").image);
            }
            GUI.Toggle(prototypeCardCheckBoxRect, m_DetailDataList[index].isSelected, String.Empty);

            Event currentEvent = Event.current;
            MouseClickOperations(currentEvent, terrainData, prototypeCardRect, rect, prototypes, detailIcons, index);

            float prototypeRectOffset = rect.width - (prototypeCardRect.width + m_ElementPadding.x + 8);

            //Prototype name
            Rect prototypeNameRect = new Rect(
                prototypeCardRect.x + prototypeCardRect.width + k_ElementPadding,
                prototypeCardRect.y - 2,
                prototypeRectOffset - k_ElementOptionsMenuXOffest,
                EditorGUIUtility.singleLineHeight);
            
            GUI.Label(prototypeNameRect, GetPrototypeName(prototypes[index]));

            //Prototype options menu
            Rect optionsMenuRect = new Rect(
                rect.xMax - ((k_ElementOptionsMenuSize / 2) + k_ElementPadding),
                prototypeNameRect.y,
                k_ElementOptionsMenuSize,
                k_ElementOptionsMenuSize);

            if (GUI.Button(optionsMenuRect, EditorGUIUtility.IconContent("d__Menu@2x"), EditorStyles.iconButton))
            {
                originalPrototypeThumbnail = detailIcons[index].image;
                DisplayPrototypeEditMenu(terrainData, prototypes, index);
            }

            //Settings - Box
            Rect settingsBoxRect = new Rect(
                prototypeNameRect.x + 2,
                prototypeNameRect.y + EditorGUIUtility.singleLineHeight + 5,
                prototypeRectOffset,
                rect.height - (prototypeNameRect.height + ((k_ElementPadding * 3) + 3)
                ));
            GUI.Box(settingsBoxRect, "", "GroupBox");

            //Settings - Foldout handle
            Rect settingsFoldoutHandle = new Rect(
                settingsBoxRect.x + 10,
                settingsBoxRect.y + 20,
                k_ElementOptionsMenuXOffest,
                k_ElementOptionsMenuXOffest
                );

            m_DetailDataList[index].isSettingsExpanded = GUI.Toggle(settingsFoldoutHandle, m_DetailDataList[index].isSettingsExpanded, GUIContent.none, EditorStyles.foldout);
            if (m_DetailDataList[index].isSettingsExpanded)
            {
                DrawDetailSettings(settingsBoxRect, settingsFoldoutHandle, prototypes, index);
            }

            //Settings - Slider
            EditorGUI.BeginChangeCheck();
            Rect settingsSliderRect = new Rect(
                settingsFoldoutHandle.x + settingsFoldoutHandle.width + k_ElementPadding,
                settingsFoldoutHandle.y - 3,
                settingsBoxRect.width - 38,
                settingsFoldoutHandle.height + 3
                );

            float targetCoverage = prototypes[index].targetCoverage * 100f;
            EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(s_Styles.targetDensityLabel).x + 3;
            targetCoverage = EditorGUI.IntSlider(settingsSliderRect, s_Styles.targetDensityLabel, (int)targetCoverage,
                k_MinTargetCoverageValue, k_MaxTargetCoverageValue);
            EditorGUIUtility.labelWidth = 0; //Reset to the default labelWidth
            prototypes[index].targetCoverage = targetCoverage / 100f;

            if (EditorGUI.EndChangeCheck())
            {
                terrainData.detailPrototypes = prototypes; //Necessary to trigger the settings to be updated
                EditorUtility.SetDirty(m_SelectedTerrain);
            }

            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, false, false, true);
        }

        void DrawDetailSettings(Rect settingsBoxRect, Rect previousReferenceRect, DetailPrototype[] prototypes, int index)
        {
            DetailPrototype prototype = prototypes[index];

            EditorGUI.BeginChangeCheck();
            GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect prototypeField, true);
            if (prototype.usePrototypeMesh)
            {
                GameObject previousPrototype = prototype.prototype;
                prototype.prototype = (GameObject)EditorGUI.ObjectField(prototypeField, s_Styles.elementPrototypeLabel, prototype.prototype, typeof(GameObject), false);

                if (!prototype.Validate(out string errorMessage))
                {
                    prototype.prototype = previousPrototype;
                    EditorUtility.DisplayDialog("Can't assign prototype", errorMessage, "OK");
                }
            }
            else
            {
                prototype.prototypeTexture = (Texture2D)EditorGUI.ObjectField(prototypeField, s_Styles.elementPrototypeLabel, prototype.prototypeTexture, typeof(GameObject), false);
            }

            GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect densityField);
            EditorGUI.BeginDisabledGroup(m_SelectedTerrain.terrainData.detailScatterMode != DetailScatterMode.CoverageMode);
            prototype.density = EditorGUI.Slider(densityField, s_Styles.elementDetailDensityLabel, prototype.density, 0, 3);
            EditorGUI.EndDisabledGroup();

            GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect alignToGroundField);
            prototype.alignToGround = EditorGUI.Slider(alignToGroundField, s_Styles.elementAlignToGround, prototype.alignToGround * 100, 0, 100) / 100;

            GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect positionJitter);
            GUI.enabled = !QualitySettings.useLegacyDetailDistribution;
            prototype.positionJitter = EditorGUI.Slider(positionJitter, s_Styles.elementPositionJitter, prototype.positionJitter * 100, 0, 100) / 100;
            GUI.enabled = true;

            GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect minWidthField);
            prototype.minWidth = Mathf.Max(0f, EditorGUI.FloatField(minWidthField, s_Styles.elementMinWidthLabel, prototype.minWidth));

            GetSettingElementRect( settingsBoxRect, previousReferenceRect, out Rect maxWidthField);
            prototype.maxWidth = Mathf.Max(prototype.minWidth, EditorGUI.FloatField(maxWidthField, s_Styles.elementMaxWidthLabel, prototype.maxWidth));

            GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect minHeightField);
            prototype.minHeight = Mathf.Max(0f, EditorGUI.FloatField(minHeightField, s_Styles.elementMinHeightLabel, prototype.minHeight));

            GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect maxHeightField);
            prototype.maxHeight = Mathf.Max(prototype.minHeight, EditorGUI.FloatField(maxHeightField, s_Styles.elementMaxHeightLabel, prototype.maxHeight));

            GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect noiseSeedField);
            prototype.noiseSeed = EditorGUI.IntField(noiseSeedField, s_Styles.elementNoiseSeedLabel, prototype.noiseSeed);

            GetSettingElementRect(settingsBoxRect, previousReferenceRect,  out Rect noiseSpreadField);
            prototype.noiseSpread = EditorGUI.FloatField(noiseSpreadField, s_Styles.elementNoiseSpreadLabel, prototype.noiseSpread);

            GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect holePaddingField);
            float holePadding = prototype.holeEdgePadding * 100;
            holePadding = EditorGUI.Slider(holePaddingField, s_Styles.elementHolePaddingLabel, holePadding, 0, 100);
            prototype.holeEdgePadding = holePadding / 100;

            if (prototype.usePrototypeMesh)
            {
                GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect renderModeField);
                if (prototype.useInstancing)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.EnumPopup(renderModeField, "Render Mode", TerrainDetailMeshRenderMode.VertexLit);
                    EditorGUI.EndDisabledGroup();

                    GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect useInstancingField);
                    prototype.useInstancing  = EditorGUI.Toggle(useInstancingField, "Use GPU Instancing", prototype.useInstancing);
                }
                else
                {
                    TerrainDetailMeshRenderMode meshRenderMode = prototype.renderMode == DetailRenderMode.Grass ? TerrainDetailMeshRenderMode.Grass : TerrainDetailMeshRenderMode.VertexLit;
                    meshRenderMode = (TerrainDetailMeshRenderMode)EditorGUI.EnumPopup(renderModeField, "Render Mode", meshRenderMode);
                    prototype.renderMode = meshRenderMode == TerrainDetailMeshRenderMode.Grass ? DetailRenderMode.Grass : DetailRenderMode.VertexLit;

                    GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect healthyColorField);
                    prototype.healthyColor = EditorGUI.ColorField(healthyColorField, "Healthy Color", prototype.healthyColor);
                    GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect dryColorField);
                    prototype.dryColor = EditorGUI.ColorField(dryColorField, "Dry Color", prototype.dryColor);

                    GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect useInstancingField);
                    prototype.useInstancing = EditorGUI.Toggle(useInstancingField, "Use GPU Instancing", prototype.useInstancing);
                }
            }
            else //Grass Texture
            {
                GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect healthyColorField);
                prototype.healthyColor = EditorGUI.ColorField(healthyColorField, "Healthy Color", prototype.healthyColor);
                GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect dryColorField);
                prototype.dryColor = EditorGUI.ColorField(dryColorField, "Dry Color", prototype.dryColor);

                GetSettingElementRect(settingsBoxRect, previousReferenceRect, out Rect useInstancingField);
                bool billboard = prototype.renderMode == DetailRenderMode.GrassBillboard;
                billboard = EditorGUI.Toggle(useInstancingField, "Billboard", billboard);
                prototype.renderMode = billboard ? DetailRenderMode.GrassBillboard : DetailRenderMode.Grass;
            }

            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedTerrain.terrainData.detailPrototypes = prototypes; //Necessary to update the settings
                EditorUtility.SetDirty(m_SelectedTerrain); 
            }
        }

        int m_ElementLevel = 1;
        void GetSettingElementRect(Rect settingsBoxRect, Rect previousReferenceRect, out Rect fieldRect, bool firstElement = false)
        {
            if(firstElement)
            {
                m_ElementLevel = 1;
            }

            fieldRect = new Rect(
                previousReferenceRect.x + k_ElementPadding,
                previousReferenceRect.y + (EditorGUIUtility.singleLineHeight * m_ElementLevel) + (k_ElementPadding * m_ElementLevel),
                settingsBoxRect.width - 24,
                EditorGUIUtility.singleLineHeight
                );

            m_ElementLevel++;
        }

        void DisplayPrototypeEditMenu(TerrainData terrainData, DetailPrototype[] prototypes, int index)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit"), false, () =>
            {
                //Taken from TerrainMenus.EditDetail(MenuCommand) 
                MenuCommand item = new MenuCommand(m_SelectedTerrain, index);

                if (prototypes[index].usePrototypeMesh)
                {
                    TerrainWizard.DisplayTerrainWizard<TerrainDetailMeshWizard>("Edit Detail Mesh", "Apply").ResetDefaults((Terrain)item.context, item.userData);
                }
                else
                {
                    TerrainWizard.DisplayTerrainWizard<TerrainDetailTextureWizard>("Edit Grass Texture", "Apply").ResetDefaults((Terrain)item.context, item.userData);
                }
            });
            menu.AddItem(new GUIContent("Remove"), false, () =>
            {

                Undo.RegisterCompleteObjectUndo(terrainData, "Remove detail object");
                terrainData.RemoveDetailPrototype(index);
            });
            menu.ShowAsContext();
        }

        void DisplayDetailAddMenu()
        {
            MenuCommand item = new MenuCommand(m_SelectedTerrain, m_PreviouslySelectedIndex);
            if (ValidateDetailTexture())
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Detail Mesh"), false, () =>
                {
                    TerrainWizard.DisplayTerrainWizard<TerrainDetailMeshWizard>("Add Detail Mesh", "Add").ResetDefaults((Terrain)item.context, -1);
                });
                menu.AddItem(new GUIContent("Grass Texture"), false, () =>
                {
                    TerrainWizard.DisplayTerrainWizard<TerrainDetailTextureWizard>("Add Grass Texture", "Add").ResetDefaults((Terrain)item.context, -1);
                });
                menu.ShowAsContext();
            }
            else
            {
                TerrainWizard.DisplayTerrainWizard<TerrainDetailMeshWizard>("Add Detail Mesh", "Add").ResetDefaults((Terrain)item.context, -1);
            }
        }

        void MouseClickOperations(Event currentEvent, TerrainData terrainData, Rect leftClickAreaRect, Rect rightClickAreaRect, DetailPrototype[] prototypes, GUIContent[] detailIcons, int index)
        {
            EventType eventType = currentEvent.GetTypeForControl(GUIUtility.GetControlID(m_DistributionPrototypeCardId, FocusType.Passive));
            if (eventType == EventType.MouseDown)
            {
                if (currentEvent.button == 0 && leftClickAreaRect.Contains(currentEvent.mousePosition)) //Left click operation
                {
                    m_DetailDataList[index].isSelected = !m_DetailDataList[index].isSelected;

                    //Shift multi-select
                    if (currentEvent.shift)
                    {
                        int start = Mathf.Min(index, m_PreviouslySelectedIndex);
                        int steps = Mathf.Abs(index - m_PreviouslySelectedIndex);
                        for (int i = start; i <= steps + start; i++)
                        {
                            m_DetailDataList[i].isSelected = m_DetailDataList[index].isSelected;
                        }
                    }
                    currentEvent.Use();
                    m_PreviouslySelectedIndex = index;
                }
                
                if (currentEvent.button == 1 && rightClickAreaRect.Contains(currentEvent.mousePosition)) //Right click operation
                {
                    originalPrototypeThumbnail = detailIcons[index].image;
                    DisplayPrototypeEditMenu(terrainData, prototypes, index);
                }
            }
        }

        bool ValidateDetailTexture() => GraphicsSettings.currentRenderPipeline == null
                || GraphicsSettings.currentRenderPipeline.terrainDetailGrassBillboardShader != null
                || GraphicsSettings.currentRenderPipeline.terrainDetailGrassShader != null;

        // Used for internal unit tests only.
        internal void SetSelectedTerrain(Terrain terrain)
        {
            m_SelectedTerrain = terrain;
        }

        internal void UpdateDetailUIData(Terrain ctxTerrain)
        {
            if (ctxTerrain == null || m_SelectedTerrain == null)
                return;

            int[] layers = m_DetailDataList.Select((v, i) => new { Value = v, Index = i })
                    .Where(b => b.Value.isSelected == true)
                    .Select(b => b.Index).ToArray();

            int oldDetailListCount = m_DetailDataList.Count;
            m_DetailDataList.Clear();
            for (int i = 0; i < ctxTerrain.terrainData.detailPrototypes.Length; i++)
            {
                m_DetailDataList.Add(
                    new DetailUIData() { 
                        isSelected = m_SelectedTerrain == ctxTerrain && i + 1 > oldDetailListCount  // Set the selection boolean to true if it's a newly added detail
                    } 
                );
            }

            for (int i = 0; i < layers.Length; i++)
            {
                int index = layers[i];

                int detailPrototype = PaintDetailsToolUtility.FindDetailPrototype(ctxTerrain, m_SelectedTerrain, index);
                if (detailPrototype != -1)
                {
                    m_DetailDataList[detailPrototype].isSelected = true;
                }
            }
        }

        //Analytics Setup
        private TerrainToolsAnalytics.IBrushParameter[] UpdateAnalyticParameters()
        {
            if (m_SelectedTerrain == null || s_Styles == null || m_DetailDataList == null || m_DetailDataList.Count == 0)
            {
                //Return an empty object for comparing data
                return new TerrainToolsAnalytics.IBrushParameter[] { };
            }
            else
            {
                return new TerrainToolsAnalytics.IBrushParameter[]
                {
                    new TerrainToolsAnalytics.BrushParameter<int>{Name = "Details Count", Value = m_DetailDataList.Count},
                    new TerrainToolsAnalytics.BrushParameter<int>{Name = "Selected Details", Value = m_DetailDataList.Count(b => b.isSelected == true)},
                    new TerrainToolsAnalytics.BrushParameter<string>{Name = "View", Value = s_Styles.viewType.ToString()},
                    new TerrainToolsAnalytics.BrushParameter<float>{Name = "Average Target Coverage", Value = m_SelectedTerrain.terrainData.detailPrototypes.Select(x => x.targetCoverage).Average()},
                };
            }
            
        }
        
        public static string GetPrototypeName(DetailPrototype prototype)
        {
            return prototype.usePrototypeMesh
                ? prototype.prototype == null
                    ? k_MissingDetailPrototype
                    : prototype.prototype.name 
                : prototype.prototypeTexture == null
                    ? k_MissingDetailTexture
                    : prototype.prototypeTexture.name;
        }
    }
}