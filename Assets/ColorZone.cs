using UnityEngine;
using System.Collections;

public class ColorZone : MonoBehaviour {
	public EmotionBall.Emotions emotion;

	public virtual void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Ball ball = other.GetComponent<Ball>();
			ball.addZone(emotion);
		}
	}

	public virtual void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Ball ball = other.GetComponent<Ball>();
			ball.removeZone(emotion);
		}
	}
}
