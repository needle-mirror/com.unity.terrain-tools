namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represent the controller for the brush's spacing.
    /// </summary>
    public interface IBrushSpacingController : IBrushController
    {
        /// <summary>
        /// Gets the brush's spacing.
        /// </summary>
        float brushSpacing { get; }

        /// <summary>
        /// Gets and sets the <c>bool</c> value that determines if painting is allowed. 
        /// </summary>
        bool allowPaint { get; set; }
    }
}
