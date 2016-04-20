﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum TargetTypes {None, Building, GUIButton, Floor, EnergyPool};

/*
	Handle player movement through towers
*/

public class PlayerController : NetworkBehaviour {
	public int playerInt;
	public string playerID;
	[SyncVar] public NetworkIdentity currentInhabitedBuilding;
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
	bool isInitialized;

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
			if (Input.GetKeyDown (KeyCode.P)) {
				InitializePlayer (0);
				GameManager.gameHasStarted = true;
			}
			if (!isInitialized) {
				InhabitClosestBuilding ();
			}
			if (currentInhabitedBuilding != null) {
				if (thisBuildingHP != null)
					thisBuildingHP.text = "This Tower's HP: " + currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnCurrentHealth ().ToString ("F0");
				if (thisBuildingCooldown != null)
					thisBuildingCooldown.text = "This Tower's Cooldown: " + currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnCurrentCooldown ().ToString ("F0");
			}
	}
		
	void HandleRightHandTargeting(RaycastHit thisHit) {
		if (currentInhabitedBuilding == null) {
			return;
		}
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
			if (currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnIsBuildingActive()) {
				GetComponent<ConstructionController> ().SwitchToPlacingBuilding ();
				currentTargetType = TargetTypes.Floor;
				GetComponent<ConstructionController> ().isTargetingEnergyField = false;
			}
			break;
		case "Energy":
			if (!currentTarget.GetComponent<EnergyField> ().ReturnIsOccupied()) {
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

	void PerformActionOnTargetedBuilding() {
		if (currentTarget.GetComponent<BuildingBase> ().ReturnOwner () == playerInt) {
			TeleportToBuilding ();
		} else {
			switch (currentInhabitedBuildingType) {
			case BuildingType.Cannon:
				if (Vector3.Distance (currentTarget.transform.position, currentInhabitedBuilding.transform.position) < Cannon.towerAttackRange) {
					NetworkInstanceId tempTargeted = currentTarget.GetComponent<NetworkIdentity> ().netId;
					CmdChangeTarget (tempTargeted);
				} else {
					Debug.Log ("Out of Range");
				}
				break;
			}
		}
	}

	void CmdChangeTarget(NetworkInstanceId thisTargetIdentity) {
		currentInhabitedBuilding.GetComponent<Cannon> ().CmdOnChangeTarget (thisTargetIdentity);
	}

	void PressGUIButton() {
		
	}

	void TeleportToBuilding () {
		currentInhabitedBuilding = currentTarget.GetComponent<NetworkIdentity>();
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

	public void InitializePlayer(int thisPlayerInt) {
		playerInt = thisPlayerInt;
		transform.name = playerID;
		//TODO

		GetComponent<ConstructionController> ().BuildInitialBuilding ();

	}

	void InhabitClosestBuilding () {
		BuildingBase[] allBuildings = FindObjectsOfType<BuildingBase> ();
		foreach (BuildingBase x in allBuildings) {
			//assign current
			if (Vector3.Distance (x.transform.position, transform.position) < 100) {
				currentInhabitedBuilding = x.GetComponent<NetworkIdentity>();
				Debug.Log ("Init building from player " + playerID);
				currentInhabitedBuilding.GetComponent<BuildingBase> ().InitializeBuilding (playerInt);
				isInitialized = true;
			}
		}
	}

}
