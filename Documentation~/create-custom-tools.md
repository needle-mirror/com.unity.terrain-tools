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
