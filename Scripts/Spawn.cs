using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Spawn : MonoBehaviour 
{
	public GameObject spawnObj;

	public float spawnRate;
	private float pastTime;
	public int maxSpawn;
	private int currentSpawn;
	public List<GameObject> Characters;
	public GameObject[][] spriteName;
	public int group;
	// Use this for initialization
	void Start () 
	{
		pastTime = spawnRate;

		spriteName = new GameObject[3][];

		spriteName[0] = new GameObject[3];
		spriteName[0][0] = Characters[0];
		spriteName[0][1] = Characters[1];
		spriteName[0][2] = Characters[2];

		spriteName[1] = new GameObject[3];
		spriteName[1][0] = Characters[0];
		spriteName[1][1] = Characters[1];
		spriteName[1][2] = Characters[2];

		spriteName[2] = new GameObject[3];
		spriteName[2][0] = Characters[0];
		spriteName[2][1] = Characters[1];
		spriteName[2][2] = Characters[2];
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
				int blah = Random.Range(0, 3);
				GameObject character = spriteName[group][blah];
				
				GameObject characterTemp = Instantiate(character, transform.position, spawnObj.transform.rotation) as GameObject;
				characterTemp.GetComponent<Character>().permentWanderPoint = this.transform.position;
				
				currentSpawn++;
			}
		}
	}
}
