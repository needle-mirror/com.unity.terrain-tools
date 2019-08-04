using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class ComplementFilter : Filter
    {
        [SerializeField]
        public float value = 1;
        
        public override string GetDisplayName()
        {
            return "Complement";
        }

        public override string GetToolTip()
        {
            return "Subtracts each pixel value in the current Brush Mask from the specified constant. To invert the mask results, leave the complement value unchanged as 1.";
        }

        public override void Eval(FilterContext fc)
        {
            FilterUtility.builtinMaterial.SetFloat("_Complement", value);

            Graphics.Blit( fc.sourceRenderTexture, fc.destinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Complement );
        }

        public override void DoGUI(Rect rect)
        {
            value = EditorGUI.FloatField(rect, value);
        }
    }
}