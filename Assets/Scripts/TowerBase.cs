using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TowerType {Nexus, Cannon, Shield, Energy};

public class TowerBase : MonoBehaviour {

	public float maxHealth;
	public float currentHealth;
	public float actionCooldown;
	public bool isOccupied;
	public GameObject parentTower;
	public int owner;
	public float cost;
	public float buildTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame

	public virtual void Die(){
	//do death things here

	} 

	void Update () {

		if (currentHealth <= 0) {
			Die ();
		}
	}

	public virtual void InitializeConstruction() {

	}
}
