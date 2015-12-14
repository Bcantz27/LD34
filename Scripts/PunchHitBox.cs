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

	void OnCollisionEnter(Collision other)
	{
		if (other.transform.tag == "Character")
		{
			float dmg = GameObject.Find("Player").GetComponent<Player>().dmg;
			other.transform.GetComponent<Character>().health -= dmg;
			if (other.transform.GetComponent<Character>().health <= 0)
			{
				if (other.transform.GetComponent<Character>().killBy == null)
				{
					int force = Random.Range(100, 200);
					if (force > 180)
					{
						force = 1000;
					}
					other.transform.GetComponent<Character>().killBy = GameObject.Find("Player").GetComponent<Player>().gameObject;
					other.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * force);
					other.transform.GetComponent<Rigidbody>().AddForce(GameObject.Find("Player").GetComponent<Player>().transform.forward * force/4);
				}
				
			}

		}
		GameObject.Destroy(gameObject);
	}
}
