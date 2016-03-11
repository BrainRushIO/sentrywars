using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConstructionController : NetworkBehaviour {
	enum ConstructionState {Inactive, PlacingBuilding, BuildBuilding};
	ConstructionState currConstructionState = ConstructionState.Inactive;
	[SerializeField] GameObject[] buildingPrefabs;
	BuildingType currentBuildingToConstructType;
	GameObject currentBuildingToConstruct;
	bool isBuildingInstantiated;


	//State Machine Switches
	bool switchToInactive, switchToPlacingBuilding, switchToBuildBuilding;
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
		buildingCosts.Add (BuildingType.MissileLauncher, 80);

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
			SelectConstructBuildingType(BuildingType.MissileLauncher);
		}

			

		//temp UI
		constructBuildingCost.text = buildingCosts[currentBuildingToConstructType].ToString();
		constructBuildingType.text = currentBuildingToConstructType.ToString ();

		switch (currConstructionState) {
		case ConstructionState.Inactive:
			if (switchToPlacingBuilding) {
				switchToPlacingBuilding = false;
				currConstructionState = ConstructionState.PlacingBuilding;
			}
			break;
		case ConstructionState.PlacingBuilding:
			if (!isBuildingInstantiated) {
				currentBuildingToConstruct = (GameObject)Instantiate (buildingPrefabs [0]) as GameObject;
				currentBuildingToConstruct.GetComponentInChildren<BuildingBase> ().DisableAllColliders ();
				isBuildingInstantiated = true;
			}
			if (switchToInactive) {
				Destroy (currentBuildingToConstruct);
				switchToInactive = false;
				currConstructionState = ConstructionState.Inactive;
				isBuildingInstantiated = false;
			} else if (switchToBuildBuilding) {
				switchToBuildBuilding = false;
				currConstructionState = ConstructionState.BuildBuilding;
			}
			break;

		case ConstructionState.BuildBuilding:
			CmdSpawnBuilding ();
			currConstructionState = ConstructionState.Inactive;
			currentBuildingToConstruct.GetComponentInChildren<BuildingBase> ().InitializeBuilding (transform.gameObject.name);
			isBuildingInstantiated = false;
			break;
		}
	}

	void OnEnable() {

		InputController.OnRightTriggerFingerDown += SwitchToConstructBuilding;
		InputController.OnSendPointerInfo += PlaceBuilding;
	}

	void OnDisable () {
		InputController.OnRightTriggerFingerDown -= SwitchToConstructBuilding;
		InputController.OnSendPointerInfo -= PlaceBuilding;
	}

	void PlaceBuilding (RaycastHit hit) {
		if (currentBuildingToConstruct != null) {
			currentBuildingToConstruct.transform.position = hit.point;
		}
	}

	[Command]
	void CmdSpawnBuilding() {
		NetworkServer.Spawn (currentBuildingToConstruct);

	}

	void SwitchToConstructBuilding() {
		if (currConstructionState == ConstructionState.PlacingBuilding) {
			switchToBuildBuilding = true;
		}
	}

	void SelectConstructBuildingType(BuildingType thisBuildingType) {
		currentBuildingToConstructType = thisBuildingType;
	}

}
