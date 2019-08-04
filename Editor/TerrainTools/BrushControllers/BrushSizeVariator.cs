
using System.Text;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class BrushSizeVariator : BaseBrushVariator, IBrushSizeController {
        
        private const float kMinBrushSize = 0.01f;
        private const float kMaxBrushSize = 500.0f;
        private const float kDefaultBrushSize = 100.0f;
        private const float kDefaultMouseSensitivity = 0.1f;

        private readonly TerrainFloatMinMaxValue m_BrushSize = new TerrainFloatMinMaxValue(styles.brushSize, kDefaultBrushSize, kMinBrushSize, kMaxBrushSize, true);
        private readonly BrushJitterHandler m_JitterHandler = new BrushJitterHandler(0.0f, kMinBrushSize, kMaxBrushSize);
        
        private bool m_AdjustingSize;
        public override bool isInUse => m_AdjustingSize;

        class Styles {
            public readonly GUIContent brushSize = EditorGUIUtility.TrTextContent("Brush Size", "Size of the brush used to paint.");
        }

        static readonly Styles styles = new Styles();

        public float brushSize
        {
            get { return m_JitterHandler.CalculateValue(m_BrushSize.value); }
            set { m_BrushSize.value = Mathf.Clamp(value, kMinBrushSize, kMaxBrushSize); }
        }

        public BrushSizeVariator(string toolName, IBrushEventHandler eventHandler, IBrushTerrainCache terrainCache) : base(toolName, eventHandler, terrainCache) {
        }

        private void BeginAdjustingSize()
        {
            LockTerrainUnderCursor(true);
            m_AdjustingSize = true;
        }

        private void EndAdjustingSize()
        {
            UnlockTerrainUnderCursor();
            m_AdjustingSize = false;
        }

        #region IBrushController
        public override void OnEnterToolMode(BrushShortcutHandler<BrushShortcutType> shortcutHandler) {
            base.OnEnterToolMode(shortcutHandler);
            
            shortcutHandler.AddActions(BrushShortcutType.Size, BeginAdjustingSize, EndAdjustingSize);
            
            m_BrushSize.value = GetEditorPrefs("TerrainBrushSize", kDefaultBrushSize);
            //m_BrushSize.mouseSensitivity = GetEditorPrefs("TerrainBrushSizeMouseSensitivity", kDefaultMouseSensitivity);
            m_JitterHandler.jitter = GetEditorPrefs("TerrainBrushSizeJitter", 0.0f);
        }
        
        public override void OnExitToolMode(BrushShortcutHandler<BrushShortcutType> shortcutHandler) {
            SetEditorPrefs("TerrainBrushSize", m_BrushSize.value);
            SetEditorPrefs("TerrainBrushSizeMouseSensitivity", m_BrushSize.mouseSensitivity);
            SetEditorPrefs("TerrainBrushSizeJitter", m_JitterHandler.jitter);

            shortcutHandler.RemoveActions(BrushShortcutType.Size);

            base.OnExitToolMode(shortcutHandler);
        }

        public override void OnSceneGUI(Event currentEvent, int controlId, Terrain terrain, IOnSceneGUI editContext) {
            base.OnSceneGUI(currentEvent, controlId, terrain, editContext);
            
            m_JitterHandler.Update();

            if(m_AdjustingSize)
            {
                float size = m_BrushSize.value;

                size += 0.002f * Mathf.Clamp(size, 1.0f, 100.0f) * currentEvent.delta.x;
                m_BrushSize.value = size;

                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.whiteTexture;
                style.fontSize = 12;
                Handles.Label(raycastHitUnderCursor.point, $"Size: {size:F1}", style);

                RequestRepaint();
            }
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            m_JitterHandler.RequestRandomization();
            return base.OnPaint(terrain, editContext);
        }

        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext) {
            base.OnInspectorGUI(terrain, editContext);
            
            // If size randomization is on, we use the min-max slider, otherwise, just a normal one.
            m_BrushSize.DrawInspectorGUI();
            m_JitterHandler.OnGuiLayout("Allow random variation of brush size");
        }

        public override void AppendBrushInfo(Terrain terrain, IOnSceneGUI editContext, StringBuilder builder)
        {
            base.AppendBrushInfo(terrain, editContext, builder);
            builder.AppendLine($"Size = {m_BrushSize.value:F3}");
        }
        
        #endregion

        #region Mouse Handling
        protected override bool OnBeginModifier()
        {
            base.OnBeginModifier();

            LockTerrainUnderCursor(false);
            return true;
        }

        protected override bool OnModifierUsingMouseMove(Event mouseEvent, Terrain terrain, IOnSceneGUI editContext)
        {
            base.OnModifierUsingMouseMove(mouseEvent, terrain, editContext);

            Vector2 delta = CalculateMouseDelta(mouseEvent, m_BrushSize.mouseSensitivity);
            float newBrushSize = m_BrushSize.value;

            newBrushSize += delta.y;
            m_BrushSize.value = newBrushSize;
            return true;
        }

        protected override bool OnEndModifier()
        {
            base.OnEndModifier();

            UnlockTerrainUnderCursor();
            return true;
        }
        #endregion
    }
}
