using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace UnityEditor.TerrainTools.UI
{ 
    internal static class BrushToolsOverlay
    {
        public static void BrushAttributesGUI(BrushOverlaysGUIFlags flags)
        {
            var tool = BrushAttributesOverlay.GetActiveOverlaysTool();
            if (!tool) return; 
            
            IBrushUIGroup commonUI = BrushAttributesOverlay.GetCommonUI(); 
            
            // call GUI 
            commonUI.OnInspectorGUI(tool.Terrain, new OnInspectorGUIContext(), true, BrushGUIEditFlags.SelectAndInspect,
                flags);
        }
        
    }
    
    // brush filters 
    [Overlay(typeof(SceneView), "Brush Filters", defaultDockPosition = DockPosition.Top, defaultDockZone = DockZone.LeftToolbar, defaultDockIndex = 1) ]
    [Icon("Packages/com.unity.terrain-tools/Editor/Icons/TerrainOverlays/BrushSettingIcons/FrameFilters.png")]
    internal class BrushFilterOverlay : Overlay, ITransientOverlay 
    {
        // determines whether the toolbar should be visible or not
        // only visible for tools which are sculpt or materials or paint details 
        public bool visible
        {
            get
            {
                var currTool = BrushesOverlay.ActiveTerrainTool as ITerrainToolPaintTool;
                if (currTool == null)
                    return false;
                return currTool.HasBrushFilters && BrushesOverlay.IsSelectedObjectTerrain();
            }
        }

        static readonly string[] k_ToolbarItems = new[]
        {
            "BrushFilterToolbar",
        };
        
        public override VisualElement CreatePanelContent()
        {
            return new BrushFilterToolbar();
        }

        public IEnumerable<string> toolbarElements => k_ToolbarItems;
    }

    [EditorToolbarElement("BrushFilterToolbar", typeof(SceneView))]
    internal class BrushFilterToolbar : OverlayToolbar
    {
        VisualElement m_RootElement;

        IBrushUIGroup m_commonUI;

        public BrushFilterToolbar()
        {
            // root element
            m_RootElement = new VisualElement();

            // imgui container
             IMGUIContainer img = new IMGUIContainer();
            img.style.minHeight = 70;
            img.style.minWidth = 300; 
            img.onGUIHandler = () => BrushToolsOverlay.BrushAttributesGUI(BrushOverlaysGUIFlags.Filter); 
            m_RootElement.Add(img);

            // add root element
            Add(m_RootElement);
        }
    }
    
}