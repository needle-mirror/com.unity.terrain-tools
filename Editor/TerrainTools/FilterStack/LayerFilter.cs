using System.Linq;
using UnityEngine;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    [System.Serializable]
    internal class LayerFilter : Filter
    {
        [SerializeField]
        public string splatmapKeyword;
        [SerializeField]
        public int layerIndex;
        [SerializeField]
        Vector2 sharpness = new Vector2(0,1);
        static GUIContent s_SharpnessLabel = EditorGUIUtility.TrTextContent("Sharpness:", "Controls how sharp the falloff is of the masking effect.");
        public override string GetDisplayName() => "Layer";
        public override string GetToolTip() => "Filter out Terrain Layers";
        protected override void OnEval(FilterContext filterContext, RenderTexture source, RenderTexture dest)
        {
            if (splatmapKeyword == null || !filterContext.rtHandleCollection.ContainsRTHandle(splatmapKeyword))
            {
                Graphics.Blit(source, dest);
                return;
            }

            var desc = dest.descriptor;
            desc.enableRandomWrite = true;
            var sourceHandle = RTUtils.GetTempHandle(desc);
            var destHandle = RTUtils.GetTempHandle(desc);
            using (sourceHandle.Scoped())
            using (destHandle.Scoped())
            {
                FilterUtility.builtinMaterial.SetVector("_SharpnessRanges", sharpness);
                FilterUtility.builtinMaterial.SetTexture("_Splatmap", filterContext.rtHandleCollection[splatmapKeyword]);

                Graphics.Blit(source, dest, FilterUtility.builtinMaterial, (int)FilterUtility.BuiltinPasses.Layer);
            }
        }

        Vector2 m_ScrollPos;
        bool m_MultiRow = false;
        const int k_ThumbnailSize = 65;
        const int k_ElementsPadding = 3;
        protected override void OnDrawGUI(Rect rect, FilterContext filterContext)
        {
            if (filterContext.diffuseTextures == null || rect.width < 0 || rect.height < 0)
                return;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = k_ThumbnailSize,
                fixedHeight = k_ThumbnailSize
            };

            GUIContent[] layerContent = filterContext.diffuseTextures.Select(
                TerrainLayer => new GUIContent (TerrainLayer.diffuseTexture, TerrainLayer.name)
                ).ToArray();

            int columns = Mathf.Max((int)((rect.width - (k_ThumbnailSize / 2)) / k_ThumbnailSize), 1);
            int rows = (layerContent.Length + columns - 1) / columns;

            float sharpnessLabelWidth = GUI.skin.label.CalcSize(s_SharpnessLabel).x;
            Rect sharpnessLabelRect = new Rect(rect.x, rect.y, sharpnessLabelWidth, EditorGUIUtility.singleLineHeight);
            Rect sharpnessFieldRect = new Rect(sharpnessLabelRect.xMax + k_ElementsPadding, sharpnessLabelRect.y, rect.width - (sharpnessLabelWidth + 30), sharpnessLabelRect.height);
            EditorGUI.LabelField(sharpnessLabelRect, s_SharpnessLabel);
            EditorGUI.MinMaxSlider(sharpnessFieldRect, ref sharpness.x, ref sharpness.y, 0, 1);
            
            Rect scrollRectView = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, 0,((k_ThumbnailSize + k_ElementsPadding) * rows) - k_ElementsPadding);
            Rect selectionGridRect = new Rect(scrollRectView.x, scrollRectView.y, rect.width, rect.height - EditorGUIUtility.singleLineHeight);
            m_ScrollPos = GUI.BeginScrollView(selectionGridRect, m_ScrollPos,
                scrollRectView, 
                false, false);
            layerIndex = GUI.SelectionGrid(selectionGridRect, layerIndex, layerContent, columns, buttonStyle);
            GUI.EndScrollView();

            m_MultiRow = columns < layerContent.Length;
        }

        const int k_SingleRowFilterHeight = 98;
        const int k_MultiRowFilterHeight = 163;
        public override float GetElementHeight() => m_MultiRow ? k_MultiRowFilterHeight : k_SingleRowFilterHeight;
    }
}