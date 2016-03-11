using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConstructionController : NetworkBehaviour {
	enum ConstructionState {Inactive, PlacingBuilding, SpawnBuilding};
	ConstructionState currConstructionState = ConstructionState.Inactive;
	[SerializeField] GameObject[] buildingPrefabs;
	BuildingType currentBuildingToConstructType;
	GameObject currentBuildingToConstruct;
	bool isBuildingTemplateInstantiated;

	Vector3 buildingPlacementPosition;

	//State Machine Switches
	bool switchToInactive, switchToPlacingBuilding, switchToSpawnBuilding;
	public void SwitchToPlacingBuilding() {
		switchToPlacingBuilding = true;
	}
	public void SwitchToInactive() {
		switchToInactive = true;
	}


	[SerializeField] Text constructBuildingType, constructBuildingCost;

	[SerializeField]
	Dictionary<BuildingType, float> buildingCosts = new Dictionary<BuildingType, float>();



	// Use this for initialization
	void Start () {
		buildingCosts.Add (BuildingType.Constructor, 20);
		buildingCosts.Add (BuildingType.Canon, 10);
		buildingCosts.Add (BuildingType.Defense, 15);
		buildingCosts.Add (BuildingType.Shield, 50);
		buildingCosts.Add (BuildingType.Energy, 20);
		buildingCosts.Add (BuildingType.Tactical, 80);

	}


	// Update is called once per frame
	void Update () {
		//temp construction selection
		if (Input.GetKeyDown(KeyCode.Q)) {
			SelectConstructBuildingType(BuildingType.Constructor);
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			SelectConstructBuildingType(BuildingType.Canon);
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			SelectConstructBuildingType(BuildingType.Energy);
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			SelectConstructBuildingType(BuildingType.Defense);
		}
		if (Input.GetKeyDown(KeyCode.T)) {
			SelectConstructBuildingType(BuildingType.Shield);
		}
		if (Input.GetKeyDown(KeyCode.Y)) {
			SelectConstructBuildingType(BuildingType.Tactical);
		}

			

		//temp UI
		constructBuildingCost.text = "Construction Cost: " + buildingCosts[currentBuildingToConstructType].ToString();
		constructBuildingType.text = "Construction Type: " + currentBuildingToConstructType.ToString ();

		switch (currConstructionState) {
		case ConstructionState.Inactive:
			if (switchToPlacingBuilding) {
				switchToPlacingBuilding = false;
				currConstructionState = ConstructionState.PlacingBuilding;
			}
			break;
		case ConstructionState.PlacingBuilding:
			//have only one building template at a time
			if (!isBuildingTemplateInstantiated) {
				InstantiateBuildingTemplate ();
				isBuildingTemplateInstantiated = true;
			}
			if (switchToInactive) {
				Destroy (currentBuildingToConstruct);
				switchToInactive = false;
				currConstructionState = ConstructionState.Inactive;
				isBuildingTemplateInstantiated = false;
			} else if (switchToSpawnBuilding) {
				switchToSpawnBuilding = false;
				currConstructionState = ConstructionState.SpawnBuilding;
			}
			break;

		case ConstructionState.SpawnBuilding:
			currentBuildingToConstruct.GetComponent<BuildingBase> ().InitializeBuilding (transform.gameObject.name);
			isBuildingTemplateInstantiated = false;
			CmdSpawnBuilding ();
			currConstructionState = ConstructionState.Inactive;
			break;
		}
	}

	void OnEnable() {

		InputController.OnRightTriggerFingerDown += SwitchToSpawnBuilding;
		InputController.OnSendPointerInfo += PlaceBuildingTemplate;
	}

	void OnDisable () {
		InputController.OnRightTriggerFingerDown -= SwitchToSpawnBuilding;
		InputController.OnSendPointerInfo -= PlaceBuildingTemplate;
	}

	void PlaceBuildingTemplate (RaycastHit hit) {
		buildingPlacementPosition = hit.transform.position;
		if (currentBuildingToConstruct != null) {
			currentBuildingToConstruct.transform.position = hit.point;
		}
	}

	[Command]
	void CmdSpawnBuilding() {
		NetworkServer.Spawn (currentBuildingToConstruct);

	}

	void SwitchToSpawnBuilding() {
		if (currConstructionState == ConstructionState.PlacingBuilding &&
		    GetComponent<PlayerStats> ().TryToSpendEnergy (buildingCosts [currentBuildingToConstructType])) { 
			switchToSpawnBuilding = true;
		} else {
			//throw some NOT ENOUGH ENERGY MESSAGE
		}
	}

	void SelectConstructBuildingType(BuildingType thisBuildingType) {
		currentBuildingToConstructType = thisBuildingType;
		SwitchBuildingTemplate ();
	}

	void InstantiateBuildingTemplate () {
		
		currentBuildingToConstruct = (GameObject)Instantiate (buildingPrefabs [(int)currentBuildingToConstructType], buildingPlacementPosition, Quaternion.identity) as GameObject;
		currentBuildingToConstruct.GetComponentInChildren<BuildingBase> ().DisableAllColliders ();
	}

	void SwitchBuildingTemplate() {
		if (currentBuildingToConstruct != null) {
			Destroy (currentBuildingToConstruct);
		}
		InstantiateBuildingTemplate ();
	}
}
