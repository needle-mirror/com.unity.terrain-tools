using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.TerrainTools
{
    // The terrain heightmap can be packed into two 8-bit channels (see PackHeightmap in UnityCG.cginc).
    // Code that reads heights as a plain float, such as compute shaders, needs an unpacked copy.
    internal static class HeightmapUnpack
    {
        static Material s_Material;

        // Single channel format for a plain height. R16_UNorm and R32_SFloat hold a 16-bit height exactly,
        // R16_SFloat and the heightmap format (the last resort, when no float format can be rendered) hold less.
        public static GraphicsFormat format =>
#pragma warning disable 0618
            SystemInfo.IsFormatSupported(GraphicsFormat.R16_UNorm, FormatUsage.Render) ? GraphicsFormat.R16_UNorm :
            SystemInfo.IsFormatSupported(GraphicsFormat.R32_SFloat, FormatUsage.Render) ? GraphicsFormat.R32_SFloat :
            SystemInfo.IsFormatSupported(GraphicsFormat.R16_SFloat, FormatUsage.Render) ? GraphicsFormat.R16_SFloat :
#pragma warning restore 0618
            Terrain.heightmapFormat;

        public static void Blit(Texture heightmap, RenderTexture dest)
        {
            if (s_Material == null)
                s_Material = new Material(Shader.Find("Hidden/TerrainTools/HeightmapUnpack")) { hideFlags = HideFlags.HideAndDontSave };

            Graphics.Blit(heightmap, dest, s_Material);
        }
    }
}
