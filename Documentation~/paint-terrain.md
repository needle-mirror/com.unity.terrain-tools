# Paint Terrain

**Paint Terrain** tools let you use a Brush to sculpt or paint the Terrain and make modifications. The Terrain Tools package adds 13 additional tools to the [6 built-in tools](https://docs.unity3d.com/Manual/terrain-Tools.html), and improves the functionality of the built-in [Paint Texture](https://docs.unity3d.com/Manual/terrain-PaintTexture.html), [Smooth Height](https://docs.unity3d.com/Manual/terrain-SmoothHeight.html), and [Stamp Terrain](https://docs.unity3d.com/Manual/terrain-StampTerrain.html) tools.

### Inspector
In the **Terrain Inspector**, click the **Paint Terrain** icon to display the list of Terrain tools.

![The top of the terrain tools inspector panel](images/Paint_Terrain.png)

### Scene View overlays

When you select a terrain, the terrain tools toolbar appears. The toolbar has two sections. You can use the first five icons to select the terrain tool category and use the rest to select the tools in that category.

![The terrain tools toolbar with the paint tools category and the raise/lower height tool selected.](images/Paint_Terrain_Toolbar.png)

To view the Paint Terrain tools, select the leftmost category icon that depicts a mountain:

![An icon that depicts a mountain](images/Paint_Terrain_Icon.png)

## Tools

The Terrain Tools package provides the following 13 additional tools, as well as improved **Paint Texture**, **Smooth Height**, and **Stamp Terrain** functionality.

* [__Bridge__](sculpt-bridge.md) creates a Brush stroke between two selected points to build a land bridge.

  ![Bridge Icon](images/Icons/Bridge.png)

* [__Clone__](sculpt-clone.md) duplicates Terrain from one region to another.

  ![Clone Icon](images/Icons/Clone.png)

* [__Noise__](sculpt-noise.md) uses different noise types and fractal types to modify Terrain height.

  ![Noise Icon](images/Icons/Noise.png)

* [__Terrace__](sculpt-terrace.md) transforms Terrain into a series of flat areas like steps.

  ![Terrace Icon](images/Icons/Terrace.png)

* [__Contrast__](effects-contrast.md) expands or shrinks the overall range of the Terrain height.

  ![Contrast Icon](images/Icons/Contrast.png)

* [__Sharpen Peaks__](effects-sharpen-peaks.md) sharpens peaks and flattens flat areas of the Terrain.

   ![Sharpen peaks icon](images/Icons/SharpenPeaks.png) 
   
* [__Slope Flatten__](effects-slope-flatten.md) flattens the Terrain while maintaining the average slope.

   ![Slope flatten icon](images/Icons/FlattenSlope.png) 

* [__Hydraulic__](erosion-hydraulic.md) simulates the effect of water flowing over the Terrain and the transport of sediment.

   ![Hydraulic icon](images/Icons/HydraulicErosion.png) 

* [__Thermal__](erosion-thermal.md) simulates the effect of sediment settling on the Terrain while maintaining a natural slope.

   ![Thermal icon](images/Icons/ThermalErosion.png) 

* [__Wind__](erosion-wind.md) simulates the effect of wind transporting and redistributing sediment.

   ![Wind icon](images/Icons/WindErosion.png) 

* [__Paint Texture__](paint-texture.md) is similar to the built-in Paint Texture tool, but with added functionality such as an improved workflow and a Terrain Layer Eyedropper tool.

   ![Paint texture icon](images/Icons/PaintTexture.png)

* [__Pinch__](transform-pinch.md) pulls the height towards or bulges it away from the center of the Brush.

   ![Pinch icon](images/Icons/Pinch.png) 

* [__Smudge__](transform-smudge.md) moves Terrain features along the path of the Brush stroke.

   ![Smudge icon](images/Icons/Smudge.png) 

* [__Twist__](transform-twist.md) rotates Terrain features around the center of the Brush, along the path of the Brush stroke.

   ![Twist icon](images/Icons/Twist.png) 

* [__Smooth Height__](smooth-height.md) is similar to the built-in Smooth Height tool, but includes two new parameters, **Verticality** and **Blur Radius**, which provide finer control when smoothing your Terrain.

   ![Smooth icon](images/Icons/Smooth.png) 

* [__Stamp Terrain__](stamp-terrain.md) is similar to the built-in Stamp Terrain tool, but with added functionality such as **Min** and **Max** operations that provide finer control over Terrain. It now also includes the Mesh Stamp tool, which projects the shape of a mesh into the Terrain's heightmap.

   ![Stamp icon](images/Icons/Stamp.png) 

You can also create your own custom Terrain painting tools. For more information, see [TerrainTools.TerrainPaintTool_1](https://docs.unity3d.com/ScriptReference/TerrainTools.TerrainPaintTool_1.html) and [Create a custom Terrain tool](create-custom-tools.md).
