using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    internal class ClampFilter : Filter
    {
        [SerializeField]
        public Vector2 range = new Vector2(0, 1);
        
        public override string GetDisplayName()
        {
            return "Clamp";
        }

        public override void Eval(RenderTexture src, RenderTexture dest, RenderTextureCollection rtCollection)
        {
            FilterUtility.builtinMaterial.SetVector("_ClampRange", range);

            Graphics.Blit( src, dest, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Clamp );
        }

        public override void DoGUI(Rect rect)
        {
            range = EditorGUI.Vector2Field(rect, "", range);
        }
    }
}