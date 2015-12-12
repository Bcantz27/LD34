using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

    public bool fixedRotation = false;
    public Sprite[] sprites;
    private ViewDirection direction = ViewDirection.Front;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        Transform spriteTransform = transform.FindChild("Sprite");

        var lookPos = Camera.main.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        spriteTransform.rotation = Quaternion.Slerp(spriteTransform.rotation, rotation, Time.deltaTime * 10);

        if (fixedRotation)
        {

            var rotVal = (Quaternion.Inverse(transform.rotation) * spriteTransform.rotation).eulerAngles.y;
            print(rotVal);

            if (rotVal > 315 || rotVal < 45)
            {
                //Front
                setDirection(ViewDirection.Front);
            }
            else if (rotVal >= 45 && rotVal <= 135) // Right Side
            {
                setDirection(ViewDirection.Right);
            }
            else if (rotVal >= 225 && rotVal <= 315)   //Left Side
            {
                setDirection(ViewDirection.Left);
            }
            else if (rotVal > 135 && rotVal < 225)
            {
                //Back
                setDirection(ViewDirection.Back);
            }

        }
	}

    public void setDirection(ViewDirection direction)
    {

        transform.FindChild("Sprite").FindChild("Sprite_" + this.direction.ToString()).GetComponent<SpriteRenderer>().enabled = false;

        this.direction = direction;

        transform.FindChild("Sprite").FindChild("Sprite_" + this.direction.ToString()).GetComponent<SpriteRenderer>().enabled = true;
    }

    public enum ViewDirection{
        Front,
        Right,
        Left,
        Back
    }
}
