using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerBase : MonoBehaviour {

	public float maxHealth;
	public float curHealth;
	public float cooldown;
	public bool isOccupied;
	public GameObject parentTower;
	public int owner;
	public float cost;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame

	public virtual void Die(){
		//do death things here
	} 

	void Update () {

		if (curHealth == 0) {
			Die ();
		}
	}
}
