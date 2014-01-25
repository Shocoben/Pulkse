using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EmotionInfos
{
	public EmotionBall.Emotions emotionName;
	public AudioClip clip;
	public float forceTempo = -1;
	public float stopTempo = 0.5f;
	public Material material;

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


	private List<EmotionSpawn> spawnPoints = new List<EmotionSpawn>();
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
					_ballPlayingByEmotions.Add(emo, new List<EmotionBall>());
				}
				else
				{
					Debug.LogError("SoundConfigutator : there is two configurations for the same emotion " + emo.ToString());
				}
			}
		}

		GameObject[] spawnsGOs = GameObject.FindGameObjectsWithTag(spawnPointsTag);

		for (int i = 0; i < spawnsGOs.Length; ++i)
		{
			EmotionSpawn spawn = spawnsGOs[i].GetComponent<EmotionSpawn>();
			spawnPoints.Add( spawn );
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

		if (_playingBallCount <= 0)
		{
			_lastTempoPlay = Time.time;
			_tempoDuration = (emoDictionary[ball.emotion].forceTempo > 0)?emoDictionary[ball.emotion].forceTempo : emoDictionary[ball.emotion].clip.length;
			_isFollowingTempo = true;
			ball.playAudioLoop();
		}
		else
		{
			addToQueue(ball);
		}

		_playingBallCount++;
		_ballPlayingByEmotions[ball.emotion].Add(ball);
	}

	public void removePlayingBall(EmotionBall ball)
	{
		if (!ball.isPlaying())
			return;


		_ballPlayingByEmotions[ball.emotion].Remove(ball);
		_playingBallCount--;

		if (_playingBallCount <= 0)
		{
			_isFollowingTempo = false;
		}
	}

	public void Update()
	{
		if (_isFollowingTempo && _lastTempoPlay + _tempoDuration <= Time.time)
		{
			for (int i = 0; i < queueToPlay.Count; ++i)
			{
				EmotionBall.Emotions cEmotion = queueToPlay[i].emotion;
				queueToPlay[i].playAudioLoop();
				queueToPlay.Remove(queueToPlay[i]);
			}
			_lastTempoPlay = Time.time;
		}
	}


}
