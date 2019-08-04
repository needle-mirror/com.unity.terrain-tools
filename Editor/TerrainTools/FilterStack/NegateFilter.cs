using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class NegateFilter : Filter
    {
        public override string GetDisplayName()
        {
            return "Negate";
        }

        public override void Eval(FilterContext fc)
        {
            Graphics.Blit( fc.sourceRenderTexture, fc.destinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Negate );
        }

        public override string GetToolTip()
        {
            return "Reverses the sign of all pixels in the current mask. For example, 1 becomes -1, 0 remains the same, and -1 becomes 1.";
        }

        public override void DoGUI(Rect rect)
        {
            
        }
    }
}