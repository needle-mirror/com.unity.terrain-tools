using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

namespace UnityEngine.TerrainTools
{
    internal class NoiseComponent : MonoBehaviour
    {
        public Material mat;
        public NoiseSettings noiseSettings;

        void Update()
        {
            if(mat != null)
            {
                noiseSettings.SetupMaterial( mat );
            }
        }
    }
}