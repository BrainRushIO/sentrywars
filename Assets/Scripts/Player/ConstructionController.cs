﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConstructionController : NetworkBehaviour {
	enum ConstructionState {Inactive, PlacingBuilding, SpawnBuilding, Cooldown};
	ConstructionState currConstructionState = ConstructionState.Inactive;

	BuildingType currentBuildingToConstructType;
	public GameObject[] buildingPrefabs;
	public GameObject currentBuildingToConstruct;
	Camera playerCamera;
	Vector3 buildingPlacementPosition;
	NetworkIdentity currentEnergyFieldTargeted;
	public bool isInConstructor = true, isTargetingEnergyField;
	bool tooCloseToOtherBuilding;
	bool isBuildingTemplateInstantiated, isBuildingTemplateGreen, canBuild;

	public const float CONSTRUCTION_RANGE = 200f;
	const float MIN_PROXIMITY_BTWN_BUILDING = 50f;
	public const float WARPIN_TIME = 3f;


	int layerIdBuilding = 10;
	int layerMaskBuilding;


	//State Machine Switches
	bool switchToInactive, switchToPlacingBuilding, switchToSpawnBuilding;

	public void SwitchToPlacingBuilding() {
		switchToPlacingBuilding = true;
	}
	public void SwitchToInactive() {
		switchToInactive = true;
	}

	[SerializeField]
	Dictionary<BuildingType, float> buildingCosts = new Dictionary<BuildingType, float>();

	void OnEnable() {
		PlayerController.OnSendPlayerInputInfo += ShowBuildingBluePrint;
		InputController.OnRightTriggerFingerDown += HandleConstructionCall;
	}

	void OnDisable() {
		PlayerController.OnSendPlayerInputInfo -= ShowBuildingBluePrint;
		InputController.OnRightTriggerFingerDown -= HandleConstructionCall;
//		Destroy (currentBuildingToConstruct);
	}

	// Use this for initialization
	void Start () {
		playerCamera = GetComponentInChildren<Camera> ();
		buildingCosts.Add (BuildingType.PowerCore, 10);
		buildingCosts.Add (BuildingType.Cannon, 40);
		buildingCosts.Add (BuildingType.Energy, 20);
		layerMaskBuilding = 1 << layerIdBuilding;
	}

	public void BuildInitialPowerCore() {
		buildingPlacementPosition = new Vector3 (transform.position.x, 0, transform.position.z);
		currConstructionState = ConstructionState.SpawnBuilding;

	}

	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}
			

		//UI
		GetComponent<GUIManager>().currentHUD.constructBuildingCost.text = "Construction Cost: " + buildingCosts[currentBuildingToConstructType].ToString();
		GetComponent<GUIManager>().currentHUD.constructBuildingType.text = "Construction Type: " + currentBuildingToConstructType.ToString ();

		//STATE MACHINE
		if (isInConstructor) {
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
				} else
					CheckIfCanBuild ();

				if (switchToInactive) {
//					Destroy (currentBuildingToConstruct);
					switchToInactive = false;
					currConstructionState = ConstructionState.Inactive;
					isBuildingTemplateInstantiated = false;
					isBuildingTemplateGreen = false;

				} else if (switchToSpawnBuilding) {
//					Destroy (currentBuildingToConstruct);
					switchToSpawnBuilding = false;
					currConstructionState = ConstructionState.SpawnBuilding;
				}
				break;
			case ConstructionState.SpawnBuilding:
				CmdSpawnBuilding (buildingPlacementPosition, GetComponent<PlayerController> ().playerInt, currentBuildingToConstructType, currentEnergyFieldTargeted, isTargetingEnergyField);
				GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().constructBuilding);
				switchToPlacingBuilding = false;
				currConstructionState = ConstructionState.Inactive;
				break;
			}
		} 
	}

	void ShowBuildingBluePrint (RaycastHit hit) {
		if (isTargetingEnergyField) { 
			//SNAP TO ENERGY FIELD
			buildingPlacementPosition = GridLogic.ConvertVector3ToGridPoint (GetComponent<PlayerController> ().ReturnCurrentTarget ().transform.position);
		} else {
			buildingPlacementPosition = GridLogic.ConvertVector3ToGridPoint (hit.point);
		}
		if (currentBuildingToConstruct != null) {
			currentBuildingToConstruct.transform.position = buildingPlacementPosition;
		}
	}
		
	[Command]
	public void CmdSpawnBuilding(Vector3 placementPos, int thisPlayerID, BuildingType thisType, NetworkIdentity thisEnergyPool, bool isEnergy) {
		isBuildingTemplateInstantiated = false;
		GameObject temp = (GameObject)Instantiate (buildingPrefabs [(int)thisType], placementPos, Quaternion.identity);
		temp.GetComponent<BuildingBase> ().enabled = true;
		if (isEnergy) {
			temp.GetComponent<BuildingBase> ().InitializeBuilding (thisPlayerID, thisEnergyPool);
		} else {
			temp.GetComponent<BuildingBase> ().InitializeBuilding (thisPlayerID);
		}
		GameObject warpFX = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs [6], placementPos+ new Vector3(0,2f,0), Quaternion.Euler(new Vector3(180,0,0)));
		NetworkServer.Spawn (warpFX);
		NetworkServer.SpawnWithClientAuthority (temp, gameObject);
		StartCoroutine (WarpSplash(placementPos));

	}

	IEnumerator WarpSplash (Vector3 pos) {
		yield return new WaitForSeconds (WARPIN_TIME);
		CmdSpawnWarpSplash (pos);
	}

	[Command]
	void CmdSpawnWarpSplash (Vector3 thisPos) {
		GameObject thisSplash = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs [7], thisPos, Quaternion.identity);
		NetworkServer.Spawn (thisSplash);
	}

	void HandleConstructionCall() {
		if (currentBuildingToConstruct != null) {
			if (!IsBuildingTemplateInConstructionRange ()) {
				GetComponent<GUIManager> ().SetAlert ("Out of Build Range");
				GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().error);

			} else if (!GetComponent<PlayerStats> ().IsThereEnoughEnergy (buildingCosts [currentBuildingToConstructType])) {
				GetComponent<GUIManager> ().SetAlert ("Not Enough Energy");
				GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().error);

			} else if (tooCloseToOtherBuilding) {
				GetComponent<GUIManager> ().SetAlert ("Too Close to Nearby Structure");
				GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().error);
			}
		}
		SwitchToSpawnBuilding ();
	}

	void SwitchToSpawnBuilding() {
		if (currConstructionState == ConstructionState.PlacingBuilding &&
		    GetComponent<PlayerStats> ().IsThereEnoughEnergy (buildingCosts [currentBuildingToConstructType]) &&
		    canBuild) { 
			HandleSpendEnergyOnBuilding ();
		} 
	}
	void HandleSpendEnergyOnBuilding() {
		GetComponent<PlayerStats> ().SpendEnergy (buildingCosts [currentBuildingToConstructType]);
		if (currentBuildingToConstructType == BuildingType.Energy) {
			GetComponent<PlayerStats> ().IncreaseEnergyUptake ();
			currentEnergyFieldTargeted = GetComponent<PlayerController> ().ReturnCurrentTarget ().GetComponent<NetworkIdentity> ();
			CmdLinkEnergyField (currentEnergyFieldTargeted);
		}
		switchToSpawnBuilding = true;
	}

	[Command]
	void CmdLinkEnergyField (NetworkIdentity thisField) {
		thisField.GetComponent<EnergyField> ().CmdSetIsOccupied(true);
	}


	public void SelectConstructBuildingType(BuildingType thisBuildingType) {
		currentBuildingToConstructType = thisBuildingType;
		SwitchToPlacingBuilding ();
	}

	void InstantiateBuildingTemplate () {
		print ("INSTANIATE");

		currentBuildingToConstruct = (GameObject)Instantiate (buildingPrefabs [(int)currentBuildingToConstructType], buildingPlacementPosition, Quaternion.identity);
		print (currentBuildingToConstruct.name);
		currentBuildingToConstruct.GetComponentInChildren<BuildingBase> ().DisableAllColliders ();
		CheckIfCanBuild ();

	}

	void RenderCurrentBuildingAsTemplate(bool isBuildable) {
		if (isBuildable && !isBuildingTemplateGreen) {
			isBuildingTemplateGreen = true;
			RenderMeshGreenRed (true);
		} else if (!isBuildable && isBuildingTemplateGreen) {
			isBuildingTemplateGreen = false;
			RenderMeshGreenRed (false);
		} else if (!isBuildingTemplateGreen) {
			RenderMeshGreenRed (false);
		} else {
			RenderMeshGreenRed (true);
		}
	}

	void RenderMeshGreenRed(bool green) {
		MeshRenderer[] allMaterialsOnBuilding = currentBuildingToConstruct.GetComponentsInChildren<MeshRenderer> ();
		if (green) {
			foreach (MeshRenderer x in allMaterialsOnBuilding) {
				x.material = GetComponent<PlayerColorManager> ().templateColorGreen;
			}
			if (currentBuildingToConstruct.GetComponent<MeshRenderer> () != null) {
				currentBuildingToConstruct.GetComponent<MeshRenderer> ().material = GetComponent<PlayerColorManager> ().templateColorGreen;
			}
		} else {
			foreach (MeshRenderer x in allMaterialsOnBuilding) {
				x.material = GetComponent<PlayerColorManager> ().templateColorRed;
			}
			if (currentBuildingToConstruct.GetComponent<MeshRenderer> () != null) {
				currentBuildingToConstruct.GetComponent<MeshRenderer> ().material = GetComponent<PlayerColorManager> ().templateColorRed;
			}
		}
	}

	public void DestroyBuildingTemplate() {
		if (currentBuildingToConstruct != null) {
//			Destroy (currentBuildingToConstruct);
		}
		isBuildingTemplateInstantiated = false;
	}

	bool IsBuildingTemplateInConstructionRange() {
		return (Vector3.Distance (buildingPlacementPosition, new Vector3(transform.position.x, 0f, transform.position.z)) < CONSTRUCTION_RANGE);
	}

	void CheckIfCanBuild () {
		if (IsBuildingTemplateInConstructionRange() && currentBuildingToConstruct!=null) {
			canBuild = true;
			if (currentBuildingToConstructType == BuildingType.Energy && !isTargetingEnergyField) {
				RenderCurrentBuildingAsTemplate (false);
				canBuild = false;
			} else if (currentBuildingToConstructType == BuildingType.Energy && isTargetingEnergyField) {
				RenderCurrentBuildingAsTemplate (true);
			} else if (CheckIfOtherBuildingsInRadius ()){
				RenderCurrentBuildingAsTemplate (false);
				canBuild = false;
			}
			else if (currentBuildingToConstructType != BuildingType.Energy && isTargetingEnergyField) {
				RenderCurrentBuildingAsTemplate (false);
				canBuild = false;
			} else {
				RenderCurrentBuildingAsTemplate (true);
			}
		} else if (currentBuildingToConstruct!=null){
			RenderCurrentBuildingAsTemplate (false);
			canBuild = false;
		}
	}

	bool CheckIfOtherBuildingsInRadius () {
		Collider[] temp = Physics.OverlapSphere(buildingPlacementPosition, MIN_PROXIMITY_BTWN_BUILDING, layerMaskBuilding);
		if (temp.Length > 0) {
			tooCloseToOtherBuilding = true;
			return true;
		} else {
			tooCloseToOtherBuilding = false;
			return false;
		}
			
	}
}
