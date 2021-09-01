
using System.Text;
using UnityEngine;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Represents an base terrain tools variator class.
    /// </summary>
    public abstract class BaseBrushVariator : IBrushController, IBrushTerrainCache
    {
        private readonly string m_NamePrefix;
        private readonly IBrushEventHandler m_EventHandler;
        private readonly IBrushTerrainCache m_TerrainCache;

        private static GUIStyle _s_SceneLabelStyle;

        /// <summary>
        /// Gets the the GUIStyle of the scene's label.
        /// </summary>
        protected static GUIStyle s_SceneLabelStyle {
            get
            {
                if (_s_SceneLabelStyle != null)
                {
                    return _s_SceneLabelStyle;
                }

                _s_SceneLabelStyle = new GUIStyle
                {
                    normal = new GUIStyleState()
                    {
                        background = Texture2D.whiteTexture
                    },
                    fontSize = 12
                };

                return _s_SceneLabelStyle;
            }
        }

        /// <summary>
        /// Checks if the brush is in use.
        /// </summary>
        public virtual bool isInUse => m_ModifierActive;

        /// <summary>
        /// Initializes and returns an instance of BaseBrushVariator.
        /// </summary>
        /// <param name="toolName">The tool's name which the variator is used with.</param>
        /// <param name="eventHandler">The brush event handler.</param>
        /// <param name="terrainCache">The cache reference of the terrain.</param>
        protected BaseBrushVariator(string toolName, IBrushEventHandler eventHandler, IBrushTerrainCache terrainCache)
        {
            m_NamePrefix = toolName;
            m_EventHandler = eventHandler;
            m_TerrainCache = terrainCache;
        }

        /// <summary>
        /// Sets the repaint request to <c>true</c>.
        /// </summary>
        protected void RequestRepaint()
        {
            m_EventHandler.RequestRepaint();
        }

        private void OnModifierKeyPressed()
        {
            m_ModifierActive = true;
            HandleBeginModifier();
        }

        private void OnModifierKeyReleased()
        {
            HandleEndModifier();
            m_ModifierActive = false;
        }

        /// <summary>
        /// Gets the editor preferences for boolean values.
        /// </summary>
        /// <param name="name">The name of the preference.</param>
        /// <param name="defaultValue">The default value of the preference.</param>
        /// <returns>Returns the editor preference value corresponding to the name.</returns>
        /// <seealso cref="SetEditorPrefs(string, bool)"/>
        protected bool GetEditorPrefs(string name, bool defaultValue)
        {
            return EditorPrefs.GetBool($"{m_NamePrefix}.{name}", defaultValue);
        }

        /// <summary>
        /// Sets the editor preferences for boolean values.
        /// </summary>
        /// <param name="name">The name of the preference.</param>
        /// <param name="currentValue">The prefence value to set.</param>
        /// <seealso cref="GetEditorPrefs(string, bool)"/>
        protected void SetEditorPrefs(string name, bool currentValue)
        {
            EditorPrefs.SetBool($"{m_NamePrefix}.{name}", currentValue);
        }

        /// <summary>
        /// Gets the editor preferences for float values.
        /// </summary>
        /// <param name="name">The name of the preference.</param>
        /// <param name="defaultValue">The default value of the preference.</param>
        /// <returns>Returns the editor preference value corresponding to the name.</returns>
        /// <seealso cref="SetEditorPrefs(string, float)"/>
        protected float GetEditorPrefs(string name, float defaultValue)
        {
            return EditorPrefs.GetFloat($"{m_NamePrefix}.{name}", defaultValue);
        }

        /// <summary>
        /// Sets the editor preferences for float values.
        /// </summary>
        /// <param name="name">The name of the preference.</param>
        /// <param name="currentValue">The prefence value to set.</param>
        /// <seealso cref="GetEditorPrefs(string, float)"/>
        protected void SetEditorPrefs(string name, float currentValue)
        {
            EditorPrefs.SetFloat($"{m_NamePrefix}.{name}", currentValue);
        }

        private bool m_ModifierActive;
        private Vector2 m_InitialMousePosition;

        /// <summary>
        /// Calculates the difference between the mouses initial and current position.
        /// </summary>
        /// <param name="mouseEvent">The current mouse event.</param>
        /// <param name="scale">The scale to multiply the delta by.</param>
        /// <returns>Returns the difference between the initial and current position of the mouse.</returns>
        /// <seealso cref="CalculateMouseDelta(Event, float)"/>
        protected Vector2 CalculateMouseDeltaFromInitialPosition(Event mouseEvent, float scale = 1.0f)
        {
            Vector2 mousePosition = mouseEvent.mousePosition;
            Vector2 delta = m_InitialMousePosition - mousePosition;
            Vector2 scaledDelta = delta * scale;

            return scaledDelta;
        }

        /// <summary>
        /// Gets the mouses delta from the mouse event.
        /// </summary>
        /// <param name="mouseEvent">The current mouse event.</param>
        /// <param name="scale">The scale to multiply the delta by.</param>
        /// <returns>Returns the mouses delta.</returns>
        /// <seealso cref="CalculateMouseDeltaFromInitialPosition(Event, float)"/>
        protected static Vector2 CalculateMouseDelta(Event mouseEvent, float scale = 1.0f)
        {
            Vector2 delta = mouseEvent.delta;
            Vector2 scaledDelta = delta * scale;

            return scaledDelta;
        }

        /// <summary>
        /// Called when the Variator is initially modified.
        /// </summary>
        /// <remarks>This method is to be overriden when creating custom variators.</remarks>
        /// <returns>Returns <c>true</c> when the modification is successful. Otherwise, returns <c>false</c>.</returns>
        protected virtual bool OnBeginModifier()
        {
            return false;
        }

        /// <summary>
        /// Called when a mouse drag is used when modifying a Variator.
        /// </summary>
        /// <param name="mouseEvent">The current mouse event.</param>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        /// <remarks>This method is to be overriden when creating custom variators.</remarks>
        /// <returns>Returns <c>true</c> when the modification is successful. Otherwise, returns <c>false</c>.</returns>
        protected virtual bool OnModifierUsingMouseMove(Event mouseEvent, Terrain terrain, IOnSceneGUI editContext)
        {
            if (!m_ModifierActive)
            {
                m_InitialMousePosition = mouseEvent.mousePosition;
            }
            return false;
        }

        /// <summary>
        /// Called when a mouse wheel is used when modifying a Variator.
        /// </summary>
        /// <param name="mouseEvent">The current mouse event.</param>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        /// <remarks>This method is to be overriden when creating custom variators.</remarks>
        /// <returns>Returns <c>true</c> when the modification is successful. Otherwise, returns <c>false</c>.</returns>
        protected virtual bool OnModifierUsingMouseWheel(Event mouseEvent, Terrain terrain, IOnSceneGUI editContext)
        {
            return false;
        }

        /// <summary>
        /// Called when the Variator's modification has finished.
        /// </summary>
        /// <remarks>This method is to be overriden when creating custom variators.</remarks>
        /// <returns>Returns <c>true</c> when the modification is successful. Otherwise, returns <c>false</c>.</returns>
        protected virtual bool OnEndModifier()
        {
            return false;
        }

        private bool HandleBeginModifier()
        {
            bool consumeEvent = OnBeginModifier();

            return consumeEvent;
        }

        private bool HandleModifierUsingMouseMove(Event mouseEvent, Terrain terrain, IOnSceneGUI editContext)
        {
            bool consumeEvent = OnModifierUsingMouseMove(mouseEvent, terrain, editContext);

            return consumeEvent;
        }

        private bool HandleModifierUsingMouseWheel(Event mouseEvent, Terrain terrain, IOnSceneGUI editContext)
        {
            bool consumeEvent = OnModifierUsingMouseWheel(mouseEvent, terrain, editContext);

            return consumeEvent;
        }

        private bool HandleEndModifier()
        {
            bool consumeEvent = OnEndModifier();

            return consumeEvent;
        }

        private bool ProcessMouseEvent(Event mouseEvent, int controlId, Terrain terrain, IOnSceneGUI editContext)
        {
            bool consumeEvent = false;

            if (m_ModifierActive)
            {
                EventType eventType = mouseEvent.GetTypeForControl(controlId);

                switch (eventType)
                {
                    case EventType.MouseMove:
                        {
                            consumeEvent |= HandleModifierUsingMouseMove(mouseEvent, terrain, editContext);
                            break;
                        }

                    case EventType.ScrollWheel:
                        {
                            consumeEvent |= HandleModifierUsingMouseWheel(mouseEvent, terrain, editContext);
                            break;
                        }
                } // End of switch.
            }

            if (consumeEvent)
            {
                // We changed something - time to repaint...
                RequestRepaint();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Defines data when the brush is selected.
        /// </summary>
        /// <param name="shortcutHandler">The shortcut handler to subscribe brush shortcuts.</param>
        /// <seealso cref="OnExitToolMode(BrushShortcutHandler{BrushShortcutType})"/>
        /// <seealso cref="BrushShortcutHandler{TKey}.AddActions(TKey, System.Action, System.Action)"/>
        public virtual void OnEnterToolMode(BrushShortcutHandler<BrushShortcutType> shortcutHandler)
        {
            shortcutHandler.AddActions(BrushShortcutType.RotationSizeStrength, OnModifierKeyPressed, OnModifierKeyReleased);
        }

        /// <summary>
        /// Defines data when the brush is deselected.
        /// </summary>
        /// <param name="shortcutHandler">The shortcut handler to unsubscribe brush shortcuts.</param>
        /// <seealso cref="OnEnterToolMode(BrushShortcutHandler{BrushShortcutType})"/>
        /// <seealso cref="BrushShortcutHandler{TKey}.AddActions(TKey, System.Action, System.Action)"/>
        public virtual void OnExitToolMode(BrushShortcutHandler<BrushShortcutType> shortcutHandler)
        {
            shortcutHandler.RemoveActions(BrushShortcutType.RotationSizeStrength);
        }

        /// <summary>
        /// Triggers events to render objects and displays within Scene view.
        /// </summary>
        /// <param name="currentEvent">The event to check whether the mouse is in use.</param>
        /// <param name="controlId">The control ID of the GUI.</param>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference</param>
        public virtual void OnSceneGUI(Event currentEvent, int controlId, Terrain terrain, IOnSceneGUI editContext)
        {
            if (currentEvent.isMouse || currentEvent.isScrollWheel)
            {
                if (ProcessMouseEvent(currentEvent, controlId, terrain, editContext))
                {
                    m_EventHandler.RegisterEvent(currentEvent);
                }
            }
        }

        /// <summary>
        /// Renders the brush's GUI within the inspector view.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext used to show the brush GUI.</param>
        public virtual void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
        }

        /// <summary>
        /// Triggers events when painting on a terrain.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        /// <returns>Returns <c>true</c> if the paint opertation is succesful. Otherise, returns <c>false</c>.</returns>
        public virtual bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            return true;
        }

        /// <summary>
        /// Adds basic information to the selected brush.
        /// </summary>
        /// <param name="terrain">The Terrain in focus.</param>
        /// <param name="editContext">The IOnSceneGUI to reference.</param>
        /// <param name="builder">The StringBuilder containing the brush information.</param>
        public virtual void AppendBrushInfo(Terrain terrain, IOnSceneGUI editContext, StringBuilder builder)
        {
        }

        /// <summary>
        /// Gets and sets the terrain in focus.
        /// </summary>
        public Terrain terrainUnderCursor => m_TerrainCache.terrainUnderCursor;

        /// <summary>
        /// Gets and sets the value associated to whether there is a raycast hit detecting a terrain under the cursor.
        /// </summary>
        public bool isRaycastHitUnderCursorValid => m_TerrainCache.isRaycastHitUnderCursorValid;

        /// <summary>
        /// Gets and sets the raycast hit that was under the cursor's position.
        /// </summary>
        public RaycastHit raycastHitUnderCursor => m_TerrainCache.raycastHitUnderCursor;

        /// <summary>
        /// Checks if the cursor is currently locked and can not be updated.
        /// </summary>
        public bool canUpdateTerrainUnderCursor => m_TerrainCache.canUpdateTerrainUnderCursor;

        /// <summary>
        /// Handles the locking of the terrain cursor in it's current position.
        /// </summary>
        /// <remarks>This method is commonly used when utilizing shortcuts.</remarks>
        /// <param name="cursorVisible">Whether the cursor is visible within the scene. When the value is <c>true</c> the cursor is visible.</param>
        /// <seealso cref="UnlockTerrainUnderCursor"/>
        public void LockTerrainUnderCursor(bool cursorVisible)
        {
            m_TerrainCache.LockTerrainUnderCursor(cursorVisible);
        }

        /// <summary>
        /// Handles unlocking of the terrain cursor.
        /// </summary>
        /// <seealso cref="LockTerrainUnderCursor(bool)"/>
        public void UnlockTerrainUnderCursor()
        {
            m_TerrainCache.UnlockTerrainUnderCursor();
        }
    }
}
