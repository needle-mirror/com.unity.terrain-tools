
using System.Text;
using UnityEngine;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class BrushSpacingVariator : BaseBrushVariator, IBrushSpacingController {
        private float m_BrushSpacing = 0.0f;

        private bool m_AllowPaint = true;

        private float m_DistanceTravelled = 0.0f;
        private Vector3 m_LastBrushPos;

        public float brushSpacing => m_BrushSpacing;
        public bool allowPaint { get => m_AllowPaint; set => m_AllowPaint = value; }

        class Styles {
            public readonly GUIContent brushSpacing = EditorGUIUtility.TrTextContent("Brush Spacing", "Distance between each brush stamp in a stroke");
        }

        static readonly Styles styles = new Styles();

        public BrushSpacingVariator(string toolName, IBrushEventHandler eventHandler, IBrushTerrainCache terrainCache) : base(toolName, eventHandler, terrainCache) {
        }

        public override void OnEnterToolMode(BrushShortcutHandler<BrushShortcutType> shortcutHandler) {
            base.OnEnterToolMode(shortcutHandler);
            m_BrushSpacing = GetEditorPrefs("TerrainBrushSpacing", 0.0f);
        }

        public override void OnExitToolMode(BrushShortcutHandler<BrushShortcutType> shortcutHandler) {
            SetEditorPrefs("TerrainBrushSpacing", m_BrushSpacing);
            base.OnExitToolMode(shortcutHandler);
        }

        public override void OnSceneGUI(Event currentEvent, int controlId, Terrain terrain, IOnSceneGUI editContext) {
            //SHOWER THOUGHT - maybe this should be scaled by brush size?
            Vector2 a = new Vector2(m_LastBrushPos.x, m_LastBrushPos.z);
            Vector2 b = new Vector2(editContext.raycastHit.point.x, editContext.raycastHit.point.z);
            float d = Vector2.Distance(a, b);

            m_DistanceTravelled += d;

            m_LastBrushPos = editContext.raycastHit.point;

        }

        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext) {
            base.OnInspectorGUI(terrain, editContext);
            m_BrushSpacing = BrushUITools.PercentSlider(styles.brushSpacing, m_BrushSpacing, 0.0f, 1.0f);
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext) {
            base.OnPaint(terrain, editContext);

            if(m_DistanceTravelled >= m_BrushSpacing * 100.0f) {
                m_DistanceTravelled = 0.0f;
                m_AllowPaint = true;
            } else {
                m_AllowPaint = false;
            }

            // Hack to prevent smooth hotkey to also paint
            if(Event.current.shift) {
                m_AllowPaint = false;
            }

            return true;
        }

        public override void AppendBrushInfo(Terrain terrain, IOnSceneGUI editContext, StringBuilder builder)
        {
            base.AppendBrushInfo(terrain, editContext, builder);
            builder.AppendLine($"Spacing = {m_BrushSpacing:F3}");
        }
    }
}
