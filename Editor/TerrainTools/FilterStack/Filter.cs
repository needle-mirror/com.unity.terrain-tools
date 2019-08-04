using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class Filter : ScriptableObject
    {
        [ SerializeField ]
        public bool             enabled = true;

        public virtual string   GetDisplayName() => "EMPTY_FILTER_NAME";
        public virtual string   GetToolTip() => "EMPTY_TOOLTIP";
        public virtual void     Eval( FilterContext filterContext ) {}
        public virtual void     DoGUI( Rect rect ) {}
        public virtual void     OnSceneGUI(Terrain terrain, IBrushUIGroup brushContext) {}
        public virtual void     DoSceneGUI2D( SceneView sceneView ) {}
        public virtual void     DoSceneGUI3D( SceneView sceneView ) {}
        public virtual float    GetElementHeight() => EditorGUIUtility.singleLineHeight * 2;
        public virtual void     OnEnable() {}
        public virtual void     OnDisable() {}

        public virtual List< UnityEngine.Object >    GetObjectsToSerialize() { return null; }
    }
}