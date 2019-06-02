using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    internal class NegateFilter : Filter
    {
        public override string GetDisplayName()
        {
            return "Negate";
        }

        public override void Eval(RenderTexture src, RenderTexture dest, RenderTextureCollection rtCollection)
        {
            Graphics.Blit( src, dest, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Negate );
        }

        public override void DoGUI(Rect rect)
        {
            
        }
    }
}