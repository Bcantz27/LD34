using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
    public float influence;
    public int health;
    public float speed;

	public float followDistance;
	public float stepBackDistance;
	public float safeDistance;

    public string status;

    public bool isLeader;
    public bool isBoss;

    public Transform leader;
	public Transform target;

	public Vector3 wanderPoint;
	public float randomRange;

	private Vector3 oldPos;

    public enum ClanType : int
    { 
        Good = 1,
        Neutral = 0,
        Evil = -1
    };
    public enum Aggerssion : int
    {
        Lawful = 1,
        Neutral = 0,
        Chaotic = -1
    };
	// Use this for initialization
	void Start () 
    {
		if (leader == null)
		{
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Character"))
			{
				if (obj.GetComponent<Character>().isLeader)
				{
					leader = obj.transform;
					break;
				}
			}
		}
		if (string.IsNullOrEmpty(status))
		{
			status = "Wander";
		}
		if (status == "Wander")
		{
			wanderPoint = CreateWanderPoint(this.transform.position);
		}
	}
	
	// Update is called once per frame
    void Update()
    {
        switch (status)
        { 
            case "Follow":
				if (isLeader)
				{
					status = "Wander";
				}
				else
				{
					Follow();
				}
                break;
			case "RunAway":
				RunAway();
				break;
			case "Wander":
				Wander();
				if (Vector3.Distance(oldPos, this.transform.position) < speed * Time.deltaTime /2)
				{
					wanderPoint = CreateWanderPoint(this.transform.position);
				}
				break;
            default: //Idle

                break;
        }
		oldPos = this.transform.position;
    }

    public void Follow()
    {
		float distance = Vector3.Distance(this.transform.position, leader.position);

		if (distance > followDistance)
		{
			//Vector3 travelTo = Vector3.LerpUnclamped(this.transform.position, leader.position, speed * Time.deltaTime);
			//travelTo.y = this.transform.position.y;
			//this.transform.position = travelTo;
			this.transform.position = Vector3.MoveTowards(this.transform.position, leader.position, speed * Time.deltaTime);
		}
		else if (distance < stepBackDistance)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, leader.position, -speed * Time.deltaTime);
		}
	
    }
	public void RunAway()
	{
		float distance = Vector3.Distance(this.transform.position, leader.position);

		if (distance < safeDistance)
		{
			Vector3 travelTo = Vector3.LerpUnclamped(this.transform.position, leader.position, -speed * Time.deltaTime);
			travelTo.y = this.transform.position.y;
			this.transform.position = travelTo;
		}
		else if (distance >= safeDistance)
		{

			Wander(CreateWanderPoint(this.transform.position));
		}
	}
	public void Wander()
	{
		float distance = Vector3.Distance(this.transform.position, wanderPoint);

		if (distance < 1)
		{
			wanderPoint = CreateWanderPoint(this.transform.position);
		}
		else
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, wanderPoint, speed * Time.deltaTime);
		}
	}
	public void Wander(Vector3 wanderPoint)
	{
		float distance = Vector3.Distance(this.transform.position, wanderPoint);

		if (distance < 1)
		{
			wanderPoint = CreateWanderPoint(this.transform.position);
		}
		else
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, wanderPoint, speed * Time.deltaTime);
		}
	}
	public Vector3 CreateWanderPoint(Vector3 area)
	{
		Vector3 retVal = new Vector3();
		retVal.y = transform.position.y;
		retVal.x = Random.Range(area.x - randomRange, area.x + randomRange);
		retVal.z = Random.Range(area.z - randomRange, area.z + randomRange);

		return retVal;
	}
}
