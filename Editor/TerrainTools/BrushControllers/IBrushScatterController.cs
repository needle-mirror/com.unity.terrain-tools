using UnityEngine;

namespace UnityEditor.TerrainTools
{
    public interface IBrushScatterController : IBrushController
    {
        float brushScatter { get; }

        void RequestRandomisation();
        Vector2 ScatterBrushStamp(Vector2 uv, float brushSize);
    }
}
