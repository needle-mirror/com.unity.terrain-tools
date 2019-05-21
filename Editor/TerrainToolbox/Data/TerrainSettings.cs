using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class TerrainSettings : ScriptableObject
{
	// Basic settings
	public int GroupingID = 0;
	public bool AutoConnect = true;
	public bool DrawHeightmap = true;
	public bool DrawInstanced = true;
	public float PixelError = 5;
	public float BaseMapDistance = 1000;
	public ShadowCastingMode ShadowCastingMode = ShadowCastingMode.TwoSided;
	public Material MaterialTemplate = null;
#if UNITY_2019_2_OR_NEWER
#else
	public Terrain.MaterialType MaterialType = Terrain.MaterialType.BuiltInStandard;
	public Color LegacySpecular = Color.gray;
	public float LegacyShininess = 0;
#endif
	public ReflectionProbeUsage ReflectionProbeUsage = ReflectionProbeUsage.BlendProbes;

	// Mesh resolution settings
	public float TerrainWidth = 500;
	public float TerrainHeight = 600;
	public float TerrainLength = 500;
	public int DetailResolutaion = 1024;
	public int DetailResolutionPerPatch = 32;

	// Texture resolution settings
	public int BaseTextureResolution = 1024;
	public int AlphaMapResolution = 512;
	public int HeightMapResolution = 513;

	// Tree and detail settings
	public bool DrawTreesAndFoliage = true;
	public bool BakeLightProbesForTrees = true;
	public bool DeringLightProbesForTrees = false;
	public bool PreserveTreePrototypeLayers = false;
	public float DetailObjectDistance = 80;
	public bool CollectDetailPatches = true;
	public float DetailObjectDensity = 1;
	public float TreeDistance = 2000;
	public float TreeBillboardDistance = 50;
	public float TreeCrossFadeLength = 5;
	public int TreeMaximumFullLODCount = 50;

	// Grass wind settings
	public float WavingGrassStrength = 0.5f;
	public float WavingGrassSpeed = 0.5f;
	public float WavingGrassAmount = 0.5f;
	public Color WavingGrassTint = new Color(0.7f, 0.6f, 0.5f, 0.0f);

	// UI
	public bool ShowBasicTerrainSettings = false;
	public bool ShowMeshResolutionSettings = false;
	public bool ShowTextureResolutionSettings = false;
	public bool ShowTreeAndDetailSettings = false;
	public bool ShowGrassWindSettings = false;
	public bool EnableBasicSettings = false;
	public bool EnableMeshResSettings = false;
	public bool EnableTextureResSettings = false;
	public bool EnableTreeSettings = false;
	public bool EnableWindSettings = false;

	public string PresetPath = string.Empty;
	public int PresetMode = 0;
}
