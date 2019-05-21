## Replace Splatmaps

The **Replace Splatmaps** tool allows you to replace splatmaps on Terrain. If you make or generate splatmaps using external tools like Photoshop or World Machine, you can use this tool to apply imported splatmaps onto Terrain. 

![](images/Toolbox_SplatmapReplace.png)

| **Property**            | **Description**                                              |
| ----------------------- | ------------------------------------------------------------ |
| **Terrain**             | The Terrain to replace and import splatmaps onto.            |
| **Splatmap Resolution** | Displays the splatmap resolution from the selected Terrain. Under **Terrain Settings**, this value is known as the **Control Texture Resolution**. |
| **Old SplatAlpha0**     | The first splatmap of the selected Terrain. Displays **None** if empty. |
| **Old SplatAlpha1**     | The second splatmap of the selected terrain. Displays **None** if empty. |
| **New SplatAlpha0**     | The texture to replace the first splatmap (**Old SplatAlpha0**) with. |
| **New SplatAlpha1**     | The texture to replace the second splatmap (**Old SplatAlpha1**) with. |

The current Terrain system only supports up to two splatmaps, allowing a maximum of eight Terrain Layers. When you add Terrain Layers on a Terrain, the Editor automatically generates splatmaps.

| **Property**          | **Description**                                              |
| --------------------- | ------------------------------------------------------------ |
| **Replace Splatmaps** | Click this button to replace old splatmaps with the assigned new splatmaps on the designated Terrain. |
| **Reset Splatmaps**   | Click this button to remove splatmap data from the designated Terrain, and set all splatmaps to their default values. |