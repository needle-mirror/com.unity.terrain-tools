namespace UnityEngine.Experimental.TerrainAPI
{
	public class TerrainGizmos : MonoBehaviour
	{
		public int GroupID = 0;
		public Color CubeColor = new Color(0f, 0.5f, 1f, 0.2f);
		public Color CubeWireColor = new Color(0f, 0.9f, 1f, 0.5f);

		void OnDrawGizmos()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = CubeColor;
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.color = CubeWireColor;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}
	}
}
