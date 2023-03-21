namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represent the controller for the brush's strength.
    /// </summary>
    public interface IBrushStrengthController : IBrushController
    {
        /// <summary>
        /// Gets and sets the brush's strength. 
        /// </summary>
        float brushStrength { get; set; }
        
        /// <summary>
        /// The strength of the brush without jitter.
        /// </summary>
        float brushStrengthVal { get; }
        
        /// <summary>
        /// Gets and sets the brush's min strength. 
        /// </summary>
        float brushStrengthMin { get; set; }
        
        /// <summary>
        /// Gets and sets the brush's max strength. 
        /// </summary>
        float brushStrengthMax { get; set; }
        
        /// <summary>
        /// Gets and sets the brush's jitter. 
        /// </summary>
        float brushStrengthJitter { get; set; }
        
        
    }
}
