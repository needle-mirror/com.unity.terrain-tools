# Create a custom Terrain tool

Terrain Tools is a package that supplements the built-in Terrain and Editor Tools API. You can use it to create and develop your own custom tools for Terrain.

To create your own Terrain Tool, you only need one script. After you create the script, your tool appears in the list of available [Paint Terrain](paint-terrain.md) tools in the Inspector.

Additionally, if you want to override an existing tool, look up the string value that the built-in tool's `GetName()` method returns, and make sure your tool has the same name.

- [Create your tool script](create-tool-script.md)
- [Add UI for Brush controls](create-add-brush-controls.md)
- [Modify the Terrain heightmap](create-modify-terrain-heightmap.md)
- [Custom Terrain Tool shaders](create-use-custom-shaders.md)
- [Filter Stacks, Filters, and procedural masks](create-filterstacks-and-filters.md)
- [Shortcut handlers](create-shortcut-handlers.md)

## A note on tool changes between 5.0 and 5.1
To create a new terrain tool that fully supports overlays, derive the new class from `TerrainPaintToolWithOverlays`.

**Note:** Terrain paint tools that derive from `TerrainPaintTool` are still supported. However, they don't have a custom icon on the toolbar, and you can't control which category they fall under.

### Upgrade a tool to support overlays
In most cases, you can derive from `TerrainPaintToolWithOverlays` rather than`TerrainPaintTool` and then implement the functions you need to specify the tool icon.
**Note:** Although `TerrainPaintTool` inherited from `ScriptableSingleton`, `TerrainPaintToolWithOverlays` inherits from `EditorTools.EditorTool`. If your class relied on some `ScriptableSingleton` behavior, you must replicate that manually.