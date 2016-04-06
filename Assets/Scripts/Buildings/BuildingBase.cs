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
	bool abilitiesActive = false, haveColorsBeenSet;
	public bool ReturnHaveColorsBeenSet () {
		return abilitiesActive;
	}
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


	NetworkIdentity objNetId;

	public void EnableTowerAbilities() {
		abilitiesActive = true;
	}

	public void DisableTowerAbilities () {
		abilitiesActive = false;

	}

	Color GetBuildingColor (bool isPowered) {
		Color temp = new Color();
		if (isPowered) {
			switch (GameManager.players.IndexOf(owner)) {
			case 0:
				temp = Color.red;

				break;
			case 1:
				temp = Color.blue;

				break;
			
			}
		} else {
			switch (GameManager.players.IndexOf(owner)) {
			case 0:
				temp = new Color(0.5f,0f,0f);
				break;
			case 1:
				temp = new Color(0f,0f,.5f);
				break;
			
			}
		}
		return temp;
	}

	void CheckIfIsEnabled() {
		Collider[] nearbyBuildings = Physics.OverlapSphere (transform.position, ConstructionController.CONSTRUCTION_RANGE);
		int totalConstructors = 0;
		foreach (Collider x in nearbyBuildings) {
			if (x.GetComponent<BuildingBase> ().thisBuildingType == BuildingType.Constructor) {
				totalConstructors++;
			}
		}
		if (totalConstructors > 0) {
			SendMessage ("EnableTowerAbilities");
		} else {
			SendMessage ("DisableTowerAbilities");
		}



	}

	void Update () {
		if (abilitiesActive && !haveColorsBeenSet) {
			Color temp = GetBuildingColor (true);
			CmdSetColor (gameObject, temp);
			haveColorsBeenSet = true;
		} else if (!abilitiesActive && haveColorsBeenSet) {
			Color temp = GetBuildingColor (true);
			CmdSetColor (gameObject, temp);
			haveColorsBeenSet = false;
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


	[Command]
	public void CmdTempSwitchColor (GameObject thisGO, Color col) {
		
		foreach (MeshRenderer x in thisGO.GetComponent<BuildingBase>().coloredMesh) {
			x.material.SetColor ("_Color", col);
			 
		}
	}

	[Command]
	void CmdSetColor(GameObject GOID, Color thisColor) {
		objNetId = GOID.GetComponent<NetworkIdentity> (); 
//		objNetId.AssignClientAuthority (connectionToClient);
		CmdTempSwitchColor (GOID, thisColor);
//		objNetId.RemoveClientAuthority (connectionToClient);
	}

}
