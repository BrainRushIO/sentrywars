using UnityEngine;
using System.Collections;


public class VRUI : MonoBehaviour {

	public GameObject currentlyHighlightedObject;
	public GameObject VRUIPanel;
	GameObject tempPanel;
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
			VRUISelectionAction x = currentlyHighlightedObject.GetComponent<OnSelectAction> ().thisVRUISelectionAction;
			VRUISelectionActionType y = currentlyHighlightedObject.GetComponent<OnSelectAction> ().thisVRUISelectionActionType;

			VRUIPanel.SetActive (false);
			InterpretAction (x, y);
			//select building type
			//enter buildmode
		}
	}

	void InterpretAction(VRUISelectionAction thisAction, VRUISelectionActionType thisActionType) {
		switch (thisAction) {
		case VRUISelectionAction.Airport:
			GetComponent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Cannon);
			break;
		case VRUISelectionAction.Cannon:
			GetComponent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Cannon);

			break;

		case VRUISelectionAction.PowerCore:
			GetComponent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Constructor);

			break;
		case VRUISelectionAction.EnergyMine:
			GetComponent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Energy);

			break;

		case VRUISelectionAction.Sniper:
			GetComponent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Cannon);

			break;

		}
	}

	void ToggleVRUI() {
		if (tempPanel == null) {
			tempPanel = (GameObject)Instantiate (VRUIPanel, transform.position, transform.rotation);
		} else {
			Destroy (tempPanel);
		}
		GetComponentInParent<ConstructionController> ().DestroyBuildingTemplate ();
	}

	void OnEnable() {
		InputController.OnRightTriggerFingerDown += SelectVRUI;
		InputController.OnRightTouchPadDown += ToggleVRUI;
	}

	void OnDisable() {
		InputController.OnRightTriggerFingerDown -= SelectVRUI;
		InputController.OnRightTouchPadDown -= ToggleVRUI;

	}
}
