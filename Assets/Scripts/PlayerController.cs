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

	void OnEnable() {
		InputController.OnSendPointerInfo += HandleRightHandTargeting;
		InputController.OnRightTriggerFingerDown += HandleRightTriggerDown;
	}

	void OnDisable() {
		InputController.OnSendPointerInfo -= HandleRightHandTargeting;
		InputController.OnRightTriggerFingerDown -= HandleRightTriggerDown;

	}
		
	[SerializeField] Text thisBuildingHP, thisBuildingCooldown;

	void Update() {
		if (currentInhabitedBuilding != null) {
			if (thisBuildingHP != null)
				thisBuildingHP.text = "This Tower's HP: " + currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnCurrentHealth ().ToString ("F0");
			if (thisBuildingCooldown != null)
				thisBuildingCooldown.text = "This Tower's Cooldown: " + currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnCurrentCooldown ().ToString ("F0");
		}
		HandleSelectBuildingVFX ();
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
			GetComponent<ConstructionController> ().isTargetingEnergyField = false;
			break;
		case "Energy":
			GetComponent<ConstructionController> ().SwitchToPlacingBuilding ();
			GetComponent<ConstructionController> ().isTargetingEnergyField = true;
			break;

		default :
			isTargetingBuilding = false;
			currentTargetType = TargetTypes.None;
			break;
		}

		if (thisHit.transform.tag != "Floor") GetComponent<ConstructionController> ().SwitchToInactive ();
	}

	void HandleSelectBuildingVFX () {
		if (currentTargetType == TargetTypes.Building && otherBuildingSelectedIndicator == null) {
			otherBuildingSelectedIndicator = Instantiate (otherBuildingSelectedIndicatorPrefab, currentTarget.GetComponent<BuildingBase> ().playerCockpit.position, Quaternion.identity) as GameObject;
		} else if (currentTargetType != TargetTypes.Building && otherBuildingSelectedIndicator != null) {
			Destroy (otherBuildingSelectedIndicator);
		}
	}
		
	void HandleRightTriggerDown() {
		print ("RIGHT TRIGGER DOWN" + currentTargetType);
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
			Debug.LogError (currentTarget.GetComponent<BuildingBase> ().ReturnOwner () + " " + gameObject.name);
		}
	}

	void PressGUIButton() {
		
	}

	void TeleportToBuilding () {
		currentInhabitedBuilding = currentTarget;
		MovePlayerToBuildingCockpit ();
 		currentInhabitedBuilding.GetComponent<BuildingBase> ().isOccupied = false;
		currentTarget.GetComponent<BuildingBase> ().isOccupied = true;
		if (currentTarget.GetComponent<BuildingBase> ().thisBuildingType != BuildingType.Constructor) {
			GetComponent<ConstructionController> ().isInConstructor = false;
		} else {
			GetComponent<ConstructionController> ().isInConstructor = true;
		}
	}

	void MovePlayerToBuildingCockpit() {
		transform.position = currentInhabitedBuilding.GetComponent<BuildingBase> ().playerCockpit.position;
	}

	public void SelectBuilding(BuildingType thisBuildingType) {
		currentSelectedBuilding = thisBuildingType;
	}

	public void InitializePlayer(string playerID) {
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
