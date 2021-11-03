using UnityEditor;
using UnityEngine;

namespace UnityEditor.TerrainTools
{
    [CustomEditor(typeof(NoiseSettings))]
    internal class NoiseSettingsEditor : Editor
    {
        NoiseSettingsGUI gui = new NoiseSettingsGUI();

        void OnEnable()
        {
            gui.Init(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            gui.OnGUI();
        }
    }
}