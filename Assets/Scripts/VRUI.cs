using UnityEngine;
using System.Collections;


public class VRUI : MonoBehaviour {

	public GameObject currentlyHighlightedObject;
	public GameObject VRUIPanel;
	GameObject tempPanel;

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

			InterpretAction (x, y);
			//select building type
			//enter buildmode
		}
	}

	void InterpretAction(VRUISelectionAction thisAction, VRUISelectionActionType thisActionType) {
		if (thisActionType == VRUISelectionActionType.Gameplay) {

			switch (thisAction) {
			case VRUISelectionAction.Airport:
				GetComponentInParent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Airport);
				break;
			case VRUISelectionAction.Cannon:
				GetComponentInParent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Cannon);

				break;

			case VRUISelectionAction.PowerCore:
				Debug.Log ("TRIGGER POWERCORE");
				GetComponentInParent<ConstructionController> ().SelectConstructBuildingType (BuildingType.PowerCore);

				break;
			case VRUISelectionAction.EnergyMine:
				GetComponentInParent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Energy);

				break;

			case VRUISelectionAction.Sniper:
				GetComponentInParent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Cannon);

				break;

			}
			ToggleVRUI ();
		}
	}

	void ToggleVRUI() {
		if (tempPanel == null) {
			tempPanel = (GameObject)Instantiate (VRUIPanel, transform.position, transform.rotation);
			GetComponentInParent<ConstructionController> ().SwitchToInactive ();
		} else {
			Destroy (tempPanel);
		}
//		GetComponentInParent<ConstructionController> ().DestroyBuildingTemplate ();
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
