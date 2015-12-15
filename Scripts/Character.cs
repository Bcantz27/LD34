using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour 
{
	public SpriteRenderer spriteSheet;
	public List<Sprite> sprite;
    public string sheetName = "npc_man0";

	public SpriteRenderer star;

    public float influence;
    public float health;
    public float speed;
	public float betray;

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

	public Vector3 permentWanderPoint;
	public Vector3 wanderPoint;
	public float randomRange;
	public float wanderTime;
	public float runAwayTime;
	public float restTime;
	public float despawnTime;

	public float restReset;
	public float wanderReset;
	public float runAwayReset;

	public ViewDirection animationDirection;
	public bool fixedRotation;
	public float animationTime;
	public float animationSpeed;

	public bool standing;
	public bool swing;
	public int swingStep;
	public float swingTime;
	public float swingForce;

	private Vector3 oldPos;

	public Vector3 rotateTest;

	public ClanType clan;

	public Aggerssion aggerssion;

	public bool keepAggerssion;

	public GameObject killBy;
	public GameObject Weapon;
	public GameObject hitBox;
	public float hitForce;
	public float dmg;
	public float startAttackDistance;

	private bool layout;
	private string currentSprite;

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
		star.enabled = false;
		if (dmg == 0)
		{
			dmg = 1;
		}
        sprite = new List<Sprite>(Resources.LoadAll<Sprite>("Images/" + sheetName));
		currentSprite = sprite[0].name;
		spriteSheet.sprite = sprite[0];

		followDistance = Random.Range(2f, 6f);
		stepBackDistance = 1.5f;
		safeDistance = Random.Range(8f, 20f);
		joinDistance = Random.Range(0f, 10f);
		if (joinDistance < 1.0f)
		{
			isLeader = true;
		}

		wanderReset = Random.Range(8f, 60f);
		restReset = Random.Range(2f, 10f);
		runAwayReset = Random.Range(10f, 40f);


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
		else if(leader != null)
		{
			if (leader.GetComponent<Character>() != null)
			{
				influence = leader.GetComponent<Character>().health;
			}
			else if (leader.GetComponent<Player>() != null)
			{
				influence = leader.GetComponent<Player>().health;
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
		if (influence == Mathf.Infinity)
		{
			if (leader.GetComponent<Character>() != null)
			{
				influence = leader.GetComponent<Character>().health;
			}
			else if (leader.GetComponent<Player>() != null)
			{
				influence = leader.GetComponent<Player>().health;
			}
		}
		if (health <= 0 && killBy != null)
		{
			GetComponent<Rigidbody>().freezeRotation = false;
			
			status = "Dead";
			standing = true;
			//if (!layout)
			//{
				this.transform.localEulerAngles = new Vector3(90, this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
			//	layout = true;
			//}
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
				if (runAwayTime > 0)
				{
					runAwayTime -= Time.deltaTime;
				}
				else
				{
					runAwayTime = runAwayReset;
					status = "Wander";
				}
				break;
			case "Wander":
				Wander();
				if (Vector3.Distance(oldPos, this.transform.position) < speed * Time.deltaTime /2)
				{
					wanderPoint = CreateWanderPoint(this.transform.position);
				}
				if (this.isLeader)
				{
					GameObject[] otherCharacters = GameObject.FindGameObjectsWithTag("Character");
					foreach (GameObject other in otherCharacters)
					{
						if (!other.transform.Equals(this.transform))
						{
							if (other.GetComponent<Character>() != null)
							{
								if (other.transform.GetComponent<Character>().isLeader)
								{
									float distance = Vector3.Distance(transform.position, other.transform.position);
									if (distance < startAttackDistance)
									{
										int deicide = Random.Range(0, 100);
										if (deicide == 0)
										{
											target = other.transform;
											status = "Attack";
											break;
										}
									}
								}
							}
						}
					}

				}
				else
				{
					GameObject[] otherCharacters = GameObject.FindGameObjectsWithTag("Character");
					foreach (GameObject other in otherCharacters)
					{
						if (!other.transform.Equals(this.transform))
						{
							if (other.GetComponent<Character>() != null)
							{
								if (other.transform.GetComponent<Character>().isLeader)
								{
									float distance = Vector3.Distance(transform.position, other.transform.position);
									if (distance < startAttackDistance)
									{
										int deicide = Random.Range(0, 100);
										if (deicide == 0)
										{
											leader = other.transform;
											status = "Follow";
											break;
										}
									}
								}
							}
						}
					}
					int becomeLeader = Random.Range(0, 2000);
					if (becomeLeader == 0)
					{
						isLeader = true;
					}
				}
				break;
			case "Attack":
				Attack();
				break;
			case "Dead":
				Color characterColor = this.spriteSheet.color;
				characterColor.a -= Time.deltaTime * 0.1f;
				this.spriteSheet.color = characterColor;
				if (despawnTime > 0)
				{
					despawnTime -= Time.deltaTime;
				}
				else
				{
					GameObject.Destroy(gameObject);
				}
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

		if (Weapon!=null)
		{
			if (status == "Attack")
			{
				if (!Weapon.GetComponent<MeshRenderer>().enabled)
				{
					Weapon.GetComponent<MeshRenderer>().enabled = true;
				}
			
			}
			else if(Weapon.GetComponent<MeshRenderer>().enabled)
			{
				Weapon.GetComponent<MeshRenderer>().enabled = false;
			}
		}

		if (status != "Attack")
		{
			if (swing)
			{
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				transform.localEulerAngles = Vector3.zero;
				swing = false;
				swingStep = 0;
				swingTime = 0.1f;
			}
		}
		if (leader != null)
		{
			if (leader.GetComponent<Character>() != null)
			{
				if (leader.GetComponent<Character>().status == "Attack")
				{
					target = leader.GetComponent<Character>().target;
					status = "Attack";
				}
			}
			float distance = Vector3.Distance(leader.position, this.transform.position);
			if (distance > 100)
			{
				leader = null;
			}
			else if (leader.Equals(transform) && leader != null)
			{
				leader = null;
			}
		}
		if (target != null)
		{	
			float distance = Vector3.Distance(target.position, this.transform.position);
			if (distance > 100)
			{
				target = null;
			}
			else if (target.Equals(transform))
			{
				target = null;
			}
			
		}
		if (runFrom != null)
		{
			float distance = Vector3.Distance(runFrom.position, this.transform.position);
			if (distance > 100)
			{
				runFrom = null;
			}
			else if (runFrom.Equals(transform))
			{
				runFrom = null;
			}
			
		}
		if (isLeader)
		{
			star.enabled = true;
			if (status == "Dead")
			{
				star.color = Color.black;
			}
		}
		else
		{
			star.enabled = false;
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
				if (leader.GetComponent<Character>() != null)
				{
					if (leader.GetComponent<Character>().health > 0)
					{
						float distance = Vector3.Distance(this.transform.position, leader.position);
						influence += ((leader.GetComponent<Character>().health / influence) / distance);
						influence -= betray;
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
						int deicide = Random.Range(0, 10);
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
							default:
								status = "Follow";
								if (leader.GetComponent<Character>() != null)
								{
									if (leader.GetComponent<Character>().killBy.GetComponent<Player>() == null)
									{
										if (leader.GetComponent<Character>().killBy.GetComponent<Character>().leader == null)
										{
											leader = leader.GetComponent<Character>().killBy.transform;
											leader.GetComponent<Character>().isLeader = true;
										}
										else if (leader.GetComponent<Character>().killBy.transform.GetComponent<Character>().leader != null)
										{
											leader = leader.GetComponent<Character>().killBy.transform.GetComponent<Character>().leader;
										}
									}
									else if (leader.GetComponent<Character>().killBy.GetComponent<Player>() != null)
									{
										leader = leader.GetComponent<Character>().killBy.transform;
										//leader.GetComponent<Player>().AddFollower(this.transform);
									}
									else
									{
										status = "Wander";
									}
								}
								else
								{
									status = "Wander";
								}
								break;
						}


					}
				}
				else if (leader.GetComponent<Player>() != null)
				{
					if (leader.GetComponent<Player>().health > 0)
					{
						float distance = Vector3.Distance(this.transform.position, leader.position);
						influence += ((leader.GetComponent<Player>().health / influence) / distance);
						influence -= betray;
						//clan = leader.GetComponent<Player>().clan;
						if (!keepAggerssion)
						{
							aggerssion = (Aggerssion)Random.Range(-1,1);
							keepAggerssion = true;
						}
					}
				}

				if (influence < 0)
				{
					target = leader;
					influence = 0;
					status = "Attack";
					betray = 0;
					leader = null;
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

	public void TookDmg(GameObject attacker)
	{
		if (attacker.GetComponent<Character>() != null || attacker.GetComponent<Player>() != null)
		{
			if (leader != null)
			{
				if (attacker.transform.Equals(leader.transform))
				{
					betray += 1;
				}
				else if (attacker.GetComponent<Character>() != null)
				{
					if (attacker.GetComponent<Character>().leader != null)
					{
						if (attacker.GetComponent<Character>().leader.Equals(leader))
						{
							if (betray > 2)
							{
								status = "Attack";
								target = attacker.transform;
							}
						}
					}
					else
					{
						status = "Attack";
						target = attacker.transform;
					}
				}
				else  
				{
					if (Random.Range(0, 4) == 0)
					{
						status = "RunAway";
						runFrom = attacker.transform;
					}
					else
					{
						status = "Attack";
						target = attacker.transform;
					}
				}
			}
			else if (isLeader)
			{
				if (Random.Range(0, 10) == 0)
				{
					status = "RunAway";
					runFrom = attacker.transform;
				}
				else
				{
					status = "Attack";
					target = attacker.transform;
				}
			}
			else
			{
				int deicide = Random.Range(0, 10);
				if (deicide == 0)
				{
					status = "RunAway";
					runFrom = attacker.transform;
				}
				else if (deicide == 1)
				{
					status = "Follow";
					leader = attacker.transform;
					if (leader.GetComponent<Character>() != null)
					{
						influence = leader.GetComponent<Character>().health;
					}
					else
					{
						influence = leader.GetComponent<Player>().health;
					}
				}
				else
				{
					status = "Attack";
					target = attacker.transform;
				}
			}
		}
	}

	public void SetSheetName(string newSheet)
	{
		sheetName = newSheet;
		sprite.Clear();
		sprite = new List<Sprite>(Resources.LoadAll<Sprite>("Images/" + sheetName));
	}

#region Movement and Animations

    public void Follow()
    {
		if (leader != null)
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
		else
		{
			status = "Wander";
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
				wanderPoint = CreateWanderPoint(this.transform.position + runFrom.forward);

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
				if (permentWanderPoint != null)
				{
					wanderPoint = CreateWanderPoint(permentWanderPoint);
				}
				else
				{
					wanderPoint = CreateWanderPoint(this.transform.position);
				}
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
				wanderTime = wanderReset;
				restTime = restReset;
			}
		}
	}
	public void Attack(Transform enemy)
	{
		if (enemy != null)
		{
			status = "Attack";
			target = enemy;
		}
		else
		{
			if (leader != null)
			{
				status = "Follow";
			}
			else 
			{
				status = "Wander";
			}
		}
	}
	public void Attack()
	{
		if (target != null)
		{
			if (target.GetComponent<Character>() != null)
			{
				if (target.GetComponent<Character>().leader != null && target.GetComponent<Character>().status == "Dead")
				{
					if (target.GetComponent<Character>().leader.Equals(this.transform))
					{
						target = null;
						status = "Wander";
					}
					else
					{
						//target = target.GetComponent<Character>().leader;
					}
				}
				else if (target.GetComponent<Character>().status == "Dead")
				{
					if (Random.Range(0, 3) > 1)
					{
						target = null;
						if (leader != null)
						{
							status = "Follow";
						}
						else
						{
							status = "Wander";
						}
					}
				}
			}
			if (!swing && target != null)
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
					swing = true;
					swingStep = 0;
				}
				LookDirection(Vector3.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime));
			}
			else if (target != null)
			{
				if (swingTime > 0)
				{
					swingTime -= Time.deltaTime;
				}
				else
				{
					if (swingStep == 0)
					{
						GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
						//GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
						GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;

						if (transform.localEulerAngles.x < 35)
						{
							transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + swingForce * Time.deltaTime / 2, transform.localEulerAngles.y, transform.localEulerAngles.z);
						}
						else
						{
							swingStep++;
							swingTime = 0.1f;
						}
					}
					else if (swingStep == 1)
					{
						if (transform.localEulerAngles.x > 315 || transform.localEulerAngles.x < 45)
						{
							transform.localEulerAngles = new Vector3(transform.localEulerAngles.x - swingForce * Time.deltaTime, transform.localEulerAngles.y, transform.localEulerAngles.z);
						}
						else
						{

							LookDirection(Vector3.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime));

							GameObject box = Instantiate(hitBox, this.transform.position, hitBox.transform.rotation) as GameObject;
							box.GetComponent<PunchHitBox>().SetParent(this.gameObject);
							box.GetComponent<Rigidbody>().AddForce(this.transform.forward * -hitForce);
							swing = false;
							swingStep = 0;
							swingTime = 0.5f;
							GetComponent<Rigidbody>().constraints = (RigidbodyConstraints)80;
						}
					}
				}
			}
		}
		else if (clan == ClanType.Evil)
		{ 
			GameObject[] otherCharacters = GameObject.FindGameObjectsWithTag("Character");
			foreach (GameObject other in otherCharacters)
			{
				if (!other.transform.Equals(this.transform))
				{
					if (other.GetComponent<Character>() != null)
					{
						if (other.transform.GetComponent<Character>().isLeader)
						{
							float distance = Vector3.Distance(transform.position, other.transform.position);
							if (distance < startAttackDistance)
							{
								int deicide = Random.Range(0, 10);
								if (deicide == 0)
								{
									target = other.transform;
									status = "Attack";
									break;
								}
							}
						}
					}
				}
			}
		}
		else
		{
			if (leader != null)
			{
				status = " Follow";
			}
			else
			{
				status = "Wander";
			}
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

	public void LookDirection(Vector3 target)
	{

		this.transform.LookAt(target);
		rotateTest = this.transform.localEulerAngles;

		GetComponent<Rigidbody>().maxAngularVelocity = 0;
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
				spriteSheet.flipX = false;
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
				spriteSheet.flipX = false;
			}
		}
		AnimationLoop(animationDirection);
	}

	public void AnimationLoop(ViewDirection direction)
	{
		string animationString = "";
		currentSprite = spriteSheet.sprite.name;
		
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
			else if (currentSprite.EndsWith("0") || currentSprite.EndsWith("0Attack"))
			{
				animationString += "1";
			}
			else
			{
				animationString += "0";
			}

			if (status == "Attack")
			{
				animationString += "Attack";
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
