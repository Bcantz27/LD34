using UnityEngine;
using System.Collections;

public class SpriteShadow : MonoBehaviour {

    public bool castShadows;
    public bool receiveShadows;

	// Use this for initialization
	void Start () {
        this.GetComponent<SpriteRenderer>().receiveShadows = receiveShadows;
        this.GetComponent<SpriteRenderer>().castShadows = castShadows; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
