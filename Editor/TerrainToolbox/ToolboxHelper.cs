using System;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.TerrainTools;
using UnityEngine.Rendering;

namespace UnityEditor.TerrainTools
{
    internal class ToolboxHelper
    {
        // Toolbox setting serialization 
        public static string LibraryPath = "/../Library/TerrainTools/";
        public static string ToolboxPrefsWindow = "ToolboxWindowPrefs";
        public static string ToolboxPrefsCreate = "ToolboxCreatePrefs";
        public static string ToolboxPrefsSettings = "ToolboxSettingsPrefs";
        public static string ToolboxPrefsUtility = "ToolboxUtilityPrefs";
        public static string ToolboxPrefsVisualization = "ToolboxVisualizationPrefs";

        public enum ByteOrder { Mac = 1, Windows = 2 };
        public enum RenderPipeline
        {
            Default,
            HD,
            LW,
            Universal,
            None
        }

        public static int[] GUITextureResolutions = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096 };
        public static string[] GUITextureResolutionNames = new string[] { "32", "64", "128", "256", "512", "1024", "2048", "4096" };
        public static int[] GUIHeightmapResolutions = new int[] { 33, 65, 129, 257, 513, 1025, 2049, 4097 };
        public static GUIContent[] GUIHeightmapResolutionNames = new GUIContent[]
        {
            EditorGUIUtility.TrTextContent("33"),
            EditorGUIUtility.TrTextContent("65"),
            EditorGUIUtility.TrTextContent("129"),
            EditorGUIUtility.TrTextContent("257"),
            EditorGUIUtility.TrTextContent("513"),
            EditorGUIUtility.TrTextContent("1025"),
            EditorGUIUtility.TrTextContent("2049"),
            EditorGUIUtility.TrTextContent("4097")
        };

        public static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        public static bool IsInteger(double x)
        {
            return (x % 1) == 0;
        }

        public static string GetPrefFilePath(string prefType)
        {
            string filePath = string.Empty;
            string dirPath = Application.dataPath + LibraryPath;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            filePath = dirPath + prefType + ".json";

            return filePath;
        }

        public static string GetBitDepth(Heightmap.Depth depth)
        {
            switch (depth)
            {
                case Heightmap.Depth.Bit16:
                    return "16 bit";
                case Heightmap.Depth.Bit8:
                    return "8 bit";
                default:
                    return "8 bit";
            }
        }

        public static Terrain[] GetSelectedTerrainsInScene()
        {
            var objs = Selection.GetFiltered(typeof(Terrain), SelectionMode.Unfiltered);
            var terrains = new Terrain[objs.Length];
            for (var i = 0; i < objs.Length; i++)
            {
                terrains[i] = objs[i] as Terrain;
            }

            return terrains;
        }

        public static Terrain[] GetAllTerrainsInScene()
        {
            return GameObject.FindObjectsOfType<Terrain>();
        }

