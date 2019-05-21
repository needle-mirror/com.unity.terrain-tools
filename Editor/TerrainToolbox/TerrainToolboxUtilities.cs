using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

namespace UnityEditor.Experimental.TerrainAPI
{
	[Serializable]
	public class UtilitySettings : ScriptableObject
	{
		// Terrain Split
		public int TileXAxis = 2;
		public int TileZAxis = 2;
		public bool AutoUpdateSettings = true;
		public string TerrainAssetDir = "Assets/Terrain";

		// Layers
		public string PalettePath = string.Empty;
		public bool ClearExistLayers = true;
		public bool ApplyAllTerrains = true;
		public LayerViewMode SelectedViewMode = LayerViewMode.List;

		// Replace splatmap
		public Terrain SplatmapTerrain;
		public Texture2D SplatmapOld0;
		public Texture2D SplatmapNew0;
		public Texture2D SplatmapOld1;
		public Texture2D SplatmapNew1;
		public ImageFormat SelectedFormat = ImageFormat.TGA;
		public string SplatFolderPath = "Assets/Splatmaps/";

		// Export heightmaps
		public string HeightmapFolderPath = "Assets/Heightmaps/";
		public Heightmap.Format HeightFormat = Heightmap.Format.RAW;
		public Heightmap.Depth HeightmapDepth = Heightmap.Depth.Bit16;
		public ToolboxHelper.ByteOrder HeightmapByteOrder = ToolboxHelper.ByteOrder.Windows;
        public float HeightmapRemapMin = 0.0f;
        public float HeightmapRemapMax = 1.0f;
		public bool FlipVertically = false;

		// Enums
		public enum LayerViewMode { List, Preview }
		public enum ImageFormat { TGA, PNG }
		public enum SplatmapChannel { R, G, B, A }

		// GUI
		public bool ShowTerrainEdit = false;
		public bool ShowTerrainLayers = false;
		public bool ShowReplaceSplatmaps = false;
		public bool ShowExportSplatmaps = false;
		public bool ShowExportHeightmaps = false;
	}

	public class TerrainToolboxUtilities
	{
		Vector2 m_ScrollPosition = Vector2.zero;
		Vector2 m_ScrollPositionLayer = Vector2.zero;
		UtilitySettings m_Settings = ScriptableObject.CreateInstance<UtilitySettings>();

		// Splatmaps
		int m_SplatmapResolution = 0;
		Terrain[] m_SplatExportTerrains;
		// Terrain Edit
		Terrain[] m_Terrains;
		// Terrain Split
		Terrain[] m_SplitTerrains;
		// Layers
		List<Layer> m_PaletteLayers = new List<Layer>();
		ReorderableList m_LayerList;
		TerrainPalette m_SelectedLayerPalette = ScriptableObject.CreateInstance<TerrainPalette>();
		// Heightmap export
		Dictionary<string, Heightmap.Depth> m_DepthOptions = new Dictionary<string, Heightmap.Depth>()
		{
			{ "16 bit", Heightmap.Depth.Bit16 },
			{ "8 bit", Heightmap.Depth.Bit8 }			
		};
		int m_SelectedDepth = 0;

		const int kMaxLayerCount = 8; // currently support up to 8 layers with 2 splat alpha maps
		
		static class Styles
		{
			public static readonly GUIContent TerrainLayers = EditorGUIUtility.TrTextContent("Terrain Layers");
			public static readonly GUIContent ReplaceSplatmaps = EditorGUIUtility.TrTextContent("Replace Splatmaps");
			public static readonly GUIContent ExportSplatmaps = EditorGUIUtility.TrTextContent("Export Splatmaps");			
			public static readonly GUIContent ExportHeightmaps = EditorGUIUtility.TrTextContent("Export Heightmaps");
			public static readonly GUIContent TerrainEdit = EditorGUIUtility.TrTextContent("Terrain Edit");
			public static readonly GUIContent DuplicateTerrain = EditorGUIUtility.TrTextContent("Duplicate");
			public static readonly GUIContent RemoveTerrain = EditorGUIUtility.TrTextContent("Clean Remove");
			public static readonly GUIContent SplitTerrain = EditorGUIUtility.TrTextContent("Split");

			public static readonly GUIContent DuplicateTerrainBtn = EditorGUIUtility.TrTextContent("Duplicate", "Start duplicating selected terrain(s) and create new terrain data.");
			public static readonly GUIContent RemoveTerrainBtn = EditorGUIUtility.TrTextContent("Remove", "Start removing selected terrain(s) and delete terrain data asset files.");

			public static readonly GUIContent PalettePreset = EditorGUIUtility.TrTextContent("Palette Preset:", "Select or make a palette preset asset.");
			public static readonly GUIContent SavePalette = EditorGUIUtility.TrTextContent("Save", "Save the current palette asset file on disk.");
			public static readonly GUIContent SaveAsPalette = EditorGUIUtility.TrTextContent("Save As", "Save the current palette asset as a new file on disk.");
			public static readonly GUIContent RefreshPalette = EditorGUIUtility.TrTextContent("Refresh", "Load selected palette and apply to list of layers.");
			public static readonly GUIContent ViewMode = EditorGUIUtility.TrTextContent("View Mode:", "Select view mode of layer list from List view or Preview view.");
			public static readonly GUIContent ClearExistingLayers = EditorGUIUtility.TrTextContent("Clear Existing Layers", "Remove existing layers on selected terrain(s).");
			public static readonly GUIContent ApplyToAllTerrains = EditorGUIUtility.TrTextContent("All Terrains in Scene", "When unchecked only apply layer changes to selected terrain(s).");
			public static readonly GUIContent AddLayersBtn = EditorGUIUtility.TrTextContent("Add to Terrain(s)", "Start adding layers to either all or selected terrain(s).");
			public static readonly GUIContent RemoveLayersBtn = EditorGUIUtility.TrTextContent("Remove All Layers", "Start removing all layers from either all or selected terrain(s)");

