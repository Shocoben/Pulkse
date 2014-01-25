using UnityEngine;
using System.Collections;

public class EmotionBallBis : EmotionBall {


	public Transform rotateAroundBall;
	public float rotateSpeed = 5;

	public void rotateAround( Vector3 O )
	{
		/*
          * A'.x = A.x * cos(θ) - A.y * sin(θ)
          * A'.y = A.x * sin(θ) + A.y * cos(θ)
         */
		float rotateSpeedBis = Time.deltaTime * rotateSpeed;
		Vector3 A = transform.parent.position;
		Vector2 AmO = new Vector2(A.x - O.x, A.y - O.y);
		Vector2 rotAmO = new Vector2(AmO.x * Mathf.Cos(rotateSpeedBis) - AmO.y * Mathf.Sin(rotateSpeedBis), AmO.x * Mathf.Sin(rotateSpeedBis) + AmO.y * Mathf.Cos(rotateSpeedBis));
		Vector2 OR = new Vector2(rotAmO.x + O.x, rotAmO.y + O.y);
		transform.parent.position = new Vector3(OR.x, OR.y, transform.parent.position.z);
	}


	
	public float radiusRotate = 3;
	private float angle = 0;
	// Update is called once per frame
	float timeSinceRotate = 0;
	public float diffSpeed = 5;
	public override void Update ()
	{
		if (rotateAroundBall != null)
		{
			angle += Mathf.Deg2Rad * rotateSpeed;
			float rad = 0.2f + ( 0.15f + (0.15f*0.5f) * Mathf.Sin( timeSinceRotate ) );
			transform.parent.position = rotateAroundBall.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), transform.position.z) * rad;
			timeSinceRotate += Time.deltaTime * diffSpeed;
		}
		else if (followBall != null)
		{
			transform.parent.right = lookAtInput(followBall.transform.position);
			transform.parent.rigidbody.MovePosition(transform.position + (transform.parent.right * speed * Time.deltaTime));
			if ( Vector3.Distance( transform.position, followBall.transform.position ) < radiusRotate )
			{
				Vector3 relPos = transform.position - followBall.transform.position;
				angle = Mathf.Atan2(relPos.y, relPos.x);
				rotateAroundBall = followBall.rotateTail;
			}
		}

	}
	
}
