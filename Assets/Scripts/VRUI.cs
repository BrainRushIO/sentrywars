using UnityEngine;
using System.Collections;


public class VRUI : MonoBehaviour {

	public GameObject currentlyHighlightedObject;
	public GameObject VRUIPowerCore, VRUIAirport;
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
				StartCoroutine ("SetBuildMode");

				break;
			case VRUISelectionAction.Cannon:
				GetComponentInParent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Cannon);
				StartCoroutine ("SetBuildMode");

				break;

			case VRUISelectionAction.PowerCore:
				GetComponentInParent<ConstructionController> ().SelectConstructBuildingType (BuildingType.PowerCore);
				StartCoroutine ("SetBuildMode");

				break;
			case VRUISelectionAction.EnergyMine:
				GetComponentInParent<ConstructionController> ().SelectConstructBuildingType (BuildingType.Energy);
				StartCoroutine ("SetBuildMode");

				break;

			case VRUISelectionAction.AntiAir:
				GetComponentInParent<ConstructionController> ().SelectConstructBuildingType (BuildingType.AntiAir);
				StartCoroutine ("SetBuildMode");

				break;

			case VRUISelectionAction.ReturnDrone:
				GetComponentInParent<PlayerController> ().currentInhabitedBuilding.GetComponent<Airport> ().ReturnDrone ();
				break;

			case VRUISelectionAction.RebuildDrone:
				if (GetComponentInParent<PlayerStats> ().GetCurrentEnergy () < 40) {
					GetComponentInParent<GUIManager> ().SetAlert ("Not Enough Energy");
				} else if (!GetComponentInParent<PlayerController> ().currentInhabitedBuilding.GetComponent<Airport> ().RebuildDrone ()) {
					GetComponentInParent<GUIManager> ().SetAlert ("Drone Already Built");
				} else {
					GetComponentInParent<PlayerStats> ().SpendEnergy (40f);
				}

				break;

			}
			Destroy (tempPanel);
		}
	}

	IEnumerator SetBuildMode () {
		//script execution order fix
		yield return new WaitForSeconds (.1f);
		GetComponentInParent<ConstructionController> ().ToggleBuildMode (true);

	}

	void ToggleVRUI() {
		if (GetComponentInParent<PlayerController>().currentInhabitedBuilding!=null &&
			GetComponentInParent<PlayerController>().currentInhabitedBuilding.GetComponent<BuildingBase>().hasVRUI) {
			GetComponentInParent<ConstructionController> ().ToggleBuildMode (false);

			if (tempPanel == null) {
//				if (GetComponentInParent<PlayerController> ().currentInhabitedBuilding.GetComponent<BuildingBase> ().thisBuildingType == BuildingType.Airport) {
//					tempPanel = (GameObject)Instantiate (VRUIAirport, transform.position, transform.rotation);
//				 if ((GetComponentInParent<PlayerController> ().currentInhabitedBuilding.GetComponent<BuildingBase> ().thisBuildingType == BuildingType.PowerCore)) {
					tempPanel = (GameObject)Instantiate (VRUIPowerCore, transform.position, transform.rotation);
//				}
				GetComponentInParent<ConstructionController> ().SwitchToInactive ();
			} else {
				Destroy (tempPanel);
				GetComponentInParent<ConstructionController> ().SwitchToInactive ();
			}
	//		GetComponentInParent<ConstructionController> ().DestroyBuildingTemplate ();
		}
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
