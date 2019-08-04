using UnityEngine;
using UnityEditor;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class RemapFilter : Filter
    {
        private static GUIContent fromLabel = EditorGUIUtility.TrTextContent("From");
        private static GUIContent toLabel = EditorGUIUtility.TrTextContent("To");

        public Vector2 fromRange = new Vector2(0, 1);
        public Vector2 toRange = new Vector2(0, 1);

        public override string GetDisplayName()
        {
            return "Remap";
        }

        public override string GetToolTip()
        {
            return "Remaps each pixel value in the Brush Mask from the From range to the To range.";
        }

        public override void Eval(FilterContext fc)
        {
            FilterUtility.builtinMaterial.SetVector( "_RemapRanges", new Vector4( fromRange.x, fromRange.y, toRange.x, toRange.y ) );

            Graphics.Blit( fc.sourceRenderTexture, fc.destinationRenderTexture, FilterUtility.builtinMaterial, ( int )FilterUtility.BuiltinPasses.Remap );
        }

        public override void DoGUI(Rect rect)
        {
            float labelWidth = Mathf.Max(GUI.skin.label.CalcSize(fromLabel).x, GUI.skin.label.CalcSize(toLabel).x) + 4f;

            Rect fromLabelRect = new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(fromLabelRect, fromLabel);

            Rect fromRangeRect = new Rect(fromLabelRect.xMax, fromLabelRect.y, rect.width - labelWidth, fromLabelRect.height);
            fromRange = EditorGUI.Vector2Field(fromRangeRect, "", fromRange);

            Rect toLabelRect = new Rect(rect.x, fromLabelRect.yMax, fromLabelRect.width, fromLabelRect.height);
            EditorGUI.LabelField(toLabelRect, toLabel);

            Rect toRangeRect = new Rect(toLabelRect.xMax, toLabelRect.y, fromRangeRect.width, fromRangeRect.height);
            toRange = EditorGUI.Vector2Field(toRangeRect, "", toRange);
        }

        public override float GetElementHeight()
        {
            return EditorGUIUtility.singleLineHeight * 3;
        }
    }
}