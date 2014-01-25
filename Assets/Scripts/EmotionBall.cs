using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class EmotionBall : LookAtObj 
{

	public Ball followBall;
	public float speed = 5;

	public enum Emotions {
		red
		,green
		,bleu
		,yellow
		,orange
		,violet
	}

	private static List<EmotionBall> queueToPlay = new List<EmotionBall>();
	private static bool followingTempo = false;


	public Emotions emotion;
	public AudioSource audioSource;
	private AudioClip audioClip;
	private bool _isStartTempo = false;

	private float _lastTempoPlay;
	private float _tempoDuration;

	private bool _isPlaying = false;

	public float pulseSpeed = 0.2f;
	private float _scale = 1;
	public Vector3 basicScale;
	private Vector3 targetScale;

	private bool _stopPlaying = false;
	private float _stopPlayingTempo = 0;

	public virtual void Start()
	{
		EmotionSoundConfig.Instance.ballByEmotions[emotion].Add(this);

		EmotionInfos infos = EmotionSoundConfig.Instance.emoDictionary[emotion];
		audioClip = infos.clip;
		_tempoDuration = (infos.forceTempo <= 0)?audioClip.length : infos.forceTempo;
		_stopPlayingTempo = infos.stopTempo;

		basicScale = transform.parent.localScale;
		targetScale = basicScale;
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		doFollowBall();
		
		if (_isStartTempo && _isPlaying)
		{
			if (_lastTempoPlay + _tempoDuration <= Time.time)
			{	
				for (int i = 0; i < queueToPlay.Count; ++i)
				{
					Emotions cEmotion = queueToPlay[i].emotion;

					queueToPlay[i].playAudioLoop(cEmotion);

				}
				_lastTempoPlay = Time.time;
				callOnEmotionTempo(emotion);
			}
		}
		else if (_isPlaying && (_lastTempoPlay + _tempoDuration) <= Time.time)
		{
			//callOnEmotionTempo(emotion);
			OnEmotionTempo();
			_lastTempoPlay = Time.time;
		}


		if (_stopPlaying && (_lastTempoPlay + _stopPlayingTempo) <= Time.time)
		{
			audioSource.Stop();
			_stopPlaying = false;
			transform.parent.gameObject.SetActive(false);
		}

	
	}

	public void callOnEmotionTempo(Emotions emotion)
	{
		List<EmotionBall> ballByEmotions = EmotionSoundConfig.Instance.ballByEmotions[emotion];
		for (int i = 0; i < ballByEmotions.Count; ++i)
		{
			ballByEmotions[i].OnEmotionTempo();
		}
	}

	public virtual void OnEmotionTempo()
	{
		if (followBall != null)
		{
			if (!followBall.emotionsZone.Contains(emotion))
			{
				diminutionScale();
			}
		}
	}

	public virtual void diminutionScale()
	{
		_scale -= pulseSpeed;
		_scale = Mathf.Max(0, Mathf.Min(_scale, 1));
		targetScale = basicScale * _scale;
		transform.parent.localScale = targetScale;
		if (_scale <= 0)
		{
			removeAudioLoop();
		}
	}

	public virtual void doFollowBall()
	{
		if (followBall != null)
		{
			transform.parent.right = lookAtInput(followBall.transform.position);
			transform.parent.rigidbody.MovePosition(transform.position + (transform.parent.right * speed * Time.deltaTime));
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if ( other.CompareTag("Player") && followBall == null)
		{
			transform.parent.right = lookAtInput(other.transform.position, false);
			followBall = other.GetComponent<Ball>();
			followBall.addEmotion(this);

			if (!followingTempo)
			{
				playAudioLoop(emotion);
				_isStartTempo = true;
				followingTempo = true;
			}
			else
			{
				queueToPlay.Add(this);
			}
		}
	}

	public void playAudioLoop(Emotions emotion)
	{
		audioSource.clip = audioClip;
		audioSource.loop = true;
		audioSource.Play();
		EmotionSoundConfig.Instance.emoDictionary[emotion].setPlaying(true);
		_lastTempoPlay = Time.time;
		_isPlaying = true;
	}

	public void removeAudioLoop()
	{
		_isPlaying = false;
		audioSource.loop = false;
		_stopPlaying = true;
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
