using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.TerrainTools.UI
{
    internal class TerrainToolsPostprocessor : AssetPostprocessor
    {
        // why is this here? When terrain tools is installed, the package includes some assets
        // that must be loaded by AssetDatabase. Some parts of the package may get instantiated
        // before said assets have been loaded. So this event gives us a way to run code once we
        // know that the assetdatabase has finished importing assets.
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if(didDomainReload)
                BrushAttributesOverlay.RebuildContent();
        }
    }
}