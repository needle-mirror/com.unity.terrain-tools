namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// A NoiseType implementation for Voronoi noise
    /// </summary>
    [System.Serializable]
    internal class VoronoiNoise : NoiseType<VoronoiNoise>
    {
        private static NoiseTypeDescriptor desc = new NoiseTypeDescriptor()
        {
            name = "Voronoi",
            outputDir = "Packages/com.unity.terrain-tools/Shaders/NoiseLib",
            sourcePath = "Packages/com.unity.terrain-tools/Shaders/NoiseLib/Implementation/VoronoiImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}