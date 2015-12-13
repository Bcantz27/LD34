using UnityEngine;
using System.Collections;

public class Territory : MonoBehaviour {

    public int followers = 5;
    public int influenceNeeded = 25;
    public int bosses = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void onTriggerEnter(Collider other){
        Debug.Log("Enter");
    }

    void onTriggerExit(Collider other)
    {
        Debug.Log("Enter");
    }
}
