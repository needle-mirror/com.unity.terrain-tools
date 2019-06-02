using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    internal class AbsFilter : Filter
    {
        public override string GetDisplayName()
        {
            return "Abs";
        }

        public override void Eval(RenderTexture src, RenderTexture dest, RenderTextureCollection rtCollection)
        {
            Graphics.Blit( src, dest, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Abs );
        }

        public override void DoGUI(Rect rect)
        {

        }
    }
}