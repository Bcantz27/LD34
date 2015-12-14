using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour 
{
	public SpriteRenderer spriteSheet;
	public List<Sprite> sprite;
    public string sheetName = "npc_man0";

    public float influence;
    public float health;
    public float speed;

	public float followDistance;
	public float stepBackDistance;
	public float safeDistance;
	public float joinDistance;

    public string status;

    public bool isLeader;
    public bool isBoss;

    public Transform leader;
	public Transform target;
	public Transform runFrom;

	public Vector3 wanderPoint;
	public float randomRange;
	public float wanderTime;
	public float restTime;

	public ViewDirection animationDirection;
	public bool fixedRotation;
	public float animationTime;
	public float animationSpeed;

	public bool standing;

	private Vector3 oldPos;

	public Vector3 rotateTest;

	public ClanType clan;

	public Aggerssion aggerssion;

	public bool keepAggerssion;

	public GameObject killBy;

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
        sprite = new List<Sprite>(Resources.LoadAll<Sprite>("Images/" + sheetName));

		followDistance = Random.Range(2f, 6f);
		stepBackDistance = Random.Range(1f, followDistance);

		joinDistance = Random.Range(0f, 10f);
		if (joinDistance < 0.5f)
		{
			isLeader = true;
		}


		if (leader == null && !isLeader)
		{
			if (GameObject.FindGameObjectsWithTag("Character").Length > 0)
			{ 
				foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Character"))
				{
					if (obj.GetComponent<Character>() != null)
					{
						float distance = Vector3.Distance(this.transform.position, obj.transform.position);
						if (obj.GetComponent<Character>().isLeader && !obj.Equals(this.gameObject) && distance < joinDistance)
						{
							if (leader == null)
							{
								leader = obj.transform;
							}
							else
							{ 
								float distanceLeader = Vector3.Distance(this.transform.position, leader.transform.position);
								float distanceMaybeLeader = Vector3.Distance(this.transform.position, obj.transform.position);
								if (distanceMaybeLeader < distanceLeader)
								{
									leader = obj.transform;
								}
							}
							//break;
						}
					}
				}
			}

			if (leader == null)
			{
				aggerssion = (Aggerssion)Random.Range(-1, 1);
				keepAggerssion = true;
			}
			else
			{
				influence = leader.GetComponent<Character>().health;
				status = "Follow";
			}
		}
		if (string.IsNullOrEmpty(status))
		{
			status = "Wander";
		}
		if (status == "Wander")
		{
			wanderTime = 1;
			wanderPoint = CreateWanderPoint(this.transform.position);
		}

		
	}
	
	// Update is called once per frame
    void Update()
    {
		if (health <= 0 && killBy != null)
		{
			GetComponent<Rigidbody>().freezeRotation = false;
			status = "Dead";
			standing = true;
		}
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
			case "Attack":
				Attack();
				break;
			case "Dead":

				break;
            default: //Idle

                break;
        }
		if (Vector3.Distance(oldPos, this.transform.position) < speed * Time.deltaTime / 2)
		{
			standing = true;
		}
		else
		{
			standing = false;
		}
		Animations();
		oldPos = this.transform.position;

		InflunceCheck();
    }

	public void InflunceCheck()
	{
		if (!isLeader)
		{
			if (leader != null)
			{
				if (leader.GetComponent<Character>().health > 0)
				{
					float distance = Vector3.Distance(this.transform.position, leader.position);
					influence += (leader.GetComponent<Character>().health / influence) / distance;
					clan = leader.GetComponent<Character>().clan;
					if (!keepAggerssion)
					{
						aggerssion = leader.GetComponent<Character>().aggerssion;
					}
				}
				else
				{
					
					influence = 0;
					clan = ClanType.Neutral;
					int deicide = Random.Range(0, 2);
					switch (deicide)
					{ 
						case 0:
							status = "Wander";
							leader = null;
							break;
						case 1:
							status = "RunAway";
							runFrom = leader.GetComponent<Character>().killBy.transform;
							leader = null;
							break;
						case 2:
							status = "Follow";
							leader = leader.GetComponent<Character>().killBy.transform;
							break;
						default:
							status = "Wander";
							leader = null;
							break;
					}
					
					
				}
			}
			else
			{
				if (GameObject.FindGameObjectsWithTag("Character").Length > 0)
				{
					foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Character"))
					{
						if (obj.GetComponent<Character>() != null)
						{
							if (obj.GetComponent<Character>().isLeader && !obj.Equals(this.gameObject))
							{
								float distance = Vector3.Distance(this.transform.position, obj.transform.position);
								if (distance < joinDistance && obj.GetComponent<Character>().health > 0)
								{
									leader = obj.transform;
									influence = leader.GetComponent<Character>().health;
									status = "Follow";
									if (!keepAggerssion)
									{
										aggerssion = leader.GetComponent<Character>().aggerssion;
										keepAggerssion = true;
									}
									break;
								}
							}
						}
					}
				}
			}
		}
	}

