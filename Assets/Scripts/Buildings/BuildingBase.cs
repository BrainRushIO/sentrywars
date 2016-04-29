using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public enum BuildingType {Constructor, Cannon, Energy};

public class BuildingBase : NetworkBehaviour {

	float maxHealth = 50;
	[SyncVar] private float currentHealth;
	public float ReturnCurrentHealth() {return currentHealth;}
	[SyncVar] private float cooldown;
	public float ReturnCurrentCooldown() {return cooldown;}
	public float actionCooldown;
	public float cost;
	public float buildTime;
	public Transform playerCockpit;
	Collider[] allColliders;
	public BuildingType thisBuildingType;
	[SyncVar] bool hasBeenDestroyed;
	[SyncVar] NetworkIdentity linkedEnergyField;

	[SyncVar] bool isOccupied = false;
	[ClientRpc]
	public void RpcSetIsOccupied (bool val) {
		isOccupied = val;
	}

	public bool ReturnIsOccupied() {
		return isOccupied;
	}

	/// <summary>
	/// The colored mesh that switches from player to player.
	/// </summary>




	void Start () {
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

	[SyncVar]
	int owner;
	public int ReturnOwner(){return owner;} 

	void Awake () {
		allColliders = GetComponents<Collider> ();
		currentHealth = maxHealth;
	}

	public void TakeDamage (float amount) {
		currentHealth -= amount;
//		CmdSetDamageState (currentHealth / maxHealth);

		if (currentHealth < 1 && !hasBeenDestroyed) {
			hasBeenDestroyed = true;
			CmdDestroyBuilding (GameManager.players [owner].netId);
			if (isOccupied) {
				NetworkServer.FindLocalObject (GameManager.players [owner].netId).GetComponent<PlayerController> ().CmdPlayerLose ();
				if (owner == 1) {
					NetworkServer.FindLocalObject (GameManager.players [0].netId).GetComponent<PlayerController> ().CmdPlayerWin ();
				} else {
					NetworkServer.FindLocalObject (GameManager.players [1].netId).GetComponent<PlayerController> ().CmdPlayerWin ();
				}

			}
		} else if (isOccupied) {
			NetworkServer.FindLocalObject (GameManager.players [owner].netId).GetComponent<PlayerController> ().CmdPlayerHit();
		}
	}
//	[Command]
//	void CmdSetDamageState(float val) {
//		if (val > .65f && val < 1f) {
//			GetComponent<BuildingStateController> ().CmdSetDamageState (0);
//		} else if (val > .4f) {
//			GetComponent<BuildingStateController> ().CmdSetDamageState (1);
//		} else if (val > .1f) {
//			GetComponent<BuildingStateController> ().CmdSetDamageState (2);
//		}
//	}

	public void InitializeBuilding(int thisOwner, NetworkIdentity thisLinkedEnergyField = null) {
		owner = thisOwner;
		if (thisLinkedEnergyField != null) {
			linkedEnergyField = thisLinkedEnergyField;
		}
		EnableAllColliders ();
		currentHealth = maxHealth;
	}


	public void DisableAllColliders() {
		SendMessage ("ShowRangeRing", true, SendMessageOptions.DontRequireReceiver);
		foreach (Collider x in allColliders) {
			x.enabled = false;
		}
	}

	public void EnableAllColliders () {
		SendMessage ("ShowRangeRing", false, SendMessageOptions.DontRequireReceiver);

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
		GameObject temp = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs[5], transform.position + Vector3.down * -20, Quaternion.identity);
		Destroy (temp, 5f);
		NetworkServer.Spawn (temp);
		Destroy (gameObject);
	}
}
