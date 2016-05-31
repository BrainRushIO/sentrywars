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


	[SyncVar]
	protected int owner;
	public int ReturnOwner(){return owner;} 
}
