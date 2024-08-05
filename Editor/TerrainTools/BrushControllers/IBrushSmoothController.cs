using UnityEngine;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represent the controller for smoothing the brush.
    /// </summary>
    public interface IBrushSmoothController
    {
        /// <summary>
        /// Checks if the smooth controller is active.
        /// </summary>
        bool active { get; }

        /// <summary>
        /// Gets and sets the smooth kernel size.
        /// </summary>
        int kernelSize { get; set; }

        /// <summary>
        /// Defines data when the brush is selected.
        /// </summary>
        void OnEnterToolMode();

        /// <summary>
        /// Defines data when the brush is deselected.
        /// </summary>
        void OnExitToolMode();

        /// <summary>
        /// Triggers events to render objects and displays within Scene view.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext);

        /// <summary>
        /// Triggers events when painting on a terrain.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        /// <param name="brushSize">The brush's size.</param>
        /// <param name="brushRotation">The brush's rotation.</param>
        /// <param name="brushStrength">The brush's strength.</param>
        /// <param name="uv">The brush's UV.</param>
        /// <returns>Returns <c>true</c> when the painting process is successful. Otherwise, returns <c>false</c>.</returns>
        bool OnPaint(Terrain terrain, IOnPaint editContext, float brushSize, float brushRotation, float brushStrength, Vector2 uv);
    }
}
