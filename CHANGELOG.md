# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [5.0.5] - 2024-08-05
### Changed:
- Updated version number to 5.0.5.

## [5.0.4] - 2023-10-23
### Fixed:
- Fixed the altitude heatmap visualization shader for more recent versions of HDRP.

## [5.0.3] - 2023-01-23
### Changed:
- Fixed the example code in the documentation to work with the latest version of terrain tools. 

## [5.0.2] - 2022-10-12
### Fixed:
- Replaced the old v4.0.0 What's new page with the new updated 5.0.2 one. 
- Fixed null reference error when painting.
- Removed faulty test from unit suite.

### Changed:
- Updated version number to 5.0.2.

## [5.0.1] - 2022-08-25
### Fixed:
- Fixed some broken code examples in the documentation.
- Button to link to example assets now shows in package manager if user is using URP or HDRP.


## [5.0.0] - 2022-04-29
### Added:
- Added Paint Details tool override including multi-detail scatter, new detail selection UI, and the detail distribution slider.
- Added Layer Filter to the Brush Mask Filter list. 
- Added tool for optimizing heightmap range.

### Changed:
- Updated version number to 5.0.0, minumum Unity version to 2022.2.

### Fixed:
- Fixed the UI of the Terrain Toolbox so that the elements resize properly and provide a better user experience.
- The hotkeys for brush strength, size, rotation have a tooltip that is displayed that shows the current value. Fixed a bug that involves the tooltip being obscured when the brush size is too large. 
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
- Fixed a bug where Split tool doesn't transfer Light Layer Mask settings correctly

## [4.0.3] - 2021-09-29
### Fixed:
- Fixed different line endings in NoiseLib.cs.

### Changed:
- No longer creating a material asset for Terrain visualization.

## [4.0.2] - 2021-09-29
### Changed:
- Moved visualization material into user assets directory, since packagecache expects only immutable assets to exist in the package.

## [4.0.1] - 2021-09-27
### Changed:
- Modified include path in builtin visualization shader.

## [4.0.0] - 2021-09-01
### Added:
- Added Brush Mask Filter previews to all Paint Terrain tools.
- Added the option to let you set default parameter values for Brushes.
- Added a warning message (specifically for Vulkan and DX12) about Brush sizes and heightmaps that are possibly too large to allocate in GPU memory.

### Changed:
- Improved Brush preview feedback when you paint holes to make it more obvious which parts of the Terrain the Paint Holes tool affects.
- Changed Noise Editor window behavior so that when you open the Noise Editor window, Unity opens an existing Noise Editor window instead of creating a new one.
- Changed the way Unity handles NaN and INF input for the transform values of Noise Settings.
- Changed the Paint Height tool's Brush Strength slider range to cover more usable values, and changed the default Brush Strength on various other tools.
- Improved Brush descriptions for standardization, and to make the descriptions for different sculpting brushes easier to understand.
- Moved Mesh Stamp tool features into the Stamp Terrain tool.
- Moved Terrain Toolbox warnings from the Console to the Inspector window.

### Fixed:
- Fixed RenderTexture leak in `ToolboxHelper.CopyTextureToTerrainHeight`.
- Fixed an exception that occurred when you specified an empty directory path in the Export Splatmaps or Export Heightmaps tool.
- Fixed serialization errors in the Noise Editor when you reload scenes.
- Fixed the Terrain Toolbox window so that it scales properly when docked.
- Fixed the Terrain Toolbox folder path GUI so that it now sizes correctly and consistently.
- Constrained Terrain splits to split equally along the X and Z axes, since heightmap resolution must be equal along the X and Z axes. (case 1358022)
- Fixed CopyTexture error due to mipcount mismatch when you added Terrain splatmaps.
- Fixed Remove Terrain so that it no longer removes child objects. It now only removes Terrain components, and leaves everything else intact. GameObjects without children nor components aren't affected.
- Fixed the Split tool so that it no longer deletes the selected Terrain GameObject. Instead, Unity removes the Terrain component, and makes the selected Terrain parent of all subtiles. Also now, you can't undo a split operation.
- Fixed an issue where the `NoiseLib` constructor required a large amount of CPU time when you entered and exited play mode, by using `TypeCache` instead.
- Fixed an issue where the split Terrain process was slow when there were many trees.
- Fixed the Split tool to prevent the split of Terrain with split values <= 1.
- Fixed an issue with overlapping Brush Mask Filter property fields.
- Fixed incorrect positioning of the Noise Field Preview tooltip.
- Fixed distorted reset icons.

## [4.0.0-pre.2] - 2021-05-12
### Fixed:
- Fixed warnings about meta files for empty or non-existent directories.

## [4.0.0-pre.1] - 2021-05-11
### Changed:
- Moved Terrain APIs out of Experimental.
- Added additional Stamp Height behaviors, and changed the stamp operation so that it's now based off the Terrain height under the cursor.

### Added:
- Added a tooltip for the Tile Height Resolution property.

