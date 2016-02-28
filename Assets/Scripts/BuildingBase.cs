using UnityEngine;
using System.Collections;

public enum BuildingType {Nexus, Cannon, Shield, Energy, MissileLauncher, MissileDefense};

public class BuildingBase : MonoBehaviour {

	public float maxHealth;
	public float currentHealth;
	public float actionCooldown;
	public bool isOccupied;
	public GameObject parentNexus;
	public float cost;
	public float buildTime;
	public Transform playerCockpit;

	int owner;
	public int ReturnOwner(){return owner;} 
	public void SetOwner(int thisOwner){owner = thisOwner;}

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
