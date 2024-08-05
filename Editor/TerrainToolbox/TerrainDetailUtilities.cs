using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Object = UnityEngine.Object;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    ///     Functions for manipulating terrain details
    /// </summary>
    public static class DetailUtility
    {
        /// <summary>
        ///     Gets a TerrainData's detailLayer as a Texture2D
        /// </summary>
        /// <param name="tData">a TerrainData object</param>
        /// <param name="detailLayer">the detail layer to use</param>
        /// <returns>Texture2D representing the grayscale values of the detail masks</returns>
        public static Texture2D GetDensityMapTexture(TerrainData tData, int detailLayer = 0)
        {
            var detailWidth = tData.detailWidth;
            var detailHeight = tData.detailHeight;

            var bucketSize = InstanceCountMultiplier(tData);
            var detailArray = tData
                .GetDetailLayer(0, 0, detailWidth, detailHeight, detailLayer)
                .Cast<int>()
                .Select(i => (byte) Mathf.Min(i * bucketSize, 255));
            
            var outputTexture = new Texture2D(detailWidth, detailHeight, TextureFormat.R8, false);
            outputTexture.name = $"{tData.name}_densitymap";
            outputTexture.SetPixelData(detailArray.ToArray(), 0);
            outputTexture.Apply(false);
            return outputTexture;
        }

        /// <summary>
        ///     Saves a Texture2D representing the detail density in <paramref name="terrain"/>
        ///     to the asset folder <paramref name="folder"/>
        /// </summary>
        /// <param name="terrain">A terrain object to save</param>
        /// <param name="folder">An asset folder to receive saved assets</param>
        /// <param name="detailLayer">The detail layer to save</param>
        public static void SaveDensityMap(Terrain terrain, DefaultAsset folder, int detailLayer)
        {
            var texToSave = GetDensityMapTexture(terrain.terrainData, detailLayer);
            var outputFile = GetDensityMapName(terrain, folder, detailLayer);
            AssetDatabase.CreateAsset(texToSave, outputFile);
            Debug.Log($"Saved {outputFile}");
        }

        /// <summary>
        ///     Save a detail density map for every prototype in the supplied terrain to <paramref name="folder"/>
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="folder"></param>
        public static void SaveAllDensityMaps(Terrain terrain, DefaultAsset folder)
        {
            var layers = terrain.terrainData.detailPrototypes.Length;
            for (var i = 0; i < layers; i++) SaveDensityMap(terrain, folder, i);
        }

        /// <summary>
        ///     Save a detail density map for every prototype in the supplied terrain to <paramref name="folder"/>
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="folderName">String path to the asset folder</param>
        public static void SaveAllDensityMaps(Terrain terrain, string folderName)
        {
            var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderName);
            if (folder is null) throw new ArgumentException($"folderAsset {folderName} not found");
            SaveAllDensityMaps(terrain, folder);
        }


        /// <summary>
        ///     Generates a predictable asset file path for a texture assert representing a detail density map
        /// </summary>
        /// <param name="terrain">The terrain object whose details will be saved</param>
        /// <param name="folderAsset">A DefaultAsset representing the Asset folder where the files will be saved</param>
        /// <param name="detailLayer">The index of the detail layer being saveds</param>
        /// <returns></returns>
        private static string GetDensityMapName(Terrain terrain, DefaultAsset folderAsset, int detailLayer)
        {
            return $"{AssetDatabase.GetAssetPath(folderAsset)}/{terrain.name}_details_{detailLayer:00}.asset";
        }

        /// <summary>
        /// Loads a saved density map for <paramref name="terrain"/> from <paramref name="folder"/>
        /// into detail layer <paramref name="detailLayer"/>
        /// </summary>
        /// <param name="terrain">Terrain object</param>
        /// <param name="folder">DefaultAsset or string folder path</param>
        /// <param name="detailLayer">integer detail layer</param>
        /// <returns> True if the expected texture asset exists and can be applied, or false otherwise.</returns>
        public static bool LoadDensityMap(Terrain terrain, DefaultAsset folder, int detailLayer)
        {
            if (detailLayer > terrain.terrainData.detailPrototypes.Length)
            {
                Debug.Log($"{terrain.name} has no detail layer {detailLayer}");
                return false;
            }
            
            var inputFile = GetDensityMapName(terrain, folder, detailLayer);

            var inputTx = AssetDatabase.LoadAssetAtPath<Texture2D>(inputFile);
            if (inputTx is null)
            {
                var expected = GetDensityMapName(terrain, folder, detailLayer);
                Debug.Log($"Could not find map {expected} for detail layer {detailLayer}");
                return false;
            }

            SetDensityMap(terrain.terrainData, inputTx, detailLayer);
            Debug.Log($"Applied {inputFile} to {terrain.name} detail layer {detailLayer}");
            return true;
        }

        /// <summary>
        /// Loads a saved density map for <paramref name="terrain"/> from <paramref name="folder"/>
        /// into detail layer <paramref name="detailLayer"/>
        /// </summary>
        /// <param name="terrain">Terrain object</param>
        /// <param name="folderName">string folder path</param>
        /// <param name="detailLayer">integer detail layer</param>
        /// <returns> True if the expected texture asset exists and can be applied, or false otherwise.</returns>
        public static bool LoadDensityMap(Terrain terrain, string folderName, int detailLayer)
        {
            var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderName);
            if (folder is null) throw new ArgumentException($"folderAsset {folderName} not found");
            return LoadDensityMap(terrain, folder, detailLayer);
        }
        
        
        
        /// <summary>
        ///     Given a DefaultAsset <paramref name="folder"/> to seach, finds the density maps corresponding to
        ///     <paramref name="terrain"/> and loads them into the corresponding detail channels of <paramref name="terrain"/>.
        /// </summary>
        /// <param name="terrain">a Terrain object</param>
        /// <param name="folder">a DefaultAsset corresponding to Asset folder containing textures</param>
        public static void LoadDensityMaps(Terrain terrain, DefaultAsset folder)
        {
            Undo.RegisterCompleteObjectUndo(terrain.terrainData, "Load saved density maps");
            var tdata = terrain.terrainData;
            var layers = tdata.detailPrototypes.Length;
            var restored = 0;
            for (var i = 0; i < layers; i++)
            {
                var loaded = LoadDensityMap(terrain, folder, i);
                if (loaded) restored++;
                i++;
            }

            if (restored > 0)
                Debug.Log($"Loaded {restored} detail maps");
            else
                Debug.LogWarning("Unable to find matching detail maps");
        }

        /// <summary>
        ///     Given a the text path of an asset folder to seach, finds the detail maps corresponding
        ///     to <paramref name="terrain"/> and loads them into the corresponding detail channels of <paramref name="terrain"/>.
        /// </summary>
        /// <param name="terrain">a Terrain object</param>
        /// <param name="folderName">an Asset folder containing textures</param>
        public static void LoadDensityMaps(Terrain terrain, string folderName)
        {
            var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderName);
            if (folder is null) throw new ArgumentException($"folderAsset {folderName} not found");

            LoadDensityMaps(terrain, folder);
        }

        /// <summary>
        ///     Populates a TerrainData's detail layer using the red channel of the supplied texture as a density
        ///     map.  Bright texels translate to more detail instances, dark texels translate to
        ///     fewer or none.
        ///     By default, the texture will be resampled to the correct size for the TerrainData's detail
        ///     array. If the optional allowResample parameter is passed as false, the method will
        ///     throw an ArgumentException if the texture and detail resolutions don't match.
        ///     This method will accept any Texture2D as an input, but for best results use a texture
        ///     which is not saved with gamma or color correction.
        /// </summary>
        /// <param name="tData">the TerrainData to populate</param>
        /// <param name="texture">a Texture2D.</param>
        /// <param name="detailLayer">the detail layer to populate</param>
        /// <param name="allowResample">
        ///     if true, resample the texture to fit the detail array. If false, accept only textures of
        ///     the same dimensions as the detail array
        /// </param>
        /// <exception cref="ArgumentException">
        ///     thrown if allowResample is false and the texture resolution does not match the
        ///     detail array resolution
        /// </exception>
        public static void SetDensityMap(TerrainData tData, in Texture2D texture, int detailLayer = 0,
            bool allowResample = true)
        {
            var w = tData.detailWidth;
            var h = tData.detailHeight;

            var assignableTexture = texture;
            if (w != texture.width || h != texture.height)
            {
                // if allowResample is false, we except on mismatched textures sizes to prevent an unintended
                // RenderTexture copy otherwise, we use Graphics.Blit to resample the texture to size
                if (!allowResample)
                {
                    var msg =
                        $"SetDensityMap expected a texture of {w}X{h} pixels, got {texture.width}X{texture.height} pixels";
                    throw new ArgumentException(msg);
                }

                using (var targetRT = new RenderTextureContext(w, h))
                {
                    Graphics.Blit(texture, targetRT);
                    assignableTexture = DensityMapFromRenderTexture(targetRT);
                }
            }

            if (assignableTexture.isReadable == false)
                throw new ArgumentException(
                    $"Texture {texture} is not readable. Set it readable in the import settings, or use a dynamically created texture");

            var grayscale = GrayscaleMultiplier(tData);     
            
            var grayValues = assignableTexture.GetPixels()
                .Select(c => Mathf.CeilToInt(grayscale * c.r));

            var detailArray = new int[w, h];
            Buffer.BlockCopy(grayValues.ToArray(), 0, detailArray, 0, w * h * sizeof(int));
            tData.SetDetailLayer(0, 0, detailLayer, detailArray);
            if (!ReferenceEquals(assignableTexture, texture)) Object.DestroyImmediate(assignableTexture);
        }


        /// <summary>
        ///     Returns a readable Texture2D from the supplied RenderTarget.
        /// </summary>
        /// <param name="renderTexture">an R8 format RenderTexture</param>
        /// <returns>Texture2D</returns>
        private static Texture2D DensityMapFromRenderTexture(in RenderTexture renderTexture)
        {
            if (renderTexture.format != RenderTextureFormat.R8)
                throw new ArgumentException(
                    $"DensityMapFromRenderTexture expects a single channel 8-bit renderTextures, got {renderTexture.format}");

            var w = renderTexture.width;
            var h = renderTexture.height;
            var copyRect = new Rect(0, 0, w, h);
            var outputTexture = new Texture2D(w, h, TextureFormat.R8, false);
            outputTexture.ClearMinimumMipmapLevel();
            outputTexture.Apply(false);

            using (var RTContext = new ActiveRenderTextureContext(renderTexture))
            {
                outputTexture.ReadPixels(copyRect, 0, 0, false);
                outputTexture.Apply(false);
            }

            return outputTexture;
        }


        /// <summary>
        ///     Sets a TerrainData's detail layer using the supplied RenderTextures
        ///     This executes synchronously
        /// </summary>
        /// <param name="tData"></param>
        /// <param name="rTexture"></param>
        /// <param name="detailLayer"></param>
        public static void SetDensityMap(TerrainData tData, RenderTexture rTexture, int detailLayer = 0)
        {
            if (rTexture.format != RenderTextureFormat.R8)
                throw new ArgumentException(
                    $"SetDensityMap expects a single channel 8-bit renderTextures, got {rTexture.format}");

            SetDensityMap(tData, DensityMapFromRenderTexture(rTexture), detailLayer);
        }


        /// <summary>
        ///     Use the supplied material to generate a detail layer map for the supplied TerrainData
        ///     The material will be blitted to an 8-bit R8 RenderTarget of the same resolution as
        ///     the TerrainData's detail layers.
        ///     The material will be given references to the TerrainData's splatmap textures
        ///     (referenced as "_Control0" and "_Control1" if present), and the heightmap
        ///     (referenced as "_Height").  If instanced rendering is enabled, the material
        ///     will also be given a reference to the TerrainData's normal map ("_Normal").
        ///     Existing detail arrays will be passed to the material as RenderTextures
        ///     (referenced as "_Density#"  where # is the detail layer number.
        ///     The material can use or ignore these inputs as desired.
        /// </summary>
        /// <param name="tData">(this) target TerrainData</param>
        /// <param name="densityMaterial">a material to blit</param>
        /// <param name="detailLayer">the detail index of the details to scatter</param>
        public static void SetDensityMap(TerrainData tData, Material densityMaterial,
            int detailLayer = 0)
        {
            var blitMat = new Material(densityMaterial);

            var splat = 0;
            foreach (var eachAlphamapTexture in tData.alphamapTextures)
            {
                blitMat.SetTexture($"_Control{splat}", eachAlphamapTexture);
                splat++;
            }

            blitMat.SetTexture("_Height", tData.heightmapTexture);

            var detailTexturesToRelease = new List<Texture2D>();
            for (var d = 0; d < tData.detailPrototypes.Length; d++)
            {
                detailTexturesToRelease.Add(GetDensityMapTexture(tData, d));
                blitMat.SetTexture($"_Density{d}", detailTexturesToRelease[d]);
            }

            foreach (var eachTerrain in Terrain.activeTerrains)
                if (eachTerrain.terrainData == tData)
                    blitMat.SetTexture("_Normal", eachTerrain.normalmapTexture);

            // @todo: Verify that this works under all pipelines!
            using (var detailRT = new RenderTextureContext(tData.detailWidth, tData.detailHeight))
            {
                using (var activeRT = new ActiveRenderTextureContext(detailRT))
                {
                    Graphics.Blit(null, detailRT, blitMat, 0);
                    SetDensityMap(tData, detailRT, detailLayer);
                }
            }

            Object.DestroyImmediate(blitMat);
            foreach (var eachTempTexture in detailTexturesToRelease) Object.DestroyImmediate(eachTempTexture);
        }

        
        
        // Prior to 2022.2, detail arrays were limited to 16 items per cell
        // after that, they have a full eight bits.  The following two methods
        // return the appropriate multipliers based on the availability
        // of the new API
        private static int InstanceCountMultiplier(TerrainData tData)
        {
#if UNITY_2022_2_OR_NEWER
        return (tData.detailScatterMode == DetailScatterMode.InstanceCountMode) ? 16 : 1;
#else
        return 16;
#endif
        }
        
        private static int GrayscaleMultiplier(TerrainData tData)
        {
#if UNITY_2022_2_OR_NEWER
        return tData.maxDetailScatterPerRes;
#else
            return 16;
#endif
        }

        
        /// <summary>
        ///     Store the current active render texture, and restore it when its context closes
        /// </summary>
        private class ActiveRenderTextureContext : IDisposable
        {
            private readonly RenderTexture _cachedRenderTexture;

            public ActiveRenderTextureContext(RenderTexture newActiveRenderTexture = null)
            {
                _cachedRenderTexture = RenderTexture.active;
                if (newActiveRenderTexture is not null) RenderTexture.active = newActiveRenderTexture;
            }

            public void Dispose()
            {
                RenderTexture.active = _cachedRenderTexture;
            }
        }

        /// <summary>
        ///     A RenderTexture which releases automatically when its context closes.
        /// </summary>
        private class RenderTextureContext : IDisposable
        {
            private readonly RenderTexture _tempRT;

            public RenderTextureContext(int width, int height)
            {
                _tempRT = RenderTexture.GetTemporary(width, height, 0, GraphicsFormat.R8_UNorm, 1);
            }

            public void Dispose()
            {
                RenderTexture.ReleaseTemporary(_tempRT);
            }

            public static implicit operator RenderTexture(RenderTextureContext ctx)
            {
                return ctx._tempRT;
            }
        }
        
    }
}