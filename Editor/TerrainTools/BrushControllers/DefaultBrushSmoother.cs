
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class DefaultBrushSmoother : IBrushSmoothController {

        public float kernelSize { get; set; }

        Material m_Material = null;
        Material GetMaterial() {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/SmoothHeight"));
            return m_Material;
        }

        public DefaultBrushSmoother(string name) {
            //m_SmoothTool = new SmoothHeightTool();
        }

        public bool active { get { return Event.current.shift; } }

        public void OnEnterToolMode() {}
        public void OnExitToolMode() {}
        public void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext) {
            //m_SmoothTool.OnSceneGUI(terrain, editContext);
        }

        public void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext) {
            //maybe have a UI here at some point? (To select different blur tools, etc...)
        }

        public bool OnPaint(Terrain terrain, IOnPaint editContext, float brushSize, float brushRotation, float brushStrength, Vector2 uv) {
            if (Event.current != null && Event.current.shift) {
                BrushTransform brushXform = TerrainPaintUtility.CalculateBrushTransform(terrain, uv, brushSize, brushRotation);
                PaintContext paintContext = TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds());

                Material mat = GetMaterial();//TerrainPaintUtility.GetBuiltinPaintMaterial();

                float m_direction = 0.0f; //TODO: UI for this

                Vector4 brushParams = new Vector4(brushStrength, 0.0f, 0.0f, 0.0f);
                mat.SetTexture("_BrushTex", editContext.brushTexture);
                mat.SetVector("_BrushParams", brushParams);
                Vector4 smoothWeights = new Vector4(
                    Mathf.Clamp01(1.0f - Mathf.Abs(m_direction)),   // centered
                    Mathf.Clamp01(-m_direction),                    // min
                    Mathf.Clamp01(m_direction),                     // max
                    kernelSize);                                          // blur kernel size
                mat.SetVector("_SmoothWeights", smoothWeights);
                TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);

                RenderTexture temp = RenderTexture.GetTemporary( paintContext.destinationRenderTexture.descriptor );

                Graphics.Blit(paintContext.sourceRenderTexture, temp, mat, 0);
                Graphics.Blit(temp, paintContext.destinationRenderTexture, mat, 1);

                RenderTexture.ReleaseTemporary( temp );

                TerrainPaintUtility.EndPaintHeightmap(paintContext, "Terrain Paint - Smooth Height");
                return true;
            }
            return false;
        }
    }
}
