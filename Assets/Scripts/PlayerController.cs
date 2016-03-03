using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum TargetTypes {Building, GUIButton, Floor, EnergyPool, None};

/*
Handle player movement through towers
*/

public class PlayerController : MonoBehaviour {


	[SerializeField] GameObject currentInhabitedBuilding;
	GameObject currentTarget;
	TargetTypes currentTargetType;
	bool isTargetingBuilding;

	private BuildingType currentSelectedBuilding;
	private int owner;
	public int ReturnOwner(){return owner;}

	// Use this for initialization
	void Start () {
//		currentInhabitedBuilding.GetComponent<BuildingBase> ().isOccupied = true;
//		MovePlayerToBuildingCockpit ();
	}

	void OnEnable() {
		InputController.OnRightTriggerFingerDown += HandleRightTriggerDown;
		InputController.OnRightTriggerFingerUp += HandleRightTriggerDown;

		InputController.OnSendPointerInfo += HandleRightHandTargeting;
	}

	void OnDisable () {
		InputController.OnRightTriggerFingerDown -= HandleRightTriggerDown;
		InputController.OnRightTriggerFingerUp -= HandleRightTriggerDown;

		InputController.OnSendPointerInfo -= HandleRightHandTargeting;
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void HandleRightHandTargeting(RaycastHit thisHit) {
		currentTarget = thisHit.collider.gameObject;
		switch(thisHit.transform.tag){
		case "Building":
			isTargetingBuilding = true;
			currentTargetType = TargetTypes.Building;
			break;
		case "GUIButton":
			currentTargetType = TargetTypes.GUIButton;
			PressGUIButton ();
			break;
		case "Floor":
			currentTargetType = TargetTypes.Floor;
			break;
		default :
			isTargetingBuilding = false;
			currentTargetType = TargetTypes.None;
			break;
		}
			

	}
		
	void HandleRightTriggerDown() {
		switch (currentTargetType) {
		case TargetTypes.Building:
			PerformActionOnTargetedBuilding ();
			break;
		case TargetTypes.GUIButton:
			PressGUIButton ();
			break;
		}

	}

	void HandleRightTriggerUp() {

	}

	void PerformActionOnTargetedBuilding() {
		if (currentTarget.GetComponent<BuildingBase> ().ReturnOwner () == owner) {
			TeleportToBuilding ();
		}
	}

	void PressGUIButton() {
		
	}

	void TeleportToBuilding () {
		
		currentInhabitedBuilding = currentTarget;
		MovePlayerToBuildingCockpit ();
 		currentInhabitedBuilding.GetComponent<BuildingBase> ().isOccupied = false;
		currentTarget.GetComponent<BuildingBase> ().isOccupied = true;
	}

	void MovePlayerToBuildingCockpit() {
		transform.position = currentInhabitedBuilding.GetComponent<BuildingBase> ().playerCockpit.position;
	}

	public void SelectBuilding(BuildingType thisBuildingType) {
		currentSelectedBuilding = thisBuildingType;
	}

}
