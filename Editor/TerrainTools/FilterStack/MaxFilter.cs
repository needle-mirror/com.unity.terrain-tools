using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class MaxFilter : Filter
    {
        [SerializeField]
        public float value;
        
        public override string GetDisplayName()
        {
            return "Max";
        }

        public override string GetToolTip()
        {
            return "Sets all pixels of the current mask to whichever is greater, the current pixel value or the input value.";
        }

        public override void Eval(FilterContext fc)
        {
            FilterUtility.builtinMaterial.SetFloat("_Max", value);

            Graphics.Blit( fc.sourceRenderTexture, fc.destinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Max );
        }

        public override void DoGUI(Rect rect)
        {
            value = EditorGUI.FloatField(rect, value);
        }
    }
}