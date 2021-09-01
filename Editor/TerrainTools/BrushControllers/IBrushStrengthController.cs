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
    }
}
