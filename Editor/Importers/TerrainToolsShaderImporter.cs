using UnityEngine;
using System.IO;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace UnityEditor.TerrainTools
{
#if USE_HIGH_DEFINITION_RENDERPIPELINE && USE_UNIVERSAL_RENDERPIPELINE
    [ScriptedImporter(0, new[] { URPExtension, HDRPExtension })]
#elif USE_UNIVERSAL_RENDERPIPELINE
    [ScriptedImporter(1, URPExtension)]
#elif USE_HIGH_DEFINITION_RENDERPIPELINE
     [ScriptedImporter(2, HDRPExtension)]
#endif
    class TerrainToolsShaderImporter : ScriptedImporter
    {
        const string HDRPExtension = "hdrpterraintoolshader";
        const string URPExtension = "urpterraintoolshader";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            Shader shader = null;
#if UNITY_2020_2_OR_NEWER
            //2020.2 or later supports shader dependencies registration
            shader = ShaderUtil.CreateShaderAsset(ctx, File.ReadAllText(ctx.assetPath), false);
#else
            //Versions of older unity don't support asset system context shader dependencies registration
            shader = ShaderUtil.CreateShaderAsset(File.ReadAllText(ctx.assetPath), false);
#endif
            ctx.AddObjectToAsset("MainAsset", shader);
            ctx.SetMainObject(shader);
        }
    }
}