			public static readonly GUIContent TerrainToReplaceSplatmap = EditorGUIUtility.TrTextContent("Terrain", "Select a terrain to replace splatmaps on.");
			public static readonly GUIContent SplatmapResolution = EditorGUIUtility.TrTextContent("Splatmap Resolution: ", "The control texture resolution setting of selected terrain.");
			public static readonly GUIContent SplatAlpha0 = EditorGUIUtility.TrTextContent("Old SplatAlpha0", "The SplatAlpha 0 texture from selected terrain.");
			public static readonly GUIContent SplatAlpha1 = EditorGUIUtility.TrTextContent("Old SplatAlpha1", "The SplatAlpha 1 texture from selected terrain.");
			public static readonly GUIContent SplatAlpha0New = EditorGUIUtility.TrTextContent("New SplatAlpha0", "Select a texture to replace the SplatAlpha 0 texture on selected terrain.");
			public static readonly GUIContent SplatAlpha1New = EditorGUIUtility.TrTextContent("New SplatAlpha1", "Select a texture to replace the SplatAlpha 1 texture on selected terrain.");
			public static readonly GUIContent ReplaceSplatmapsBtn = EditorGUIUtility.TrTextContent("Replace Splatmaps", "Replace splatmaps with new splatmaps on selected terrain.");
			public static readonly GUIContent ResetSplatmapsBtn = EditorGUIUtility.TrTextContent("Reset Splatmaps", "Clear splatmap textures on selected terrain(s).");
			public static readonly GUIContent ExportSplatmapFolderPath = EditorGUIUtility.TrTextContent("Export Folder Path", "Select or input a folder path where splatmap textures will be saved.");
			public static readonly GUIContent ExportSplatmapFormat = EditorGUIUtility.TrTextContent("Splatmap Format", "Texture format of exported splatmap(s).");
			public static readonly GUIContent ExportSplatmapsBtn = EditorGUIUtility.TrTextContent("Export Splatmaps", "Start exporting splatmaps into textures as selected format from selected terrain(s).");

			public static readonly GUIContent OriginalTerrain = EditorGUIUtility.TrTextContent("Original Terrain", "Select a terrain to split into smaller tiles.");
			public static readonly GUIContent TilesX = EditorGUIUtility.TrTextContent("Tiles X Axis", "Number of tiles along X axis.");
			public static readonly GUIContent TilesZ = EditorGUIUtility.TrTextContent("Tiles Z Axis", "Number of tiles along Z axis.");
			public static readonly GUIContent AutoUpdateSetting = EditorGUIUtility.TrTextContent("Auto Update Terrain Settings", "Automatically copy terrain settings to new tiles from original tiles upon create.");
			public static readonly GUIContent SplitTerrainBtn = EditorGUIUtility.TrTextContent("Split", "Start splitting original terrain into small tiles.");

			public static readonly GUIContent ExportHeightmapsBtn = EditorGUIUtility.TrTextContent("Export Heightmaps", "Start exporting raw heightmaps for selected terrain(s).");
			public static readonly GUIContent HeightmapSelectedFormat = EditorGUIUtility.TrTextContent("Heightmap Format", "Select the image format for exported heightmaps.");
			public static readonly GUIContent ExportHeightmapFolderPath = EditorGUIUtility.TrTextContent("Export Folder Path", "Select or input a folder path where heightmaps will be saved.");
			public static readonly GUIContent HeightmapBitDepth = EditorGUIUtility.TrTextContent("Heightmap Depth", "Select heightmap depth option from 8 bit or 16 bit.");
			public static readonly GUIContent HeightmapByteOrder = EditorGUIUtility.TrTextContent("Heightmap Byte Order", "Select heightmap byte order from Windows or Mac.");
            public static readonly GUIContent HeightmapRemap = EditorGUIUtility.TrTextContent("Levels Correction", "Remap the height range before export.");
            public static readonly GUIContent HeightmapRemapMin = EditorGUIUtility.TrTextContent("Min", "Minimum input height");
            public static readonly GUIContent HeightmapRemapMax = EditorGUIUtility.TrTextContent("Max", "Maximum input height");
            public static readonly GUIContent FlipVertically = EditorGUIUtility.TrTextContent("Flip Vertically", "Flip heights vertically when export. Enable this if using heightmap in external program like World Machine. Or use the Flip Y Axis option in World Machine instead.");

			public static readonly GUIStyle ToggleButtonStyle = "LargeButton";
			public static readonly GUIContent[] LayerViewModes =
			{
				EditorGUIUtility.TrTextContent("List", "Shows layers in an reorderable list view."),
				EditorGUIUtility.TrTextContent("Preview", "Shows layers in a thumbnail preview view.")
			};
		}

		public static void DrawSeperatorLine()
		{
			Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(12));
			rect.height = 1;
			rect.y = rect.y + 5;
			rect.x = 2;
			rect.width += 6;
			EditorGUI.DrawRect(rect, new Color(0.35f, 0.35f, 0.35f));
		}

		public void OnLoad()
		{
			if (m_Settings.PalettePath != string.Empty)
			{
				LoadPalette();
			}
		}

		public void OnGUI()
		{
			// scroll view of settings
			EditorGUIUtility.hierarchyMode = true;
			DrawSeperatorLine();
			m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

			// Terrain Edit			
			m_Settings.ShowTerrainEdit = TerrainToolGUIHelper.DrawHeaderFoldout(Styles.TerrainEdit, m_Settings.ShowTerrainEdit);
			++EditorGUI.indentLevel;
			if (m_Settings.ShowTerrainEdit)
			{
				ShowTerrainEditGUI();
			}
			--EditorGUI.indentLevel;
			DrawSeperatorLine();

			// Terrain Layers
			m_Settings.ShowTerrainLayers = TerrainToolGUIHelper.DrawHeaderFoldout(Styles.TerrainLayers, m_Settings.ShowTerrainLayers);
			++EditorGUI.indentLevel;
			if (m_Settings.ShowTerrainLayers)
			{
				ShowTerrainLayerGUI();
			}
			--EditorGUI.indentLevel;
			DrawSeperatorLine();

			// Terrain Splatmaps
			m_Settings.ShowReplaceSplatmaps = TerrainToolGUIHelper.DrawHeaderFoldout(Styles.ReplaceSplatmaps, m_Settings.ShowReplaceSplatmaps);
			++EditorGUI.indentLevel;
			if (m_Settings.ShowReplaceSplatmaps)
			{
				ShowReplaceSplatmapGUI();
			}
			--EditorGUI.indentLevel;
			DrawSeperatorLine();

			// Export Spaltmaps
			m_Settings.ShowExportSplatmaps = TerrainToolGUIHelper.DrawHeaderFoldout(Styles.ExportSplatmaps, m_Settings.ShowExportSplatmaps);
			++EditorGUI.indentLevel;
			if (m_Settings.ShowExportSplatmaps)
			{
				ShowExportSplatmapGUI();
			}
			--EditorGUI.indentLevel;
			DrawSeperatorLine();

			// Export Heightmaps
			m_Settings.ShowExportHeightmaps = TerrainToolGUIHelper.DrawHeaderFoldout(Styles.ExportHeightmaps, m_Settings.ShowExportHeightmaps);
			++EditorGUI.indentLevel;
			if (m_Settings.ShowExportHeightmaps)
			{
				ShowExportHeightmapGUI();
			}
			--EditorGUI.indentLevel;

			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
		}

