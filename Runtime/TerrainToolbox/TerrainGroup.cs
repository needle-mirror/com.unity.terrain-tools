using UnityEngine;

/// <summary>
/// Provides methods for managing terrain groups.
/// </summary>
[System.Serializable]
public class TerrainGroup : MonoBehaviour
{
	/// <summary>
	/// The terrain group's identifier.
	/// </summary>
	public int GroupID = 0;

	/// <summary>
	/// Updates the grouping ID of the parented terrains.
	/// </summary>
	public void UpdateChildTerrains()
	{
		Terrain[] childTerrains = GetComponentsInChildren<Terrain>();

		foreach (Terrain terrain in childTerrains)
		{
			GameObject existingGameObject = terrain.gameObject;
			terrain.groupingID = GroupID;
		}
	}
	
	/// <summary>
	/// Destroys all parented terrains.
	/// </summary>
	public void DestroyChildTerrains()
	{
		Terrain[] childTerrains = GetComponentsInChildren<Terrain>();

		foreach (Terrain terrain in childTerrains)
		{
			GameObject existingGameObject = terrain.gameObject;
			DestroyImmediate(existingGameObject);
		}
	}
}
