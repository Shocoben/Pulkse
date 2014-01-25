using UnityEngine;
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

public class EmotionSoundConfig : Singleton<EmotionSoundConfig> {


	public EmotionInfos[] emotionsInfos;
	public Dictionary<EmotionBall.Emotions, EmotionInfos> emoDictionary = new Dictionary<EmotionBall.Emotions, EmotionInfos>();
	public Dictionary<EmotionBall.Emotions, List<EmotionBall>> ballByEmotions = new Dictionary<EmotionBall.Emotions, List<EmotionBall>>();
	private bool _initializedEmotionsTempo = false;

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
					ballByEmotions.Add(emo, new List<EmotionBall>());
				}
				else
				{
					Debug.LogError("SoundConfigutator : there is two configurations for the same emotion " + emo.ToString());
				}
			}
		}
	}
}
