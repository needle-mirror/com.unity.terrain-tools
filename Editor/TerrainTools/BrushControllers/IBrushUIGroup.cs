using System.Text;
using UnityEngine;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Calls the methods in its invocation list when a brush's settings are reset.
    /// </summary>
    public delegate void ResetBrush();

    /// <summary>
    /// An interface that represent the brush's common UI.
    /// </summary>
    public interface IBrushUIGroup
    {
        /// <summary>
        /// The normalized size of the brush.
        /// </summary>
        float brushSize { get; }

        /// <summary>
        /// The rotation of the brush (in degrees).
        /// </summary>
        float brushRotation { get; }

        /// <summary>
        /// The normalized strength of the brush when applied.
        /// </summary>
        float brushStrength { get; }

        /// <summary>
        /// The spacing used when applying certain brushes.
        /// </summary>
        float brushSpacing { get; }

        /// <summary>
        /// Gets and sets the message for validating terrain parameters.
        /// </summary>
        string validationMessage { get; set; }

        /// <summary>
        /// Are we allowed to paint with this brush?
        /// </summary>
        bool allowPaint { get; }

        /// <summary>
        /// Inverts the brush's strength.
        /// </summary>
        bool InvertStrength { get; }

        /// <summary>
        /// Checks if the brush is in use.
        /// </summary>
        bool isInUse { get; }

        /// <summary>
        /// Gets the brush mask's Filter stack view.
        /// </summary>
        FilterStackView brushMaskFilterStackView { get; }

        /// <summary>
        /// Gets the brush mask's Filter stack.
        /// </summary>
        FilterStack brushMaskFilterStack { get; }

        /// <summary>
        /// Checks if the brush has enabled filters.
        /// </summary>
        bool hasEnabledFilters { get; }

        /// <summary>
        /// Gets a reference to the terrain under the cursor.
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

        /// <summary>
        /// Renders the brush's GUI within the inspector view.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext used to show the brush GUI.</param>
        /// <param name="brushFlags">The brushflags to use when displaying the brush GUI.</param>
        void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext, BrushGUIEditFlags brushFlags = BrushGUIEditFlags.SelectAndInspect);

        /// <summary>
        /// Defines data when the brush is selected.
        /// </summary>
        /// <seealso cref="OnExitToolMode"/>
        void OnEnterToolMode();

        /// <summary>
        /// Defines data when the brush is deselected.
        /// </summary>
        /// <seealso cref="OnEnterToolMode"/>
        void OnExitToolMode();

        /// <summary>
        /// Triggers events when painting on a terrain.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        void OnPaint(Terrain terrain, IOnPaint editContext);

        /// <summary>
        /// Triggers events to render a 2D GUI within the Scene view.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        /// <seealso cref="OnSceneGUI(Terrain, IOnSceneGUI)"/>
        void OnSceneGUI2D(Terrain terrain, IOnSceneGUI editContext);

        /// <summary>
        /// Triggers events to render objects and displays within Scene view.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        /// <seealso cref="OnSceneGUI(Terrain, IOnSceneGUI)"/>
        void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext);

        /// <summary>
        /// Adds basic information to the selected brush.
        /// </summary>
        /// <param name="terrain">The Terrain in focus.</param>
        /// <param name="editContext">The IOnSceneGUI to reference.</param>
        /// <param name="builder">The StringBuilder containing the brush information.</param>
        void AppendBrushInfo(Terrain terrain, IOnSceneGUI editContext, StringBuilder builder);

        /// <summary>
        /// Generates the brush mask.
        /// </summary>
        /// <param name="sourceRenderTexture">The source render texture to blit from.</param>
        /// <param name="destinationRenderTexture">The destination render texture for bliting to.</param>
        /// <seealso cref="GenerateBrushMask(Terrain, RenderTexture, RenderTexture, Vector3, float, float)"/>
        /// <remarks>Use this overload method to let Unity handle passing the brush's parameters and terrain reference to the main GenerateBrushMask meethod.</remarks>
        void GenerateBrushMask(RenderTexture sourceRenderTexture, RenderTexture destinationRenderTexture);

        /// <summary>
        /// Generates the brush mask.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="sourceRenderTexture">The source render texture to blit from.</param>
        /// <param name="destinationRenderTexture">The destination render texture for bliting to.</param>
        /// <seealso cref="GenerateBrushMask(Terrain, RenderTexture, RenderTexture, Vector3, float, float)"/>
        /// <remarks>Use this overload method to let Unity handle passing the brush's parameters to the main GenerateBrushMask meethod.</remarks>
        void GenerateBrushMask(Terrain terrain, RenderTexture sourceRenderTexture, RenderTexture destinationRenderTexture);

        /// <summary>
        /// Generates the brush mask.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="sourceRenderTexture">The source render texture to blit from.</param>
        /// <param name="destinationRenderTexture">The destination render texture for bliting to.</param>
        /// <param name="position">The brush's position.</param>
        /// <param name="scale">The brush's scale.</param>
        /// <param name="rotation">The brush's rotation.</param>
        /// <remarks>This is the main overload method for generating brush mask.</remarks>
        void GenerateBrushMask(Terrain terrain, RenderTexture sourceRenderTexture, RenderTexture destinationRenderTexture, Vector3 position, float scale, float rotation);

        /// <summary>
        /// Scatters the brush around the specified UV on the specified terrain. If the scattered UV leaves
        /// the current terrain then the terrain AND UV are modified for the terrain the UV is now over.
        /// </summary>
        /// <param name="terrain">The terrain the scattered UV co-ordinate is actually on.</param>
        /// <param name="uv">The UV co-ordinate passed in transformed into the UV co-ordinate relative to the scattered terrain.</param>
        /// <returns>"true" if we scattered to a terrain, "false" if we fell off ALL terrains.</returns>
        bool ScatterBrushStamp(ref Terrain terrain, ref Vector2 uv);

        /// <summary>
        /// Activates a modifier key controller.
        /// </summary>
        /// <param name="k">The modifier key to activate.</param>
        /// <returns>Returns false when the modifier key controller is null.</returns>
        bool ModifierActive(BrushModifierKey k);
    }
}
