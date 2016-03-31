using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public enum BuildingType {Constructor, Canon, Defense, Shield, Energy, Tactical};

public class BuildingBase : NetworkBehaviour {

	float maxHealth = 50;

	[SyncVar] private float currentHealth;
	public float ReturnCurrentHealth(){return currentHealth;}
	[SyncVar] private float cooldown;
	public float ReturnCurrentCooldown() {return cooldown;}
	BuildingType currentBuildingType = BuildingType.Constructor;
	public float actionCooldown;
	public bool isOccupied;
	public GameObject parentNexus;
	public float cost;
	public float buildTime;
	public Transform playerCockpit;
	Collider[] allColliders;
	public BuildingType thisBuildingType;

	[SyncVar]
	string owner;
	public string ReturnOwner(){return owner;} 

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

	public virtual void InitializeBuilding(string thisOwner) {
		owner = thisOwner;
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
		switch (currentBuildingType) {
		case BuildingType.Energy:
			GameObject.Find (owner).GetComponent<PlayerStats> ().DecreaseEnergyUptake ();
			break;
		}
		Destroy (gameObject);
	}

}
