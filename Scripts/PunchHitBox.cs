using UnityEngine;
using System.Collections;

public class PunchHitBox : MonoBehaviour 
{
	public float timeTilDeath;

	// Use this for initialization
	void Start () 
	{
		Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), GameObject.Find("Player").GetComponent<Collider>());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (timeTilDeath > 0)
		{
			timeTilDeath -= Time.deltaTime;
		}
		else
		{
			GameObject.Destroy(gameObject);
		}
	}
}
