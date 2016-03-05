﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public enum BuildingType {Nexus, Cannon, Shield, Energy, MissileLauncher, MissileDefense};

public class BuildingBase : NetworkBehaviour {

	public float maxHealth;

	[SyncVar] public float currentHealth;

	public float actionCooldown;
	public bool isOccupied;
	public GameObject parentNexus;
	public float cost;
	public float buildTime;
	public Transform playerCockpit;

	[SerializeField]
	string owner;
	public string ReturnOwner(){return owner;} 
	public void SetOwner(string thisOwner){owner = thisOwner;}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame

	public virtual void Die(){

	} 

	void Update () {

		if (currentHealth <= 0) {
			Die ();
		}
	}

	public virtual void InitializeConstruction() {

	}

}
