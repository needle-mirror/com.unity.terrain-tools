namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represent the controller for the brush's size.
    /// </summary>
    public interface IBrushSizeController : IBrushController
    {
        /// <summary>
        /// Gets and sets the brush's rotation.
        /// </summary>
        float brushSize { get; set; }
    }
}
