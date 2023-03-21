using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
internal class TerrainVisualizationSettings : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    [FormerlySerializedAs("ColorSelection")]
    // Heatmap 
    private Color[] _colorSelection =
    {
        Color.blue,
        Color.cyan,
        Color.green,
        Color.yellow,
        Color.red,
        Color.white,
        Color.white,
        Color.white
    };
    public Color[] ColorSelection
    {
        get => _colorSelection;
        set
        {
            _colorSelection = value;
            FixSelections();
        }
    }

    [SerializeField]
    [FormerlySerializedAs("DistanceSelection")]
    private float[] _distanceSelection ={ 0, 150, 300, 450, 600, 600, 600, 600 }; 
    public float[] DistanceSelection
    {
        get => _distanceSelection;
        set
        {
            _distanceSelection = value;
            FixSelections();
        }
    }
    
    public enum REFERENCESPACE { LocalSpace, WorldSpace};
    public REFERENCESPACE ReferenceSpace;
    public enum MEASUREMENTS { Meters, Feet };
    public MEASUREMENTS CurrentMeasure;
    public const float CONVERSIONNUM = 3.280f;
    public float TerrainMaxHeight;
    public float MinDistance = 100;
    public float MaxDistance = 500;
	public int HeatLevels = 5;
    public float SeaLevel;
    public bool WorldSpace = false;
    public bool ModeWarning = false;
    public const int VALUE_COUNT = 8;
    public void OnBeforeSerialize() {}

    public void OnAfterDeserialize()
    {
        FixSelections();
    }
    private void FixSelections()
    {
        if (DistanceSelection.Length < VALUE_COUNT)
        {
            var originalDistance = DistanceSelection;
            DistanceSelection = new float[VALUE_COUNT];
            originalDistance.CopyTo(DistanceSelection, 0);
            var lastIndex = originalDistance.Length-1;
            for (int i = lastIndex; i < VALUE_COUNT; i++)
            {
                DistanceSelection[i] = originalDistance[lastIndex];
            }
        }
        if (ColorSelection.Length < VALUE_COUNT)
        {
            var originalColors = ColorSelection;
            ColorSelection = new Color[VALUE_COUNT];
            originalColors.CopyTo(ColorSelection, 0);
            var lastIndex = originalColors.Length-1;
            for (int i = lastIndex; i < VALUE_COUNT; i++)
            {
                ColorSelection[i] = originalColors[lastIndex];
            }
        }
    }
}
