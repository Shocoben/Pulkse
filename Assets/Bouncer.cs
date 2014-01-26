using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bouncer : MonoBehaviour {
	public Ball myBall;
	public List<string> bounceLayer;

	// Update is called once per frame
	void OnTriggerEnter (Collider other) {
		if (bounceLayer.Contains(other.tag))
		{
			myBall.bounce(other.gameObject);
		}
	}
}
