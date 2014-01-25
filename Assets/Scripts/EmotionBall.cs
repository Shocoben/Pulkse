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
	private static EmotionInfos tempoToFollow;


	public Emotions emotion;
	public AudioSource audioSource;
	private AudioClip audioClip;
	private bool _isStartTempo = false;

	private float _lastTempoPlay;
	private float _tempoDuration;

	private bool _isPlaying = false;

	public virtual void Start()
	{
		EmotionSoundConfig.Instance.ballByEmotions[emotion].Add(this);

		EmotionInfos infos = EmotionSoundConfig.Instance.emoDictionary[emotion];
		audioClip = infos.clip;
		_tempoDuration = (infos.forceTempo <= 0)?audioClip.length : infos.forceTempo;

	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		doFollowBall();
		
		if (_isStartTempo)
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
			callOnEmotionTempo(emotion);
			_lastTempoPlay = Time.time;
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
				
			}
		}
	}

	public virtual void diminutionScale()
	{

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

			if (tempoToFollow == null)
			{

				playAudioLoop(emotion);
				_isStartTempo = true;
				tempoToFollow = EmotionSoundConfig.Instance.emoDictionary[emotion];
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
