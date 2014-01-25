using UnityEngine;
using System.Collections;

public class EmotionBallBis : EmotionBall {


	private Transform rotateAroundBall;
	public float rotateSpeed = 5;
	
	public float radiusRotate = 3;
	private float angle = 0;
	// Update is called once per frame
	float timeSinceRotate = 0;
	public float diffSpeed = 5;
	public override void doFollowBall ()
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
