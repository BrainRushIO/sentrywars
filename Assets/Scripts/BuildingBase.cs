using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public enum BuildingType {Constructor, Canon, Defense, Shield, Energy, Tactical};

public class BuildingBase : NetworkBehaviour {

	public float maxHealth;

	[SyncVar] private float currentHealth;

	BuildingType currentBuildingType = BuildingType.Constructor;
	public float actionCooldown;
	public bool isOccupied;
	public GameObject parentNexus;
	public float cost;
	public float buildTime;
	public Transform playerCockpit;
	[SerializeField] Collider[] allColliders;

	[SerializeField]
	string owner;
	public string ReturnOwner(){return owner;} 


	// Update is called once per frame

	public virtual void Die(){

	} 

	[ClientRpc]
	public void RpcTakeDamage (float amount) {

	}

	public virtual void InitializeBuilding(string thisOwner) {
		owner = thisOwner;
		EnableAllColliders ();
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

}