		void ShowTerrainEditGUI()
		{
			// Duplicate Terrain
			EditorGUILayout.LabelField(Styles.DuplicateTerrain, EditorStyles.boldLabel);
			++EditorGUI.indentLevel;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Select terrain(s) to make a copy from with new terrain data assets: ");
			if (GUILayout.Button(Styles.DuplicateTerrain, GUILayout.Height(30), GUILayout.Width(200)))
			{
				DuplicateTerrains();
			}
			EditorGUILayout.EndHorizontal();

			// Clean Delete
			--EditorGUI.indentLevel;
			EditorGUILayout.LabelField(Styles.RemoveTerrain, EditorStyles.boldLabel);
			++EditorGUI.indentLevel;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Select terrain(s) to remove and delete associated terrain data assets: ");
			if (GUILayout.Button(Styles.RemoveTerrainBtn, GUILayout.Height(30), GUILayout.Width(200)))
			{
				RemoveTerrains();
			}
			EditorGUILayout.EndHorizontal();

			// Split Terrain
			--EditorGUI.indentLevel;
			EditorGUILayout.LabelField(Styles.SplitTerrain, EditorStyles.boldLabel);
			++EditorGUI.indentLevel;
			EditorGUILayout.LabelField("Select terrain(s) to split: ");
			m_Settings.TileXAxis = EditorGUILayout.IntField(Styles.TilesX, m_Settings.TileXAxis);
			m_Settings.TileZAxis = EditorGUILayout.IntField(Styles.TilesZ, m_Settings.TileZAxis);
			m_Settings.AutoUpdateSettings = EditorGUILayout.Toggle(Styles.AutoUpdateSetting, m_Settings.AutoUpdateSettings);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button(Styles.SplitTerrainBtn, GUILayout.Height(30), GUILayout.Width(200)))
			{
				SplitTerrains();
			}
			EditorGUILayout.EndHorizontal();
			--EditorGUI.indentLevel;
		}

		void ShowTerrainLayerGUI()
		{
			// Layer Palette preset
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Styles.PalettePreset);
			EditorGUI.BeginChangeCheck();
			m_SelectedLayerPalette = (TerrainPalette)EditorGUILayout.ObjectField(m_SelectedLayerPalette, typeof(TerrainPalette), false);
			if (EditorGUI.EndChangeCheck() && m_SelectedLayerPalette != null)
			{
				if (EditorUtility.DisplayDialog("Confirm", "Load palette from selected?", "OK", "Cancel"))
				{
					LoadPalette();
				}
			}
			if (GUILayout.Button(Styles.SavePalette))
			{
				if (GetPalette())
				{
					m_SelectedLayerPalette.PaletteLayers.Clear();
					foreach (var layer in m_PaletteLayers)
					{
						m_SelectedLayerPalette.PaletteLayers.Add(layer.AssignedLayer);
					}
					AssetDatabase.SaveAssets();
				}
			}
			if (GUILayout.Button(Styles.SaveAsPalette)) 
			{
				CreateNewPalette();
			}
			if (GUILayout.Button(Styles.RefreshPalette))
			{
				if (GetPalette())
				{
					LoadPalette();
				}
			}
			EditorGUILayout.EndHorizontal();

			// Layer View Mode
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("View Mode");
			m_Settings.SelectedViewMode = (UtilitySettings.LayerViewMode)GUILayout.Toolbar((int)m_Settings.SelectedViewMode, Styles.LayerViewModes, Styles.ToggleButtonStyle, GUI.ToolbarButtonSize.Fixed);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginVertical("Box");
			// List View
			if (m_Settings.SelectedViewMode == UtilitySettings.LayerViewMode.List)
			{
				if (m_LayerList == null)
				{
					m_LayerList = new ReorderableList(m_PaletteLayers, typeof(Layer), true, true, true, true);
				}
				m_LayerList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Material Layer Palette");
				m_LayerList.drawElementCallback = DrawLayerElement;
				m_LayerList.onAddCallback = OnAddElement;
				m_LayerList.onRemoveCallback = OnRemoveElement;
				m_LayerList.onCanAddCallback = OnCanAddElement;
				m_LayerList.DoLayoutList();
			}

			// Preview view
			if (m_Settings.SelectedViewMode == UtilitySettings.LayerViewMode.Preview)
			{				
				Texture2D[] layerIcons = new Texture2D[m_PaletteLayers.Count];				
				m_ScrollPositionLayer = EditorGUILayout.BeginScrollView(m_ScrollPositionLayer, false, false, GUILayout.MinHeight(180));
				EditorGUILayout.BeginHorizontal();
				for (int i = 0; i < m_PaletteLayers.Count; i++)
				{
					if (m_PaletteLayers[i] == null)
					{
						continue;
					}

					if (m_PaletteLayers[i].AssignedLayer == null || m_PaletteLayers[i].AssignedLayer.diffuseTexture == null)
					{
						layerIcons[i] = EditorGUIUtility.whiteTexture;
					}
					else
					{
						layerIcons[i] = AssetPreview.GetAssetPreview(m_PaletteLayers[i].AssignedLayer.diffuseTexture);
					}						

					DrawLayerIcon(layerIcons[i], i);
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndScrollView();				
			}
			EditorGUILayout.EndVertical();

			// Apply button			
			m_Settings.ClearExistLayers = EditorGUILayout.Toggle(Styles.ClearExistingLayers, m_Settings.ClearExistLayers);
			m_Settings.ApplyAllTerrains = EditorGUILayout.Toggle(Styles.ApplyToAllTerrains, m_Settings.ApplyAllTerrains);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button(Styles.AddLayersBtn, GUILayout.Height(30), GUILayout.Width(200)))
			{
				AddLayersToSelectedTerrains();
			}
			// Clear button
			if (GUILayout.Button(Styles.RemoveLayersBtn, GUILayout.Height(30), GUILayout.Width(200)))
			{
				RemoveLayersFromSelectedTerrains();
			}
			EditorGUILayout.EndHorizontal();
		}
		 
		const int kElementHeight = 16;
		const int kElementHeightPadding = 2;
		const int kElementObjectFieldWidth = 270;
		const int kElementToggleWidth = 20;

		void DrawLayerElement(Rect rect, int index, bool selected, bool focused)
		{
			rect.height = rect.height + kElementHeightPadding;
			var rectToggle = new Rect((rect.x + 2), rect.y, kElementToggleWidth, kElementHeight);
			var rectObject = new Rect((rectToggle.x + kElementToggleWidth), rect.y, kElementObjectFieldWidth, kElementHeight);
			if (m_PaletteLayers.Count > 0 && m_PaletteLayers[index] != null)
			{
				EditorGUI.BeginChangeCheck();
				List<TerrainLayer> existLayers = m_PaletteLayers.Select(l => l.AssignedLayer).ToList();
				TerrainLayer oldLayer = m_PaletteLayers[index].AssignedLayer;
				m_PaletteLayers[index].AssignedLayer = EditorGUI.ObjectField(rectObject, m_PaletteLayers[index].AssignedLayer, typeof(TerrainLayer), false) as TerrainLayer;
				if (EditorGUI.EndChangeCheck())
				{					
					if (existLayers.Contains(m_PaletteLayers[index].AssignedLayer))
					{
						EditorUtility.DisplayDialog("Error", "Layer exists. Please select a different layer.", "OK");
						m_PaletteLayers[index].AssignedLayer = oldLayer;
					}
				}				
			}
		}

		bool OnCanAddElement(ReorderableList list)
		{
			return list.count < kMaxLayerCount;
		}

		void OnAddElement(ReorderableList list)
		{
			Layer newLayer = ScriptableObject.CreateInstance<Layer>();
			newLayer.IsSelected = true;
			m_PaletteLayers.Add(newLayer);
			m_LayerList.index = m_PaletteLayers.Count - 1;
		}

		void OnRemoveElement(ReorderableList list)
		{
			m_PaletteLayers.RemoveAt(list.index);
			list.index = 0;
		}

		void ShowReplaceSplatmapGUI()
		{
			// Replace Splatmap
			EditorGUI.BeginChangeCheck();
			m_Settings.SplatmapTerrain = EditorGUILayout.ObjectField(Styles.TerrainToReplaceSplatmap, m_Settings.SplatmapTerrain, typeof(Terrain), true) as Terrain;
			if (EditorGUI.EndChangeCheck())
			{
				if (m_Settings.SplatmapTerrain != null)
				{
					TerrainData terrainData = m_Settings.SplatmapTerrain.terrainData;
					if (terrainData.alphamapTextureCount == 1)
					{
						m_Settings.SplatmapOld0 = terrainData.alphamapTextures[0];
						m_Settings.SplatmapOld1 = null;
					}
					if (terrainData.alphamapTextureCount == 2)
					{
						m_Settings.SplatmapOld0 = terrainData.alphamapTextures[0];
						m_Settings.SplatmapOld1 = terrainData.alphamapTextures[1];
					}
					m_SplatmapResolution = terrainData.alphamapResolution;
				}
				else
				{
					m_Settings.SplatmapOld0 = null;
					m_Settings.SplatmapOld1 = null;
					m_SplatmapResolution = 0;
				}
			}
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Styles.SplatmapResolution.text + m_SplatmapResolution.ToString());
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			m_Settings.SplatmapOld0 = EditorGUILayout.ObjectField(Styles.SplatAlpha0, m_Settings.SplatmapOld0, typeof(Texture2D), false) as Texture2D;
			m_Settings.SplatmapNew0 = EditorGUILayout.ObjectField(Styles.SplatAlpha0New, m_Settings.SplatmapNew0, typeof(Texture2D), false) as Texture2D;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			m_Settings.SplatmapOld1 = EditorGUILayout.ObjectField(Styles.SplatAlpha1, m_Settings.SplatmapOld1, typeof(Texture2D), false) as Texture2D;
			m_Settings.SplatmapNew1 = EditorGUILayout.ObjectField(Styles.SplatAlpha1New, m_Settings.SplatmapNew1, typeof(Texture2D), false) as Texture2D;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button(Styles.ReplaceSplatmapsBtn, GUILayout.Height(30), GUILayout.Width(200)))
			{
				ReplaceSplatmaps();
			}
			if (GUILayout.Button(Styles.ResetSplatmapsBtn, GUILayout.Height(30), GUILayout.Width(200)))
			{
				ResetSplatmaps();
			}
			EditorGUILayout.EndHorizontal();
		}

		void ShowExportSplatmapGUI()
		{
			// Export Splatmaps
			EditorGUILayout.BeginHorizontal();
			m_Settings.SplatFolderPath = EditorGUILayout.TextField(Styles.ExportSplatmapFolderPath, m_Settings.SplatFolderPath);
			if (GUILayout.Button("...", GUILayout.Width(25)))
			{
				m_Settings.SplatFolderPath = EditorUtility.OpenFolderPanel("Select a folder...", m_Settings.SplatFolderPath, "");
			}
			EditorGUILayout.EndHorizontal();
			m_Settings.SelectedFormat = (UtilitySettings.ImageFormat)EditorGUILayout.EnumPopup(Styles.ExportSplatmapFormat, m_Settings.SelectedFormat);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button(Styles.ExportSplatmapsBtn, GUILayout.Height(30), GUILayout.Width(200)))
			{
				var selectedTerrains = Selection.GetFiltered(typeof(Terrain), SelectionMode.Unfiltered);
				ExportSplatmaps(selectedTerrains);
			}
			EditorGUILayout.EndHorizontal();
		}

		void ShowExportHeightmapGUI()
		{
			EditorGUILayout.BeginHorizontal();
			m_Settings.HeightmapFolderPath = EditorGUILayout.TextField(Styles.ExportHeightmapFolderPath, m_Settings.HeightmapFolderPath);
			if (GUILayout.Button("...", GUILayout.Width(25)))
			{
				m_Settings.HeightmapFolderPath = EditorUtility.OpenFolderPanel("Select a folder...", m_Settings.HeightmapFolderPath, "");
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.LabelField("Heightmap Format: .raw");
			EditorGUILayout.BeginHorizontal();
			m_SelectedDepth = EditorGUILayout.Popup(Styles.HeightmapBitDepth, m_SelectedDepth, m_DepthOptions.Keys.ToArray());
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			m_Settings.HeightmapByteOrder = (ToolboxHelper.ByteOrder)EditorGUILayout.EnumPopup(Styles.HeightmapByteOrder, m_Settings.HeightmapByteOrder);
			EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.MinMaxSlider(Styles.HeightmapRemap, ref m_Settings.HeightmapRemapMin, ref m_Settings.HeightmapRemapMax, 0f, 1.0f);
            EditorGUILayout.LabelField(Styles.HeightmapRemapMin, GUILayout.Width(40.0f));
            m_Settings.HeightmapRemapMin = EditorGUILayout.FloatField(m_Settings.HeightmapRemapMin, GUILayout.Width(75.0f));
            EditorGUILayout.LabelField(Styles.HeightmapRemapMax, GUILayout.Width(40.0f));
            m_Settings.HeightmapRemapMax = EditorGUILayout.FloatField(m_Settings.HeightmapRemapMax, GUILayout.Width(75.0f));
            EditorGUILayout.EndHorizontal();
            m_Settings.FlipVertically = EditorGUILayout.Toggle(Styles.FlipVertically, m_Settings.FlipVertically);
			//Future to support PNG and TGA. 
			//m_Settings.HeightFormat = (Heightmap.Format)EditorGUILayout.EnumPopup(Styles.HeightmapSelectedFormat, m_Settings.HeightFormat);
			//if (m_Settings.HeightFormat == Heightmap.Format.RAW)
			//{
			//	EditorGUILayout.BeginHorizontal();
			//	m_Settings.HeightmapDepth = (Heightmap.Depth)EditorGUILayout.EnumPopup(Styles.HeightmapBitDepth, m_Settings.HeightmapDepth);
			//	EditorGUILayout.EndHorizontal();
			//	EditorGUILayout.BeginHorizontal();
			//	m_Settings.HeightmapByteOrder = (ToolboxHelper.ByteOrder)EditorGUILayout.EnumPopup(Styles.HeightmapByteOrder, m_Settings.HeightmapByteOrder);
			//	EditorGUILayout.EndHorizontal();
			//}			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button(Styles.ExportHeightmapsBtn, GUILayout.Height(30), GUILayout.Width(200)))
			{
				var selectedTerrains = Selection.GetFiltered(typeof(Terrain), SelectionMode.Unfiltered);
				ExportHeightmaps(selectedTerrains);
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();
		}

		void DrawLayerIcon(Texture icon, int index)
		{
			if (icon == null)
				return;

			int width = icon.width;
			Rect position = new Rect(0, width * index, width, width);
			int size = Mathf.Min((int)position.width, (int)position.height);
			if (size >= icon.width * 2)
				size = icon.width * 2;

			FilterMode filterMode = icon.filterMode;
			icon.filterMode = FilterMode.Point;
			EditorGUILayout.BeginVertical("Box", GUILayout.Width(140));
			GUILayout.Label(icon);
			
			if (m_PaletteLayers[index] != null && m_PaletteLayers[index].AssignedLayer != null)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(m_PaletteLayers[index].AssignedLayer.name, GUILayout.Width(90));
				GUILayout.EndHorizontal();
			}			
			
			EditorGUILayout.EndVertical();
			icon.filterMode = filterMode;
		}

		void AddLayersToSelectedTerrains()
		{
			Terrain[] terrains;
			if (m_Settings.ApplyAllTerrains)
			{
				terrains = ToolboxHelper.GetAllTerrainsInScene();
			}
			else
			{
				terrains = ToolboxHelper.GetSelectedTerrainsInScene();
			}
			
			if (terrains == null || terrains.Length == 0)
			{
				EditorUtility.DisplayDialog("Warning", "No selected terrain found. Please select to continue.", "OK");
				return;
			}

			int index = 0;
			if (terrains.Length > 0 && m_PaletteLayers.Count > 0)
			{
				foreach (var terrain in terrains)
				{
					if (!terrain || !terrain.terrainData)
					{
						continue;
					}

					EditorUtility.DisplayProgressBar("Applying terrain layers", string.Format("Updating terrain tile ({0})", terrain.name), ((float)index / (terrains.Count())));
					TerrainToolboxLayer.AddLayersToTerrain(terrain.terrainData, m_PaletteLayers.Select(l => l.AssignedLayer).ToList(), m_Settings.ClearExistLayers);

					index++;
				}

				AssetDatabase.SaveAssets();
				EditorUtility.ClearProgressBar();
			}
		}

		void RemoveLayersFromSelectedTerrains()
		{
			Terrain[] terrains;
			if (m_Settings.ApplyAllTerrains)
			{
				terrains = ToolboxHelper.GetAllTerrainsInScene();
			}
			else
			{
				terrains = ToolboxHelper.GetSelectedTerrainsInScene();
			}

			if (terrains == null || terrains.Length == 0)
			{
				EditorUtility.DisplayDialog("Warning", "No selected terrain found. Please select to continue.", "OK");
				return;
			}

			if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to remove all existing layers from terrain(s)?", "Continue", "Cancel"))
			{
				int index = 0;
				if (terrains.Length > 0)
				{
					foreach (var terrain in terrains)
					{
						EditorUtility.DisplayProgressBar("Removing terrain layers", string.Format("Updating terrain tile ({0})", terrain.name), ((float)index / (terrains.Count())));
						if (!terrain || !terrain.terrainData)
						{
							continue;
						}

						var layers = terrain.terrainData.terrainLayers;
						if (layers == null || layers.Length == 0)
						{
							continue;
						}

						TerrainToolboxLayer.RemoveAllLayers(terrain.terrainData);
						index++;
					}

					AssetDatabase.SaveAssets();
					EditorUtility.ClearProgressBar();
				}			
			}
		}

		void DuplicateTerrains()
		{
			m_Terrains = ToolboxHelper.GetSelectedTerrainsInScene();

			if (m_Terrains == null || m_Terrains.Length == 0)
			{
				EditorUtility.DisplayDialog("Error", "No terrain selected. Please select and try again.", "OK");
				return;
			}

			foreach (var terrain in m_Terrains)
			{
				// copy terrain data asset to be the new terrain data asset
				var dataPath = AssetDatabase.GetAssetPath(terrain.terrainData);
				var dataPathNew = AssetDatabase.GenerateUniqueAssetPath(dataPath);
				AssetDatabase.CopyAsset(dataPath, dataPathNew);
				TerrainData terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(dataPathNew);				
				// clone terrain from old terrain
				GameObject newGO = UnityEngine.Object.Instantiate(terrain.gameObject);
				newGO.transform.localPosition = terrain.gameObject.transform.position;
				newGO.GetComponent<Terrain>().terrainData = terrainData;
				// parent to parent if any
				if (terrain.gameObject.transform.parent != null)
				{
					newGO.transform.SetParent(terrain.gameObject.transform.parent);
				}				
				// update terrain data reference in terrain collider 
				TerrainCollider collider = newGO.GetComponent<TerrainCollider>();
				collider.terrainData = terrainData;

				Undo.RegisterCreatedObjectUndo(newGO, "Duplicate terrain");
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		void RemoveTerrains()
		{
			m_Terrains = ToolboxHelper.GetSelectedTerrainsInScene();			

			if (m_Terrains == null || m_Terrains.Length == 0)
			{
				EditorUtility.DisplayDialog("Error", "No terrain selected. Please select and try again.", "OK");
				return;
			}

			if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to delete selected terrain(s) And their data assets? This process is not undoable.", "Continue", "Cancel"))
			{
				foreach (var terrain in m_Terrains)
				{
					if (terrain.terrainData)
					{
						var path = AssetDatabase.GetAssetPath(terrain.terrainData);
						AssetDatabase.DeleteAsset(path);
					}
					
					UnityEngine.Object.DestroyImmediate(terrain.gameObject);
				}

				AssetDatabase.Refresh();
			}			
		}

		bool MultipleIDExist(List<Terrain> terrains)
		{
			int[] ids = terrains.Select(t => t.groupingID).ToArray();
			if (ids.Distinct().ToArray().Length > 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		void SplitTerrains()
		{
			m_Terrains = ToolboxHelper.GetSelectedTerrainsInScene();

			if (m_Terrains == null || m_Terrains.Length == 0)
			{
				EditorUtility.DisplayDialog("Error", "No terrain selected. Please select and try again.", "OK");
				return;
			}

			// check if multiple grouping ids selected
			if (MultipleIDExist(m_Terrains.ToList()))
			{
				EditorUtility.DisplayDialog("Error", "The terrains selected have inconsistent Grouping IDs.", "OK");
				return;
			}

			int new_id = GetGroupIDForSplittedNewTerrain(m_Terrains);

			foreach (var terrain in m_Terrains)
			{
				TerrainData terrainData = terrain.terrainData;
				Vector3 startPosition = terrain.transform.position;
				float tileWidth = terrainData.size.x / m_Settings.TileXAxis;
				float tileLength = terrainData.size.z / m_Settings.TileZAxis;
				float tileHeight = terrainData.size.y;
				Vector2Int tileResolution = new Vector2Int((int)(terrainData.size.x / m_Settings.TileXAxis), (int)(terrainData.size.z / m_Settings.TileZAxis));
				Vector2Int heightOffset = Vector2Int.zero;
				Vector2Int controlOffset = Vector2Int.zero;
				Vector3 tilePosition = terrain.transform.position;

				// get terrain group
				GameObject groupGO = null;
				if (terrain.transform.parent != null && terrain.transform.parent.gameObject != null)
				{
					var parent = terrain.transform.parent.gameObject;
					var groupComp = parent.GetComponent<TerrainGroup>();
					if (parent != null && groupComp != null)
					{
						groupGO = parent;
					}
				}

				int newHeightmapRes = (terrainData.heightmapResolution - 1) / m_Settings.TileXAxis;
				if (!ToolboxHelper.IsPowerOfTwo(newHeightmapRes))
				{
					EditorUtility.DisplayDialog("Error", "Heightmap resolution of new tiles is not power of 2 with current settings.", "OK");
					return;
				}

				// control map resolution
				int newControlRes = terrainData.alphamapResolution / m_Settings.TileXAxis;
				if (!ToolboxHelper.IsPowerOfTwo(newControlRes))
				{
					EditorUtility.DisplayDialog("Error", "Splat control map resolution of new tiles is not power of 2 with current settings.", "OK");
					return;
				}

				int tileIndex = 0;				
				int tileCount = m_Settings.TileXAxis * m_Settings.TileZAxis;
				Terrain[] terrains = new Terrain[tileCount];

				try
				{
					for (int x = 0; x < m_Settings.TileXAxis; x++, heightOffset.x += newHeightmapRes, controlOffset.x += newControlRes, tilePosition.x += tileWidth)
					{
						heightOffset.y = 0;
						controlOffset.y = 0;
						tilePosition.z = startPosition.z;

						for (int y = 0; y < m_Settings.TileZAxis; y++, heightOffset.y += newHeightmapRes, controlOffset.y += newControlRes, tilePosition.z += tileLength)
						{
							EditorUtility.DisplayProgressBar("Creating terrains", string.Format("Updating terrain tile ({0}, {1})", x, y), ((float)tileIndex / tileCount));

							TerrainData terrainDataNew = new TerrainData();
							GameObject newGO = Terrain.CreateTerrainGameObject(terrainDataNew);
							Terrain newTerrain = newGO.GetComponent<Terrain>();

							Guid newGuid = Guid.NewGuid();
							string terrainName = $"Terrain_{x}_{y}_{newGuid}";
							newGO.name = terrainName;
							newTerrain.transform.position = tilePosition;
							newTerrain.groupingID = new_id;
							newTerrain.allowAutoConnect = true;
							newTerrain.drawInstanced = true;
							if (groupGO != null)
							{
								newTerrain.transform.SetParent(groupGO.transform);
							}

							// get and set heights
							terrainDataNew.heightmapResolution = newHeightmapRes + 1;
							var heightData = terrainData.GetHeights(heightOffset.x, heightOffset.y, (newHeightmapRes + 1), ((newHeightmapRes + 1)));
							terrainDataNew.SetHeights(0, 0, heightData);
							terrainDataNew.size = new Vector3(tileWidth, tileHeight, tileLength);

							string assetPath = $"{m_Settings.TerrainAssetDir}/{terrainName}.asset";
							if (!Directory.Exists(m_Settings.TerrainAssetDir))
							{
								Directory.CreateDirectory(m_Settings.TerrainAssetDir);
							}
							AssetDatabase.CreateAsset(terrainDataNew, assetPath);

							// note that add layers and alphamap operations need to happen after terrain data asset being created, so cached splat 0 and 1 data gets cleared to avoid bumping to splat 2 map.
							// get and set terrain layers
							TerrainToolboxLayer.AddLayersToTerrain(terrainDataNew, terrainData.terrainLayers.ToList(), true);

							// get and set alphamaps
							float[,,] alphamap = terrainData.GetAlphamaps(controlOffset.x, controlOffset.y, newControlRes, newControlRes);
							terrainDataNew.alphamapResolution = newControlRes;
							terrainDataNew.SetAlphamaps(0, 0, alphamap);

							// update other terrain settings
							if (m_Settings.AutoUpdateSettings)
							{
								ApplySettingsFromSourceToTargetTerrain(terrain, newTerrain);
							}

							terrains[tileIndex] = newTerrain;
							tileIndex++;

							Undo.RegisterCreatedObjectUndo(newGO, "Split terrain");
						}
					}
					m_SplitTerrains = terrains;
					ToolboxHelper.CalculateAdjacencies(m_SplitTerrains, m_Settings.TileXAxis, m_Settings.TileZAxis);
				}
				finally
				{
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					EditorUtility.ClearProgressBar();
					EditorSceneManager.SaveOpenScenes();

					foreach (var t in m_Terrains)
					{
						GameObject.DestroyImmediate(t.gameObject);
					}
				}
			}
		}

		

		int GetGroupIDForSplittedNewTerrain(Terrain[] exclude_terrains)
		{
			// check all other terrains in scene to see if group ID exists
			Terrain[] all_terrains = ToolboxHelper.GetAllTerrainsInScene();
			Terrain[] remaining_terrains = all_terrains.Except(exclude_terrains).ToArray();
			List<int> ids = new List<int>();
			int original_id = exclude_terrains[0].groupingID;
			ids.Add(original_id);
			bool exist = false;
			foreach (var terrain in remaining_terrains)
			{
				if (terrain.groupingID == original_id)
				{
					exist = true;
				}

				ids.Add(terrain.groupingID);
			}
			List<int> unique_ids = ids.Distinct().ToList();
			int max_id = unique_ids.Max();

			// if found id exist in scene, give a new id with largest id + 1, otherwise use original terrain's id
			if (exist)
			{
				return max_id + 1;
			}
			else
			{
				return original_id;
			}
		}

		void ApplySettingsFromSourceToTargetTerrain(Terrain sourceTerrain, Terrain targetTerrain)
		{
			targetTerrain.allowAutoConnect = sourceTerrain.allowAutoConnect;
			targetTerrain.drawHeightmap = sourceTerrain.drawHeightmap;
			targetTerrain.drawInstanced = sourceTerrain.drawInstanced;
			targetTerrain.heightmapPixelError = sourceTerrain.heightmapPixelError;
			targetTerrain.basemapDistance = sourceTerrain.basemapDistance;
			targetTerrain.shadowCastingMode = sourceTerrain.shadowCastingMode;
			targetTerrain.materialTemplate = sourceTerrain.materialTemplate;
			targetTerrain.reflectionProbeUsage = sourceTerrain.reflectionProbeUsage;
#if UNITY_2019_2_OR_NEWER
#else
			targetTerrain.materialType = sourceTerrain.materialType;			
			targetTerrain.legacySpecular = sourceTerrain.legacySpecular;
			targetTerrain.legacyShininess = sourceTerrain.legacyShininess;
#endif
			targetTerrain.terrainData.baseMapResolution = sourceTerrain.terrainData.baseMapResolution;
			targetTerrain.terrainData.SetDetailResolution(sourceTerrain.terrainData.detailResolution, sourceTerrain.terrainData.detailResolutionPerPatch);

			targetTerrain.drawTreesAndFoliage = sourceTerrain.drawTreesAndFoliage;
			targetTerrain.bakeLightProbesForTrees = sourceTerrain.bakeLightProbesForTrees;
			targetTerrain.deringLightProbesForTrees = sourceTerrain.deringLightProbesForTrees;
			targetTerrain.preserveTreePrototypeLayers = sourceTerrain.preserveTreePrototypeLayers;
			targetTerrain.detailObjectDistance = sourceTerrain.detailObjectDistance;
			targetTerrain.collectDetailPatches = sourceTerrain.collectDetailPatches;
			targetTerrain.detailObjectDensity = sourceTerrain.detailObjectDistance;
			targetTerrain.treeDistance = sourceTerrain.treeDistance;
			targetTerrain.treeBillboardDistance = sourceTerrain.treeBillboardDistance;
			targetTerrain.treeCrossFadeLength = sourceTerrain.treeCrossFadeLength;
			targetTerrain.treeMaximumFullLODCount = sourceTerrain.treeMaximumFullLODCount;

			targetTerrain.terrainData.wavingGrassStrength = sourceTerrain.terrainData.wavingGrassStrength;
			targetTerrain.terrainData.wavingGrassSpeed = sourceTerrain.terrainData.wavingGrassSpeed;
			targetTerrain.terrainData.wavingGrassAmount = sourceTerrain.terrainData.wavingGrassAmount;
			targetTerrain.terrainData.wavingGrassTint = sourceTerrain.terrainData.wavingGrassTint;
		}

		void ReplaceSplatmaps()
		{
			if (m_Settings.SplatmapNew0 == null && m_Settings.SplatmapNew1 == null)
			{
				if (EditorUtility.DisplayDialog("Confirm", "You don't have new splatmaps assigned. Would you like to reset splatmaps to defaults on selected terrain?", "OK", "Cancel"))
				{
					// reset splatmaps
					ResetSplatmapsOnTerrain(m_Settings.SplatmapTerrain);
					return;
				}
				return;
			}

			if (m_Settings.SplatmapOld0 != null && m_Settings.SplatmapNew0 != null)
			{
				ReplaceSplatmapTexture(m_Settings.SplatmapOld0, m_Settings.SplatmapNew0);
			}

			if (m_Settings.SplatmapOld1 != null && m_Settings.SplatmapNew1 != null)
			{
				ReplaceSplatmapTexture(m_Settings.SplatmapOld1, m_Settings.SplatmapNew1);
			}

			AssetDatabase.SaveAssets();
		}

		void ReplaceSplatmapTexture(Texture2D oldTexture, Texture2D newTexture)
		{
			if (newTexture.width != newTexture.height)
			{
				EditorUtility.DisplayDialog("Error", "Could not replace splatmap. Non-square sized splatmap found.", "OK");
				return;
			}

			var undoObjects = new List<UnityEngine.Object>();
			undoObjects.Add(m_Settings.SplatmapTerrain.terrainData);
			undoObjects.AddRange(m_Settings.SplatmapTerrain.terrainData.alphamapTextures);
			Undo.RegisterCompleteObjectUndo(undoObjects.ToArray(), "Replace splatmaps");

			// set new texture to be readable through Import Settings, so we can use GetPixels() later
			if (!newTexture.isReadable)
			{
				var newPath = AssetDatabase.GetAssetPath(newTexture);
				var newImporter = AssetImporter.GetAtPath(newPath) as TextureImporter;
				if (newImporter != null)
				{
					newImporter.isReadable = true;
					AssetDatabase.ImportAsset(newPath);
					AssetDatabase.Refresh();
				}
			}

			if (newTexture.width != oldTexture.width)
			{
				if (EditorUtility.DisplayDialog("Confirm", "Mismatched splatmap resolution found.", "Use New Resolution", "Use Old Resolution"))
				{
					// resize to new texture size
					oldTexture.Resize(newTexture.width, newTexture.height, oldTexture.format, true);
					// update splatmap resolution on terrain settings as well
					m_Settings.SplatmapTerrain.terrainData.alphamapResolution = newTexture.width;
					m_SplatmapResolution = newTexture.width;
				}
				else
				{
					// resize to old texture size
					newTexture.Resize(oldTexture.width, oldTexture.height, newTexture.format, true);
				}
			}

			var pixelsNew = newTexture.GetPixels();
			oldTexture.SetPixels(pixelsNew);
			oldTexture.Apply();
		}

		void ResetSplatmaps()
		{
			var terrains = ToolboxHelper.GetSelectedTerrainsInScene();
			int index = 0;
			foreach (var terrain in terrains)
			{
				EditorUtility.DisplayProgressBar("Resetting Splatmaps", string.Format("Resetting splatmaps on terrain {0}", terrain.name), (index / (terrains.Count())));
				ResetSplatmapsOnTerrain(terrain);
				index++;
			}
			EditorUtility.ClearProgressBar();
		}

		void ResetSplatmapsOnTerrain(Terrain terrain)
		{
			TerrainData terrainData = terrain.terrainData;
			if (terrainData.alphamapTextureCount < 1) return;

			var undoObjects = new List<UnityEngine.Object>();
			undoObjects.Add(terrainData);
			undoObjects.AddRange(terrainData.alphamapTextures);
			Undo.RegisterCompleteObjectUndo(undoObjects.ToArray(), "Reset splatmaps");

			Color splatDefault = new Color(1, 0, 0, 0); // red
			Color splatZero = new Color(0, 0, 0, 0);

			var pixelsFirst = terrainData.alphamapTextures[0].GetPixels();
			for (int p = 0; p < pixelsFirst.Length; p++)
			{
				pixelsFirst[p] = splatDefault;
			}
			terrainData.alphamapTextures[0].SetPixels(pixelsFirst);
			terrainData.alphamapTextures[0].Apply();

			for (int i = 1; i < terrainData.alphamapTextureCount; i++)
			{
				var pixels = terrainData.alphamapTextures[i].GetPixels();
				for (int j = 0; j < pixels.Length; j++)
				{
					pixels[j] = splatZero;
				}
				terrainData.alphamapTextures[i].SetPixels(pixels);
				terrainData.alphamapTextures[i].Apply();
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		void ExportSplatmaps(UnityEngine.Object[] terrains)
		{
			if (terrains == null || terrains.Length == 0)
			{
				EditorUtility.DisplayDialog("Error", "No terrain selected. Please select some terrain tile(s) to continue.", "OK");
				return;
			}

			if (!Directory.Exists(m_Settings.SplatFolderPath))
			{
				Directory.CreateDirectory(m_Settings.SplatFolderPath);
			}

			var fileExtension = m_Settings.SelectedFormat == UtilitySettings.ImageFormat.TGA ? ".tga" : ".png";
			int index = 0;

			foreach (var t in terrains)
			{				
				var terrain = t as Terrain;
				EditorUtility.DisplayProgressBar("Exporting Splatmaps", string.Format("Exporting splatmaps on terrain {0}", terrain.name), (index / (terrains.Count())));
				TerrainData data = terrain.terrainData;
				for (var i = 0; i < data.alphamapTextureCount; i++)
				{
					Texture2D tex = data.alphamapTextures[i];
					byte[] bytes;
					if (m_Settings.SelectedFormat == UtilitySettings.ImageFormat.TGA)
					{
						bytes = tex.EncodeToTGA();
					}
					else
					{
						bytes = tex.EncodeToPNG();
					}
					string filename = terrain.name + "_splatmap_" + i + fileExtension;
					File.WriteAllBytes($"{m_Settings.SplatFolderPath}/{filename}", bytes);
				}

				index++;
			}
			
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.ClearProgressBar();
		}

		void ExportHeightmaps(UnityEngine.Object[] terrains)
		{
			if (terrains == null || terrains.Length == 0)
			{
				EditorUtility.DisplayDialog("Error", "No terrain selected. Please select some terrain tile(s) to continue.", "OK");
				return;
			}

			if (!Directory.Exists(m_Settings.HeightmapFolderPath))
			{
				Directory.CreateDirectory(m_Settings.HeightmapFolderPath);
			}

			int index = 0;
			m_Settings.HeightmapDepth = m_DepthOptions.ElementAt(m_SelectedDepth).Value;

			foreach (var t in terrains)
			{
				var terrain = t as Terrain;
				EditorUtility.DisplayProgressBar("Exporting Heightmaps", string.Format("Exporting heightmap on terrain {0}", terrain.name), (index / (terrains.Count())));
				TerrainData terrainData = terrain.terrainData;
				string fileName = terrain.name + "_heightmap";
				string path = Path.Combine(m_Settings.HeightmapFolderPath, fileName);
				ToolboxHelper.ExportTerrainHeightsToRawFile(terrainData, path, m_Settings.HeightmapDepth, m_Settings.FlipVertically, m_Settings.HeightmapByteOrder, new Vector2(m_Settings.HeightmapRemapMin, m_Settings.HeightmapRemapMax));

				//switch (m_Settings.HeightFormat)
				//{
				//	case Heightmap.Format.RAW:
				//		ToolboxHelper.ExportTerrainHeightsToRawFile(terrainData, path, m_Settings.HeightmapDepth, false, m_Settings.HeightmapByteOrder);
				//		break;
				//	default:
				//		ToolboxHelper.ExportTerrainHeightsToTexture(terrainData, m_Settings.HeightFormat, path);
				//		break;
				//}				

				index++;
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.ClearProgressBar();
		}

		void CreateNewPalette()
		{
			string filePath = EditorUtility.SaveFilePanelInProject("Create New Palette", "New Layer Palette.asset", "asset", "");
			m_SelectedLayerPalette = ScriptableObject.CreateInstance<TerrainPalette>();
			foreach (var layer in m_PaletteLayers)
			{
				m_SelectedLayerPalette.PaletteLayers.Add(layer.AssignedLayer);
			}
			AssetDatabase.CreateAsset(m_SelectedLayerPalette, filePath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		void LoadPalette()
		{
			if (!GetPalette())
				return;

			m_PaletteLayers.Clear();
			foreach (var layer in m_SelectedLayerPalette.PaletteLayers)
			{
				Layer newLayer = ScriptableObject.CreateInstance<Layer>();
				newLayer.AssignedLayer = layer;
				newLayer.IsSelected = true;
				m_PaletteLayers.Add(newLayer);
			}			
		}

		bool GetPalette()
		{
			if (m_SelectedLayerPalette == null)
			{
				if (EditorUtility.DisplayDialog("Error", "No layer palette found, create a new one?", "OK", "Cancel"))
				{
					CreateNewPalette();
					return true;
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public void SaveSettings()
		{
			if (m_SelectedLayerPalette != null)
			{
				m_Settings.PalettePath = AssetDatabase.GetAssetPath(m_SelectedLayerPalette);
			}
			else
			{
				m_Settings.PalettePath = string.Empty;
			}

			string filePath = ToolboxHelper.GetPrefFilePath(ToolboxHelper.ToolboxPrefsUtility);
			string utilitySettings = JsonUtility.ToJson(m_Settings);
			File.WriteAllText(filePath, utilitySettings);
		}

		public void LoadSettings()
		{
			string filePath = ToolboxHelper.GetPrefFilePath(ToolboxHelper.ToolboxPrefsUtility);
			if (File.Exists(filePath))
			{
				string utilitySettingsData = File.ReadAllText(filePath);
				JsonUtility.FromJsonOverwrite(utilitySettingsData, m_Settings);
			}

			if (m_Settings.PalettePath == string.Empty)
			{
				m_SelectedLayerPalette = null;
			}
			else
			{
				m_SelectedLayerPalette = AssetDatabase.LoadAssetAtPath(m_Settings.PalettePath, typeof(TerrainPalette)) as TerrainPalette;
			}
		}
	}
}
