using UnityEngine;
using System.Collections;

public class PunchHitBox : MonoBehaviour 
{
	public float timeTilDeath;
	public GameObject hitboxParent;
	public bool ignoreHappen;
	public bool hit;
	public AudioClip sound;

	// Use this for initialization
	void Start () 
	{
		ignoreHappen = false;
		GetComponent<Collider>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!ignoreHappen)
		{
			Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), hitboxParent.GetComponent<Collider>());
			GetComponent<Collider>().enabled = true;
			ignoreHappen = true;
		}
		if (timeTilDeath > 0)
		{
			timeTilDeath -= Time.deltaTime;
		}
		else
		{
			if (!GetComponent<AudioSource>().isPlaying)
			{
				GameObject.Destroy(gameObject);
			}
		}
		if (hit && !GetComponent<AudioSource>().isPlaying)
		{
			GameObject.Destroy(gameObject);
		}
	}
	public void SetParent(GameObject newParent)
	{
		hitboxParent = newParent;
	}
	void OnCollisionEnter(Collision other)
	{
		if (hitboxParent.name == "Player")
		{
			if (other.transform.tag == "Character")
			{
				hitboxParent.GetComponent<Player>().FollowersAttack(other.transform);
			}
		}
		if (other.transform.tag == "Character")
		{
			if (hitboxParent.name == "Player")
			{
				GetComponent<AudioSource>().PlayOneShot(sound);
				float dmg = GameObject.Find("Player").GetComponent<Player>().dmg;
				other.transform.GetComponent<Character>().health -= dmg;
				other.transform.GetComponent<Character>().TookDmg(GameObject.Find("Player"));

				if (other.transform.GetComponent<Character>().health <= 0)
				{
					if (other.transform.GetComponent<Character>().killBy == null)
					{

						int force = Random.Range(100, 200);
						if (force > 180)
						{
							force = 1000;
						}
						other.transform.GetComponent<Character>().killBy = hitboxParent;
						other.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * force);
						other.transform.GetComponent<Rigidbody>().AddForce(hitboxParent.transform.forward * force / 4);
					}

				}
			}
			else if (hitboxParent.tag == "Character")
			{
				GetComponent<AudioSource>().PlayOneShot(sound);
				float dmg = hitboxParent.GetComponent<Character>().dmg;
				other.transform.GetComponent<Character>().health -= dmg;
				other.transform.GetComponent<Character>().TookDmg(hitboxParent);

				if (other.transform.GetComponent<Character>().health <= 0)
				{
					if (other.transform.GetComponent<Character>().killBy == null)
					{

						int force = Random.Range(100, 200);
						if (force > 180)
						{
							force = 1000;
						}
						other.transform.GetComponent<Character>().killBy = hitboxParent;
						other.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * force);
						other.transform.GetComponent<Rigidbody>().AddForce(hitboxParent.transform.forward * force / 4);
					}

				}
			}

		}
		else if (other.transform.name == "Player")
		{
			other.transform.GetComponent<Player>().health -= hitboxParent.GetComponent<Character>().dmg;
			GetComponent<AudioSource>().PlayOneShot(sound);
		}
		if (other.collider.tag == "HitBox")
		{
			Physics.IgnoreCollision(GetComponent<Collider>(), other.collider);
		}
		else
		{
			hit = true;
			GetComponent<Collider>().enabled = false;
		}
	}
}
