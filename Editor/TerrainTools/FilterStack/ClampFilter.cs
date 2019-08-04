using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class ClampFilter : Filter
    {
        [SerializeField]
        public Vector2 range = new Vector2(0, 1);
        
        public override string GetDisplayName()
        {
            return "Clamp";
        }

        public override string GetToolTip()
        {
            return "Clamps the pixels of a mask to the specified range. Change the X value to specify the low end of the range, and change the Y value to specify the high end of the range.";
        }

        public override void Eval(FilterContext fc)
        {
            FilterUtility.builtinMaterial.SetVector("_ClampRange", range);

            Graphics.Blit( fc.sourceRenderTexture, fc.destinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Clamp );
        }

        public override void DoGUI(Rect rect)
        {
            range = EditorGUI.Vector2Field(rect, "", range);
        }
    }
}