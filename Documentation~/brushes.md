# Brushes

The Terrain Tools package enhances Unity's collection of [built-in Brushes](https://docs.unity3d.com/Manual/class-Brush.html). It adds updated [Brush Controls](brush-controls-shortcut-keys.md) and [Brush Mask Filters](brush-mask-filters.md).

These updated Brush controls rename **Opacity** to **Brush Strength**, and provide **Brush Rotation**, **Brush Spacing**, and **Brush Scatter** controls. You can now also set the maximum and minimum values of each Brush.

**Brush Mask Filters** add additional operations to the Brush before computing the final Brush Mask output. There are two types of filters: those that use existing Terrain Data to calculate the resulting mask, and those that use math operations to directly modify the mask. Filters let you modify the base Brush Mask Texture, and specify which Terrain regions your paint operations affect. For details about the available Brush Mask Filters, see the [List of Brush Mask Filters](brush-mask-filters-list.md).
