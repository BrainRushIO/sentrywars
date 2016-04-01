using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public enum BuildingType {Constructor, Canon, Defense, Shield, Energy, Tactical};

public class BuildingBase : NetworkBehaviour {

	float maxHealth = 50;

	[SyncVar] private float currentHealth;
	public float ReturnCurrentHealth(){return currentHealth;}
	[SyncVar] private float cooldown;
	public float ReturnCurrentCooldown() {return cooldown;}
	public float actionCooldown;
	public bool isOccupied;
	bool abilitiesActive, haveColorsBeenSet;
	public GameObject parentNexus;
	public float cost;
	public float buildTime;
	public Transform playerCockpit;
	Collider[] allColliders;
	public BuildingType thisBuildingType;
	/// <summary>
	/// The colored mesh that switches from player to player.
	/// </summary>
	public MeshRenderer[] coloredMesh;
	Material coloredMaterial;
	[SyncVar] Color syncBuildingColor;


	NetworkIdentity objNetId;

	public void EnableTowerAbilities() {
		Debug.Log ("BUILDIG BASE ENABLED");
		abilitiesActive = true;	
	}
	void Update () {
		if (abilitiesActive&&!haveColorsBeenSet) {
			if (owner == GameManager.players.Keys.ElementAt(0)) {
				Debug.Log ("Changed Color " + thisBuildingType);
				CmdSetColor (gameObject, Color.red);
			}
			haveColorsBeenSet = true;
		}
	}

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

	public void InitializeBuilding(string thisOwner) {
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
		GameObject curOwner = GameObject.Find (owner);

		switch (thisBuildingType) {
		case BuildingType.Energy:
			curOwner.GetComponent<PlayerStats> ().DecreaseEnergyUptake ();
			break;
		}
		if (curOwner.GetComponent<PlayerController> ().currentInhabitedBuilding == gameObject) {
		}
		Destroy (gameObject);
	}


	[ClientRpc]
	public void RpcTempSwitchColor (GameObject thisGO, Color col) {
		
		foreach (MeshRenderer x in thisGO.GetComponent<BuildingBase>().coloredMesh) {
			x.material.SetColor ("_Color", col);
			print ("changed color");
			 
		}
	}

	[Command]
	void CmdSetColor(GameObject GOID, Color thisColor) {
		objNetId = GOID.GetComponent<NetworkIdentity> (); 
//		objNetId.AssignClientAuthority (connectionToClient);
		RpcTempSwitchColor (GOID, thisColor);
//		objNetId.RemoveClientAuthority (connectionToClient);
	}

}
