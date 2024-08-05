using UnityEngine;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represent the brush terrain caching system.
    /// </summary>
    public interface IBrushTerrainCache
    {
        /// <summary>
        /// Handles the locking of the terrain cursor in it's current position.
        /// </summary>
        /// <remarks>This method is commonly used when utilizing shortcuts.</remarks>
        /// <param name="cursorVisible">Whether the cursor is visible within the scene. When the value is <c>true</c> the cursor is visible.</param>
        /// <seealso cref="UnlockTerrainUnderCursor"/>
        void LockTerrainUnderCursor(bool cursorVisible);

        /// <summary>
        /// Handles unlocking of the terrain cursor.
        /// </summary>
        /// <seealso cref="LockTerrainUnderCursor(bool)"/>
        void UnlockTerrainUnderCursor();

        /// <summary>
        /// Checks if the cursor is currently locked and can not be updated.
        /// </summary>
        bool canUpdateTerrainUnderCursor { get; }

        /// <summary>
        /// Gets and sets the terrain in focus.
        /// </summary>
        Terrain terrainUnderCursor { get; }

        /// <summary>
        /// Gets and sets the value associated to whether there is a raycast hit detecting a terrain under the cursor.
        /// </summary>
        bool isRaycastHitUnderCursorValid { get; }

        /// <summary>
        /// Gets and sets the raycast hit that was under the cursor's position.
        /// </summary>
        RaycastHit raycastHitUnderCursor { get; }
    }
}
