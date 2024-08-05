
using System.Text;
using UnityEngine;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represents a brush's controller.
    /// </summary>
	/// <remarks>This interface is used for implementing custom controllers for handling brush properties.</remarks>
    /// <seealso cref="IBrushStrengthController"/>
	/// <seealso cref="IBrushSizeController"/>
	/// <seealso cref="IBrushRotationController"/>
	/// <seealso cref="IBrushScatterController"/>
    /// <seealso cref="IBrushSpacingController"/>
    /// <seealso cref="IBrushSmoothController"/>
    /// <seealso cref="IBrushModifierKeyController"/>
    public interface IBrushController
	{
		/// <summary>
		/// Determines if the brush controller is in use.
		/// </summary>
		bool isInUse { get; }
		
		/// <summary>
		/// Defines data when the brush is selected.
		/// </summary>
		/// <param name="shortcutHandler">The shortcut handler used to add and refernce shortcuts.</param>
		void OnEnterToolMode(BrushShortcutHandler<BrushShortcutType> shortcutHandler);

		/// <summary>
		/// Defines data when the brush is deselected.
		/// </summary>
		/// <param name="shortcutHandler">The shortcut handler used to add and refernce shortcuts.</param>
		void OnExitToolMode(BrushShortcutHandler<BrushShortcutType> shortcutHandler);

        /// <summary>
        /// Triggers events to render objects and displays within Scene view.
        /// </summary>
        /// <param name="currentEvent">The event state within the OnSceneGUI call.</param>
        /// <param name="controlId">The control identification of the OnSceneGUI</param>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        void OnSceneGUI(Event currentEvent, int controlId, Terrain terrain, IOnSceneGUI editContext);

        /// <summary>
        /// Triggers events to render objects and displays within Scene view.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext);

        /// <summary>
        /// Triggers events when painting on a terrain.
        /// </summary>
        /// <param name="terrain">The <see cref="Terrain"/> in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        /// <returns>Returns <c>true</c> if the paint operation is succesful. Otherwise, returns <c>false</c>.</returns>
        bool OnPaint(Terrain terrain, IOnPaint editContext);

        /// <summary>
        /// Adds basic information to the selected brush.
        /// </summary>
        /// <param name="terrain">The <see cref="Terrain"/> in focus.</param>
        /// <param name="editContext">The <see cref="IOnSceneGUI"/> to reference.</param>
        /// <param name="builder">The <see cref="StringBuilder"/> containing the brush information. </param>
        void AppendBrushInfo(Terrain terrain, IOnSceneGUI editContext, StringBuilder builder);
	}
}
