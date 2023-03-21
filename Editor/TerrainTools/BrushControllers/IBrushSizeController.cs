namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represent the controller for the brush's size.
    /// </summary>
    public interface IBrushSizeController : IBrushController
    {
        /// <summary>
        /// Gets and sets the brush's size.
        /// </summary>
        float brushSize { get; set; }
        
        
        /// <summary>
        /// Gets the brush's size without jitter.
        /// </summary>
        float brushSizeVal { get; }
        
        /// <summary>
        /// Gets and sets the brush's min size. 
        /// </summary>
        float brushSizeMin { get; set; }
        
        /// <summary>
        /// Gets and sets the brush's max size. 
        /// </summary>
        float brushSizeMax { get; set; }
        
        /// <summary>
        /// Gets and sets the brush's jitter. 
        /// </summary>
        float brushSizeJitter { get; set; }

    }
}
