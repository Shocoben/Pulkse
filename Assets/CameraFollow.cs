using UnityEngine;
using System.Collections;

public class CameraFollow : LookAtObj {

	public Transform target;
	public Vector3 _direction;
	public float speed = 15;

	// Use this for initialization
	void Start () {
		_direction = target.transform.position - transform.position; 
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 difference = target.transform.position - transform.position;
		_direction = Vector3.Lerp(_direction, difference, reactivity);
		_direction.z = 0;

		transform.position += _direction.normalized * Time.deltaTime * speed;
	}
}
