using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserSelectionInterface : MonoBehaviour {

	//TODO make this automated

	float verticalAngle = 15f;
	float horizontalAngle = 15f;

	public List<GameObject> userProfilePivots;
	// Use this for initialization
	void Start () {
		for(int i = 0; i < transform.childCount ; i++) {
			userProfilePivots.Add (transform.GetChild(i).gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
