using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public enum BuildingType {Constructor, Cannon, Energy};

public class BuildingBase : NetworkBehaviour {

	float maxHealth = 10;
	[SyncVar] private float currentHealth;
	public float ReturnCurrentHealth() {return currentHealth;}
	[SyncVar] private float cooldown;
	public float ReturnCurrentCooldown() {return cooldown;}
	public float actionCooldown;
	public bool isOccupied;
	public float cost;
	public float buildTime;
	public Transform playerCockpit;
	Collider[] allColliders;
	public BuildingType thisBuildingType;
	[SyncVar] bool hasBeenDestroyed;
	NetworkIdentity linkedEnergyField;
	/// <summary>
	/// The colored mesh that switches from player to player.
	/// </summary>


	void Start() {
		CheckIfIsPowered ();
	}

	[SyncVar] bool abilitiesActive = false;
	public bool ReturnIsBuildingActive() {
		return abilitiesActive;
	}
	public void EnableTowerAbilities() {
		if (isServer) {
			abilitiesActive = true;
			GetComponent<BuildingStateController> ().SetMeshRendererColor (abilitiesActive);
		}
	}

	public void DisableTowerAbilities () {
		if (isServer) {
			abilitiesActive = false;
			GetComponent<BuildingStateController> ().SetMeshRendererColor (abilitiesActive);
		}

	}

	void Update() {
	}

	void CheckIfIsPowered() {
		Collider[] nearbyBuildings = Physics.OverlapSphere (transform.position, ConstructionController.CONSTRUCTION_RANGE);
		int totalConstructors = 0;
		foreach (Collider x in nearbyBuildings) {
			if (x.GetComponent<BuildingBase> ()!=null && x.GetComponent<BuildingBase> ().thisBuildingType == BuildingType.Constructor) {
				totalConstructors++;
			}
		}
		if (totalConstructors > 0) {
			SendMessage ("EnableTowerAbilities");
		} else {
			SendMessage ("DisableTowerAbilities");
		}
	}

	[SyncVar][SerializeField]
	int owner;
	public int ReturnOwner(){return owner;} 

	void Awake () {
		allColliders = GetComponents<Collider> ();
		currentHealth = maxHealth;
	}

	public void TakeDamage (float amount) {
//		if (isServer) TODO
		currentHealth -= amount;
		if (currentHealth < 0 && !hasBeenDestroyed) {
			hasBeenDestroyed = true;

			//GhettoFix for Win state on player
			Collider[] nearbyBuildings = Physics.OverlapSphere (transform.position, 1000000f);
			foreach (Collider x in nearbyBuildings) {
				if (x.GetComponent<BuildingBase>() != null) {
					if (x.GetComponent<BuildingBase> ().isOccupied && x.GetComponent<BuildingBase> ().owner != owner) {
						CmdWinGameForBuildingOwner(GameManager.players [x.GetComponent<BuildingBase> ().owner].netId);
					}
				}
			}
//			if (isOccupied) {
//				RpcSendEndMatchMessages(GameManager.players [owner].netId);
//				CmdSendEndMatchMessages(GameManager.players [owner].netId);
//			}
		}

	}
//	[Command]
//	void CmdSendEndMatchMessages(NetworkInstanceId thisNetId) {
//		foreach (NetworkConnection c in NetworkServer.connections) {
//			foreach (NetworkInstanceId x in c.clientOwnedObjects) {
//				NetworkServer.FindLocalObject (x).SendMessage ("CmdPlayerWin", thisNetId, SendMessageOptions.DontRequireReceiver);
//			}
//		}
//	}
//	[ClientRpc]
//	void RpcSendEndMatchMessages(NetworkInstanceId thisNetId) {
//		foreach (NetworkConnection c in NetworkServer.connections) {
//			foreach (NetworkInstanceId x in c.clientOwnedObjects) {
//				NetworkServer.FindLocalObject (x).SendMessage ("CmdPlayerWin", thisNetId, SendMessageOptions.DontRequireReceiver);
//			}
//		}
//	}

	public void InitializeBuilding(int thisOwner, NetworkIdentity thisLinkedEnergyField = null) {
		owner = thisOwner;
//		towerNetID = gameObject.GetComponent<NetworkBehaviour> ().netId;
		if (thisLinkedEnergyField != null) {
			linkedEnergyField = thisLinkedEnergyField;
		}
		EnableAllColliders ();
		currentHealth = maxHealth;
	}


	public void DisableAllColliders() {
		foreach (Collider x in allColliders) {
			x.enabled = false;
		}
	}

	public void EnableAllColliders () {
		foreach (Collider x in allColliders) {
			x.enabled = true;
		}
	}

	[Command]
	void CmdDestroyBuilding (NetworkInstanceId thisOwnerId) {
		switch (thisBuildingType) {
		case BuildingType.Energy:
			linkedEnergyField.GetComponent<EnergyField> ().CmdSetIsOccupied(false);
			NetworkServer.FindLocalObject(thisOwnerId).GetComponent<PlayerStats> ().CmdDecreaseEnergyUptake ();
			break;
		}
		NetworkServer.FindLocalObject(thisOwnerId).GetComponent<PlayerController> ().CmdCheckIfPlayerDeath(GetComponent<NetworkIdentity>().netId);
		Destroy (gameObject);
	}

	[Command]
	public void CmdWinGameForBuildingOwner (NetworkInstanceId thisOwnerId) {
		NetworkServer.FindLocalObject(thisOwnerId).GetComponent<PlayerController> ().CmdPlayerWin();

	}
}
