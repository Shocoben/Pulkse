using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ball : LookAtObj {

    private static List<Ball> _instances = new List<Ball>();
    private static Ball _selectedBall = null;

    public string boxTag = "Boxes";
    public string baseTag = "Base";
    public Vector2 startDirection;
    public float speed = 5;

	public Transform rotateTail;

    private Vector2 _direction;

	private List<EmotionBall> _emotions = new List<EmotionBall>();

	public float lastTempo = 0;
	public float tempo = 1;
	public void addEmotion(EmotionBall emo)
	{
		if (_emotions.Count <= 0)
			lastTempo = Time.time;
		_emotions.Add(emo);

	}

	public void removeEmotion(EmotionBall emo)
	{
		_emotions.Remove(emo);
	}

    public static List<Ball> instances
    {
        get
        {
            return _instances;
        }
    }

    // Use this for initialization
    void Start()
    {
        _direction = startDirection;
        _instances.Add(this);
    }

    public void OnDestroy()
    {
        _instances.Remove(this);
    }

    public Vector2 direction
    {
        get
        {
            return _direction;
        }
        set
        {
            _direction = value.normalized;
        }
    }
    public static Vector3 getPlaneIntersection(ref Plane plane, Vector3 screenPosition)
    {
        float dist;
        Camera mainCam = Camera.main;
        Ray camRay = mainCam.ScreenPointToRay(screenPosition);
        plane.Raycast(camRay, out dist);
        return camRay.GetPoint(dist);
    }

    public Vector3 screenPositionToPlayerWorldPlane(Vector3 screenPosition)
    {
        Transform _myTransform = transform;
        Plane myPlaneLookingCamera = new Plane(Vector3.back, _myTransform.position);
        return getPlaneIntersection(ref myPlaneLookingCamera, screenPosition);
    }



	// Update is called once per frame
	override public void Update () 
    {   
        #if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
            mouseInputs();
        #else 
            touchInputs();
        #endif

        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
	}
	

    public float timeSlow = 0.5f;
    void touchInputs()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            transform.right = lookAtInput(screenPositionToPlayerWorldPlane( touch.position ));
        }
    }

    void mouseInputs()
    {
        if (Input.GetMouseButton(0))
			transform.right = lookAtInput(screenPositionToPlayerWorldPlane ( Input.mousePosition ));
    } 

}
