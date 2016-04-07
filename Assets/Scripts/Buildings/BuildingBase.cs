using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public enum BuildingType {Constructor, Canon, Energy};

public class BuildingBase : NetworkBehaviour {

	float maxHealth = 50;
	[SyncVar] private float currentHealth;
	public float ReturnCurrentHealth() {return currentHealth;}
	[SyncVar] private float cooldown;
	public float ReturnCurrentCooldown() {return cooldown;}
	public float actionCooldown;
	public bool isOccupied;
	bool haveColorsBeenSet = false;
	public GameObject parentNexus;
	public float cost;
	public float buildTime;
	public Transform playerCockpit;
	Collider[] allColliders;
	public BuildingType thisBuildingType;

	public bool ReturnHaveColorsBeenSet () {
		return abilitiesActive;
	}

	public MeshRenderer[] coloredMesh;
	[SyncVar] bool abilitiesActive = false;
	[SyncVar] Color thisBuildingColor = new Color();
	[SyncVar] bool colorHasChanged;
	[SyncVar] NetworkInstanceId towerNetID;


	void Update () {
		Debug.Log (connectionToServer);
		foreach (NetworkInstanceId x in connectionToClient.clientOwnedObjects) {
			Debug.Log (x);
		}
		if (abilitiesActive && !haveColorsBeenSet) {
			haveColorsBeenSet = true;
			towerNetID = GetComponent<NetworkIdentity> ().netId;
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
		EnableTowerAbilities ();
		CmdUpdateColor ();

	}


	void UpdateColor() {
		if (colorHasChanged) {
			CmdSwitchColor (true, owner);
			foreach (MeshRenderer x in GetComponent<BuildingBase>().coloredMesh) {
				x.material.SetColor ("_Color", thisBuildingColor); 
			}
			colorHasChanged = false;
		}
	}
		
	[Command]
	void CmdSwitchColor (bool isPowered, string owner) {
		thisBuildingColor = PlayerColorManager.GetBuildingColor(isPowered, owner);
	}

	[Command]
	void CmdUpdateColor() {
		colorHasChanged = true;
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

	public void EnableTowerAbilities() {
		abilitiesActive = true;
	}

	public void DisableTowerAbilities () {
		abilitiesActive = false;
	}
	
}