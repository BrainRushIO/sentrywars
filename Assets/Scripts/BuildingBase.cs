﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;


/// <summary>
/// DEV MUST ASSIGN COLLIDERS IN INSPECTOR SO THEY CAN BE DEACTIVATED DURING PLACEMENT PROCESS
/// </summary>

public enum BuildingType {Constructor, Canon, Defense, Shield, Energy, Tactical};

public class BuildingBase : NetworkBehaviour {

	public float maxHealth = 50;

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
	}

	public virtual void Die(){

	} 

	[ClientRpc]
	public void RpcTakeDamage (float amount) {

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

}
