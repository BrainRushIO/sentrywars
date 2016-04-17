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

	[SyncVar]
	int owner;
	public int ReturnOwner(){return owner;} 

	void Awake () {
		allColliders = GetComponents<Collider> ();
		currentHealth = maxHealth;
	}

	[ClientRpc]
	public void RpcTakeDamage (float amount) {
		currentHealth -= amount;
		if (currentHealth < 0) {
			DestroyBuilding ();
		}
	}

	public void InitializeBuilding(int thisOwner) {
		owner = thisOwner;
//		towerNetID = gameObject.GetComponent<NetworkBehaviour> ().netId;
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

	void DestroyBuilding () {
		if (isServer) {
			GameObject curOwner = GameObject.Find ("Player" + (owner + 1).ToString ());
			switch (thisBuildingType) {
			case BuildingType.Energy:
				Collider[] energyPools = Physics.OverlapSphere (transform.position, 100, 11);
				foreach (Collider x in energyPools) {
					x.GetComponent<EnergyField> ().isOccupied = false;
				}
				curOwner.GetComponent<PlayerStats> ().RpcDecreaseEnergyUptake ();
				break;
			}
		}
//		if (curOwner.GetComponent<PlayerController> ().currentInhabitedBuilding == gameObject) {
//		}
		Destroy (gameObject);
	}



}
