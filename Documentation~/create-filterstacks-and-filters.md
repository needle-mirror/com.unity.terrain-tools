# Filter Stacks, Filters, and procedural masks

Next, you can add a Filter Stack to your Terrain Tool. You can use Filter Stacks to generate procedural masks that influence the inputs and outputs of your tool. For example, if you add a Filter Stack with a Slope Filter to the Smooth tool, it only smooths regions of the Terrain's heightmap within the slope ranges you specified on the Slope Filter.

Create a shader to apply the effect to the terrain heightmap; it must reference the filter texture for the filter mask to work:

```
Shader "TerrainTool/BrushMaskFilterExample"
{
    Properties { _MainTex ("Texture", any) = "" {} }

    SubShader
    {
        ZTest Always Cull Off ZWrite Off

        HLSLINCLUDE

        #include "UnityCG.cginc"
        #include "Packages/com.unity.terrain-tools/Shaders/TerrainTools.hlsl"

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height

        sampler2D _BrushTex;
        sampler2D _FilterTex;

        float4 _BrushParams;
        #define BRUSH_STRENGTH      (_BrushParams[0])
        #define BRUSH_TARGETHEIGHT  (_BrushParams[1])
        #define kMaxHeight          (32766.0f/65535.0f)

        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 pcUV : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 pcUV : TEXCOORD0;
        };

        v2f vert(appdata_t v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.pcUV = v.pcUV;
            return o;
        }

        ENDHLSL

        Pass
        {
            Name "CustomTerrainTool"

            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            float4 frag(v2f i) : SV_Target
            {
                float2 brushUV = PaintContextUVToBrushUV(i.pcUV);
                // out of bounds multiplier
                float oob = all(saturate(brushUV) == brushUV) ? 1.0f : 0.0f;

                // Sample the MainTex, which is a region of the source Heightmap texture, to get the current height value at the provided UV
                // UnpackHeightmap is necessary here because it unpacks the height value from R and G channels if the current platform/graphics device doesn't support R16_UNorm texture formats. If R16_UNorm formats are supported, UnpackHeightmap just reads from the R channel.
                float height = UnpackHeightmap(tex2D(_MainTex, i.pcUV));
                float filter = UnpackHeightmap(tex2D(_FilterTex, i.pcUV));
                float brush = UnpackHeightmap(tex2D(_BrushTex, brushUV));
                // Calculate the influence from the composited mask
                float brushShape = oob * brush * filter;
                height = height + BRUSH_STRENGTH * brushShape;

                // Store the new height into the destination RenderTexture. Clamp between 0.0f and 0.5f because the Heightmap itself is signed but is treated as an unsigned texture when rendering the Terrain
                // PackHeightmap is necessary here because it packs the height value into R and G channels if the current platform/graphics device doesn't support R16_UNorm texture formats. If R16_UNorm formats are supported, PackHeightmap just writes to the R channel.
                return PackHeightmap(clamp(height, 0, kMaxHeight));
            }

            ENDHLSL
        }
    }
}
```

Next, create the tool itself, which uses the filter stack and the shader to apply the tool effect:

