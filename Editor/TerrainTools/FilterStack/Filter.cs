using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Provides methods for implementing Filters that operate on Textures. Used as elements in a FilterStack but can also be
    /// used by themselves. Inherit from this class and override functions to create your own Filter.
    /// </summary>
    [System.Serializable]
    public class Filter : ScriptableObject
    {
        /// <summary>
        /// Flag that is used to determine whether or not the Filter is enabled and should be evaluated when part of a
        /// FilterStack. If it is not enabled, it is skipped during FilterStack evaluation
        /// </summary>
        [ SerializeField ]
        public bool              enabled = true;

        /// <summary>
        /// Gets the display name for the Filter. This is used when drawing the Filter's user interface.
        /// </summary>
        /// <returns>Returns the display name of the Filter.</returns>
        public virtual string GetDisplayName() => "EMPTY_FILTER_NAME";

        /// <summary>
        /// Gets the tooltip for the Filter.
        /// </summary>
        /// <returns>Returns the tooltip of the Filter.</returns>
        public virtual string GetToolTip() => "EMPTY_TOOLTIP";

        /// <summary>
        /// Sets up necessary data needed before the Filter is evaluated. 
        /// </summary>
        /// <remarks>
        /// While the data is being set the system is informed
        /// whether or not the Filter is supported based on current hardware, the data provided by the provided
        /// FilterContext, etc. This is completed by returning True or False.
        /// This method is called before OnEval.
        /// </remarks>
        /// <param name="filterContext">The FilterContext related to the current evaluation</param>
        /// <param name="message">The message to display from validation results.</param>
        /// <returns>Whether or not the Filter is supported and should be evaluated. True = supported and is okay to evaluate.
        /// False = the Filter is not supported with the current hardware and should be skipped.</returns>
        public virtual bool ValidateFilter(FilterContext filterContext, out string message)
        {
            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Evaluates the Filter. 
        /// </summary>
        /// <remarks>
        /// This method calls <see cref="ValidateFilter(FilterContext, out string)"/> and checks if it returns false performing a default blit instead of
        /// calling OnEval.
        /// </remarks>
        /// <param name="filterContext">The FilterContext related to the current evaluation.</param>
        /// <param name="source">The source RenderTexture on which the Filter operates.</param>
        /// <param name="dest">The destination RenderTexture to which the Filter blits the results of the Filter evaluation.</param>
        public void Eval(FilterContext filterContext, RenderTexture source, RenderTexture dest)
        {
            if (filterContext == null || dest == null || !ValidateFilter(filterContext, out var message))
            {
                // perform default blit in this case so eval can continue
                Graphics.Blit(source, dest);
                return;
            }

            OnEval(filterContext, source, dest);
        }

        /// <summary>
        /// Evaluates the Filter. Override this function for custom Filter logic.
        /// </summary>
        /// <param name="filterContext">The FilterContext related to the current evaluation.</param>
        /// <param name="source">The source RenderTexture on which the Filter operates.</param>
        /// <param name="dest">The destination RenderTexture to which the Filter blits the results of the Filter operation.</param>
        protected virtual void OnEval(FilterContext filterContext, RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(source, dest);
        }

        /// <summary>
        /// Draws the GUI for the Filter.
        /// </summary>
        /// <param name="rect">The Rect where the GUI should be drawn.</param>
        /// <param name="filterContext">The FilterContext related to the current evaluation. Use this to show different information in the Filter GUI.</param>
        public void DrawGUI(Rect rect, FilterContext filterContext)
        {
            if (!ValidateFilter(filterContext, out var message))
            {
                EditorGUI.HelpBox(rect, string.Empty, MessageType.Warning);
                var labelRect = new Rect(rect.x + 36f, rect.y + 2f, rect.width - 40f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelRect, $"{GetDisplayName()} will not be evaluated.", EditorStyles.wordWrappedLabel);
                labelRect.y = labelRect.yMax;
                labelRect.height = rect.height - labelRect.height;
                EditorGUI.LabelField(labelRect, message, EditorStyles.wordWrappedMiniLabel);
                return;
            }

            OnDrawGUI(rect, filterContext);
        }

        /// <summary>
        /// Draws the GUI for the Filter.
        /// </summary>
        /// <param name="rect">The Rect where the GUI should be drawn.</param>
        /// <param name="filterContext">The FilterContext related to the current evaluation. Use this to show different information in the Filter GUI.</param>
        protected virtual void OnDrawGUI(Rect rect, FilterContext filterContext) { }

        /// <summary>
        /// Handles SceneView related logic or rendering.
        /// </summary>
        /// <param name="sceneView">The <see cref="SceneView"/> in focus.</param>
        /// <param name="filterContext">The FilterContext related to the current evaluation.</param>
        public void SceneGUI(SceneView sceneView, FilterContext filterContext)
        {
            if (!ValidateFilter(filterContext, out var message))
            {
                return;
            }

            OnSceneGUI(sceneView, filterContext);
        }
        /// <summary>
        /// Handles SceneView related logic or rendering.
        /// </summary>
        /// <param name="sceneView">The <see cref="SceneView"/> in focus.</param>
        /// <param name="filterContext">The FilterContext related to the current evaluation.</param>
        protected virtual void OnSceneGUI(SceneView sceneView, FilterContext filterContext) { }
        /// <summary>
        /// Gets the height of the Filter element when drawn as part of a FilterStack GUI.
        /// </summary>
        /// <returns>The height of the Filter in the FilterStack GUI.</returns>
        public virtual float GetElementHeight() => EditorGUIUtility.singleLineHeight * 2;
        /// <summary>
        /// Sets data when the Filter is first created.
        /// </summary>
        public virtual void OnEnable() { }
        /// <summary>
        /// Sets data when the Filter is disabled.
        /// </summary>
        public virtual void OnDisable() { }
        /// <summary>
        /// Gets a list of Unity Objects to serialize along with the Filter object.
        /// </summary>
        /// <returns>List of Unity Objects to serialize.</returns>
        public virtual List<UnityEngine.Object> GetObjectsToSerialize() { return null; }
    }
}