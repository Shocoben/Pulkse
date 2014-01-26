
using UnityEngine;
using System.Collections;

public class EmotionSpawn : MonoBehaviour {

	public string poolName;
	public bool showDebug = false;
	public EmotionBall.Emotions[] spawnEmotions;

	public bool emotionBallMoving = false;

	public void Spawn()
	{

		PoolableObject pGO = PoolManager.instance.getPoolableObject(poolName, false);
		pGO.toActiveAndDesactive.transform.position = transform.position;
		int id = Mathf.FloorToInt(Random.Range(0, spawnEmotions.Length ));
		if (showDebug)
			Debug.Log(id +"go" + pGO);

		EmotionBall pBall = pGO.GetComponent<EmotionBall>();

		pBall.emotion = spawnEmotions[id];
		pBall.move = emotionBallMoving;

		pGO.Alive();
	}
}
