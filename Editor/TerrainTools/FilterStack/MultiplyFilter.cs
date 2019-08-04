using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class MultiplyFilter : Filter
    {
        [SerializeField]
        public float value = 1;

        public override string GetDisplayName()
        {
            return "Multiply";
        }

        public override string GetToolTip()
        {
            return "Multiply the Brush Mask filter stack by a constant";
        }

        public override void Eval(FilterContext fc)
        {
            FilterUtility.builtinMaterial.SetFloat("_Multiply", value);

            Graphics.Blit( fc.sourceRenderTexture, fc.destinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Multiply );
        }

        public override void DoGUI(Rect rect)
        {
            value = EditorGUI.FloatField(rect, value);
        }
    }
}