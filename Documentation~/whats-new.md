# What’s new in 5.0.2
Summary of changes in Terrain Tools package version 5.0.2

The main updates in this release include:

## Added
- New [Details Painting and Scattering tool](paint-details.md) replaces the old Paint Details tool. The new tool enables brush mask filters for detail scattering, as well as scattering multiple detail types simultaneously.
- Layer filter added to the Brush Mask Filter list.
- New tool for optimizing heightmap ranges.

## Updated
- Minimum required Unity version updated to 2022.2.

## Fixed
- Fixed the UI of the Terrain Toolbox so that the elements resize properly and provide a better user experience.
- The hotkeys for brush strength, size, rotation have a tooltip that is displayed that shows the current value.
- Fixed a bug that involves the tooltip being obscured when the brush size is too large.
- Fixed terrain Toolbox's create terrain import heightmap's tiles mode not showing a warning and disabling the create button when a invalid file path is selected.
- Fixed selected parented terrains being improperly duplicated when using Toolbox's Utilities duplicate feature.
- Fixed terrain transform tools target UI disabling the tool when no targets are selected.
- Fixed transform tools being disabled when a user deselects both target options.
- Fixed terrain Toolbox gizmo tooltip not displaying the proper modifier key for the currently used platform.
- Fixed the improper modifier key being displayed within Create Terrain depending on the users Platform.
- Fixed warnings being thrown when a terrain is split and the detail resolution per patch is set to a value lower than the minimum.
- Fixed splitting terrain setting a detail resolution per patch lower than the minimum allowed value of 8.
- Fixed terrain toolbox's layers utilities not clearing the alphamap when "Clear Existing Layers" is selected.
- Fixed Import Heightmap error when flipping heightmap axis.
- Fixed the bridge tool's description, the wording was previously confusing in regards to setting an end point for the bridge.
- Fixed terrain altitude visualization not updating properly after parameters are altererd.
- Fixed null layers causing index out of bounds exceptions and missing layers in PaintTextureTool and TerrainToolboxUtilities.
- Updated the preview splatmaps shaders in Terrain Tools to use the Universal Rendering Pipeline rather than the old Lightweight Render Pipeline.
- Fixed a bug where Split tool doesn't transfer Light Layer Mask settings correctly.
- Fixed occasional null reference error when selecting paint tool.

For a full list of changes and updates in this version, see the Terrain Tools package [changelog](../changelog/CHANGELOG.html).