using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum TargetTypes {None, Building, VRUIObject, Floor, EnergyPool};


/// <summary>
/// Handle player movement through towers.
/// </summary>
public class PlayerController : NetworkBehaviour {

	enum PlayerMode {Inactive, Active, GameOver};
	PlayerMode curPlayerMode = PlayerMode.Active;

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
	[SerializeField] GameObject loseSphere, gameplayGui;

	public delegate void SendPlayerInputInfo(RaycastHit thisHit);
	public static event SendPlayerInputInfo OnSendPlayerInputInfo;
	RaycastHit currentRayCastHit;

	float buildCoolDown = 0, buildCooldownTimer;
	public float ReturnCooldownTimer() {
		return buildCooldownTimer;
	}
	public void SetCoolDown () {
		buildCooldownTimer = 0;
	}

	void OnEnable() {
		InputController.OnSendPointerInfo += HandleRightHandTargeting;
		InputController.OnRightTriggerFingerDown += HandleRightTriggerDown;
	}

	void OnDisable() {
		InputController.OnSendPointerInfo -= HandleRightHandTargeting;
		InputController.OnRightTriggerFingerDown -= HandleRightTriggerDown;
	}

	void Update() {
		switch (curPlayerMode) {
		case PlayerMode.Inactive:
			buildCooldownTimer += Time.deltaTime;
			if (buildCooldownTimer > buildCoolDown) {
				curPlayerMode = PlayerMode.Active;
				GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().cooldownOver);

			}
			break;
		case PlayerMode.Active:
			OnSendPlayerInputInfo (currentRayCastHit);
			break;
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			InitializePlayer (0);
			GameManager.gameHasStarted = true;
		}

