namespace UnityEditor.TerrainTools
{
    public interface IBrushRotationController : IBrushController
    {
        float brushRotation { get; set; }
        float currentRotation { get; }
    }
}
