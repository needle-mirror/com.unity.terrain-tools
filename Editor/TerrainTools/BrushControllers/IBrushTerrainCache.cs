using UnityEngine;

namespace UnityEditor.TerrainTools
{
    public interface IBrushTerrainCache
    {
        void LockTerrainUnderCursor(bool cursorVisible);
        void UnlockTerrainUnderCursor();
        bool canUpdateTerrainUnderCursor { get; }

        Terrain terrainUnderCursor { get; }
        bool isRaycastHitUnderCursorValid { get; }
        RaycastHit raycastHitUnderCursor { get; }
    }
}
