using UnityEngine;
using System.Collections;

public class EmotionSpawn : MonoBehaviour {

	public string poolName;

	public void Spawn(Vector3 position)
	{
		PoolableObject pGO = PoolManager.instance.getPoolableObject(poolName, false);
		pGO.transform.position = position;
		pGO.Alive();
	}
}
