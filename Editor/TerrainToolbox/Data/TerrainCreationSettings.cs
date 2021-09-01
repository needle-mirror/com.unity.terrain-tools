using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.TerrainTools;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    [Serializable]
    internal class TerrainCreationSettings : ScriptableObject
    {
        // Terrain Size	
        public float TerrainWidth = 1000;
        public float TerrainLength = 1000;
        public float TerrainHeight = 600;
        public Vector3 StartPosition = new Vector3(0, 0, 0);
        public int TilesX = 1;
        public int TilesZ = 1;

        // Terrain Group Settings
        public int GroupID = 0;
        public bool AutoConnect = true;
        public bool DrawInstanced = true;
        public int PixelError = 5;
        public int BaseMapDistance = 1000;
        public int BaseTextureResolution = 1024;
        public int ControlTextureResolution = 512;
        public int DetailResolution = 1024;
        public int DetailResolutionPerPatch = 32;
        public Material MaterialOverride = null;
        public int HeightmapResolution = 513;

        // Terrain Heightmap Settings
        public bool EnableHeightmapImport = false;
        public bool UseGlobalHeightmap = false;
        public Heightmap.Mode HeightmapMode = Heightmap.Mode.Global;
        public bool UseRawFile = false;
        public int HeightmapWidth = 0;
        public int HeightmapHeight = 0;
        public float HeightmapRemapMax = 500;
        public float HeightmapRemapMin = 0;
        public Heightmap.Depth HeightmapDepth = Heightmap.Depth.Bit16;
        public Heightmap.Flip FlipMode = Heightmap.Flip.None;
        public string BatchHeightmapFolder = string.Empty;
        public string GlobalHeightmapPath = string.Empty;
        public List<string> TileHeightmapPaths = new List<string>();

        // Gizmo Settings
        public bool EnableGizmo = false;
        public bool EditGizmoBounds = false;

        // other settings
        public string TerrainAssetDirectory = "Assets/Terrain/";
        public bool EnableGuid = true;
        public bool EnableClearExistingData = false;
        public bool EnableLightingAutoBake = false;
        public string PresetPath = string.Empty;

        // UI
        public bool ShowGroupSettings = false;
        public bool ShowHeightmapSettings = false;
        public bool ShowGizmoSettings = false;
        public bool ShowOptions = true;
    }
}
