using UnityEngine;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represent the controller for scattering the brush.
    /// </summary>
    public interface IBrushScatterController : IBrushController
    {
        /// <summary>
        /// Gets the brush's scatter value.
        /// </summary>
        float brushScatter { get; }

        /// <summary>
        /// Randomizes the brush location for scattering.
        /// </summary>
        void RequestRandomisation();

        /// <summary>
        /// Gets the scatter brush stamp location
        /// </summary>
        /// <param name="uv">The UV location of the brush.</param>
        /// <param name="brushSize">The size of the brush.</param>
        /// <returns>Returns the new scattered UV location.</returns>
        Vector2 ScatterBrushStamp(Vector2 uv, float brushSize);
    }
}
