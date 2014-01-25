using UnityEngine;
using System.Collections;

public class PoolableObject : MonoBehaviour {
    public string poolCategoryName;
	public GameObject toActiveAndDesactive;
	
	public virtual void Start()
	{

	}

	public virtual void Setup()
	{
		if (toActiveAndDesactive == null)
		{
			toActiveAndDesactive = this.gameObject;
		}
	}
	
    public virtual void Die()
    {
        PoolManager.instance.addToPool(poolCategoryName, this);
		toActiveAndDesactive.SetActive( false );
    }

    public virtual void Alive()
    {

        PoolManager.instance.removeFromPool(poolCategoryName, this);
		toActiveAndDesactive.SetActive( true );
    }
    
}
