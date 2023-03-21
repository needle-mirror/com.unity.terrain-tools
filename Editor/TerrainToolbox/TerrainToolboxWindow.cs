using System.IO;
using UnityEngine;

namespace UnityEditor.TerrainTools
{
    internal class TerrainToolboxWindow : EditorWindow
    {
#if UNITY_2019_1_OR_NEWER
        [MenuItem("Window/Terrain/Terrain Toolbox", false, 3020)]
        static void CreateMangerWindow()
        {
            TerrainToolboxWindow window = GetWindow<TerrainToolboxWindow>("Terrain Toolbox");
            window.minSize = new Vector2(200, 150);
            window.Show();
        }
#endif

        TerrainManagerMode m_SelectedMode = TerrainManagerMode.CreateTerrain;

        enum TerrainManagerMode
        {
            CreateTerrain = 0,
            Settings = 1,
            Utilities = 2,
            Visualization = 3
        }

        internal TerrainToolboxCreateTerrain m_CreateTerrainMode;
        internal TerrainToolboxSettings m_TerrainSettingsMode;
        internal TerrainToolboxUtilities m_TerrainUtilitiesMode;
        internal TerrainToolboxVisualization m_TerrainVisualizationMode;

        const string PrefName = "TerrainToolbox.Window.Mode";
        
        Vector2 m_ScrollPosition = Vector2.zero;
        private bool scrollBarActive = false; 

        static class Styles
        {
            public static readonly GUIContent[] ModeToggles =
            {
                EditorGUIUtility.TrTextContent("Create New Terrain"),
                EditorGUIUtility.TrTextContent("Terrain Settings"),
                EditorGUIUtility.TrTextContent("Terrain Utilities"),
                EditorGUIUtility.TrTextContent("Terrain Visualization")
                
            };
            // use this one if the width is smaller

            public static readonly GUIContent[] CompactModeToggles =
            {
                new GUIContent("Create", "Create New Terrain"),
                new GUIContent("Settings", "Terrain Settings"),
                new GUIContent("Utilities", "Terrain Utilities"),
                new GUIContent("Visualization", "Terrain Visualization")
            };
            
            public static readonly GUIStyle ButtonStyle = "LargeButton";
        }

        public void OnEnable()
        {
            m_CreateTerrainMode = new TerrainToolboxCreateTerrain(this);
            m_TerrainSettingsMode = new TerrainToolboxSettings();
            m_TerrainUtilitiesMode = new TerrainToolboxUtilities();
            m_TerrainVisualizationMode = new TerrainToolboxVisualization();

            m_CreateTerrainMode.LoadSettings();
            m_CreateTerrainMode.OnEnable();
            m_TerrainSettingsMode.LoadSettings();
            m_TerrainUtilitiesMode.LoadSettings();
            m_TerrainUtilitiesMode.OnLoad();
            m_TerrainVisualizationMode.LoadSettings();
            LoadSettings();
        }
        
        public void OnDisable()
        {
            m_CreateTerrainMode.OnDisable();
            m_CreateTerrainMode.SaveSettings();
            m_TerrainSettingsMode.SaveSettings();
            m_TerrainUtilitiesMode.SaveSettings();
            m_TerrainVisualizationMode.SaveSettings();
            SaveSettings();
        }
        public void OnGUI()
        {

            EditorGUILayout.Space();
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, GUILayout.Width(EditorGUIUtility.currentViewWidth));
            GUIStyle vertStyle = new GUIStyle();
            // force the vertical box to be smaller than the scrollview in width so that the horizontal scrollbar is suppressed 
            vertStyle.fixedWidth = EditorGUIUtility.currentViewWidth - (scrollBarActive ? 15 : 5);
            // encapsulate the vertical box in the scrollview 
            EditorGUILayout.BeginVertical(vertStyle);
            ToggleManagerMode();
            EditorGUILayout.Space();

            switch (m_SelectedMode)
            {
                case TerrainManagerMode.CreateTerrain:
                    m_CreateTerrainMode.OnGUI();
                    break;

                case TerrainManagerMode.Settings:
                    m_TerrainSettingsMode.OnGUI();
                    break;

                case TerrainManagerMode.Utilities:
                    m_TerrainUtilitiesMode.OnGUI();
                    break;

                case TerrainManagerMode.Visualization:
                    m_TerrainVisualizationMode.OnGUI();
                    break;
            }

            // before ending the scrollView, get the y value 
            float scrollY = 0f;
            if (Event.current.type == EventType.Repaint)
            {
                scrollY = GUILayoutUtility.GetLastRect().y;
            }

            // close the views 
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            float scrollHeight = 0f;
            // after ending the scrollview, get its height and save it 
            if (Event.current.type == EventType.Repaint)
            {
                scrollHeight = GUILayoutUtility.GetLastRect().height;
            }

            // update the scrollBarActive variable 
            if (Event.current.type == EventType.Repaint)
            {
                scrollBarActive = scrollY > scrollHeight;
            }
        }

        void ToggleManagerMode()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            
            // changes toggles based on the screen width of the GUI 
            var toggleNames= EditorGUIUtility.currentViewWidth < 453 ? Styles.CompactModeToggles : Styles.ModeToggles;
            m_SelectedMode = (TerrainManagerMode)GUILayout.Toolbar((int)m_SelectedMode, toggleNames, Styles.ButtonStyle, GUI.ToolbarButtonSize.FitToContents);
            
            if (EditorGUI.EndChangeCheck())
            {
                GUIUtility.keyboardControl = 0;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void OnLostFocus()
        {
            m_TerrainUtilitiesMode.OnLostFocus();
        }

        void OnDestroy()
        {
            m_TerrainVisualizationMode.RevertMaterial();
        }

        void SaveSettings()
        {
            string filePath = ToolboxHelper.GetPrefFilePath(ToolboxHelper.ToolboxPrefsWindow);
            File.WriteAllText(filePath, ((int)m_SelectedMode).ToString());
        }

        void LoadSettings()
        {
            string filePath = ToolboxHelper.GetPrefFilePath(ToolboxHelper.ToolboxPrefsWindow);
            if (File.Exists(filePath))
            {
                string windowSettingsData = File.ReadAllText(filePath);
                int value = 0;
                if (int.TryParse(windowSettingsData, out value))
                {
                    m_SelectedMode = (TerrainManagerMode)value;
                }
            }
        }
    }
}