### Fixed:
- Fixed a Render Texture leak in `FilterContext`.
- Fixed an issue where the Brush cursor disappeared after you removed a Brush Mask Filter.
- Fixed an exception that occasionally appeared when you created a new Terrain with custom `TerrainData`.
- Fixed an issue where warnings appeared when you used the Mesh Stamp tool.
- Fixed an issue where warnings sometimes appeared when you set a Terrain's height.
- Fixed an issue where Brush Masks and Filters didn't rotate properly when you rotated the Brush.
- Fixed a bug where gaps appeared between Terrain objects when you used the Terrain Toolbox to resize existing Terrain.
- Fixed an issue where Altitude Heatmap settings were lost when you reduced or restored the number of levels.
- Fixed an issue where Import Heightmap settings were applied even when you created new Terrain with the Import Heightmap checkbox disabled.
- Fixed display issues with custom Terrain Layer GUIs when you set HDRP or URP as the current render pipeline.
- Fixed an issue where Altitude Heatmap Visualization didn't render properly in URP with Deferred Lighting.
- Fixed an issue where Detail Meshes were removed from Terrain objects after you used the Terrain Toolbox to split them.
- Fixed some tooltip alignment issues. Tooltips should now properly appear beneath your cursor.
- Fixed an issue where the Bridge Tool anchor didn't resize properly when you changed the Brush Size.
- Fixed an issue where the Brush Size slider didn't update properly  when you changed the Brush Size.
- Fixed an issue where tooltips didn't appear when you pressed multiple hotkeys at the same time.
- Fixed an issue where the Altitude Heightmap didn't render properly when you set the Graphics API to OpenGL.
- Fixed a Render Texture leak when you used the Hydraulic Erosion Tool.
- Fixed an exception that occasionally occurred when you attempted to add a Terrain Layer that had already been added.
- Fixed an issue where paint actions wouldn't undo properly if you had five or more Terrain Layers on your Terrain.

## [3.0.2-preview.3] - 2020-11-04
### Fixed
- Fixed regression with SetHeight Flatten Tile option
- Fixed brush mask filters with SetHeight painting

## [3.0.2-preview.2] - 2020-11-02
### Added
- Editor Analytics integration.
- Fallback shader to the HDRP Visualization shader.
- PNG/TGA heightmaps support level remapping and vertical flipping.
- Create New Terrain supports heightmaps in Texture2D format.
- Validation function for Brush Filters.

### Changed
- Changed the package name that's used to get core Universal Render Pipeline (URP) HLSL code from Lightweight to Universal.
- Added missing Terrain visualization settings.
- Removed ability to create Filter Stack assets.
- Changed Base Map Maximum Distance to 20000 to match the range in the Terrain Inspector.
- Terrain Toolbox splatmap modifications are now applied to Terrain only after you click **Apply To Terrain** in the Terrain Toolbox.

