using UnityEngine;

namespace UnityEditor.TerrainTools
{
    public interface ITerrainToolPaintTool 
    {
        bool HasBrushFilters => false;
    }

    public abstract class TerrainToolsPaintTool<T> : TerrainPaintToolWithOverlays<T>, ITerrainToolPaintTool
        where T : TerrainToolsPaintTool<T>
    {
        [SerializeField]
        public IBrushUIGroup m_commonUI { get; protected set; }
        
        public virtual bool HasBrushFilters => false;
            
        protected TerrainToolsPaintTool() { }

        private static T s_instance;

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
    
    [System.Flags]
    public enum BrushOverlaysGUIFlags
    {
        None = 0,
        Filter = 1,
        Strength = 2,
        Size = 4,
        Rotation = 8,
        Spacing = 16,
        Scatter = 32, 
        All = Filter | Strength | Size | Rotation | Spacing | Scatter,
        
    }
}