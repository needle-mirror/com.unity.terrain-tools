using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    internal class PowerFilter : Filter
    {
        [SerializeField]
        public float value = 2;
        
        public override string GetDisplayName()
        {
            return "Power";
        }

        public override void Eval(RenderTexture src, RenderTexture dest, RenderTextureCollection rtCollection)
        {
            FilterUtility.builtinMaterial.SetFloat("_Pow", value);

            Graphics.Blit( src, dest, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Power );
        }

        public override void DoGUI(Rect rect)
        {
            value = EditorGUI.FloatField(rect, value);
        }
    }
}