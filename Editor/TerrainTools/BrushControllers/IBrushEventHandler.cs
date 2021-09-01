using UnityEngine;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represent handling methods for brush events.
    /// </summary>
    public interface IBrushEventHandler
    {
        /// <summary>
        /// Register a system event for processing later.
        /// </summary>
        /// <param name="newEvent">The event to register.</param>
        void RegisterEvent(Event newEvent);

        /// <summary>
        /// Consume previously registered events.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The IOnSceneGUI to reference.</param>
        void ConsumeEvents(Terrain terrain, IOnSceneGUI editContext);

        /// <summary>
        /// Allows us to request a repaint of the GUI and scene-view.
        /// </summary>
        void RequestRepaint();
    }
}
