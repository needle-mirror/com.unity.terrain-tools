using System;

namespace UnityEditor.TerrainTools
{
    [Flags]
    public enum BrushShortcutType
    {
        Rotation = 1 << 0,
        Size = 1 << 1,
        Strength = 1 << 2,

        RotationSizeStrength = Rotation | Size | Strength,
    }
}
