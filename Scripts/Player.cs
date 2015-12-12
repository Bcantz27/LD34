using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public float speed = 10;
	public float jump = 300;
	public float runSpeed = 15;
	public float enegry;

	public GameObject camera;
	// Use this for initialization
	void Start () 
    {
		enegry = 100;
	}
	
	// Update is called once per frame
	void Update () 
    {
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


		if (Input.GetKey(KeyCode.W))
		{
			transform.position += forward * Time.deltaTime * speed;
		}
		if (Input.GetKey(KeyCode.S))
		{
			transform.position -= forward * Time.deltaTime * speed;
		}
		if (Input.GetKey(KeyCode.A))
		{
			transform.position -= right * Time.deltaTime * speed;
		}
		if (Input.GetKey(KeyCode.D))
		{
			transform.position += right * Time.deltaTime * speed;
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GetComponent<Rigidbody>().AddForce(transform.up*jump);
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
}
