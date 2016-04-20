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
			CmdDestroyBuilding ();
		}
	}

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
	void CmdDestroyBuilding () {
		GameObject curOwner = GameManager.players [owner].gameObject;
		switch (thisBuildingType) {
		case BuildingType.Energy:
			linkedEnergyField.GetComponent<EnergyField> ().CmdSetIsOccupied( false );
			curOwner.GetComponent<PlayerStats> ().DecreaseEnergyUptake ();
			break;
		}
//		if (curOwner.GetComponent<PlayerController> ().currentInhabitedBuilding.netId == GetComponent<NetworkIdentity>().netId) {
//			foreach (PlayerController x in GameManager.players) {
//				if (x.playerInt == owner) {
//					x.EndMatch (false);
//				} else {
//					x.EndMatch (true);
//				}
//			}
//		}
		Destroy (gameObject);
	}



}
