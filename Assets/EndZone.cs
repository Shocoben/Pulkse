using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EndZoneInfos
{
	public  EmotionBall.Emotions emotion;
	public int nbrNeeded;
}

public class EndZone : MonoBehaviour {

	public EndZoneInfos[] infos;

	private Dictionary<EmotionBall.Emotions, EndZoneInfos> endInfos = new Dictionary<EmotionBall.Emotions, EndZoneInfos>();
	public int nextLevelID = 0;
	public string nextLevelName = "";

	public void Start()
	{
		for (int i = 0; i < infos.Length; ++i)
		{
			if (endInfos.ContainsKey(infos[i].emotion))
			{
				Debug.LogError("EndZone : there is too much configs for emotion " + infos[i].emotion);
			}
			else
			{
				endInfos.Add(infos[i].emotion, infos[i]);
			}
		}
	}
	void OnTriggerEnter(Collider other)
	{
		Ball otherBall = other.GetComponent<Ball>();
		if (otherBall == null)
			return;

		Dictionary<EmotionBall.Emotions, int> count = new Dictionary<EmotionBall.Emotions, int>();
		bool good = true;
		foreach(var Emo in endInfos)
		{
			List< EmotionBall > balls = otherBall.getBallsOfEmotion(Emo.Key);
			if (balls.Count < Emo.Value.nbrNeeded)
			{
				good = false;
			}
			count.Add(Emo.Key, balls.Count);
		}

		if (!good)
		{
			OnNotEnoughBalls();
		}
		else
		{
			OnNextLevel();
		}
	}

	public void OnNotEnoughBalls()
	{
		Debug.Log("NotEngouBalls");
	}

	public void OnNextLevel()
	{
		if (nextLevelName.Length >0)
		{
			Application.LoadLevel(nextLevelName);
		}
		else
		{
			Application.LoadLevel(nextLevelID);
		}
	}

}
