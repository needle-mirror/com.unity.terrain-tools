# Add UI for Brush controls

[Create your tool script](create-tool-script.md) showed you how to create a new custom tool without any additional functionality. The example below shows you how to add UI for Brush controls in your script.

```
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

class CustomTerrainToolWithBrushUI : TerrainPaintToolWithOverlays<CustomTerrainToolWithBrushUI>
{
    private float m_BrushRotation;

    // Override this function to add UI elements to the tool settings
    public override void OnToolSettingsGUI(Terrain terrain, IOnInspectorGUI editContext)
    {
        EditorGUILayout.HelpBox("Rotation is specific to this tool", MessageType.Info);
        m_BrushRotation = EditorGUILayout.Slider("Rotation", m_BrushRotation, 0, 360);
    }

    // You can call the tool settings here to duplicate the tool settings UI in the inspector.
    public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
    {
        // This example only draws the brushes in the inspector code. The overlay has its own window for the
        // brushes, which is why the OnToolsSettingsGUI method doesn't contain this.
        editContext.ShowBrushesGUI(5, BrushGUIEditFlags.Select | BrushGUIEditFlags.Opacity | BrushGUIEditFlags.Size);

        OnToolSettingsGUI(terrain, editContext);
    }

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
    
    // Return true for this property to display the brush attributes overlay
    public override bool HasBrushAttributes => true;

    // Return true for this property to display the brush selector overlay
    public override bool HasBrushMask => true;

    // Return true for this property to display the tool settings overlay
    public override bool HasToolSettings => true;

    // File names of the light theme icons - prepend d_ for the file name of dark theme variants.
    // Override these if you want to specify your own icon.
    // public override string OnIcon => "Assets/Icon_on.png";   // Will use dark icon at 'Assets/d_Icons_on.png' if available.
    // public override string OffIcon => "Assets/Icon_off.png"; // Will use dark icon at 'Assets/d_Icons_off.png' if available.

    // The toolbar category the icon appears under.
    public override TerrainCategory Category => TerrainCategory.CustomBrushes;

    // Where in the icon list the icon appears.
    public override int IconIndex => 100;
}
```

When you create UI for your tool, you can specify both the UI that can appear in the tool settings overlay and the UI that appears in the inspector. Most of the time, these are identical, so we recommend that you implement your UI in `OnToolSettingsGUI` and call it from `OnInspectorGUI`. 

**Note**: For the Tools Settings overlay to appear, you must override the `HasToolSettings` property to return true.
