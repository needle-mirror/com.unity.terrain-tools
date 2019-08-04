using UnityEngine;

namespace UnityEditor.Experimental.TerrainAPI {
    [System.Serializable]
    public class AspectFilter : Filter {
        static readonly int RemapTexWidth = 1024;

        [SerializeField]
        private float m_Epsilon = 1.0f; //kernel size
        [SerializeField]
        private float m_EffectStrength = 1.0f;  //overall strength of the effect

        //We bake an AnimationCurve to a texture to control value remapping
        [SerializeField]
        private AnimationCurve m_RemapCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
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
        ComputeShader m_AspectCS = null;
        ComputeShader GetComputeShader() {
            if (m_AspectCS == null) {
                m_AspectCS = (ComputeShader)Resources.Load("Aspect");
            }
            return m_AspectCS;
        }

        public override string GetDisplayName() {
            return "Aspect";
        }

        public override string GetToolTip()
        {
            return "Uses the slope aspect of the heightmap to mask the effect of the chosen Brush, and uses Brush rotation to control the aspect direction.";
        }

        public override void Eval(FilterContext fc) {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("AspectRemap");

            //using 1s here so we don't need a multiple-of-8 texture in the compute shader (probably not optimal?)
            int[] numWorkGroups = { 1, 1, 1 };

            Texture2D remapTex = GetRemapTexture();

            float rotRad = (fc.properties["brushRotation"] - 90.0f) * Mathf.Deg2Rad;

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.sourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", fc.renderTextureCollection["heightMap"]);
            cs.SetTexture(kidx, "OutputTex", fc.destinationRenderTexture);
            cs.SetTexture(kidx, "RemapTex", remapTex);
            cs.SetInt("RemapTexRes", remapTex.width);
            cs.SetFloat("EffectStrength", m_EffectStrength);
            cs.SetVector("TextureResolution", new Vector4(fc.sourceRenderTexture.width, fc.sourceRenderTexture.height, 0.0f, 0.0f));
            cs.SetVector("AspectValues", new Vector4(Mathf.Cos(rotRad), Mathf.Sin(rotRad), m_Epsilon, 0.0f));

            cs.Dispatch(kidx, fc.sourceRenderTexture.width / numWorkGroups[0], fc.sourceRenderTexture.height / numWorkGroups[1], numWorkGroups[2]);
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
            m_EffectStrength = EditorGUI.Slider(strengthSliderRect, m_EffectStrength, 0.0f, 1.0f);

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

        public override void OnSceneGUI(Terrain terrain, IBrushUIGroup brushContext) {
            Quaternion windRot = Quaternion.AngleAxis(brushContext.brushRotation, new Vector3(0.0f, 1.0f, 0.0f));
            Handles.ArrowHandleCap(0, brushContext.raycastHitUnderCursor.point, windRot, 0.5f * brushContext.brushSize, EventType.Repaint);
        }

        public override float GetElementHeight() {
            return EditorGUIUtility.singleLineHeight * 5;
        }

        private static GUIContent strengthLabel = EditorGUIUtility.TrTextContent("Strength", "Controls the strength of the masking effect.");
        private static GUIContent epsilonLabel = EditorGUIUtility.TrTextContent("Feature Size", "Specifies the scale of Terrain features that affect the mask.");
        private static GUIContent modeLabel = EditorGUIUtility.TrTextContent("Mode");
        private static GUIContent curveLabel = EditorGUIUtility.TrTextContent("Remap Curve", "Remaps the concavity input before computing the final mask.");
    }
}