### Fixed
#### Brushes
- Brush sizes are clamped to a maximum based on texture size. ([1276290](https://issuetracker.unity3d.com/product/unity/issues/guid/1276290/))
- Clone tool correctly blends heightmap.
- Clone tool clones alphamaps as well as height.
- Clone tool correctly maintains clone position when you rotate the Scene Camera or click on the UI.
- Corrected Smooth tool behavior when you paint near the edges of Terrain tiles so that it produces less discontinuities. ([1186005](https://issuetracker.unity3d.com/issues/terrain-tools-terrain-gets-terraced-when-smoothed-near-the-edge-of-the-terrain-tile))
- Removed artifacts that occurred when you used the Twist Brush. ([1276448](https://issuetracker.unity3d.com/issues/terrain-tools-twist-tool-certain-values-cause-value-spikes-in-the-heightmap))
- Added support for Brush Mask Filters to the Wind Erosion tool.
- Various RenderTexture leaks.
- Brush Filters don't throw errors on certain Graphics APIs when UAV access is not supported for used GraphicsFormat.

#### Splatmaps
- Corrected blending when you apply splatmaps from the Terrain Toolbox.
- Fixed error thrown when you highlight a splatmap in the Terrain Toolbox.
- Fixed Undo/Redo functions after you apply a splatmap to a Terrain from the Terrain Toolbox.
- Fixed error thrown when you export a splatmap to a Terrain without selecting a Terrain. ([1240314](https://issuetracker.unity3d.com/issues/terrain-tools-terrain-toolbox-splatmap-export-to-terrain-throws-dividebyzeroexception-when-no-terrain-is-selected))
- Fixed Flip Adjustment on splatmap. ([1276204](https://issuetracker.unity3d.com/issues/terrain-tools-utilities-splatmap-flip-horizontal-and-flip-vertical-are-reversed))

#### Heightmaps
- Corrected heightmap when you create Terrain using a RAW file type in the Terrain Toolbox.
- Fixed Flip Axis when you use Import Heightmap to create Terrain.
- Heightmaps can be exported in 8-bit mode from the Terrain Toolbox. ([1276244](https://issuetracker.unity3d.com/issues/terrain-tools-toolbox-utilities-export-heightmaps-heightmap-depth-always-resets-to-16-bit))
- Terrain Toolbox heightmap export properly remaps levels.

#### Noise
- Noise Editor displays correctly when you use the Personal (light) theme.
- Noise shader import works correctly on Linux. ([1188556](https://issuetracker.unity3d.com/issues/linux-adding-terrain-tools-package-makes-an-endless-loop-of-importing))
- You can now open only one Export Noise window at a time. ([1269540](https://issuetracker.unity3d.com/issues/terraintools-new-instance-of-export-noice-to-texture-window-is-created-every-time-even-though-another-is-already-open))
- Removed non-functional normalize option from the Export Noise UI.
- Unique asset path is generated when you export noise.
- Removed shader warnings from noise shaders.
- GraphicsFormats for Noise now depend on active Graphics API and should look better on Vulkan and OpenGLES3.

#### Terrain Splitting
- Details and Trees are preserved when you split Terrain in the Terrain Toolbox. ([1248489](https://issuetracker.unity3d.com/product/unity/issues/guid/1248489/))
- Splitting Terrain with resolution of 33x33 warns the user and allows for an alternate split mode. ([1276273](https://issuetracker.unity3d.com/issues/terrain-tools-toolbox-utilities-splitting-terrain-with-heightmap-resolution-33-breaks-terrain-and-causes-error))
- Scenes now don't automatically save when you split Terrain in the Terrain Toolbox.

#### Additional Fixes
- Editing the Remap Curve in the Image Filter Stack no longer throws error. ([1254251](https://issuetracker.unity3d.com/issues/terraintools-nullreferenceexception-is-thrown-on-editing-the-remap-curve-in-the-image-filter-stack))
- Reverting to values in a Terrain preset now correctly reverts to saved values.
- Base texture size now appears correctly in the Terrain Inspector drop-down menu for Terrains you create with the Terrain Toolbox.
- Fixed toggle button behavior in the Terrain Toolbox, Brushes, and Brush Mask Filters.

## [3.0.1-preview] - 2020-02-06

- Removed test meta file

## [3.0.0-preview] - 2020-01-30

- Added Common Brush Controls and Brush Mask Filters to Paint Holes tool
- Added Terrain Visualization tool support for Universal Render Pipeline
- Added world space height support for Set Height tool
- Fixed bugs for foldout UI
- Toolbox Split Terrain tool bug fixing and added support for terrain holes
- Added terrain holes support to Toolbox Heightmap resolution change

## [2.0.0-preview] - 2019-08-05
- Brush Mask Filter Stack and Filters
- Added Brush Mask Filter Stack to each tool
- New noise type "Strata"
- Noise Filter
- Noise Editor Window
- Wind Erosion bug fixes. Looks great!
- TerrainToolbox Material Updates
- Can import splatmaps with the Terrain Toolbox
- Bug fixing for Terrain Toolbox Gizmo
- Terrain visualization utilies in Terrain Toolbox
- Improved Paint Texture tool. Now uses brush controllers for size, rotation, etc.
- Reorderable Layer List/Palette Assets
- Eyedropper feature for Paint Texture Tool that selects the most prominent Terrain Layer in a given area
- Fixed Mesh Stamp Tool
- Rotation for Mesh Stamp now treats brush rotation and mesh rotation as one transformation
- Fixed depth for Mesh Stamp
- Moved Mesh to RenderTexture to public API for folks to use in their tools
- Ability to generate noise based on input Texture. Noise Filter uses this to pipe the heightmap into the noise generation for another way of doing strata
- Options to "Reset" brush settings to defaults added on the Tool foldout headers
- Removed AssetDatabase.Refresh from static constructor of NoiseLib
- LOTS of bug fixing

## [1.1.4-preview] - 2019-05-22
- Updating Mesh Stamp Tool to use Brush Controllers for size, rotation, etc.
- Fix errors with Noise shader generation writing to read-only files and manually setting locale (so commas dont get used for decimals)

## [1.1.2-preview] - 2019-05-22
- Removing TestRunnerOptions.json

## [1.1.1-preview] - 2019-05-22
- Removed Samples directory

## [1.1.0-preview] - 2019-05-22
- Added more automated tests
- Deprecated terrain material type for users on 2019.2 in terrain toolbox

## [1.0.0-preview] - 2019-05-17

### This is the first release of *Unity Package \<Terrain Tools\>*.
- Terrain Tools package released in preview: Terrain Tools package helps improve the workflow for creating Terrain in Unity. It includes a number of brand new sculpting Brushes, and a collection of terrain tools in a new Terrain Toolbox to help automate terrain workflows.
- New sculpting brushes.
- New Terrain Toolbox.
- Initial package manual and documentations.


## [0.1.0-preview] - 2019-04-30

- Testing publish pipeline of Unity Package <Terrain Tools>.
