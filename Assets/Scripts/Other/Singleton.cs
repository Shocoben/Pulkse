using UnityEngine;
using System.Collections.Generic;

public class SingletonWithListByEnum<T, E, L> : Singleton<T> where T : MonoBehaviour
{
	private Dictionary<E, List<L>> _dictionaryListByEnum = new Dictionary<E, List<L>>();
	private bool _initializedDictionary = false;
	
	protected override void Awake ()
	{
		base.Awake ();
		generateList();
	}
	
	public List<L> getListByEnum(E stateEnum)
	{
        generateList();
		return _dictionaryListByEnum[stateEnum];
	}
	
	public void generateList()
	{
	  	if (!_initializedDictionary)
        {
            foreach (var value in System.Enum.GetValues(typeof(E)))
            {
                E cState = (E)value;
                _dictionaryListByEnum.Add(cState, new List<L>());
            }
            _initializedDictionary = true;
        }	
	}
	
	public void addToList(E stateEnum, L element)
	{
		generateList();
		_dictionaryListByEnum[stateEnum].Add(element);
	}
	
	public void removeToList(E stateEnum, L element)
	{
		if (_dictionaryListByEnum.ContainsKey(stateEnum))
		{
			_dictionaryListByEnum[stateEnum].Remove(element);
		}
	}

	
}

/// <summary>
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;
	
	public bool destroyOnLoadLevel = true;
	
	private static object _lock = new object();
	
	public static T Instance{
		get
		{
			return _instance;
		}
	}
	
	protected virtual void Awake()
	{
		if (_instance != null)
		{
			GameObject.DestroyImmediate(this.gameObject);
			return;
		}
		else
		{
			_instance = this as T;	
		}
	}
 
	private static bool applicationIsQuitting = false;
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public void OnDestroy () 
	{
		if (destroyOnLoadLevel)
			_instance = null;
	}
}