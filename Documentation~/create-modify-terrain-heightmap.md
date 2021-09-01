# Modify the Terrain heightmap

The example code below tells Unity to use the built-in painting Material to modify the Terrain's heightmap, and render a Brush preview in the Scene view.

```
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

internal class CustomTerrainTool : TerrainPaintTool<CustomTerrainTool>
{
    private float m_BrushOpacity;
    private float m_BrushSize;
    private float m_BrushRotation;

    // Name of the Terrain Tool. This appears in the tool UI.
    public override string GetName()
    {
        return "Examples/Custom Terrain Tool";
    }

    // Description for the Terrain Tool. This appears in the tool UI.
    public override string GetDesc()
    {
        return "This is a custom Terrain Tool that modifies the Terrain heightmap.";
    }

    public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
    {
        editContext.ShowBrushesGUI(5, BrushGUIEditFlags.Select);
        m_BrushOpacity = EditorGUILayout.Slider("Opacity", m_BrushOpacity, 0, 1);
        m_BrushSize = EditorGUILayout.Slider("Size", m_BrushSize, .001f, 100f);
        m_BrushRotation = EditorGUILayout.Slider("Rotation", m_BrushRotation, 0, 360);
    }

    // Ease of use function for rendering modified Terrain Texture data into a PaintContext. This is used in both OnRenderBrushPreview and OnPaint.
    private void RenderIntoPaintContext(UnityEngine.TerrainTools.PaintContext paintContext, Texture brushTexture, UnityEngine.TerrainTools.BrushTransform brushXform)
    {
        // Get the built-in painting Material reference
        Material mat = UnityEngine.TerrainTools.TerrainPaintUtility.GetBuiltinPaintMaterial();
        // Bind the current brush texture
        mat.SetTexture("_BrushTex", brushTexture);
        // Bind the tool-specific shader properties
        var opacity = Event.current.control ? -m_BrushOpacity : m_BrushOpacity;
        mat.SetVector("_BrushParams", new Vector4(opacity, 0.0f, 0.0f, 0.0f));
        // Setup the material for reading from/writing into the PaintContext texture data. This is a necessary step to setup the correct shader properties for appropriately transforming UVs and sampling textures within the shader
        UnityEngine.TerrainTools.TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
        // Render into the PaintContext's destinationRenderTexture using the built-in painting Material and the built-in ID for the RaiseLowerHeight shader pass
        Graphics.Blit(paintContext.sourceRenderTexture, paintContext.destinationRenderTexture, mat, (int)UnityEngine.TerrainTools.TerrainPaintUtility.BuiltinPaintMaterialPasses.RaiseLowerHeight);
    }

    // Render Tool previews in the SceneView
    public override void OnRenderBrushPreview(Terrain terrain, IOnSceneGUI editContext)
    {
        // Dont render preview if this isnt a Repaint
        if (Event.current.type != EventType.Repaint) return;
        
        // Only do the rest if user mouse hits valid terrain
        if (!editContext.hitValidTerrain) return;

        // Get the current BrushTransform under the mouse position relative to the Terrain
        UnityEngine.TerrainTools.BrushTransform brushXform = UnityEngine.TerrainTools.TerrainPaintUtility.CalculateBrushTransform(terrain, editContext.raycastHit.textureCoord, m_BrushSize, m_BrushRotation);
        // Get the PaintContext for the current BrushTransform. This has a sourceRenderTexture from which to read existing Terrain texture data
        UnityEngine.TerrainTools.PaintContext paintContext = UnityEngine.TerrainTools.TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds(), 1);
        // Get the built-in Material for rendering Brush Previews
        Material previewMaterial = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();
        // Specify which PaintContext RenderTexture to use for the brush preview
        TerrainPaintUtilityEditor.BrushPreview previewTexture = TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture;
        // Render the brush preview for the sourceRenderTexture. This will show up as a projected brush mesh rendered on top of the Terrain
        TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, previewTexture, editContext.brushTexture, brushXform, previewMaterial, 0);
        // Render changes into the PaintContext destinationRenderTexture
        RenderIntoPaintContext(paintContext, editContext.brushTexture, brushXform);
        // Restore old render target. This is necessary because... TODO
        RenderTexture.active = paintContext.oldRenderTexture;
        // Bind the sourceRenderTexture to the preview Material. This is used to compute deltas in height
        previewMaterial.SetTexture("_HeightmapOrig", paintContext.sourceRenderTexture);
        // Specify now that the destinationRenderTexture is going to be used for the next brush preview rendering
        previewTexture = TerrainPaintUtilityEditor.BrushPreview.DestinationRenderTexture;
        // Render a procedural mesh displaying the delta/displacement in height from the source Terrain texture data. When modifying Terrain height, this shows how much the next paint operation will alter the Terrain height 
        TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, previewTexture, editContext.brushTexture, brushXform, previewMaterial, 1);
        // Cleanup resources
        UnityEngine.TerrainTools.TerrainPaintUtility.ReleaseContextResources(paintContext);
    }

    // Perform painting operations that modify the Terrain texture data
    public override bool OnPaint(Terrain terrain, IOnPaint editContext)
    {
        // Get the current BrushTransform under the mouse position relative to the Terrain
        UnityEngine.TerrainTools.BrushTransform brushXform = UnityEngine.TerrainTools.TerrainPaintUtility.CalculateBrushTransform(terrain, editContext.uv, m_BrushSize, m_BrushRotation);
        // Get the PaintContext for the current BrushTransform. This has a sourceRenderTexture from which to read existing Terrain texture data
        // and a destinationRenderTexture into which to write new Terrain texture data
        UnityEngine.TerrainTools.PaintContext paintContext = UnityEngine.TerrainTools.TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds());
        // Call the common rendering function used by OnRenderBrushPreview and OnPaint
        RenderIntoPaintContext(paintContext, editContext.brushTexture, brushXform);
        // Commit the modified PaintContext with a provided string for tracking Undo operations. This function handles Undo and resource cleanup for you
        UnityEngine.TerrainTools.TerrainPaintUtility.EndPaintHeightmap(paintContext, "Terrain Paint - Raise or Lower Height");
        
        // Return whether or not Trees and Details should be hidden while painting with this Terrain Tool
        return true;
    }
}
```
