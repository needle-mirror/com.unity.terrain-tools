using UnityEngine;

namespace UnityEditor.TerrainTools
{
    internal class BlurFilter : Filter
    {
        private static readonly GUIContent s_BlurAmount = EditorGUIUtility.TrTextContent("Amount", "The amount of blurring to apply to the texture");
        private static readonly GUIContent s_BlurDirection = EditorGUIUtility.TrTextContent("Direction", "The direction in which the blur will be applied. Blur only up (1.0), only down (-1.0) or both (0.0)");
        
        private Material m_Material;
        private Material Material
        {
            get
            {
                if (m_Material != null) return m_Material;
                
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/Blur"));
                return m_Material;
            }
        }

        [SerializeField] private int m_Amount;
        [SerializeField] private float m_Direction;

        public override string GetDisplayName() => "Blur";
        public override string GetToolTip() => "Applies a specified amount of blurring to the input texture";

        protected override void OnEval(FilterContext filterContext, RenderTexture source, RenderTexture dest)
        {
            var mat = Material;
            Vector4 smoothWeights = new Vector4(
                Mathf.Clamp01(1.0f - Mathf.Abs(m_Direction)),   // centered
                Mathf.Clamp01(-m_Direction),                    // min
                Mathf.Clamp01(m_Direction),                     // max
                0);
            mat.SetVector("_SmoothWeights", smoothWeights);
            mat.SetInt("_KernelSize", Mathf.Max(1, m_Amount));  // kernel size
            
            // Two pass blur (first horizontal, then vertical)
            var tmpRT = RTUtils.GetTempHandle(dest.descriptor);
            tmpRT.RT.wrapMode = TextureWrapMode.Clamp;
            mat.SetVector("_BlurDirection", Vector2.right);
            Graphics.Blit(source, tmpRT, mat);
            mat.SetVector("_BlurDirection", Vector2.up);
            Graphics.Blit(tmpRT, dest, mat);
            RTUtils.Release(tmpRT);
        }

        protected override void OnDrawGUI(Rect rect, FilterContext filterContext)
        {
            var height = EditorGUIUtility.singleLineHeight;
            var width = Mathf.Max(GUI.skin.label.CalcSize(s_BlurAmount).x, GUI.skin.label.CalcSize(s_BlurDirection).x) + 4;
            var amountRect = new Rect(rect.x, rect.y, rect.width, height);
            var directionRect = new Rect(rect.x, amountRect.yMax + EditorGUIUtility.standardVerticalSpacing, rect.width, height);

            var prevWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
            m_Amount = EditorGUI.IntSlider(amountRect, s_BlurAmount, m_Amount, 1, 100);
            m_Direction = EditorGUI.Slider(directionRect, s_BlurDirection, m_Direction, -1, 1);
            EditorGUIUtility.labelWidth = prevWidth;
        }

        public override float GetElementHeight() => EditorGUIUtility.singleLineHeight * 3;
    }
}