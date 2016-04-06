using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum TargetTypes {None, Building, GUIButton, Floor, EnergyPool};

/*
Handle player movement through towers
*/

public class PlayerController : NetworkBehaviour {

	public string playerID;
	public GameObject currentInhabitedBuilding;
	[SerializeField] GameObject otherBuildingSelectedIndicatorPrefab, teleportPrefab;
	GameObject currentTarget;
	NetworkInstanceId currentBuildingID;
	public GameObject ReturnCurrentTarget() {
		return currentTarget;
	}
	TargetTypes currentTargetType;
	GameObject otherBuildingSelectedIndicator;
	Camera playerCamera;
	BuildingType currentInhabitedBuildingType;

	void OnEnable() {
		InputController.OnSendPointerInfo += HandleRightHandTargeting;
		InputController.OnRightTriggerFingerDown += HandleRightTriggerDown;
	}

	void OnDisable() {
		InputController.OnSendPointerInfo -= HandleRightHandTargeting;
		InputController.OnRightTriggerFingerDown -= HandleRightTriggerDown;

	}
		
	[SerializeField] Text thisBuildingHP, thisBuildingCooldown, youlose;

	void Update() {
		if (currentInhabitedBuilding != null) {
			if (thisBuildingHP != null)
				thisBuildingHP.text = "This Tower's HP: " + currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnCurrentHealth ().ToString ("F0");
			if (thisBuildingCooldown != null)
				thisBuildingCooldown.text = "This Tower's Cooldown: " + currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnCurrentCooldown ().ToString ("F0");
		}
	}
		
	void HandleRightHandTargeting(RaycastHit thisHit) {
		currentTarget = thisHit.collider.gameObject;
		if (currentTarget == currentInhabitedBuilding) {
			GetComponent<ConstructionController> ().SwitchToInactive ();
			return;
		}
		switch(thisHit.transform.tag){
		case "Building":
			currentTargetType = TargetTypes.Building;
			currentBuildingID = currentTarget.GetComponent<NetworkIdentity> ().netId;
			break;
		case "GUIButton":
			currentTargetType = TargetTypes.GUIButton;
			PressGUIButton ();
			break;
		case "Floor":
			if (currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnHaveColorsBeenSet()) {
				GetComponent<ConstructionController> ().SwitchToPlacingBuilding ();
				currentTargetType = TargetTypes.Floor;
				GetComponent<ConstructionController> ().isTargetingEnergyField = false;
			}
			break;
		case "Energy":
			if (currentTarget.GetComponent<EnergyField> ().isOccupied != true) {
				GetComponent<ConstructionController> ().SwitchToPlacingBuilding ();
				currentTargetType = TargetTypes.Floor;
				GetComponent<ConstructionController> ().isTargetingEnergyField = true;
			} 
			break;

		default :
			currentTargetType = TargetTypes.None;
			break;
		}
		HandleSelectBuildingVFX ();
		if (currentTargetType!=TargetTypes.Floor) GetComponent<ConstructionController> ().SwitchToInactive ();
	}

	void HandleSelectBuildingVFX () {
		if (currentTarget.GetComponent<BuildingBase>()!=null && otherBuildingSelectedIndicator == null && currentTarget!=currentInhabitedBuilding) {
			otherBuildingSelectedIndicator = Instantiate (otherBuildingSelectedIndicatorPrefab, currentTarget.GetComponent<BuildingBase> ().playerCockpit.position, Quaternion.identity) as GameObject;
		} else if (currentTargetType != TargetTypes.Building && otherBuildingSelectedIndicator != null) {
			Destroy (otherBuildingSelectedIndicator);
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
		if (currentTarget.GetComponent<BuildingBase> ().ReturnOwner () == gameObject.name) {
			TeleportToBuilding ();
		} else {
			switch (currentInhabitedBuildingType) {
			case BuildingType.Canon:
				ChangeTarget (currentBuildingID);
				break;
			}
		}

	}

	void ChangeTarget(NetworkInstanceId thisID) {
		if (isLocalPlayer) {
			currentInhabitedBuilding.GetComponent<Tower> ().CmdTargetNewBuilding (thisID);
		}
	}

	void PressGUIButton() {
		
	}

	void TeleportToBuilding () {
		currentInhabitedBuilding = currentTarget;
		GameObject tempTeleportVFX = (GameObject)Instantiate (teleportPrefab, currentInhabitedBuilding.GetComponent<BuildingBase> ().playerCockpit.position, Quaternion.identity);
		Destroy (tempTeleportVFX, 4f);
		MovePlayerToBuildingCockpit ();
 		currentInhabitedBuilding.GetComponent<BuildingBase> ().isOccupied = false;
		currentTarget.GetComponent<BuildingBase> ().isOccupied = true;
		currentInhabitedBuildingType = currentTarget.GetComponent<BuildingBase> ().thisBuildingType;
		if (currentInhabitedBuildingType != BuildingType.Constructor) {
			GetComponent<ConstructionController> ().isInConstructor = false;
		} else {
			GetComponent<ConstructionController> ().isInConstructor = true;
		}
	}

	void MovePlayerToBuildingCockpit() {
		transform.position = currentInhabitedBuilding.GetComponent<BuildingBase> ().playerCockpit.position;
		Destroy (otherBuildingSelectedIndicator);
	}
		
	public void InitializePlayer() {
		transform.name = playerID;
		BuildingBase[] allBuildings = FindObjectsOfType<BuildingBase> ();
		foreach (BuildingBase x in allBuildings) {
			//assign current
			if (Vector3.Distance (x.transform.position, transform.position) < 100) {
				currentInhabitedBuilding = x.gameObject;
				currentInhabitedBuilding.GetComponent<BuildingBase> ().InitializeBuilding (playerID);
			}
		}
	}

}