		//For Beginning of Game
		if (!isInitialized) {
			InhabitClosestBuilding ();
		}
		if (currentInhabitedBuilding != null) {
			GetComponent<GUIManager>().currentHUD.thisBuildingHP.text = "This Tower's HP: " + currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnCurrentHealth ().ToString ("F0");
		}
	}
		
	void HandleRightHandTargeting(RaycastHit thisHit) {
		if (currentInhabitedBuilding == null) {
			return;
		}
		currentRayCastHit = thisHit;
		currentTarget = thisHit.collider.gameObject;
		if (currentTarget == currentInhabitedBuilding.gameObject) {
			GetComponent<ConstructionController> ().SwitchToInactive ();
			return;
		}

		switch(thisHit.transform.tag){
		case "Building":
			HandleSelectBuildingVFX ();
			currentTargetType = TargetTypes.Building;
			currentBuildingID = currentTarget.GetComponent<NetworkIdentity> ().netId;
			break;
		case "Floor":
			if (curPlayerMode == PlayerMode.Active) {
				currentTargetType = TargetTypes.Floor;
				GetComponent<ConstructionController> ().SwitchToTargetingFloor ();
				GetComponent<ConstructionController> ().isTargetingEnergyField = false;
			}
			break;
		case "Energy":
			if (!currentTarget.GetComponent<EnergyField> ().ReturnIsOccupied ()&&curPlayerMode == PlayerMode.Active) {
				currentTargetType = TargetTypes.Floor;
				GetComponent<ConstructionController> ().isTargetingEnergyField = true;
				GetComponent<ConstructionController> ().SwitchToTargetingFloor ();
			} 
			break;
		case "Wall":
			GetComponent<ConstructionController> ().SwitchToInactive ();
			break;

		default :
			currentTargetType = TargetTypes.None;
			break;
		}

	}

	void HandleSelectBuildingVFX () {
		if (currentTarget.GetComponent<BuildingBase>()!=null && otherBuildingSelectedIndicator == null && currentTarget!=currentInhabitedBuilding.gameObject) {
			otherBuildingSelectedIndicator = Instantiate (otherBuildingSelectedIndicatorPrefab, currentTarget.GetComponent<BuildingBase> ().playerCockpit.position, Quaternion.identity) as GameObject;
			GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().selectTarget);

		} else if (currentTargetType != TargetTypes.Building && otherBuildingSelectedIndicator != null) {
			Destroy (otherBuildingSelectedIndicator);
		}
	}
		
	void HandleRightTriggerDown() {
		switch (currentTargetType) {
		case TargetTypes.Building:
			if (curPlayerMode != PlayerMode.GameOver) {
				PerformActionOnTargetedBuilding ();
			}
			break;
		}
	}
	void PerformActionOnTargetedBuilding() {
		if (currentTarget.GetComponent<BuildingBase> ()!=null&&currentTarget.GetComponent<BuildingBase> ().ReturnOwner () == playerInt && currentTargetType!=TargetTypes.EnergyPool) {
			TeleportToBuilding ();
		} else {
			switch (currentInhabitedBuildingType) {
			case BuildingType.Cannon:
				if (Vector3.Distance (currentTarget.transform.position, currentInhabitedBuilding.transform.position) < Cannon.towerAttackRange) {
					NetworkInstanceId tempTargeted = currentTarget.GetComponent<NetworkIdentity> ().netId;
					GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().changeTarget);
					CmdChangeTarget (tempTargeted);
				} else {
					GetComponent<GUIManager> ().SetAlert ("Target Out of Range");
					GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().error);
				}
				break;
			}
		}
	}

	void CmdChangeTarget(NetworkInstanceId thisTargetIdentity) {
		currentInhabitedBuilding.GetComponent<TargetingBase> ().CmdOnChangeTarget (thisTargetIdentity);
	}

	void TeleportToBuilding () {
		if (currentTarget.GetComponent<BuildingStateController> ().damageState == 2) {
			GetComponent<GUIManager> ().SetAlert ("Cannot Teleport into Severely Damaged Tower");
			return;
		} else if (currentTarget.GetComponent<BuildingStateController> ().ReturnIsWarpingIn ()) {
			GetComponent<GUIManager> ().SetAlert ("Cannot Teleport into Incomplete Tower");
			return;
		}
			
		CmdSetIsOccupiedOnCurBuilding (currentInhabitedBuilding, currentTarget.GetComponent<NetworkIdentity> ());
		currentInhabitedBuilding = currentTarget.GetComponent<NetworkIdentity>();
		GameObject tempTeleportVFX = (GameObject)Instantiate (teleportPrefab, currentInhabitedBuilding.GetComponent<BuildingBase> ().playerCockpit.position, Quaternion.identity);
		Destroy (tempTeleportVFX, 4f);
		MovePlayerToBuildingCockpit ();
		GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().warp);
		currentInhabitedBuildingType = currentTarget.GetComponent<BuildingBase> ().thisBuildingType;
		if (currentInhabitedBuildingType != BuildingType.PowerCore) {
			GetComponent<ConstructionController> ().isInPowerCore = false;
		} else {
			GetComponent<ConstructionController> ().isInPowerCore = true;
		}
	}

	[Command] 
	void CmdSetIsOccupiedOnCurBuilding(NetworkIdentity curr, NetworkIdentity target){
		if (isServer) {
			if (curr != null) {
				NetworkServer.FindLocalObject (curr.netId).GetComponent<BuildingBase> ().RpcSetIsOccupied (false);
			}
			if (target != null) {
				NetworkServer.FindLocalObject (target.netId).GetComponent<BuildingBase> ().RpcSetIsOccupied (true);
			}
		}
	}

	void MovePlayerToBuildingCockpit() {
		transform.position = currentInhabitedBuilding.GetComponent<BuildingBase> ().playerCockpit.position;
		Destroy (otherBuildingSelectedIndicator);
	}

	public void InitializePlayer(int thisPlayerInt) {
		playerInt = thisPlayerInt;
		playerID = "Player" + thisPlayerInt.ToString ();
		transform.name = playerID;
		GetComponent<ConstructionController> ().BuildInitialPowerCore ();
	}

	public void EndGame() {
		curPlayerMode = PlayerMode.GameOver;
	}

	void InhabitClosestBuilding () {
		BuildingBase[] allBuildings = FindObjectsOfType<BuildingBase> ();
		foreach (BuildingBase x in allBuildings) {
			//assign current
			if (Vector3.Distance (x.transform.position, transform.position) < 100) {
				currentInhabitedBuilding = x.GetComponent<NetworkIdentity>();
				GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().constructBuilding);
				Debug.Log ("Init building from player " + playerID);
				currentInhabitedBuilding.GetComponent<BuildingBase> ().InitializeBuilding (playerInt,null, true);
				CmdSetIsOccupiedOnCurBuilding (null, currentInhabitedBuilding);
				isInitialized = true;
			}
		}
	}
}
