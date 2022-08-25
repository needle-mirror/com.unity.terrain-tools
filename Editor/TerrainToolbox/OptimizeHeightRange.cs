using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    ///     Exposes functions for making the best available use of the heightmap range on a given terrain
    /// </summary>
    public static class OptimizeHeightRange
    {
        /// <summary>
        ///     Performs RemapTerrainHeights on the currently selected Terrain with a target efficiency of 95%
        /// </summary>
        /// <param name="padBottom">Padding added below the height range; 0 - 1, relative the total height.</param>
        /// <param name="padTop">Padding added above the height range; 0 - 1, relative the total height.</param>
        public static void OptimizeSelectedTerrains(float padBottom = 0.025f, float padTop = 0.025f)
        {
            var selectedTerrains = SelectedContiguousTerrains(out bool isSelectedContiguous);
            
            if (! isSelectedContiguous)
            {
                Debug.LogWarning("Choose one or more terrains from the same Terrain group before calling OptimizeSelectedTerrains");
                return;
            }
            
            Undo.RegisterCompleteObjectUndo(GetUndoArray(selectedTerrains), "Optimize heightmap");
            var terrainDatas= new List<TerrainData>();

            var bottom = float.MaxValue;
            var range = float.MinValue;
            foreach (var eachTerrain in selectedTerrains)
            {
                terrainDatas.Add(eachTerrain.terrainData);
                bottom = Mathf.Min(eachTerrain.transform.position.y, bottom);
                range = Mathf.Max(eachTerrain.terrainData.size.y, range);
            }
            
            GetTerrainHeightRange(out float minHeight, out float maxHeight, terrainDatas.ToArray());
            
            var worldBottom = (minHeight * range) + bottom;
            var worldTop = (maxHeight * range) + bottom;
            var existingRange = worldTop - worldBottom;
            
            var  paddedBottom = Mathf.FloorToInt(worldBottom  -  (padBottom * existingRange));
            var  paddedTop = Mathf.CeilToInt(worldTop +  (padTop * existingRange));

            RemapSelectedTerrains(paddedBottom, paddedTop);
            Debug.Log($"Optimized terrain heights to range {paddedBottom}-{paddedTop}");
        }

        /// <summary>
        /// Remap selected terrain objects to use the specified height range.
        ///
        /// Unlike OptimizeSelectedTerrains this can be applied to multiple objects
        /// </summary>
        /// <param name="minY">The lower world-space bound of the intended height range</param>
        /// <param name="maxY">The upper world-space bound of the intended height range</param>
        public static void RemapSelectedTerrains(int minY, int maxY)
        {
            if (maxY <= minY)
            {
                Debug.LogWarning("Upper bound must be higher than lower bound");
                return;
            }
            
            var selectedTerrains = SelectedContiguousTerrains(out bool isSelectedContiguous);
            
            if (! isSelectedContiguous)
            {
                Debug.LogWarning("Select one or more terrains from the same Terrain group before calling RemapSelectedTerrains");
                return;
            }
            
            Undo.RegisterCompleteObjectUndo(GetUndoArray(selectedTerrains), $"Remap Terrain Heights {minY}-{maxY}");
            foreach (var eachObject in selectedTerrains)
            {
                RemapTerrainHeights(eachObject, minY, maxY);
            }
            Debug.Log($"Remap Terrain Heights to range {minY}-{maxY}");
        }

        /// <summary>
        ///     Remap the heightmap pixels in a given Terrain so they completely fill a user-supplied range of heightmap values
        ///     while preserving their current world positions.
        ///     Heights outside the supplied range will be truncated to fit.
        /// </summary>
        /// <param name="targetTerrain">Terrain object to edit</param>
        /// <param name="heightMapMin">The world Y coordinate of the new lower bound of the the target terrain</param>
        /// <param name="heightMapMax">The world Y coordinate of the new upper bound of the the target terrain</param>
        public static void RemapTerrainHeights(Terrain targetTerrain, int heightMapMin, int heightMapMax)
        {
            
            
            var xform = targetTerrain.gameObject.transform;
            var terrainData = targetTerrain.terrainData;
            var oldPos = xform.position;
            var originalPos = oldPos.y;
            var oldScaleFactor = terrainData.heightmapScale.y;
            var newRange = heightMapMax - heightMapMin;

            if (newRange <= 0)
            {
                Debug.LogWarning("Remap range must be at least one unit");
                return;
            }
            
            
            float Remapped(float h)
            {
                var hmapToWorld =  originalPos + (oldScaleFactor * h);
                return  Mathf.Clamp01(Mathf.Max(0, hmapToWorld - heightMapMin) / newRange);
            }
            
            var stride = terrainData.heightmapResolution;
            var existingHeights =
                terrainData.GetHeights(0, 0, stride, stride);

            for (var x = 0; x < stride; x++)
            for (var y = 0; y < stride; y++)
                existingHeights[x, y] = Remapped(existingHeights[x, y]);

            terrainData.SetHeights(0, 0, existingHeights);
            terrainData.size = new Vector3(terrainData.size.x, newRange, terrainData.size.z);
            oldPos.y = heightMapMin;
            targetTerrain.transform.position = oldPos;
        }
        

        /// <summary>
        ///     Returns the minimum and maximum world height values for
        ///     <terrainData>
        ///         . The results are
        ///         passed as out parameters.
        ///     </terrainData>
        /// </summary>
        /// <param name="minHeight">float (will be reset)</param>
        /// <param name="maxHeight">float (will be reset)</param>
        /// <param name="terrainDatas">One or more Terrain object</param>
        public static void GetTerrainHeightRange(out float minHeight, out float maxHeight, params TerrainData[] terrainDatas)
        {
            minHeight = float.MaxValue;
            maxHeight = float.MinValue;
            foreach (var eachData in terrainDatas)
            { 
                foreach (var eachExtent in eachData.GetPatchMinMaxHeights())
                {
                    minHeight = Mathf.Min(minHeight, eachExtent.min);
                    maxHeight = Mathf.Max(maxHeight, eachExtent.max);
                }
            }
        }

        /// <summary>
        /// Returns the selected contiguous Terrains as an array.
        ///
        /// If the there are no selected terrains or the selection includes terrains with
        /// more than one groupingID, the out parameter will be  false.
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        internal static Terrain[] SelectedContiguousTerrains(out bool success)
        {
            var selectedTerrains = Selection.GetFiltered<Terrain>(SelectionMode.Deep);
            success = selectedTerrains.Length == 1;
            if (selectedTerrains.Length < 2)
            {           
                return selectedTerrains;
            }
            int groupID = selectedTerrains[0].groupingID;
            success = true;
            foreach (var eachTerrain in selectedTerrains)
            {
                success = success && eachTerrain.groupingID == groupID;
            }
            return selectedTerrains;
        }

        internal static Object[] GetUndoArray(in Terrain[] terrains)
        {
            List<Object> undoArray = new List<Object>();
            foreach (var eachTerrain in terrains)
            {
                undoArray.Add(eachTerrain.terrainData);
                undoArray.Add(eachTerrain.transform);
                undoArray.Add(eachTerrain);
            }
            return undoArray.ToArray();
        }
    }
}