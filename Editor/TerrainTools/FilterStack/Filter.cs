using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    internal class Filter : ScriptableObject
    {
        public bool             enabled = true;

        public virtual string   GetDisplayName() => "EMPTY_FILTER_NAME";
        public virtual void     Eval( RenderTexture src, RenderTexture dest, RenderTextureCollection rtCollection ) {}
        public virtual void     DoGUI( Rect rect ) {}
        public virtual void     DoSceneGUI2D( SceneView sceneView ) {}
        public virtual void     DoSceneGUI3D( SceneView sceneView ) {}
        public virtual float    GetElementHeight() => EditorGUIUtility.singleLineHeight * 2;
    }
}