        public static void CalculateAdjacencies(Terrain[] terrains, int tilesX, int tilesZ)
        {
            if (terrains == null || terrains.Length == 0)
            {
                return;
            }

            // set neighbor terrains to update normal maps
            for (int y = 0; y < tilesZ; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    int index = (y * tilesX) + x;
                    Terrain terrain = terrains[index];
                    Terrain leftTerrain = (x > 0) ? terrains[index - 1] : null;
                    Terrain rightTerrain = (x < tilesX - 1) ? terrains[index + 1] : null;
                    Terrain topTerrain = (y > 0) ? terrains[index - tilesX] : null;
                    Terrain bottomTerrain = (y < tilesZ - 1) ? terrains[index + tilesX] : null;

                    // NOTE: "top" and "bottom" are reversed because of the way the terrain is handled...
                    terrain.SetNeighbors(leftTerrain, bottomTerrain, rightTerrain, topTerrain);
                }
            }
        }
        public static Texture2D GetTextureCopy(Texture2D texture)
        {
            var creationFlags = texture.mipmapCount > 0
                ? TextureCreationFlags.MipChain
                : TextureCreationFlags.None;
            var textureCopy = new Texture2D(texture.width, texture.height, texture.graphicsFormat, texture.mipmapCount,
                creationFlags);
            Graphics.CopyTexture(texture, textureCopy);
            return textureCopy;
        }

        public static Texture2D GetPartialTexture(Texture2D sourceTexture, Vector2Int resolution, Vector2Int offset)
        {
            if (offset.x > resolution.x || offset.y > resolution.y)
                return null;

            var destColor = sourceTexture.GetPixels(offset.x, offset.y, resolution.x, resolution.y);
            Texture2D newTexture = new Texture2D(resolution.x, resolution.y);
            newTexture.SetPixels(destColor);

            return newTexture;
        }

        // referencing from TerrainInspector.ResizeControltexture()
        public static void ResizeControlTexture(TerrainData terrainData, int resolution)
        {
            RenderTexture oldRT = RenderTexture.active;
            RenderTexture[] oldAlphaMaps = new RenderTexture[terrainData.alphamapTextureCount];
            for (int i = 0; i < oldAlphaMaps.Length; i++)
            {
                terrainData.alphamapTextures[i].filterMode = FilterMode.Bilinear;
                oldAlphaMaps[i] = RenderTexture.GetTemporary(resolution, resolution, 0, SystemInfo.GetGraphicsFormat(DefaultFormat.HDR));
                Graphics.Blit(terrainData.alphamapTextures[i], oldAlphaMaps[i]);
            }

            Undo.RegisterCompleteObjectUndo(terrainData, "Resize alphamap");

            terrainData.alphamapResolution = resolution;
            for (int i = 0; i < oldAlphaMaps.Length; i++)
            {
                RenderTexture.active = oldAlphaMaps[i];

                CopyActiveRenderTextureToTexture(terrainData.GetAlphamapTexture(i), new RectInt(0, 0, resolution, resolution), Vector2Int.zero, false);
            }
            terrainData.SetBaseMapDirty();
            RenderTexture.active = oldRT;
            for (int i = 0; i < oldAlphaMaps.Length; i++)
            {
                RenderTexture.ReleaseTemporary(oldAlphaMaps[i]);
            }

            terrainData.SetBaseMapDirty();
        }

        // referencing from TerrainData.GPUCopy.CopyActiveRenderTextureToTexture()
        public static void CopyActiveRenderTextureToTexture(Texture2D dstTexture, RectInt sourceRect, Vector2Int dest, bool allowDelayedCPUSync)
        {
            var source = RenderTexture.active;
            if (source == null)
                throw new InvalidDataException("Active RenderTexture is null.");

            int dstWidth = dstTexture.width;
            int dstHeight = dstTexture.height;

            allowDelayedCPUSync = allowDelayedCPUSync && SupportsCopyTextureBetweenRTAndTexture;
            if (allowDelayedCPUSync)
            {
                if (dstTexture.mipmapCount > 1)
                {
                    var tmp = RenderTexture.GetTemporary(new RenderTextureDescriptor(dstWidth, dstHeight, source.format));
                    if (!tmp.IsCreated())
                    {
                        tmp.Create();
                    }
                    Graphics.CopyTexture(dstTexture, 0, 0, tmp, 0, 0);
                    Graphics.CopyTexture(source, 0, 0, sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height, tmp, 0, 0, dest.x, dest.y);

                    tmp.GenerateMips();
                    Graphics.CopyTexture(tmp, dstTexture);
                    RenderTexture.ReleaseTemporary(tmp);
                }
                else
                {
                    Graphics.CopyTexture(source, 0, 0, sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height, dstTexture, 0, 0, dest.x, dest.y);
                }
            }
            else
            {
                if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal || !SystemInfo.graphicsUVStartsAtTop)
                    dstTexture.ReadPixels(new Rect(sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height), dest.x, dest.y);
                else
                    dstTexture.ReadPixels(new Rect(sourceRect.x, source.height - sourceRect.yMax, sourceRect.width, sourceRect.height), dest.x, dest.y);
                dstTexture.Apply(true);
            }
        }

        private static bool SupportsCopyTextureBetweenRTAndTexture {
            get
            {
                const CopyTextureSupport kRT2TexAndTex2RT = CopyTextureSupport.RTToTexture | CopyTextureSupport.TextureToRT;
                return (SystemInfo.copyTextureSupport & kRT2TexAndTex2RT) == kRT2TexAndTex2RT;
            }
        }

        public static float kNormalizedHeightScale => 32766.0f / 65535.0f;
        public static void CopyTextureToTerrainHeight(TerrainData terrainData, Texture2D heightmap, Vector2Int indexOffset, int resolution, int numTiles, float baseLevel, float remap)
        {
            terrainData.heightmapResolution = resolution + 1;

            float hWidth = heightmap.height;
            float div = hWidth / numTiles;

            float scale = ((resolution / (resolution + 1.0f)) * (div + 1)) / hWidth;
            float offset = ((resolution / (resolution + 1.0f)) * div) / hWidth;

            Vector2 scaleV = new Vector2(scale, scale);
            Vector2 offsetV = new Vector2(offset * indexOffset.x, offset * indexOffset.y);

            Material blitMaterial = GetHeightBlitMaterial();
            blitMaterial.SetFloat("_Height_Offset", baseLevel * kNormalizedHeightScale);
            blitMaterial.SetFloat("_Height_Scale", remap * kNormalizedHeightScale);
            RenderTexture heightmapRT = RenderTexture.GetTemporary(terrainData.heightmapTexture.descriptor);
            Graphics.Blit(heightmap, heightmapRT, blitMaterial);

            Graphics.Blit(heightmapRT, terrainData.heightmapTexture, scaleV, offsetV);

            terrainData.DirtyHeightmapRegion(new RectInt(0, 0, terrainData.heightmapTexture.width, terrainData.heightmapTexture.height), TerrainHeightmapSyncControl.HeightAndLod);
            RenderTexture.ReleaseTemporary(heightmapRT);
        }

        public static void ResizeHeightmap(TerrainData terrainData, int resolution)
        {
            RenderTexture oldRT = RenderTexture.active;

            RenderTexture oldHeightmap = RenderTexture.GetTemporary(terrainData.heightmapTexture.descriptor);
            Graphics.Blit(terrainData.heightmapTexture, oldHeightmap);

#if UNITY_2019_3_OR_NEWER
            // terrain holes
            RenderTexture oldHoles = RenderTexture.GetTemporary(terrainData.holesTexture.width, terrainData.holesTexture.height);
            Graphics.Blit(terrainData.holesTexture, oldHoles);
#endif

            Undo.RegisterCompleteObjectUndo(terrainData, "Resize heightmap");

            float sUV = 1.0f;
            int dWidth = terrainData.heightmapResolution;
            int sWidth = resolution;

            Vector3 oldSize = terrainData.size;
            terrainData.heightmapResolution = resolution;
            terrainData.size = oldSize;

            oldHeightmap.filterMode = FilterMode.Bilinear;

            // Make sure textures are offset correctly when resampling
            // tsuv = (suv * swidth - 0.5) / (swidth - 1)
            // duv = (tsuv(dwidth - 1) + 0.5) / dwidth
            // duv = (((suv * swidth - 0.5) / (swidth - 1)) * (dwidth - 1) + 0.5) / dwidth
            // k = (dwidth - 1) / (swidth - 1) / dwidth
            // duv = suv * (swidth * k)		+ 0.5 / dwidth - 0.5 * k

            float k = (dWidth - 1.0f) / (sWidth - 1.0f) / dWidth;
            float scaleX = sUV * (sWidth * k);
            float offsetX = (float)(0.5 / dWidth - 0.5 * k);
            Vector2 scale = new Vector2(scaleX, scaleX);
            Vector2 offset = new Vector2(offsetX, offsetX);

            Graphics.Blit(oldHeightmap, terrainData.heightmapTexture, scale, offset);
            RenderTexture.ReleaseTemporary(oldHeightmap);

#if UNITY_2019_3_OR_NEWER
            oldHoles.filterMode = FilterMode.Point;
            Graphics.Blit(oldHoles, (RenderTexture)terrainData.holesTexture);
            RenderTexture.ReleaseTemporary(oldHoles);
#endif

            RenderTexture.active = oldRT;

            terrainData.DirtyHeightmapRegion(new RectInt(0, 0, terrainData.heightmapTexture.width, terrainData.heightmapTexture.height), TerrainHeightmapSyncControl.HeightAndLod);
#if UNITY_2019_3_OR_NEWER
            terrainData.DirtyTextureRegion(TerrainData.HolesTextureName, new RectInt(0, 0, terrainData.holesTexture.width, terrainData.holesTexture.height), false);
#endif
        }

        // Unity PNG encoder does not support 16bit export, change will come later 2019
        public static void ExportTerrainHeightsToTexture(TerrainData terrainData, Heightmap.Format format, string path, bool flipVertical, Vector2 inputLevelsRange)
        {
            RenderTexture oldRT = RenderTexture.active;
            int width = terrainData.heightmapTexture.width - 1;
            int height = terrainData.heightmapTexture.height - 1;
            var texture = new Texture2D(width, height, terrainData.heightmapTexture.graphicsFormat, TextureCreationFlags.None);
            RenderTexture.active = terrainData.heightmapTexture;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            //Remap Texture
            Color[] pixels = texture.GetPixels();
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i].r = (pixels[i].r * 2) * (inputLevelsRange.y - inputLevelsRange.x) + inputLevelsRange.x;
                pixels[i + 1].r = (pixels[i + 1].r * 2) * (inputLevelsRange.y - inputLevelsRange.x) + inputLevelsRange.x;
                pixels[i + 2].r = (pixels[i + 2].r * 2) * (inputLevelsRange.y - inputLevelsRange.x) + inputLevelsRange.x;
                pixels[i + 3].r = (pixels[i + 3].r * 2) * (inputLevelsRange.y - inputLevelsRange.x) + inputLevelsRange.x;
            }
            texture.SetPixels(pixels);
            texture.Apply();

            //Flip Texture
            if (flipVertical)
                ToolboxHelper.FlipTexture(texture, true);

            byte[] bytes;
            switch (format)
            {
                case Heightmap.Format.TGA:
                    bytes = texture.EncodeToTGA();
                    path = path + ".tga";
                    break;
                default:
                    bytes = texture.EncodeToPNG();
                    path = path + ".png";
                    break;
            }

            File.WriteAllBytes(path, bytes);
            RenderTexture.active = oldRT;
        }

        /// <summary>
        /// Get a path relative to the project directory for saving assets.
        /// If the path is outside the assets directory it will return the Assets folder.
        /// </summary>
        /// <param name="requestedDirectory">The directory we want to save to, absolute or relative</param>
        /// <param name="forceToAssets">If we want the directory to be forced into the assets directory</param>
        /// <returns>A relative path, inside the project directory</returns>
        internal static string GetProjectRelativeSaveDirectory(string requestedDirectory, bool forceToAssets = true)
        {
            var referenceUri = new Uri(Application.dataPath + "/..");
            if (Path.IsPathRooted(requestedDirectory))
            {
                try
                {
                    var fileUri = new Uri(requestedDirectory);
                    requestedDirectory = Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString());
                }
                catch
                {
                    if (forceToAssets)
                        requestedDirectory = "Assets";
                    return requestedDirectory;
                }
            }
            else
            {
                var fileUri = new Uri(Application.dataPath + "/../" + requestedDirectory);
                requestedDirectory = Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString());
            }
            // if the directory doesn't start with "Assets+Path.DirectorySeparatorChar"
            // then the terraindata will not end up in the assetdatabase, and not correctly be loaded
            // so force it to the correct location
            if (forceToAssets && (!requestedDirectory.StartsWith("Assets") || !requestedDirectory.StartsWith("Assets/")))
                requestedDirectory = "Assets";
            return requestedDirectory;
        }

        /// <summary>
        /// Reports if a directory is within the Project/Assets folder.
        /// Directory does not need to exist.
        /// </summary>
        /// <param name="requestedDirectory"></param>
        /// <returns></returns>
        internal static bool IsDirectoryWithinAssets(string requestedDirectory)
        {
            return GetProjectRelativeSaveDirectory(requestedDirectory, false) ==
                   GetProjectRelativeSaveDirectory(requestedDirectory);
        }

        public static void ExportTerrainHeightsToRawFile(TerrainData terrainData, string path, Heightmap.Depth depth, bool flipVertical, ByteOrder byteOrder, Vector2 inputLevelsRange)
        {
            // trim off the extra 1 pixel, so we get a power of two sized texture
#if UNITY_2019_3_OR_NEWER
            int heightmapWidth = terrainData.heightmapResolution - 1;
            int heightmapHeight = terrainData.heightmapResolution - 1;
#else
            int heightmapWidth = terrainData.heightmapWidth - 1;
            int heightmapHeight = terrainData.heightmapHeight - 1;
#endif
            float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
            byte[] data = new byte[heightmapWidth * heightmapHeight * (int)depth];

            if (depth == Heightmap.Depth.Bit16)
            {
                float normalize = (1 << 16);
                for (int y = 0; y < heightmapHeight; ++y)
                {
                    for (int x = 0; x < heightmapWidth; ++x)
                    {
                        int index = x + y * heightmapWidth;
                        int srcY = flipVertical ? heightmapHeight - 1 - y : y;

                        float remappedHeight = heights[srcY, x] * (inputLevelsRange.y - inputLevelsRange.x) + inputLevelsRange.x;

                        int height = Mathf.RoundToInt(remappedHeight * normalize);
                        ushort compressedHeight = (ushort)Mathf.Clamp(height, 0, ushort.MaxValue);

                        byte[] byteData = System.BitConverter.GetBytes(compressedHeight);
                        if ((byteOrder == ByteOrder.Mac) == System.BitConverter.IsLittleEndian)
                        {
                            data[index * 2 + 0] = byteData[1];
                            data[index * 2 + 1] = byteData[0];
                        }
                        else
                        {
                            data[index * 2 + 0] = byteData[0];
                            data[index * 2 + 1] = byteData[1];
                        }
                    }
                }
            }
            else
            {
                float normalize = (1 << 8);
                for (int y = 0; y < heightmapHeight; ++y)
                {
                    for (int x = 0; x < heightmapWidth; ++x)
                    {
                        int index = x + y * heightmapWidth;
                        int srcY = flipVertical ? heightmapHeight - 1 - y : y;

                        float remappedHeight = heights[y, x] * (inputLevelsRange.y - inputLevelsRange.x) + inputLevelsRange.x;

                        int height = Mathf.RoundToInt(remappedHeight * normalize);
                        byte compressedHeight = (byte)Mathf.Clamp(height, 0, byte.MaxValue);
                        data[index] = compressedHeight;
                    }
                }
            }

            FileStream fs = new FileStream((path + ".raw"), FileMode.Create);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }

        public static Material GetHeightBlitMaterial()
        {
            return new Material(Shader.Find("Hidden/TerrainTools/HeightBlit"));
        }

        public static void FlipTexture(Texture2D texture, bool isHorizontal)
        {
            Color[] originalPixels = texture.GetPixels();
            Color[] flippedPixels = new Color[originalPixels.Length];
            int width = texture.width;
            int height = texture.height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int flippedIndex = isHorizontal ? y * width + width - 1 - x : (height - 1) * width - y * width + x;
                    int originalIndex = y * width + x;
                    if (flippedIndex < 0)
                        continue;
                    flippedPixels[flippedIndex] = originalPixels[originalIndex];
                }
            }
            texture.SetPixels(flippedPixels);
            texture.Apply();
        }

        public static RenderPipeline GetRenderPipeline()
        {
            if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset == null)
            {
                return RenderPipeline.Default;
            }
            else if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().FullName
                == "UnityEngine.Rendering.HighDefinition.HDRenderPipelineAsset")
            {
                return RenderPipeline.HD;
            }
            else if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().FullName
                == "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset")
            {
                return RenderPipeline.Universal;
            }
            else if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().FullName
                == "UnityEngine.Rendering.LWRP.LightweightRenderPipelineAsset")
            {
                return RenderPipeline.LW;
            }

            return RenderPipeline.Default;
        }
    }
}
