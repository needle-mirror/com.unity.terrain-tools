using UnityEngine;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represents a terrain paint tool.
    /// </summary>
    public interface ITerrainToolPaintTool
    {
        /// <summary>
        /// Gets whether the paint tool supports brush filters.
        /// </summary>
        bool HasBrushFilters => false;
    }

    /// <summary>
    /// Base class for terrain paint tools that provides common functionality and brush UI support.
    /// </summary>
    /// <typeparam name="T">The specific type of terrain paint tool implementing this class.</typeparam>
    public abstract class TerrainToolsPaintTool<T> : TerrainPaintToolWithOverlays<T>, ITerrainToolPaintTool
        where T : TerrainToolsPaintTool<T>
    {
        /// <summary>
        /// The common brush UI group associated with this paint tool.
        /// </summary>
        public IBrushUIGroup m_commonUI { get; protected set; }

        /// <summary>
        /// Gets whether the paint tool supports brush filters.
        /// </summary>
        public virtual bool HasBrushFilters => false;

        /// <summary>
        /// Protected constructor for terrain paint tools.
        /// </summary>
        protected TerrainToolsPaintTool() { }

        private static T s_instance;

        /// <summary>
        /// Gets the instance of the terrain paint tool.
        /// </summary>
        public static T instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = ScriptableObject.CreateInstance<T>();
                }
                return s_instance;
            }
        }
    }

    /// <summary>
    /// Enumeration of brush overlay GUI flags that determine which brush controls are displayed.
    /// </summary>
    [System.Flags]
    public enum BrushOverlaysGUIFlags
    {
        /// <summary>
        /// No brush overlays.
        /// </summary>
        None = 0,

        /// <summary>
        /// Show filter overlay.
        /// </summary>
        Filter = 1,

        /// <summary>
        /// Show strength overlay.
        /// </summary>
        Strength = 2,

        /// <summary>
        /// Show size overlay.
        /// </summary>
        Size = 4,

        /// <summary>
        /// Show rotation overlay.
        /// </summary>
        Rotation = 8,

        /// <summary>
        /// Show spacing overlay.
        /// </summary>
        Spacing = 16,

        /// <summary>
        /// Show scatter overlay.
        /// </summary>
        Scatter = 32,

        /// <summary>
        /// Show all brush overlays.
        /// </summary>
        All = Filter | Strength | Size | Rotation | Spacing | Scatter,

    }
}
