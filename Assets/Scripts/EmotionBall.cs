using UnityEngine;
using System.Collections;

public class EmotionBall : LookAtObj {

	public Ball followBall;
	public float speed = 5;

	public virtual void Start()
	{

	}

	public AudioSource audioSource;
	public AudioClip audioClip;
	public void play()
	{
		audioSource.PlayOneShot(audioClip);
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		if (followBall != null)
		{
			transform.parent.right = lookAtInput(followBall.transform.position);
			transform.parent.rigidbody.MovePosition(transform.position + (transform.parent.right * speed * Time.deltaTime));
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.CompareTag("Player"));

		if ( other.CompareTag("Player") && followBall == null)
		{
			transform.parent.right = lookAtInput(other.transform.position, false);
			followBall = other.GetComponent<Ball>();
			followBall.addEmotion(this);
		}

	}

	public void OnTriggerExit(Collider other)
	{
		/*
		if (followBall != null && other.CompareTag("Player"))
		{
			Ball cBall = other.GetComponent<Ball>();
			if (cBall.GetInstanceID() == followBall.GetInstanceID())
			{
				followBall = null;
			}
		}*/
	}
}
