using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum TargetTypes {None, Building, GUIButton, Floor, EnergyPool};

/*
Handle player movement through towers
*/

public class PlayerController : MonoBehaviour {
	
	GameObject currentInhabitedBuilding;
	[SerializeField] GameObject otherBuildingSelectedIndicatorPrefab;
	GameObject currentTarget;
	TargetTypes currentTargetType;
	bool isTargetingBuilding;
	GameObject otherBuildingSelectedIndicator;
	Camera playerCamera;
	private BuildingType currentSelectedBuilding;
	public static float TOWER_RANGE = 1000f;

	void Start () {
		playerCamera = GetComponentInChildren<Camera> ();
	}

	[SerializeField] Text thisBuildingHP, thisBuildingCooldown;

	void Update() {
		if (currentInhabitedBuilding != null) {
			if (thisBuildingHP!=null)thisBuildingHP.text = "This Tower's HP: "+currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnCurrentHealth ().ToString ("F0");
			if (thisBuildingCooldown!=null)thisBuildingCooldown.text = "This Tower's Cooldown: "+currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnCurrentCooldown ().ToString ("F0");
		}
		if (currentTargetType == TargetTypes.Building && otherBuildingSelectedIndicator == null) {
			otherBuildingSelectedIndicator = Instantiate (otherBuildingSelectedIndicatorPrefab, currentTarget.GetComponent<BuildingBase> ().playerCockpit.position, Quaternion.identity) as GameObject;
		} else if (currentTargetType != TargetTypes.Building && otherBuildingSelectedIndicator != null) {
			Destroy (otherBuildingSelectedIndicator);
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			HandleRightTriggerDown ();
		}
	}

	void FixedUpdate () {
		CastRayFromDebugReticle ();
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
			GetComponent<ConstructionController> ().SwitchToPlacingBuilding ();
			currentTargetType = TargetTypes.Floor;
			break;
		case "Energy":

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
		if (currentInhabitedBuilding!=null) currentInhabitedBuilding.GetComponent<BuildingBase>().InitializeBuilding (playerID);
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

	void CastRayFromDebugReticle () {
		Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, TOWER_RANGE)) {
			HandleRightHandTargeting (hit);
		}

	}

}
