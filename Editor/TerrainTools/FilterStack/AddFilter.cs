using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    internal class AddFilter : Filter
    {
        [SerializeField]
        public float value;

        public override string GetDisplayName()
        {
            return "Add";
        }

        public override void Eval(RenderTexture src, RenderTexture dest, RenderTextureCollection rtCollection)
        {
            FilterUtility.builtinMaterial.SetFloat("_Add", value);

            Graphics.Blit( src, dest, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Add );
        }

        public override void DoGUI(Rect rect)
        {
            value = EditorGUI.FloatField(rect, value);
        }
    }
}