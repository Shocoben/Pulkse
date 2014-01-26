using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BallEmosInfos
{
	public EmotionBall.Emotions emotion;
	public GameObject goAnim;
	public GameObject stayAnim;
}

public class Ball : LookAtObj 
{

    private static List<Ball> _instances = new List<Ball>();
    private static Ball _selectedBall = null;

    public string boxTag = "Boxes";
    public string baseTag = "Base";
    public Vector2 startDirection;
    public float speed = 5;

	public Transform rotateTail;

    private Vector2 _direction;


	public BallEmosInfos[] infos;

	private List<EmotionBall> _emotions = new List<EmotionBall>();
	private Dictionary<EmotionBall.Emotions, List<EmotionBall>> _ballsByEmotion = new Dictionary<EmotionBall.Emotions, List<EmotionBall>>();
	private List<EmotionBall.Emotions> _emotionsZone = new List<EmotionBall.Emotions>();

	private Dictionary<EmotionBall.Emotions, BallEmosInfos> _emotionsInfos = new Dictionary<EmotionBall.Emotions, BallEmosInfos>();

	private EmotionBall.Emotions cEmo;
	private float lastChangeEmo = -1000;

	private Animator transitionAnimation = null;

	public List<EmotionBall.Emotions> emotionsZone
	{
		get
		{
			return _emotionsZone;
		}
	}

	public List<EmotionBall> getBallsOfEmotion( EmotionBall.Emotions emo)
	{
		return _ballsByEmotion[emo];
	}
	

	public void addEmotion(EmotionBall emo)
	{
		_emotions.Add(emo);
		_ballsByEmotion[emo.emotion].Add(emo);
	}

	public void addZone(EmotionBall.Emotions emo)
	{
		for (int i = 0; i < _ballsByEmotion[emo].Count; ++i)
		{
			_ballsByEmotion[emo][i].resetScale();
		}

		if (emo != cEmo)
		{
			changeEmotion(emo);
		}
		_emotionsZone.Add(emo);
	}

	public void changeEmotion(EmotionBall.Emotions emo)
	{
		_emotionsInfos[cEmo].goAnim.gameObject.SetActive(false);
		cEmo = emo;
		lastChangeEmo = Time.time;
		_emotionsInfos[cEmo].goAnim.gameObject.SetActive(true);
	}

	public void removeZone(EmotionBall.Emotions emo)
	{
		if (_emotionsZone.Contains(emo))
		{
			_emotionsZone.Remove(emo);
		}
	}

	public void removeEmotion(EmotionBall emo)
	{
		_emotions.Remove(emo);
		_ballsByEmotion[emo.emotion].Remove(emo);
	}

    public static List<Ball> instances
    {
        get
        {
            return _instances;
        }
    }

    // Use this for initialization
    public override void Start()
    {
		base.Start();
        _direction = startDirection;
        _instances.Add(this);
		cEmo = EmotionBall.Emotions.white;

		for (int i = 0; i < infos.Length; ++i)
		{
			_emotionsInfos.Add(infos[i].emotion, infos[i]);
			_ballsByEmotion.Add(infos[i].emotion, new List<EmotionBall>());
		}

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


	public float returnToWhiteTime = 1;
	// Update is called once per frame
	override public void Update () 
    {   
        #if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
            mouseInputs();
        #else
            touchInputs();
        #endif

        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);

		if (_emotionsZone.Count <= 0 && lastChangeEmo + returnToWhiteTime <= Time.time)
		{
			changeEmotion(EmotionBall.Emotions.white);
		}

		/*if (transitionAnimation != null && !transitionAnimation.animation.isPlaying)
		{
			transitionAnimation = null;
			_emotionsInfos[cEmo].goAnim.gameObject.SetActive(true);
		}*/

	}
	

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
