using UnityEngine;

namespace UnityEditor.Experimental.TerrainAPI {
    [System.Serializable]
    public class SlopeFilter : Filter {
        static readonly int RemapTexWidth = 1024;

        [SerializeField]
        private float m_Epsilon = 1.0f; //kernel size
        [SerializeField]
        private float m_FilterStrength = 1.0f;  //overall strength of the effect

        //We bake an AnimationCurve to a texture to control value remapping
        [SerializeField]
        private AnimationCurve m_RemapCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(0.25f, 0.0f), new Keyframe(1.0f, 0.0f));
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
        ComputeShader m_ConcavityCS = null;
        ComputeShader GetComputeShader() {
            if (m_ConcavityCS == null) {
                m_ConcavityCS = (ComputeShader)Resources.Load("Slope");
            }
            return m_ConcavityCS;
        }

        public override string GetDisplayName() {
            return "Slope";
        }

        public override string GetToolTip()
        {
            return "Uses the slope angle of the heightmap to mask the effect of the chosen Brush.";
        }

        public override void Eval(FilterContext fc) {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("GradientMultiply");

            Texture2D remapTex = GetRemapTexture();

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.sourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", fc.renderTextureCollection["heightMap"]);
            cs.SetTexture(kidx, "OutputTex", fc.destinationRenderTexture);
            cs.SetTexture(kidx, "RemapTex", remapTex);
            cs.SetInt("RemapTexRes", remapTex.width);
            cs.SetFloat("EffectStrength", m_FilterStrength);
            cs.SetVector("TerrainDimensions", new Vector4(fc.terrain.terrainData.size.x, fc.terrain.terrainData.size.y, fc.terrain.terrainData.size.z, 0.0f));
            cs.SetVector("TextureResolution", new Vector4(fc.sourceRenderTexture.width, fc.sourceRenderTexture.height, m_Epsilon, fc.properties["terrainScale"]));

            //using 1s here so we don't need a multiple-of-8 texture in the compute shader (probably not optimal?)
            cs.Dispatch(kidx, fc.sourceRenderTexture.width, fc.sourceRenderTexture.height, 1);
        }

        public override void DoGUI(Rect rect) {

            //Precaculate dimensions
            float epsilonLabelWidth = GUI.skin.label.CalcSize(epsilonLabel).x;
            float modeLabelWidth = GUI.skin.label.CalcSize(modeLabel).x;
            float strengthLabelWidth = GUI.skin.label.CalcSize(strengthLabel).x;
            float curveLabelWidth = GUI.skin.label.CalcSize(curveLabel).x;
            float labelWidth = Mathf.Max(Mathf.Max(Mathf.Max(modeLabelWidth, epsilonLabelWidth), strengthLabelWidth), curveLabelWidth) + 4.0f;

            //Strength Slider
            Rect strengthLabelRect = new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(strengthLabelRect, strengthLabel);
            Rect strengthSliderRect = new Rect(strengthLabelRect.xMax, strengthLabelRect.y, rect.width - labelWidth, strengthLabelRect.height);
            m_FilterStrength = EditorGUI.Slider(strengthSliderRect, m_FilterStrength, 0.0f, 1.0f);

            //Epsilon (kernel size) Slider
            Rect epsilonLabelRect = new Rect(rect.x, strengthSliderRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(epsilonLabelRect, epsilonLabel);
            Rect epsilonSliderRect = new Rect(epsilonLabelRect.xMax, epsilonLabelRect.y, rect.width - labelWidth, epsilonLabelRect.height);
            m_Epsilon = EditorGUI.Slider(epsilonSliderRect, m_Epsilon, 1.0f, 20.0f);

            //Value Remap Curve
            Rect curveLabelRect = new Rect(rect.x, epsilonSliderRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(curveLabelRect, curveLabel);
            Rect curveRect = new Rect(curveLabelRect.xMax, curveLabelRect.y, rect.width - labelWidth, curveLabelRect.height);

            EditorGUI.BeginChangeCheck();
            m_RemapCurve = EditorGUI.CurveField(curveRect, m_RemapCurve);
            if(EditorGUI.EndChangeCheck()) {
                Vector2 range = TerrainTools.Utility.AnimationCurveToRenderTexture(m_RemapCurve, ref m_RemapTex);
            }
        }

        public override float GetElementHeight() {
            return EditorGUIUtility.singleLineHeight * 5;
        }

        private static GUIContent strengthLabel = EditorGUIUtility.TrTextContent("Strength", "Controls the strength of the masking effect.");
        private static GUIContent epsilonLabel = EditorGUIUtility.TrTextContent("Feature Size", "Specifies the scale of Terrain features that affect the mask.");
        private static GUIContent modeLabel = EditorGUIUtility.TrTextContent("Mode");
        private static GUIContent curveLabel = EditorGUIUtility.TrTextContent("Remap Curve", "Remaps the slope input before computing the final mask. This helps you visualize how the Terrain's slope affects the generated mask.");
    }
}