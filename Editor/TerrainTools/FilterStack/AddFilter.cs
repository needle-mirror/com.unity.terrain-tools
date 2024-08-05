using UnityEngine;

namespace UnityEditor.TerrainTools
{
    [System.Serializable]
    internal class AddFilter : Filter
    {
        [SerializeField]
        public float value;
        public override string GetDisplayName() => "Add";
        public override string GetToolTip() => "Adds a constant to the Brush Mask filter stack";
        protected override void OnEval(FilterContext filterContext, RenderTexture source, RenderTexture dest)
        {
            FilterUtility.builtinMaterial.SetFloat("_Add", value);
            Graphics.Blit(source, dest, FilterUtility.builtinMaterial, (int)FilterUtility.BuiltinPasses.Add);
        }
        protected override void OnDrawGUI(Rect rect, FilterContext filterContext) => value = EditorGUI.FloatField(rect, value);
    }
}