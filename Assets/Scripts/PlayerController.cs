using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum TargetTypes {None, Building, GUIButton, Floor, EnergyPool};


/// <summary>
/// Handle player movement through towers.
/// </summary>
public class PlayerController : NetworkBehaviour {

	enum PlayerMode {CoolDown, Active, GameOver};
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

	float buildCoolDown = 1.5f, buildCooldownTimer;
	public float ReturnCooldownTimer() {
		return buildCooldownTimer;
	}
	public void SetCoolDown () {
		buildCooldownTimer = 0;
		curPlayerMode = PlayerMode.CoolDown;
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
		case PlayerMode.CoolDown:
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
		if( GetComponent<InputController>().playInVR && SteamVR.active ) {
			if( GetComponent<InputController>().rightController.gripButtonDown ) {
				InitializePlayer (0);
				GameManager.gameHasStarted = true;
			}
		}else {
			if (Input.GetKeyDown (KeyCode.P)) {
				InitializePlayer (0);
				GameManager.gameHasStarted = true;
			}
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
			if (currentInhabitedBuilding.GetComponent<BuildingBase> ().ReturnIsBuildingActive()&&curPlayerMode == PlayerMode.Active) {
				GetComponent<ConstructionController> ().SwitchToPlacingBuilding ();
				currentTargetType = TargetTypes.Floor;
				GetComponent<ConstructionController> ().isTargetingEnergyField = false;
			}
			break;
		case "Energy":
			if (!currentTarget.GetComponent<EnergyField> ().ReturnIsOccupied ()&&curPlayerMode == PlayerMode.Active) {
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
		case TargetTypes.GUIButton:
			PressGUIButton ();
			break;
		}
	}
	void PerformActionOnTargetedBuilding() {
		if (currentTarget.GetComponent<BuildingBase> ().ReturnOwner () == playerInt && currentTargetType!=TargetTypes.EnergyPool) {
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
		currentInhabitedBuilding.GetComponent<Cannon> ().CmdOnChangeTarget (thisTargetIdentity);
	}
		

	void PressGUIButton() {
		
	}

	void TeleportToBuilding () {
		CmdSetIsOccupiedOnCurBuilding (currentInhabitedBuilding, currentTarget.GetComponent<NetworkIdentity> ());
		currentInhabitedBuilding = currentTarget.GetComponent<NetworkIdentity>();
		GameObject tempTeleportVFX = (GameObject)Instantiate (teleportPrefab, currentInhabitedBuilding.GetComponent<BuildingBase> ().playerCockpit.position, Quaternion.identity);
		Destroy (tempTeleportVFX, 4f);
		MovePlayerToBuildingCockpit ();
		GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().warp);
		currentInhabitedBuildingType = currentTarget.GetComponent<BuildingBase> ().thisBuildingType;
		if (currentInhabitedBuildingType != BuildingType.Constructor) {
			GetComponent<ConstructionController> ().isInConstructor = false;
		} else {
			GetComponent<ConstructionController> ().isInConstructor = true;
		}
	}

	[Command] 
	void CmdSetIsOccupiedOnCurBuilding(NetworkIdentity curr, NetworkIdentity target){
		if (isServer) {
			NetworkServer.FindLocalObject(curr.netId).GetComponent<BuildingBase> ().RpcSetIsOccupied (false);
			NetworkServer.FindLocalObject(target.netId).GetComponent<BuildingBase> ().RpcSetIsOccupied (true);
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
		//TODO

		GetComponent<ConstructionController> ().BuildInitialBuilding ();

	}
		
	[Command]
	public void CmdPlayerLose() {
		if (isServer) {
			RpcPlayerLose ();
		}
	}

	[ClientRpc]
	void RpcPlayerLose() {
		if (isLocalPlayer && curPlayerMode == PlayerMode.Active) {
			GetComponent<GUIManager> ().currentHUD.endMatch.text = "Defeat";
			loseSphere.SetActive (true);
			GetComponent<ConstructionController> ().enabled = false;
			GetComponent<InputController> ().enabled = false;
			gameplayGui.SetActive (false);
			curPlayerMode = PlayerMode.GameOver;
			GameObject.Find ("Soundtrack").SetActive (false);
			GameObject.Find ("loseSound").GetComponent<AudioSource> ().Play ();

		}
	}

	[Command]
	public void CmdPlayerWin () {
		if (isServer) {
			RpcPlayerWin ();
		}
	}

	[ClientRpc]
	void RpcPlayerWin () {
		if (isLocalPlayer && curPlayerMode == PlayerMode.Active) {
			GetComponent<ConstructionController> ().enabled = false;
			GetComponent<InputController> ().enabled = false;
			GetComponent<GUIManager> ().currentHUD.endMatch.text = "Victory";
			gameplayGui.SetActive (false);
			curPlayerMode = PlayerMode.GameOver;
			GameObject.Find ("Soundtrack").SetActive (false);
			GameObject.Find ("winSound").GetComponent<AudioSource> ().Play ();


		}
	}

	[Command]
	public void CmdPlayerHit () {
		if (isServer) {
			RpcPlayerHit ();
		}
	}

	[ClientRpc]
	void RpcPlayerHit () {
		if (isLocalPlayer) {
			StartCoroutine ("FlashPlayerScreenRed");
		}
	}
	IEnumerator FlashPlayerScreenRed() {
		loseSphere.SetActive (true);
		GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().playerHit);
		yield return new WaitForSeconds (.12f);
		loseSphere.SetActive (false);
	}

	void InhabitClosestBuilding () {
		BuildingBase[] allBuildings = FindObjectsOfType<BuildingBase> ();
		foreach (BuildingBase x in allBuildings) {
			//assign current
			if (Vector3.Distance (x.transform.position, transform.position) < 100) {
				currentInhabitedBuilding = x.GetComponent<NetworkIdentity>();
				GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().constructBuilding);
				Debug.Log ("Init building from player " + playerID);
				currentInhabitedBuilding.GetComponent<BuildingBase> ().InitializeBuilding (playerInt);
				isInitialized = true;
			}
		}
	}
}
