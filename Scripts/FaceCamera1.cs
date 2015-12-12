using UnityEngine;
using System.Collections;

public class FaceCamera1 : MonoBehaviour 
{
    public GameObject mainCamera;

    public float rotXOffset;
    public float rotYOffset;
    public float rotZOffset;
    public bool enabled = true;

	void Start()
	{ 
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}

	// Update is called once per frame
	void Update () 
    {
            if (enabled)
            {
				Quaternion rotateDest = mainCamera.transform.rotation;
				rotateDest.x = 0;
				transform.rotation = rotateDest;

                transform.Rotate(rotXOffset, rotYOffset, rotZOffset, Space.Self);

				var fixedRotate = transform.localEulerAngles;

				fixedRotate.x = 0;
				fixedRotate.z = 0;
				transform.localEulerAngles = fixedRotate;

            }
    }
}
