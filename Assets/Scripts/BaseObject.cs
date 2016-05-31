using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BaseObject : NetworkBehaviour {

	public float cost;
	public float maxHealth = 50;
	[SyncVar] protected float currentHealth;
	public float ReturnCurrentHealth() {return currentHealth;}
	[SyncVar] protected float cooldown;
	public float ReturnCurrentCooldown() {return cooldown;}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
