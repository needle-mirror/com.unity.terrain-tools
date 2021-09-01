# Shortcut handlers

You can use the `ShortcutManagement` API to add shortcuts to your custom Terrain tools. Shortcut handlers let you use hotkeys to select specific tools or quickly modify tool settings without the need for you to interact with the tool's UI.

```
using UnityEngine;
using UnityEditor.ShortcutManagement;
using UnityEditor.TerrainTools;

internal class CustomTerrainToolWithShortcut : TerrainPaintTool<CustomTerrainToolWithShortcut>
{
    // Add the Shortcut attribute and provide the name of the shortcut and context Type
    [Shortcut("Terrain/Select CustomTerrainToolWithShortcut", typeof(UnityEditor.TerrainTools.TerrainToolShortcutContext))]
    static void SelectShortcut(ShortcutArguments args)
    {
        // Get the TerrainTool-specifc context from the ShortcutArguments
        UnityEditor.TerrainTools.TerrainToolShortcutContext context = (UnityEditor.TerrainTools.TerrainToolShortcutContext)args.context;
        // Select this tool
        context.SelectPaintTool<CustomTerrainToolWithShortcut>();
    }

    public override string GetName()
    {
        return "Custom Terrain Tool With Shortcut";
    }

    public override string GetDesc()
    {
        return "My custom Terrain Tool is amazing!";
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
