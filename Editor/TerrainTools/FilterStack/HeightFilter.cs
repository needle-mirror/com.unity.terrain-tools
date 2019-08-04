using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.Experimental.TerrainAPI {
    [System.Serializable]
    public class HeightFilter : Filter {
        static readonly int RemapTexWidth = 1024;

        [SerializeField]
        private float m_ConcavityStrength = 1.0f;  //overall strength of the effect

        [SerializeField]
        private Vector2 m_Height = new Vector2(0.0f, 1.0f);
        [SerializeField]
        private float m_HeightFeather = 0.0f;

        //We bake an AnimationCurve to a texture to control value remapping
        [SerializeField]
        private AnimationCurve m_RemapCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.25f, 1.0f), new Keyframe(0.75f, 1.0f), new Keyframe(1.0f, 0.0f));
        Texture2D m_RemapTex = null;

        Texture2D GetRemapTexture() {
            if (m_RemapTex == null) {
                m_RemapTex = new Texture2D(RemapTexWidth, 1, TextureFormat.RFloat, false, true);
                m_RemapTex.wrapMode = TextureWrapMode.Clamp;

                TerrainTools.Utility.AnimationCurveToRenderTexture(m_RemapCurve, ref m_RemapTex);
            }

            return m_RemapTex;
        }

        //Compute Shader resource helper
        ComputeShader m_HeightCS = null;
        ComputeShader GetComputeShader() {
            if (m_HeightCS == null) {
                m_HeightCS = (ComputeShader)Resources.Load("Height");
            }
            return m_HeightCS;
        }

        public override string GetDisplayName() {
            return "Height";
        }

        public override string GetToolTip()
        {
            return "Uses the height of the heightmap to mask the effect of the chosen Brush.";
        }

        public override void Eval(FilterContext fc) {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("HeightRemap");

            Texture2D remapTex = GetRemapTexture();

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.sourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", fc.renderTextureCollection["heightMap"]);
            cs.SetTexture(kidx, "OutputTex", fc.destinationRenderTexture);
            cs.SetTexture(kidx, "RemapTex", remapTex);
            cs.SetInt("RemapTexRes", remapTex.width);
            cs.SetFloat("EffectStrength", m_ConcavityStrength);
            cs.SetVector("HeightRange", new Vector4(m_Height.x, m_Height.y, m_HeightFeather, 0.0f));

            //using workgroup size of 1 here to avoid needing to resize render textures
            cs.Dispatch(kidx, fc.sourceRenderTexture.width, fc.sourceRenderTexture.height, 1);
        }

        public override void DoGUI(Rect rect) {

            //Precaculate dimensions
            float strengthLabelWidth = GUI.skin.label.CalcSize(strengthLabel).x;
            float rangeLabelWidth = GUI.skin.label.CalcSize(rangeLabel).x;
            float featherLabelWidth = GUI.skin.label.CalcSize(featherLabel).x;
            float curveLabelWidth = GUI.skin.label.CalcSize(curveLabel).x;
            float labelWidth = Mathf.Max(Mathf.Max(Mathf.Max(rangeLabelWidth, featherLabelWidth), strengthLabelWidth), curveLabelWidth) + 4.0f;

            // Strength Slider
            Rect strengthLabelRect = new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(strengthLabelRect, strengthLabel);
            Rect strengthSliderRect = new Rect(strengthLabelRect.xMax, strengthLabelRect.y, rect.width - labelWidth, strengthLabelRect.height);
            m_ConcavityStrength = EditorGUI.Slider(strengthSliderRect, m_ConcavityStrength, 0.0f, 1.0f);

            // Height Range Slider
            Rect rangeLabelRect = new Rect(rect.x, strengthSliderRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rangeLabelRect, rangeLabel);
            //Rect rangeSliderRect = new Rect(rangeLabelRect.xMax, rangeLabelRect.y, rect.width - labelWidth, rangeLabelRect.height);
            Rect rangeLeftRect = new Rect(rangeLabelRect.xMax, rangeLabelRect.y, (rect.width - labelWidth) / 2, rangeLabelRect.height);
            Rect rangeRightRect = new Rect(rangeLeftRect.xMax, rangeLabelRect.y, (rect.width - labelWidth) / 2, rangeLabelRect.height);
            m_Height.x = EditorGUI.FloatField(rangeLeftRect, m_Height.x);
            m_Height.y = EditorGUI.FloatField(rangeRightRect, m_Height.y);
            //EditorGUI.MinMaxSlider(rangeSliderRect, GUIContent.none, ref m_Height.x, ref m_Height.y, 0.0f, 1.0f);

            //Value Remap Curve
            Rect curveLabelRect = new Rect(rect.x, rangeLeftRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(curveLabelRect, curveLabel);
            Rect curveRect = new Rect(curveLabelRect.xMax, curveLabelRect.y, rect.width - labelWidth, curveLabelRect.height);

            EditorGUI.BeginChangeCheck();
            m_RemapCurve = EditorGUI.CurveField(curveRect, m_RemapCurve);
            if (EditorGUI.EndChangeCheck()) {
                Vector2 range = TerrainTools.Utility.AnimationCurveToRenderTexture(m_RemapCurve, ref m_RemapTex);
            }
        }

        public override float GetElementHeight() {
            return EditorGUIUtility.singleLineHeight * 4;
        }

        private static GUIContent strengthLabel = EditorGUIUtility.TrTextContent("Strength", "Controls the strength of the masking effect.");
        private static GUIContent rangeLabel = EditorGUIUtility.TrTextContent("Height Range", "Specifics the height range to which to apply the effect.");
        private static GUIContent featherLabel = EditorGUIUtility.TrTextContent("Feather");
        private static GUIContent curveLabel = EditorGUIUtility.TrTextContent("Remap Curve", "Remaps the height input before computing the final mask.");
    }
}