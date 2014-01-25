using UnityEngine;
using System.Collections;

public class LookAtObj : PoolableObject {
	public float reactivity = 0.1f;
	// Update is called once per frame
	public virtual void Update () {
	
	}

	public Vector3 lookAtInput(Vector3 inputPos, bool lerp = true)
	{
		
		Vector3 dir =   inputPos - transform.position;
		dir.Normalize();
		if (lerp)
			return Vector3.Lerp(transform.right, dir, reactivity * Time.deltaTime);
		else
			return dir;
	}
}
