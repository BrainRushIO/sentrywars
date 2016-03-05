using UnityEngine;
using System.Collections;

public class TabToDisable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Tab)) {
			transform.GetChild (0).gameObject.SetActive (!transform.GetChild (0).gameObject.activeSelf);
		}
	}
}
