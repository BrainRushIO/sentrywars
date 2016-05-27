using UnityEngine;
using System.Collections;

public class VRUI : MonoBehaviour {

	public GameObject currentlyHighlightedObject;
	public GameObject VRUIPanel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {

		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, 1000f)) {
			if (hit.transform.tag == "VRUIObject") {
				hit.transform.GetComponent<VRUIObject> ().HoverOver ();
				currentlyHighlightedObject = hit.transform.gameObject;                                                                                                                                                                                                                                                                                                                                                                                       
			}
		} else {
			currentlyHighlightedObject = null;
		}
	}

	void SelectVRUI() {
		if (currentlyHighlightedObject != null) {
			VRUIPanel.SetActive (false);
			//select building type
			//enter buildmode
		}
	}

	void OnEnable() {
		InputController.OnRightTriggerFingerDown += SelectVRUI;
	}

	void OnDisable() {
		InputController.OnRightTriggerFingerDown -= SelectVRUI;
	}
}
