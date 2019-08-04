using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

[Serializable]
public class TerrainVisualizationSettings : ScriptableObject
{
    // General
    public bool ModeWarning = false;

    // Heatmap 
    public List<Color> ColorSelection = new List<Color> { Color.blue, Color.cyan, Color.green, Color.yellow, Color.red };
    public List<float> DistanceSelection = new List<float> { 100, 200, 300, 400, 500 };
    public enum REFERENCESPACE { LocalSpace, WorldSpace};
    public REFERENCESPACE ReferenceSpace;
    public enum MEASUREMENTS { Meters, Feet };
    public MEASUREMENTS CurrentMeasure;
    public const float CONVERSIONNUM = 3.280f;
    public float MinDistance = 100;
    public float MaxDistance = 500;
    public float TerrainMaxHeight = 500;
    public float SeaLevel;
    public int HeatLevels;
    public bool WorldSpace = false;

    // Other settings
    public string PresetPath = string.Empty;
}
