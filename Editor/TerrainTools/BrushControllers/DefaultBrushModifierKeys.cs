using System;

namespace UnityEditor.TerrainTools
{
    internal class DefaultBrushModifierKeys : IBrushModifierKeyController
    {
        public event Action<BrushModifierKey> OnModifierPressed;
        public event Action<BrushModifierKey> OnModifierReleased;
        
        private static readonly BrushShortcutHandler<BrushModifierKey> s_ShortcutHandler = new BrushShortcutHandler<BrushModifierKey>();

        private void HandleModifier1Pressed()
        {
            OnModifierPressed?.Invoke(BrushModifierKey.BRUSH_MOD_1);
        }

        private void HandleModifier1Released()
        {
            OnModifierReleased?.Invoke(BrushModifierKey.BRUSH_MOD_1);
        }

        private void HandleModifier2Pressed()
        {
            OnModifierPressed?.Invoke(BrushModifierKey.BRUSH_MOD_2);
        }

        private void HandleModifier2Released()
        {
            OnModifierReleased?.Invoke(BrushModifierKey.BRUSH_MOD_2);
        }

        private void HandleModifier3Pressed()
        {
            OnModifierPressed?.Invoke(BrushModifierKey.BRUSH_MOD_3);
        }

        private void HandleModifier3Released()
        {
            OnModifierReleased?.Invoke(BrushModifierKey.BRUSH_MOD_3);
        }

        private void HandleInvertStrengthPressed()
        {
            OnModifierPressed?.Invoke(BrushModifierKey.BRUSH_MOD_INVERT);
        }

        private void HandleInvertStrengthReleased()
        {
            OnModifierReleased?.Invoke(BrushModifierKey.BRUSH_MOD_INVERT);
        }
        
        public void OnEnterToolMode()
        {
            s_ShortcutHandler.AddActions(BrushModifierKey.BRUSH_MOD_1, HandleModifier1Pressed, HandleModifier1Released);
            s_ShortcutHandler.AddActions(BrushModifierKey.BRUSH_MOD_2, HandleModifier2Pressed, HandleModifier2Released);
            s_ShortcutHandler.AddActions(BrushModifierKey.BRUSH_MOD_3, HandleModifier3Pressed, HandleModifier3Released);
            s_ShortcutHandler.AddActions(BrushModifierKey.BRUSH_MOD_INVERT, HandleInvertStrengthPressed, HandleInvertStrengthReleased);
        }

        public void OnExitToolMode()
        {
            s_ShortcutHandler.RemoveActions(BrushModifierKey.BRUSH_MOD_1);
            s_ShortcutHandler.RemoveActions(BrushModifierKey.BRUSH_MOD_2);
            s_ShortcutHandler.RemoveActions(BrushModifierKey.BRUSH_MOD_3);
            s_ShortcutHandler.RemoveActions(BrushModifierKey.BRUSH_MOD_INVERT);
        }

        public bool ModifierActive(BrushModifierKey k)
        {
            return s_ShortcutHandler.IsShortcutPressed(k);
        }
    }
}
