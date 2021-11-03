namespace UnityEditor.TerrainTools
{
    public interface IBrushSpacingController : IBrushController
    {
        float brushSpacing { get; }
        bool allowPaint { get; set; }
    }
}
