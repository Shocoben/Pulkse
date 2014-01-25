using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EmotionInfos
{
	public EmotionBall.Emotions emotionName;
	public AudioClip clip;
	public float forceTempo = -1;
	
	private bool _isPlaying = false;

	public bool isPlaying()
	{
		return _isPlaying;
	}

	public void setPlaying(bool nBool)
	{
		_isPlaying = nBool;
	}
}

public class EmotionBall : LookAtObj 
{

	public Ball followBall;
	public float speed = 5;

	public enum Emotions {
		red
		,green
	}

	public EmotionInfos[] emotionsInfos;
	private static Dictionary<Emotions, EmotionInfos> _emoDictionary = new Dictionary<Emotions, EmotionInfos>();
	private static Dictionary<Emotions, List<EmotionBall>> _ballByEmotions = new Dictionary<Emotions, List<EmotionBall>>();
	private static bool _initializedEmotionsTempo = false;
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
		if (!_initializedEmotionsTempo)
		{
			_initializedEmotionsTempo = true;
			for (int i = 0; i < emotionsInfos.Length; ++i)
			{
				Emotions emo = emotionsInfos[i].emotionName;
				if( !_emoDictionary.ContainsKey(emo) )
				{
					_emoDictionary.Add(emo, emotionsInfos[i]);
					_ballByEmotions.Add(emo, new List<EmotionBall>());
				}
			}
		}

		audioClip = _emoDictionary[emotion].clip;
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
					if ( !_emoDictionary[cEmotion].isPlaying() )
					{
						queueToPlay[i].playAudioLoop(cEmotion);
					}
				}
				_lastTempoPlay = Time.time;
				callOnEmotionTempo(emotion);
			}
		}
		else if (_isPlaying && (_lastTempoPlay + _emoDictionary[emotion].clip.length) <= Time.time)
		{
			callOnEmotionTempo(emotion);
			_lastTempoPlay = Time.time;
		}
	}

	public void callOnEmotionTempo(Emotions emotion)
	{
		for (int i = 0; i < _ballByEmotions[emotion].Count; ++i)
		{
			_ballByEmotions[emotion][i].OnEmotionTempo();
		}
	}

	public virtual void OnEmotionTempo()
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
				EmotionInfos infos = _emoDictionary[emotion];
				_tempoDuration = (infos.forceTempo <= 0)?infos.clip.length : _emoDictionary[emotion].forceTempo;
				playAudioLoop(emotion);
				_isStartTempo = true;
				tempoToFollow = _emoDictionary[emotion];
			}
			else
			{
				if (!_emoDictionary[emotion].isPlaying())
				{
					queueToPlay.Add(this);
				}
			}
		}
	}

	public void playAudioLoop(Emotions emotion)
	{
		audioSource.clip = audioClip;
		audioSource.loop = true;
		audioSource.Play();
		_emoDictionary[emotion].setPlaying(true);
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
