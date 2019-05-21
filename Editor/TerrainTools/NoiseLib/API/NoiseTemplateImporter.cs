using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

namespace UnityEditor.Experimental.TerrainAPI
{
    /// <summary>
    /// Asset importer for noise template files. Used to detect when a noise template is imported and checks to see if the shaders need to be regenerated.
    /// </summary>
    [ScriptedImporter(1, "noisehlsltemplate")]
    public class NoiseTemplateImporter : ScriptedImporter
    {
        /// <summary>
        /// Function that is called when an asset with the ".noisehlsltemplate" extension is imported by the AssetDatabase
        /// </summary>
        /// <param name = "ctx"> The context for the imported asset </param>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            NoiseLib.GenerateHeaderFiles();
            NoiseLib.GenerateShaders();
        }
    }
}