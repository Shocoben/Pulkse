using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ClipInfo
{
	public AudioClip clip;
	public float forceTempo = -1;
	public float stopTempo = 0.5f;
	public float volumeStart = 1;

	private List<EmotionBall> _playingClip = new List<EmotionBall>();
	public void addPlayingClip(EmotionBall ball)
	{
		_playingClip.Add(ball);
	}

	public void removePlayingClip(EmotionBall ball)
	{
		_playingClip.Remove(ball);
	}

	public List<EmotionBall> getPlayingList()
	{
		return _playingClip;
	}

	public float getTempoDuration()
	{
		return (forceTempo <= 0)? clip.length : forceTempo;
	}
}

[System.Serializable]
public class EmotionInfos
{
	public EmotionBall.Emotions emotionName;

	public Material material;

	
	public ClipInfo[] clipsInfos;
	private bool _isPlaying = false;
	
	public bool isPlaying()
	{
		return _isPlaying;
	}
	
	public void setPlaying(bool nBool)
	{
		_isPlaying = nBool;
	}

	private int cID = 0;

	public ClipInfo getCurrentClipInfo()
	{
		ClipInfo clip = clipsInfos[cID];
		cID ++;
		if (cID >= clipsInfos.Length)
		{
			cID = 0;
		}
		return clip;
	}


}

public class EmotionSoundConfig : Singleton<EmotionSoundConfig> {


	public EmotionInfos[] emotionsInfos;
	public float generalTempo = 1;
	public AudioClip generalAudioClipTempo;

	public Dictionary<EmotionBall.Emotions, EmotionInfos> emoDictionary = new Dictionary<EmotionBall.Emotions, EmotionInfos>();
	private Dictionary<EmotionBall.Emotions, List<EmotionBall>> _ballPlayingByEmotions = new Dictionary<EmotionBall.Emotions, List<EmotionBall>>();
	private int _playingBallCount = 0;
	private bool _initializedEmotionsTempo = false;

	private float _lastTempoPlay = -1000;
	private float _tempoDuration = -1000;
	private bool _isFollowingTempo = false;
	private List<EmotionBall> queueToPlay = new List<EmotionBall>();


	private List<EmotionSpawn> _spawnPoints = new List<EmotionSpawn>();
	public string spawnPointsTag = "SpawnPoint";

	protected override void Awake ()
	{
		base.Awake ();
		if (!_initializedEmotionsTempo)
		{
			_initializedEmotionsTempo = true;
			for (int i = 0; i < emotionsInfos.Length; ++i)
			{
				EmotionBall.Emotions emo = emotionsInfos[i].emotionName;
				if( !emoDictionary.ContainsKey(emo) )
				{
					emoDictionary.Add(emo, emotionsInfos[i]);
					_ballPlayingByEmotions.Add( emo, new List<EmotionBall>() );
				}
				else
				{
					Debug.LogError( "SoundConfigutator : there is two configurations for the same emotion " + emo.ToString() );
				}
			}
		}

		GameObject[] spawnsGOs = GameObject.FindGameObjectsWithTag(spawnPointsTag);

		for (int i = 0; i < spawnsGOs.Length; ++i)
		{
			EmotionSpawn spawn = spawnsGOs[i].GetComponent<EmotionSpawn>();
			_spawnPoints.Add( spawn );
			spawn.Spawn();
		}


	}
	
	public void addToQueue(EmotionBall ball)
	{
		queueToPlay.Add(ball);
	}
	
	public void addPlayingBall(EmotionBall ball)
	{
		if (ball.isPlaying())
			return;

		ClipInfo clipInfo = emoDictionary[ball.emotion].getCurrentClipInfo();

		if (_playingBallCount <= 0)
		{
			_lastTempoPlay = Time.time;
			_tempoDuration = clipInfo.getTempoDuration();
			_isFollowingTempo = true;
			ball.setupAudioLoop(clipInfo);
			ball.playAudioLoop(0);
			clipInfo.addPlayingClip(ball);
		}
		else
		{
			if (clipInfo.getPlayingList().Count <= 0)
			{
				addToQueue(ball);
			}
			clipInfo.addPlayingClip(ball);
			ball.setupAudioLoop(clipInfo);
		}

		_playingBallCount++;
		_ballPlayingByEmotions[ball.emotion].Add(ball);
	}

	public void removePlayingBall(EmotionBall ball)
	{
		if (!ball.isPlaying())
			return;

		_ballPlayingByEmotions[ball.emotion].Remove(ball);

		ClipInfo infos = ball.getCClipInfo();
		infos.removePlayingClip(ball);
		_playingBallCount--;

		if (infos.getPlayingList().Count > 0)
			infos.getPlayingList()[0].playAudioLoop(ball.audioSource.time);

		if (_playingBallCount <= 0)
		{
			_isFollowingTempo = false;
		}
	
			


		spawnNewBall();

	}

	public bool isOutOfCamera(Vector3 pos)
	{
		if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1)
		{
			return true;
		}
		return false;
	}

	public void spawnNewBall()
	{
		for (int i = 0; i < _spawnPoints.Count; ++i)
		{
			Vector3 spawnViewportpOs = Camera.main.WorldToViewportPoint(_spawnPoints[i].transform.position);
			if (isOutOfCamera(spawnViewportpOs))
			{
				EmotionSpawn spawnPoint = _spawnPoints[i];
				spawnPoint.Spawn();
				_spawnPoints.Remove(spawnPoint);
				_spawnPoints.Add(spawnPoint);
				return;
			}
		}
	}

	public void Update()
	{
		if (_isFollowingTempo && _lastTempoPlay + _tempoDuration <= Time.time)
		{
			EmotionBall[] copyBallsQueue = new EmotionBall[queueToPlay.Count];
			queueToPlay.CopyTo(copyBallsQueue);

			for (int i = 0; i < copyBallsQueue.Length; ++i)
			{
				EmotionBall.Emotions cEmotion = copyBallsQueue[i].emotion;
				copyBallsQueue[i].playAudioLoop(0);
				queueToPlay.Remove( copyBallsQueue[i] );
			}
			_lastTempoPlay = Time.time;
		}
	}


}
