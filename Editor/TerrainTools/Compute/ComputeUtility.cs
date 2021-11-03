using UnityEditor;
using UnityEngine;

internal static class ComputeUtility
{
    internal static ComputeShader GetShader(string name) {
        var computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Packages/com.unity.terrain-tools/Editor/TerrainTools/Compute/" + name + ".compute");
        if (computeShader == null) {
            throw new MissingReferenceException("Could not find compute shader with name " + name);
        }
        return computeShader;
    }
}