```
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine.TerrainTools;

class CustomTerrainToolWithMaskFilters : TerrainPaintTool<CustomTerrainToolWithMaskFilters>
{
    // Set up the default values here - if you don't do this, the brush preview will be zero pixels
    // in size and it will look like it isn't working.
    private float m_BrushOpacity = 0.1f;
    private float m_BrushSize = 100.0f;
    private float m_BrushRotation = 0.0f;

    // Creates the material used to apply the effect to the heightmap.
    Material m_Material;
    Material material
    {
        get
        {
            if(m_Material != null) return m_Material;

            m_Material = new Material(Shader.Find("TerrainTool/BrushMaskFilterExample"));
            return m_Material;
        }
    }

    // Create the FilterStack - stores the filters in use with this tool.
    FilterStack m_FilterStack;
    FilterStack filterStack
    {
        get
        {
            if(m_FilterStack != null) return m_FilterStack;

            m_FilterStack = ScriptableObject.CreateInstance<FilterStack>();
            return m_FilterStack;
        }
    }

    // Create the UI view for the FilterStack
    FilterStackView m_FilterStackView;
    FilterStackView filterStackView
    {
        get
        {
            if(m_FilterStackView != null && m_FilterStackView.serializedFilterStack.targetObject != null)
                return m_FilterStackView;

            m_FilterStackView = new FilterStackView(new GUIContent("Brush Mask Filters"), new SerializedObject( filterStack ) );
            m_FilterStackView.FilterContext = filterContext;

            return m_FilterStackView;
        }
    }

    // Create a FilterContext. This is a property-bag of sorts used by Filters in a FilterStack
    FilterContext m_FilterContext;
    private FilterContext filterContext
    {
        get
        {
            if (m_FilterContext != null) return m_FilterContext;

            m_FilterContext = new FilterContext(FilterUtility.defaultFormat, Vector3.zero, 1f, 0f);
            return m_FilterContext;
        }
    }

    public override string GetName()
    {
        return "Examples/Custom Terrain Tool With Mask Filters";
    }

    public override string GetDescription()
    {
        return "My custom Terrain Tool is amazing!";
    }

    // Override this function to add UI elements to the inspector.
    public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
    {
        editContext.ShowBrushesGUI(5, BrushGUIEditFlags.Select);
        m_BrushOpacity = EditorGUILayout.Slider("Opacity", m_BrushOpacity, 0, 1);
        m_BrushSize = EditorGUILayout.Slider("Size", m_BrushSize, .001f, 1000f);
        m_BrushRotation = EditorGUILayout.Slider("Rotation", m_BrushRotation, 0, 360);

        // Render the UI for the FilterStack. This will allow you to add and remove Filters through the Terrain Tool UI
        filterStackView.OnGUI();
    }

    void BlitFilterStackTexture(Terrain terrain, RenderTexture source, RenderTexture dest, Vector3 brushPos)
    {
        // Prepare the FilterContext
        var filterContext = new FilterContext(FilterUtility.defaultFormat, brushPos, m_BrushSize, m_BrushRotation);

        using(new ActiveRenderTextureScope(null))
        {
            // Bind any necessary properties that the Filters in the FilterStack might use. ie some of the Terrain Tools Filters rely on the size of the Terrain like Concavity, Slope, etc.
            TerrainData terrainData = terrain.terrainData;
            filterContext.floatProperties[FilterContext.Keywords.TerrainScale] = Mathf.Sqrt(terrainData.size.x * terrainData.size.x + terrainData.size.z * terrainData.size.z);
            filterContext.vectorProperties["_TerrainSize"] = new Vector4(terrainData.size.x, terrainData.size.y, terrainData.size.z, 0.0f);

            // Bind Terrain Texture data that might be used by Filters in the FilterStack
            filterContext.rtHandleCollection.AddRTHandle(0, FilterContext.Keywords.Heightmap, source.graphicsFormat);
            filterContext.rtHandleCollection.GatherRTHandles(source.width, source.height);
            Graphics.Blit(source, filterContext.rtHandleCollection[FilterContext.Keywords.Heightmap]);
            filterStack.Eval(filterContext, source, dest);
        }

        filterContext.ReleaseRTHandles();
    }

    private void RenderIntoPaintContext(Terrain terrain, UnityEngine.TerrainTools.PaintContext paintContext, Texture brushTexture, UnityEngine.TerrainTools.BrushTransform brushXform, Vector3 brushPos)
    {
        // Generates a mask rendertexture that is used to modulate the brush texture when rendering the effect
        RTHandle filterTexture = RTUtils.GetTempHandle(paintContext.sourceRenderTexture.width, paintContext.sourceRenderTexture.height, 0, FilterUtility.defaultFormat);
        BlitFilterStackTexture(terrain, paintContext.sourceRenderTexture, filterTexture, brushPos);

        // Set up the material properties for rendering the effect
        material.SetTexture("_FilterTex", filterTexture);
        material.SetTexture("_BrushTex", brushTexture);
        var opacity = Event.current.control ? -m_BrushOpacity : m_BrushOpacity;
        material.SetVector("_BrushParams", new Vector4(opacity, 0.0f, 0.0f, 0.0f));
        UnityEngine.TerrainTools.TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushXform, material);

        // Blit over the heightmap using the effect material
        Graphics.Blit(paintContext.sourceRenderTexture, paintContext.destinationRenderTexture, material, (int)TerrainBuiltinPaintMaterialPasses.RaiseLowerHeight);

        // Release the RenderTexture for the FilterStack
        RTUtils.Release(filterTexture);
    }

    public override void OnRenderBrushPreview(Terrain terrain, IOnSceneGUI editContext)
    {
        // Only render the preview if we're in a repaint event and are over a terrain
        if (Event.current.type != EventType.Repaint) return;
        if (!editContext.hitValidTerrain) return;

        // Get the transform for the brush, so we can work out where to paint to
        UnityEngine.TerrainTools.BrushTransform brushXform = UnityEngine.TerrainTools.TerrainPaintUtility.CalculateBrushTransform(terrain, editContext.raycastHit.textureCoord, m_BrushSize, m_BrushRotation);
        UnityEngine.TerrainTools.PaintContext paintContext = UnityEngine.TerrainTools.TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds(), 1);

        // Continue rendering the brush preview
        Material previewMaterial = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();
        TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, TerrainBrushPreviewMode.SourceRenderTexture, editContext.brushTexture, brushXform, previewMaterial, 0);
        RenderIntoPaintContext(terrain, paintContext, editContext.brushTexture, brushXform, editContext.raycastHit.point);
        RenderTexture.active = paintContext.oldRenderTexture;
        previewMaterial.SetTexture("_HeightmapOrig", paintContext.sourceRenderTexture);
        TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, TerrainBrushPreviewMode.DestinationRenderTexture, editContext.brushTexture, brushXform, previewMaterial, 1);
        UnityEngine.TerrainTools.TerrainPaintUtility.ReleaseContextResources(paintContext);
    }

    public override bool OnPaint(Terrain terrain, IOnPaint editContext)
    {
        // Get the transform for the brush to determin where to paint
        UnityEngine.TerrainTools.BrushTransform brushXform = UnityEngine.TerrainTools.TerrainPaintUtility.CalculateBrushTransform(terrain, editContext.uv, m_BrushSize, m_BrushRotation);
        UnityEngine.TerrainTools.PaintContext paintContext = UnityEngine.TerrainTools.TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds());

        RenderIntoPaintContext(terrain, paintContext, editContext.brushTexture, brushXform, editContext.raycastHit.point);
        UnityEngine.TerrainTools.TerrainPaintUtility.EndPaintHeightmap(paintContext, "Terrain Paint - Raise or Lower Height");

        return true;
    }
}
```
