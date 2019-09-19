# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.0.2-preview] - 2019-09-20
- Fixed TypeLoadExceptions for NoiseLib and Filters

## [2.0.1-preview] - 2019-08-14

- Fixed a bug of Terrain Gizmo. Now tools and selection only runs in Editor mode.
- Fixed a bug of saving Layer Palette asset in the Paint Texture Tool. 
- Thermal erosion improvements. 
- Test updates. 

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
