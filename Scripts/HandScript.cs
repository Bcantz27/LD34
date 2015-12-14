using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandScript : MonoBehaviour 
{
	public List<Sprite> sprites;
	public SpriteRenderer spriteRend;

	public bool punch;

	public float punchSpeed;

	public float animationSpeed;
	public int currentSprite;
	public bool pulledBack;

	public Vector3 origRotation;
	public Vector3 origPostion;
	public Vector3 origScale;

	public Vector3 punchRotation;
	public Vector3 punchPostion;
	public Vector3 punchScale;

	public bool fistUp;

	// Use this for initialization
	void Start () 
	{
		currentSprite = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (fistUp)
		{
			spriteRend.enabled = true;
			if (punch)
			{
				Punch();
			}
		}
		else
		{
			punch = false;
			spriteRend.enabled = false;
		}
	}

	public void Punch()
	{
		if (animationSpeed > 0)
		{
			animationSpeed -= 1 * Time.deltaTime;
		}
		else
		{
			if (!pulledBack)
			{
				if (currentSprite == 0)
				{
					currentSprite++;
					animationSpeed = punchSpeed;
					origRotation = this.transform.localEulerAngles;
					origPostion = this.transform.localPosition;
					origScale = this.transform.localScale;
					this.transform.localEulerAngles = punchRotation;
					this.transform.localPosition = punchPostion;
					this.transform.localScale = punchScale;

					currentSprite++;
					animationSpeed = punchSpeed;
				}	
				else if (currentSprite == 3)
				{
					//currentSprite++;
					//animationSpeed = punchSpeed;
					pulledBack = true;
				}
				else
				{
					currentSprite++;
					animationSpeed = punchSpeed;
				}
			
			}
			else
			{

				if (currentSprite > 0)
				{
					currentSprite--;
					animationSpeed = punchSpeed;
				}
				else if (currentSprite == 0)
				{
					this.transform.localEulerAngles = origRotation;
					this.transform.localScale = origScale;
					this.transform.localPosition = origPostion;
					animationSpeed = punchSpeed;
					pulledBack = false;
					punch = false;
				}
			}
			spriteRend.sprite = sprites[currentSprite];
		}
	}
}
