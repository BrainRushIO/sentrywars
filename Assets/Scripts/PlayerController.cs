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


	// Use this for initialization
	void Start () {
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
			print ("HITTING FLOOR");
			GetComponent<ConstructionController> ().SwitchToPlacingBuilding ();
			currentTargetType = TargetTypes.Floor;
			break;
		default :
			isTargetingBuilding = false;
			currentTargetType = TargetTypes.None;
			break;
		}

		if (thisHit.transform.tag != "Floor") GetComponent<ConstructionController> ().SwitchToInactive ();


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
		if (currentTarget.GetComponent<BuildingBase> ().ReturnOwner () == transform.name) {
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

	public void InitializePlayer(string playerID) {
		AssignPlayerToBuilding ();
		transform.name = playerID;
		currentInhabitedBuilding.GetComponent<BuildingBase>().SetOwner (playerID);
	}

	void AssignPlayerToBuilding () {
		BuildingBase[] allBuildings = FindObjectsOfType<BuildingBase> ();
		foreach (BuildingBase x in allBuildings) {
			//assign current
			if (Vector3.Distance (x.transform.position, transform.position) < 100) {
				print ("FOUND INIT BUILDING");
				currentInhabitedBuilding = x.gameObject;
			}
		}
	}

}
