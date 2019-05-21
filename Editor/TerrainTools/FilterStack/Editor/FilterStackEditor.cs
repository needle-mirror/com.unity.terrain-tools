using UnityEditor;
using UnityEngine;

namespace UnityEditor.Experimental.TerrainAPI
{
    [CustomEditor(typeof(FilterStack))]
    internal class FilterStackEditor : Editor
    {
        private FilterStackView m_view;

        void OnEnable()
        {
            m_view = new FilterStackView(new GUIContent("Filters"), serializedObject, target as FilterStack);
        }

        public override void OnInspectorGUI()
        {
            m_view.OnGUI();
        }
    }
}