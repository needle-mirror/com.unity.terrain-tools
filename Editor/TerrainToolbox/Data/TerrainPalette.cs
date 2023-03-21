using System;
using System.Collections.Generic;

namespace UnityEngine.TerrainTools
{
    [Serializable]
    internal class TerrainPalette : ScriptableObject
    {
        public List<TerrainLayer> PaletteLayers = new List<TerrainLayer>();
    }
}
