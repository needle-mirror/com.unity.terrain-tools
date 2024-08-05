# Add UI for Brush controls

[Create your tool script](create-tool-script.md) showed you how to create a new custom tool without any additional functionality. The example below shows you how to add UI for Brush controls in your script.

```
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

class CustomTerrainToolWithBrushUI : TerrainPaintTool<CustomTerrainToolWithBrushUI>
{
    private float m_BrushRotation;

    // Name of the Terrain Tool. This appears in the tool UI.
    public override string GetName()
    {
        return "Examples/Custom Terrain Tool with Brush UI";
    }

    // Description for the Terrain Tool. This appears in the tool UI.
    public override string GetDescription()
    {
        return "This terrain tool shows how to add custom UI to a tool.";
    }

    // Override this function to add UI elements to the inspector
    public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
    {
        editContext.ShowBrushesGUI(5, BrushGUIEditFlags.Select | BrushGUIEditFlags.Opacity | BrushGUIEditFlags.Size);

        EditorGUILayout.HelpBox("Rotation is specific to this tool", MessageType.Info);
        m_BrushRotation = EditorGUILayout.Slider("Rotation", m_BrushRotation, 0, 360);
    }
}
```
