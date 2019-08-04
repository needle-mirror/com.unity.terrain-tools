using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class PowerFilter : Filter
    {
        [SerializeField]
        public float value = 2;
        
        public override string GetDisplayName()
        {
            return "Power";
        }

        public override string GetToolTip()
        {
            return "Applies an exponential function to each pixel on the Brush Mask. The function is pow(value, e), where e is the input value.";
        }

        public override void Eval(FilterContext fc)
        {
            FilterUtility.builtinMaterial.SetFloat("_Pow", value);

            Graphics.Blit( fc.sourceRenderTexture, fc.destinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Power );
        }

        public override void DoGUI(Rect rect)
        {
            value = EditorGUI.FloatField(rect, value);
        }
    }
}