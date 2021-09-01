using System;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Sets which shortcut type to use when utilizing the common brush shortcuts.
    /// </summary>
    [Flags]
    public enum BrushShortcutType
    {
        /// <summary>
        /// Use the Rotation shortcut.
        /// </summary>
        Rotation = 1 << 0,

        /// <summary>
        /// Use the Size shortcut.
        /// </summary>
        Size = 1 << 1,

        /// <summary>
        /// Use the Strength shortcut.
        /// </summary>
        Strength = 1 << 2,

        /// <summary>
        /// Use multiple shortcuts.
        /// </summary>
        RotationSizeStrength = Rotation | Size | Strength,
    }
}
