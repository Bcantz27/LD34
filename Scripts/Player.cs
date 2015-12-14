﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{
	public float health = 10;
	public float speed = 10;
	public float jump = 300;
	public float runSpeed = 15;
	public float enegry;

	private float savedSpeed;
	private Vector3 oldPastPostion;

	public GameObject camera;

	public GameObject handLeft;
	public GameObject handRight;

	public GameObject punchHitBox;
	public float punchForce;
	public float distToGround;

	public float dmg;

	// Use this for initialization
	void Start () 
    {
		enegry = 100;
		savedSpeed = speed;
		distToGround = this.GetComponent<Collider>().bounds.extents.y;
		//Physics.IgnoreCollision(handLeft.GetComponent<Collider>(), transform.GetComponent<Collider>(),true);
		//Physics.IgnoreCollision(handRight.GetComponent<Collider>(), transform.GetComponent<Collider>(), true);
		//Physics.IgnoreCollision(handLeft.GetComponent<Collider>(), handRight.GetComponent<Collider>(), true);
		if (dmg == 0)
		{
			dmg = 1;
		}
	}
	
	// Update is called once per frame
	void Update () 
    {
		//oldPastPostion = transform.position;
		//float zTrans = 0;
		//float xTrans = 0;
		
		//zTrans = Input.GetAxis("Vertical") * speed * Time.deltaTime;
		//xTrans = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

		float xF = minMove(camera.transform.forward.x,0.05f);
		float zF = minMove(camera.transform.forward.z, 0.05f);

		float xR = minMove(camera.transform.right.x, 0.05f);
		float zR = minMove(camera.transform.right.z, 0.05f);


		Vector3 forward = new Vector3(xF,0, zF);
		Vector3 right = new Vector3(xR, 0, zR);

		if (Mathf.Abs(GetComponent<Rigidbody>().velocity.x) < speed / 10 && Mathf.Abs(GetComponent<Rigidbody>().velocity.z) < speed / 10)
		{ 
			if (Input.GetKey(KeyCode.W))
			{
				GetComponent<Rigidbody>().velocity += forward * Time.deltaTime * speed;
			}
			if (Input.GetKey(KeyCode.S))
			{
				GetComponent<Rigidbody>().velocity -= forward * Time.deltaTime * speed;
			}
			if (Input.GetKey(KeyCode.A))
			{
				GetComponent<Rigidbody>().velocity -= right * Time.deltaTime * speed;
			}
			if (Input.GetKey(KeyCode.D))
			{
				GetComponent<Rigidbody>().velocity += right * Time.deltaTime * speed;
			}
		}
		if (Input.GetKeyDown(KeyCode.Space)&& IsGrounded())
		{
			GetComponent<Rigidbody>().AddForce(transform.up*jump);
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (!handLeft.GetComponent<HandScript>().punch && handRight.GetComponent<HandScript>().fistUp)
			{
				handLeft.GetComponent<HandScript>().punch = true;
				GameObject hitbox = Instantiate(punchHitBox as Object, Camera.main.transform.position, punchHitBox.transform.rotation) as GameObject;
				hitbox.GetComponent<PunchHitBox>().SetParent(this.gameObject);
				hitbox.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * punchForce);
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			if (!handRight.GetComponent<HandScript>().punch && handLeft.GetComponent<HandScript>().fistUp)
			{
				handRight.GetComponent<HandScript>().punch = true;
				GameObject hitbox = Instantiate(punchHitBox as Object, Camera.main.transform.position, punchHitBox.transform.rotation) as GameObject;
				hitbox.GetComponent<PunchHitBox>().SetParent(this.gameObject);
				hitbox.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * punchForce);
			}
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			handRight.GetComponent<HandScript>().fistUp = !handRight.GetComponent<HandScript>().fistUp;
			handLeft.GetComponent<HandScript>().fistUp = !handLeft.GetComponent<HandScript>().fistUp;
		}
		
		//Vector3 cameraAngles = camera.transform.localEulerAngles;

		//transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, cameraAngles.y, this.transform.localEulerAngles.z);
	}

	private float minMove(float move, float min)
	{
		if (move > 0)
		{
			if (move >= min)
			{
				return move;
			}
			else
			{
				return min;
			}
		}
		else if (move < 0)
		{
			if (move <= min)
			{
				return move;
			}
			else
			{
				return -min;
			}
		}
		else
		{
			return 0;
		}
	}

	public bool IsGrounded()
	{
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "World")
		{
			Debug.LogWarning("Boop");

			if (GetComponent<Rigidbody>().velocity.z > 0)
            {
                Debug.Log("force added");
				GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 50));
                //bounceForceApplied = true;
            }
			else if (GetComponent<Rigidbody>().velocity.z < 0)
            {
				GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -50));
                //bounceForceApplied = true;
            }
			GetComponent<Rigidbody>().AddForce(new Vector3(0, -400, 0));
		}
		if(collision.collider.tag == "HitBox")
		{
			GetComponent<Rigidbody>().AddForce(Vector3.up * (jump / 2));
		}
	}
}
