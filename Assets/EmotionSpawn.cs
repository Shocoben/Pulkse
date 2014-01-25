using UnityEngine;
using System.Collections;

public class EmotionSpawn : MonoBehaviour {

	public string poolName;
	public EmotionBall.Emotions emotion;

	public void Spawn()
	{
		PoolableObject pGO = PoolManager.instance.getPoolableObject(poolName, false);
		pGO.toActiveAndDesactive.transform.position = transform.position;
		pGO.GetComponent<EmotionBall>().emotion = emotion;
		pGO.Alive();


	}
}
