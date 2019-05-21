using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

[Serializable]
public class TerrainCreationSettings : ScriptableObject
{
	// Terrain Size	
	public int TerrainWidth = 500;
	public int TerrainLength = 500;
	public int TerrainHeight = 500;
	public Vector3 StartPosition = new Vector3(0, 0, 0);
	public int TilesX = 1;
	public int TilesZ = 1;

	// Terrain Group Settings
	public int GroupID = 0;
	public bool AutoConnect = true;
	public bool DrawInstanced = true;
	public float PixelError = 5.0f;
	public int BaseMapDistance = 1024;	
	public int BaseTextureResolution = 1000;
	public int ControlTextureResolution = 512;
	public int DetailResolution = 1024;
	public int DetailResolutionPerPatch = 32;
	public Material MaterialOverride = null;
	public int HeightmapResolution = 512;

	// Terrain Heightmap Settings
	public bool EnableHeightmapImport = false;
	public bool UseGlobalHeightmap = false;
	public Heightmap.Mode HeightmapMode = Heightmap.Mode.Global;
	public int HeightmapWidth = 0;
	public int HeightmapHeight = 0;
	public float HeightmapRemapMax = 500;
	public float HeightmapRemapMin = 0;
	public Heightmap.Depth HeightmapDepth = Heightmap.Depth.Bit16;
	public Heightmap.Flip FlipMode = Heightmap.Flip.None;
	public string BatchHeightmapFolder = string.Empty;
	public string GlobalHeightmapPath = string.Empty;
	public List<string> TileHeightmapPaths = new List<string>();

	// other settings
	public string TerrainAssetDirectory = "Assets/Terrain/";
	public bool EnableGuid = true;
	public bool EnableClearExistingData = false;
	public bool EnableLightingAutoBake = false;
	public string PresetPath = string.Empty;

	// UI
	public bool ShowGroupSettings = false;
	public bool ShowHeightmapSettings = false;
	public bool ShowOptions = true;
}
