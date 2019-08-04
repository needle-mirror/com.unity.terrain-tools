using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class AbsFilter : Filter
    {
        public override string GetDisplayName()
        {
            return "Abs";
        }

        public override string GetToolTip()
        {
            return "Sets all pixels of an existing Brush Mask to their absolute values";
        }

        public override void Eval(FilterContext fc)
        {
            Graphics.Blit(fc.sourceRenderTexture, fc.destinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Abs );
        }

        public override void DoGUI(Rect rect)
        {

        }
    }
}