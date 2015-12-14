using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour 
{
	public GameObject spawnObj;

	public float spawnRate;
	private float pastTime;
	public int maxSpawn;
	private int currentSpawn;
	// Use this for initialization
	void Start () 
	{
		pastTime = spawnRate;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (currentSpawn < maxSpawn)
		{
			if (pastTime > 0)
			{
				pastTime -= Time.deltaTime;
			}
			else
			{
				pastTime = spawnRate;
				Instantiate(spawnObj, transform.position, spawnObj.transform.rotation);
				currentSpawn++;
			}
		}
	}
}
