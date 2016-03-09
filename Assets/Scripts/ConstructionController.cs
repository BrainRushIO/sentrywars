using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ConstructionController : NetworkBehaviour {
	enum ConstructionState {Inactive, PlacingBuilding, BuildBuilding};
	ConstructionState currConstructionState = ConstructionState.Inactive;
	public enum ConstructionOptions {Nexus, Canon, Shield, Defense, Energy, MissileLauncher};
	[SerializeField] GameObject[] buildingPrefabs;

	GameObject currentBuildingToBuild;
	bool isBuildingInstantiated;


	//Switches
	bool switchToInactive, switchToPlacingBuilding, switchToBuildBuilding;
	public void SwitchToPlacingBuilding() {
		switchToPlacingBuilding = true;
	}
	public void SwitchToInactive() {
		switchToInactive = true;
	}
		


	// Update is called once per frame
	void Update () {
		switch (currConstructionState) {
		case ConstructionState.Inactive:
			if (switchToPlacingBuilding) {
				switchToPlacingBuilding = false;
				currConstructionState = ConstructionState.PlacingBuilding;
			}
			break;
		case ConstructionState.PlacingBuilding:
			if (!isBuildingInstantiated) {
				currentBuildingToBuild = (GameObject)Instantiate (buildingPrefabs [0]) as GameObject;
				currentBuildingToBuild.GetComponentInChildren<BuildingBase> ().DisableAllColliders ();
				isBuildingInstantiated = true;
			}
			if (switchToInactive) {
				Destroy (currentBuildingToBuild);
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
			currentBuildingToBuild.GetComponentInChildren<BuildingBase> ().InitializeBuilding (transform.gameObject.name);
			isBuildingInstantiated = false;
			break;
		}
	}

	void OnEnable() {

		InputController.OnRightTriggerFingerDown += SwitchToBuildBuilding;
		InputController.OnSendPointerInfo += PlaceBuilding;
	}

	void OnDisable () {
		InputController.OnRightTriggerFingerDown -= SwitchToBuildBuilding;
		InputController.OnSendPointerInfo -= PlaceBuilding;
	}

	void PlaceBuilding (RaycastHit hit) {
		if (currentBuildingToBuild != null) {
			currentBuildingToBuild.transform.position = hit.point;
		}
	}

	[Command]
	void CmdSpawnBuilding() {
		NetworkServer.Spawn (currentBuildingToBuild);

	}

	void SwitchToBuildBuilding() {
		if (currConstructionState == ConstructionState.PlacingBuilding) {
			switchToBuildBuilding = true;
		}
	}


}
