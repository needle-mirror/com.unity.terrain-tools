# What’s new in 4.0.0
Summary of changes in Terrain Tools package version 4.0.0.

The main updates in this release include:

## Added
- Added Brush Mask Filter previews to all Brushes.
- Added more warnings to inform users about potential conflicts when they use shaders that might be unsuitable for Terrain.

## Updated
- Updated the Paint Holes Brush preview to help users visualize the area of the Brush Mask in use. Parts of the Terrain affected by the Paint Holes tool are now more obvious to users.
- Updated Stamp Terrain to include additional functionality such as Min and Max operations that provide finer control over Terrain. The Stamp Height offset is now calculated relative to the height under the cursor rather than the tile space. The Mesh Stamp tool was merged into Stamp Terrain. For more information, see [Stamp Terrain](stamp-terrain.md).
- Updated the Terrain Tool’s Min and Max sliders to improve the user experience and provide a more intuitive UI layout.
- Updated tooltips that were missing or unclear. Also improved Brush shortcut tooltip behaviors in Scene view.
- Changed a portion of the Terrain Tools API access modifiers from public to internal.

## Fixed
- Fixed error by constraining Terrain splits to split equally along the X and Z axes. This approach works best as the heightmap resolution of a Terrain must be equal along the X and Z axes.
- Fixed the Terrain Toolbox window so that it scales properly when docked.
- Fixed the Paint Texture tool’s memory leak when users interacted with the Layers UI. Fixed redo and undo artifacting when users removed Terrain Layers. Fixed display issues with custom Terrain Layer GUIs.
- Fixed HDRP Visualization shader method errors for HDRP version 10.x and above. Terrain Tools Visualization now works as expected on OpenGL.
- Fixed an issue where Terrain Layers + button was disabled when users first opened the window. Users can now redo and undo splatmap data affected by changes to Terrain Layers.

For a full list of changes and updates in this version, see the Terrain Tools package [changelog](../changelog/CHANGELOG.html).