#region Movement and Animations

    public void Follow()
    {
		float distance = Vector3.Distance(this.transform.position, leader.position);

		if (distance > followDistance)
		{
			//Vector3 travelTo = Vector3.LerpUnclamped(this.transform.position, leader.position, speed * Time.deltaTime);
			//travelTo.y = this.transform.position.y;
			//this.transform.position = travelTo;
			
			this.transform.position = Vector3.MoveTowards(this.transform.position, leader.position, speed * Time.deltaTime);
			LookDirection(Vector3.MoveTowards(this.transform.position, leader.position, speed * Time.deltaTime));
		}
		else if (distance < stepBackDistance)
		{
			
			this.transform.position = Vector3.MoveTowards(this.transform.position, leader.position, -speed * Time.deltaTime);
			LookDirection(leader.position);
			
		}
	
    }

	public void RunAway()
	{
		if (runFrom != null)
		{
			float distance = Vector3.Distance(this.transform.position, runFrom.position);

			if (distance < safeDistance)
			{

				this.transform.position = Vector3.MoveTowards(this.transform.position, runFrom.position, -speed * Time.deltaTime);
				LookDirection(Vector3.MoveTowards(this.transform.position, runFrom.position, -speed * Time.deltaTime));
				CreateWanderPoint(this.transform.position);

			}
			else if (distance >= safeDistance)
			{

				Wander();
			}
		}
		else
		{
			Wander();
		}
	}

	public void Wander()
	{

		if (wanderTime > 0)
		{
			float distance = Vector3.Distance(this.transform.position, wanderPoint);

			if (distance < 1)
			{
				wanderPoint = CreateWanderPoint(this.transform.position);
			}
			else
			{
				
				this.transform.position = Vector3.MoveTowards(this.transform.position, wanderPoint, speed * Time.deltaTime);
				//LookDirection(Vector3.MoveTowards(this.transform.position, wanderPoint, speed * Time.deltaTime));
				LookDirection(wanderPoint);
				
			}
			wanderTime -= Time.deltaTime;
		}
		else
		{
			if (restTime >= 0)
			{
				restTime -= Time.deltaTime;
				standing = true;
			}
			else 
			{
				standing = false;
				wanderTime = 60;
				restTime = 30;
			}
		}
	}

	public void Attack()
	{
		float distance = Vector3.Distance(this.transform.position, target.position);
		if (distance > 2)
		{
			//MoveTowards
			this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime);
		}
		else
		{ 
			//Attack

		}
		LookDirection(Vector3.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime));
	}

	public Vector3 CreateWanderPoint(Vector3 area)
	{
		Vector3 retVal = new Vector3();
		retVal.y = transform.position.y;
		retVal.x = Random.Range(area.x - randomRange, area.x + randomRange);
		retVal.z = Random.Range(area.z - randomRange, area.z + randomRange);

		return retVal;
	}

	public void LookDirection(Vector3 target)
	{
		
		this.transform.LookAt(target);
		rotateTest = this.transform.localEulerAngles;

		//this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y-180, this.transform.localEulerAngles.z);
		this.transform.localEulerAngles = new Vector3(0, this.transform.localEulerAngles.y - 180, 0);

	}

	public void Animations()
	{
		if (fixedRotation)
		{
			var rotVal = (Quaternion.Inverse(transform.rotation) *  transform.GetChild(0).transform.rotation).eulerAngles.y;
			//print(rotVal);

			if (rotVal > 315 || rotVal < 45)
			{
				//Front
				setDirection(ViewDirection.Front);
			}
			else if (rotVal >= 45 && rotVal <= 135) // Right Side
			{
				setDirection(ViewDirection.Right);
				spriteSheet.flipX = true;
			}
			else if (rotVal >= 225 && rotVal <= 315)   //Left Side
			{
				setDirection(ViewDirection.Left);
				spriteSheet.flipX = false;
			}
			else if (rotVal > 135 && rotVal < 225)
			{
				//Back
				setDirection(ViewDirection.Back);
			}
		}
		AnimationLoop(animationDirection);
	}

	public void AnimationLoop(ViewDirection direction)
	{
		string animationString = "";
		string currentSprite = spriteSheet.sprite.name;
		
		//animationString += "man" + id+ "_";
		if (animationTime > 0)
		{
			animationTime -= Time.deltaTime;
		}
		else
		{
			if (direction == ViewDirection.Front)
			{
				animationString += "F";
			}
			else if (direction == ViewDirection.Back)
			{
				animationString += "B";
			}
			else if (direction == ViewDirection.Left)
			{
				animationString += "S";
				spriteSheet.flipX = false;
			}
			else if (direction == ViewDirection.Right)
			{
				animationString += "S";
				spriteSheet.flipX = true;
			}

			if (standing)
			{
				animationString += "Stand";
			}
			else
			{
				animationString += "Walk";
			}

			List<Sprite> temp = new List<Sprite>();

			if (killBy != null)
			{
				animationString += "0";
			}
			else if (currentSprite.EndsWith("0"))
			{
				animationString += "1";
			}
			else
			{
				animationString += "0";
			}

			

			bool didBreak = false;
			//Debug.Log(animationString);
			foreach (Sprite s in sprite)
			{
                if (s.name.Contains(animationString))
				{
					spriteSheet.sprite = s;
					didBreak = true;
					break;
				}
			}
			if (!didBreak)
			{
                Debug.LogError(":,(");
			}

			animationTime = animationSpeed * Time.deltaTime;
		}
	}

	public void setDirection(ViewDirection direction)
	{
		this.animationDirection = direction;
	}

	public enum ViewDirection
	{
		Front,
		Right,
		Left,
		Back
	}
#endregion

}
