# Add UI for Brush controls

[Create your tool script](create-tool-script.md) showed you how to create a new custom tool without any additional functionality. The example below shows you how to add UI for Brush controls in your script.

```
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

internal class CustomTerrainToolWithBrushUI : TerrainPaintTool<CustomTerrainToolWithBrushUI>
{
    private float m_BrushRotation;

    // Name of the Terrain Tool. This appears in the tool UI.
    public override string GetName()
    {
        return "Examples/Custom Terrain Tool with Brush UI";
    }

    // Description for the Terrain Tool. This appears in the tool UI.
    public override string GetDesc()
    {
        return "This is a very basic Terrain Tool that doesn't do anything aside from appear in the list of Paint Terrain tools.";
    }

    public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
    {
        editContext.ShowBrushesGUI(5, BrushGUIEditFlags.Select | BrushGUIEditFlags.Opacity | BrushGUIEditFlags.Size);
        m_BrushRotation = EditorGUILayout.Slider("Rotation", m_BrushRotation, 0, 360);
    }

    public override void OnRenderBrushPreview(Terrain terrain, IOnSceneGUI editContext)
    {

    }

    public override bool OnPaint(Terrain terrain, IOnPaint editContext)
    {
        return true;
    }
}
```

This example still doesn't do much, but it does give some useful information about the Brush.
