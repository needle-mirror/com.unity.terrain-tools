using System;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Options for brush modifier key shortcuts.
    /// </summary>
    [Flags]
    public enum BrushModifierKey
    {
        /// <summary>
        /// Use invert modifier.
        /// </summary>
        BRUSH_MOD_INVERT = 0,

        /// <summary>
        /// Use brush modifier 1.
        /// </summary>
        BRUSH_MOD_1 = 1,

        /// <summary>
        /// Use brush modifier 2.
        /// </summary>
        BRUSH_MOD_2 = 2,

        /// <summary>
        /// Use brush modifier 3.
        /// </summary>
        BRUSH_MOD_3 = 3
    }

    /// <summary>
    /// An interface that represent the controller for brush modifier keys.
    /// </summary>
    public interface IBrushModifierKeyController
    {
        /// <summary>
        /// Calls the methods in its invocation list when the modifier key is pressed.
        /// </summary>
        event Action<BrushModifierKey> OnModifierPressed;

        /// <summary>
        /// Calls the methods in its invocation list when the modifier key is released.
        /// </summary>
        event Action<BrushModifierKey> OnModifierReleased;

        /// <summary>
        /// Defines data when the tool is selected.
        /// </summary>
        void OnEnterToolMode();

        /// <summary>
        /// Defines data when the tool is deselected.
        /// </summary>
        void OnExitToolMode();

        /// <summary>
        /// Checks if the modifier key is active.
        /// </summary>
        /// <param name="k">The modifier key to check.</param>
        /// <returns>Returns <c>true</c> when the key is active.</returns>
        bool ModifierActive(BrushModifierKey k);
    }
}
