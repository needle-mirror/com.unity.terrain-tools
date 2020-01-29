using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.Experimental.TerrainAPI
{
    //[FilePathAttribute("Library/TerrainTools/SetHeight", FilePathAttribute.Location.ProjectFolder)]
    internal class SetHeightTool : TerrainPaintTool<SetHeightTool>
    {
        private static bool s_showToolControls = true;

#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Set Height Tool", typeof(TerrainToolShortcutContext))]
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;
            context.SelectPaintTool<SetHeightTool>();
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
                    m_commonUI = new DefaultBrushUIGroup( "SetHeightTool" );
                    m_commonUI.OnEnterToolMode();
                }

                return m_commonUI;
            }
        }


        Material m_Material = null;
        Material GetPaintMaterial()
        {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/SetExactHeight"));
            return m_Material;
        }


        const string toolName = "Set Height";
		[SerializeField]
		float m_TargetHeight;

#if UNITY_2019_3_OR_NEWER
		private enum HeightSpace
		{
			World,
			Local
		}
		
		[SerializeField]
		HeightSpace m_HeightSpace;
#endif

		private static class Styles
        {
            public static readonly GUIContent controlHeader = EditorGUIUtility.TrTextContent("Set Height Controls");
            public static readonly GUIContent description = EditorGUIUtility.TrTextContent("Left click to set the height.\n\nHold Ctrl and left click to sample the target height.");
			public static readonly GUIContent space = EditorGUIUtility.TrTextContent("Space", "The heightmap space in which the painting operates.");
			public static readonly GUIContent flattenAll = EditorGUIUtility.TrTextContent("Flatten all", "If selected, it will traverse all neighbors and flatten them too");
            public static readonly GUIContent height = EditorGUIUtility.TrTextContent("Height", "You can set the Height property manually or you can Ctrl-click on the terrain to sample the height at the mouse position (rather like the 'eyedropper' tool in an image editor).");
            public static readonly GUIContent flatten = EditorGUIUtility.TrTextContent("Flatten Tile", "The Flatten button levels the whole terrain to the chosen height.");
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
            
            using(IBrushRenderPreviewUnderCursor brushRender = new BrushRenderPreviewUIGroupUnderCursor(commonUI, "SetHeightTool", brushTexture))
            {
                if(brushRender.CalculateBrushTransform(out BrushTransform brushTransform))
                {
                    Rect brushBounds = brushTransform.GetBrushXYBounds();
                    PaintContext paintContext = brushRender.AcquireHeightmap(false, brushBounds, 1);
                    Material material = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();

                    brushRender.RenderBrushPreview(paintContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, brushTransform, material, 0);

                    // draw result preview
                    {
                        ApplyBrushInternal(paintContext, brushRender, commonUI.brushStrength, brushTexture, brushTransform, terrain);

                        // restore old render target
                        RenderTexture.active = paintContext.oldRenderTexture;

                        material.SetTexture("_HeightmapOrig", paintContext.sourceRenderTexture);

                        brushRender.RenderBrushPreview(paintContext, TerrainPaintUtilityEditor.BrushPreview.DestinationRenderTexture, brushTransform, material, 1);
                    }
                }
            }
        }

        private void ApplyBrushInternal(PaintContext paintContext, IBrushRenderUnderCursor brushRender, float brushStrength, Texture brushTexture, BrushTransform brushTransform, Terrain terrain)
        {
            Material mat = TerrainPaintUtility.GetBuiltinPaintMaterial();
#if UNITY_2019_3_OR_NEWER
			float brushTargetHeight = Mathf.Clamp01((m_TargetHeight - paintContext.heightWorldSpaceMin) / paintContext.heightWorldSpaceSize);
            Vector4 brushParams = new Vector4(brushStrength * 0.01f, PaintContext.kNormalizedHeightScale * brushTargetHeight, 0.0f, 0.0f);
#else
			float terrainHeight = Mathf.Clamp01((m_TargetHeight - terrain.transform.position.y) / terrain.terrainData.size.y);
            Vector4 brushParams = new Vector4(brushStrength * 0.01f, 0.5f * terrainHeight, 0.0f, 0.0f);
#endif
			Vector3 brushPos = new Vector3( commonUI.raycastHitUnderCursor.point.x, 0, commonUI.raycastHitUnderCursor.point.z );
            FilterContext fc = new FilterContext( terrain, brushPos, commonUI.brushSize, commonUI.brushRotation );
            fc.renderTextureCollection.GatherRenderTextures(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height);
            RenderTexture filterMaskRT = commonUI.GetBrushMask(fc, paintContext.sourceRenderTexture);
            mat.SetTexture("_FilterTex", filterMaskRT);


            mat.SetTexture("_BrushTex", brushTexture);
            mat.SetVector("_BrushParams", brushParams);

            brushRender.SetupTerrainToolMaterialProperties(paintContext, brushTransform, mat);
            brushRender.RenderBrush(paintContext, mat, (int)TerrainPaintUtility.BuiltinPaintMaterialPasses.SetHeights);
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            commonUI.OnPaint(terrain, editContext);

            if(commonUI.allowPaint)
            {
                if(Event.current.control)
                {
                    Terrain currentTerrain = commonUI.terrainUnderCursor;
                    m_TargetHeight = currentTerrain.terrainData.GetInterpolatedHeight(editContext.uv.x, editContext.uv.y) + currentTerrain.GetPosition().y;
                    editContext.Repaint();
                    SaveSetting();
                    return true;
                }
                else
                {
                    Texture brushTexture = editContext.brushTexture;
                    
                    using(IBrushRenderUnderCursor brushRender = new BrushRenderUIGroupUnderCursor(commonUI, "SetHeightTool", brushTexture))
                    {
                        if(brushRender.CalculateBrushTransform(out BrushTransform brushTransform))
                        {
                            Rect brushBounds = brushTransform.GetBrushXYBounds();
                            PaintContext paintContext = brushRender.AcquireHeightmap(true, brushBounds);
                        
                            ApplyBrushInternal(paintContext, brushRender, commonUI.brushStrength, brushTexture, brushTransform, terrain);
                        }
                    }
                }
            }

            return true;
        }

        void Flatten(Terrain terrain)
        {
            Undo.RegisterCompleteObjectUndo(terrain.terrainData, "Set Height - Flatten Tile");

            RenderTexture heightmap = terrain.terrainData.heightmapTexture;

            Material mat = GetPaintMaterial();

            float terrainHeight = Mathf.Clamp01((m_TargetHeight - terrain.transform.position.y) / terrain.terrainData.size.y);

            Vector4 brushParams = new Vector4(0, 0.5f * terrainHeight, 0.0f, 0.0f);
            mat.SetVector("_BrushParams", brushParams);

            Vector3 brushPos = new Vector3( commonUI.raycastHitUnderCursor.point.x, 0, commonUI.raycastHitUnderCursor.point.z );
            FilterContext fc = new FilterContext( terrain, brushPos, commonUI.brushSize, commonUI.brushRotation );
            fc.renderTextureCollection.GatherRenderTextures(heightmap.width, heightmap.height);
            RenderTexture filterMaskRT = commonUI.GetBrushMask(fc, heightmap);
            mat.SetTexture("_FilterTex", filterMaskRT);
            mat.SetTexture("_MainTex", heightmap);
            RenderTexture temp = RenderTexture.GetTemporary(heightmap.descriptor);
            Graphics.Blit(heightmap, temp); // copy heightmap into temp
            
            Graphics.Blit(temp, heightmap, mat, 1);

            RenderTexture.ReleaseTemporary(temp);

            terrain.terrainData.DirtyHeightmapRegion(new RectInt(0, 0, heightmap.width, heightmap.height), TerrainHeightmapSyncControl.HeightAndLod);
            terrain.terrainData.SyncHeightmap();
        }

        void FlattenAll(Terrain terrain)
        {
            Terrain[] terrains = TerrainFillUtility.GetTerrainsInGroup(terrain);

            for(int i = 0; i < terrains.Length; ++i)
            {
                Flatten(terrains[i]);
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

            s_showToolControls = TerrainToolGUIHelper.DrawHeaderFoldoutForBrush(Styles.controlHeader, s_showToolControls, ()=> { m_TargetHeight = 0; });

            if (s_showToolControls)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                {
#if UNITY_2019_3_OR_NEWER
					EditorGUI.BeginChangeCheck();
					m_HeightSpace = (HeightSpace)EditorGUILayout.EnumPopup(Styles.space, m_HeightSpace);
					if (EditorGUI.EndChangeCheck())
					{
						if (m_HeightSpace == HeightSpace.Local)
							m_TargetHeight = Mathf.Clamp(m_TargetHeight, terrain.GetPosition().y, terrain.terrainData.size.y + terrain.GetPosition().y);
					}

					if (m_HeightSpace == HeightSpace.Local)
					{
						m_TargetHeight = EditorGUILayout.Slider(Styles.height, m_TargetHeight - terrain.GetPosition().y, 0, terrain.terrainData.size.y) + terrain.GetPosition().y;
					}
					else
					{
						m_TargetHeight = EditorGUILayout.FloatField(Styles.height, m_TargetHeight);
					}
#else
					 m_TargetHeight = EditorGUILayout.Slider(Styles.height, m_TargetHeight, 0, terrain.terrainData.size.y);
#endif
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(Styles.flatten, GUILayout.ExpandWidth(false)))
					{
						Flatten(terrain);
					}
					if (GUILayout.Button(Styles.flattenAll, GUILayout.ExpandWidth(false)))
					{
						FlattenAll(terrain);
					}
					GUILayout.EndHorizontal();

					if (EditorGUI.EndChangeCheck())
					{
						Save(true);
						SaveSetting();
					}					
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void SaveSetting()
        {
            EditorPrefs.SetFloat("Unity.TerrainTools.SetHeight.Height", m_TargetHeight);

        }

        private void LoadSettings()
        {
            m_TargetHeight = EditorPrefs.GetFloat("Unity.TerrainTools.SetHeight.Height", 0.0f);

        }
    }
}
