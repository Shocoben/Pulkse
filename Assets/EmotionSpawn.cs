using UnityEngine;
using System.Collections;

public class EmotionSpawn : MonoBehaviour {

	public string poolName;

	public EmotionBall.Emotions[] spawnEmotions;

	public void Spawn()
	{
		PoolableObject pGO = PoolManager.instance.getPoolableObject(poolName, false);
		pGO.toActiveAndDesactive.transform.position = transform.position;
		int id = Mathf.FloorToInt(Random.Range(0, spawnEmotions.Length ));

		pGO.GetComponent<EmotionBall>().emotion = spawnEmotions[id];;
		pGO.Alive();
	}
}
