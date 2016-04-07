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
	bool abilitiesActive = false, haveColorsBeenSet = false;
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

	[SyncVar] public Color thisBuildingColor = new Color();

	[SyncVar] NetworkInstanceId towerNetID;

	public void EnableTowerAbilities() {
		abilitiesActive = true;
	}

	public void DisableTowerAbilities () {
		abilitiesActive = false;

	}

	void GetBuildingColor (bool isPowered) {
		if (isPowered) {
			switch (GameManager.players.IndexOf(owner)) {
			case 0:
				thisBuildingColor = Color.red;

				break;
			case 1:
				thisBuildingColor = Color.blue;

				break;
			
			}
		} else {
			
			switch (GameManager.players.IndexOf(owner)) {
			case 0:
				thisBuildingColor = new Color(0.5f,0f,0f);
				break;
			case 1:
				thisBuildingColor = new Color(0f,0f,.5f);
				break;
			
			}
		}
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
			GetBuildingColor (true);
			haveColorsBeenSet = true;
			towerNetID = GetComponent<NetworkIdentity> ().netId;
			CmdSetColor (towerNetID, thisBuildingColor);
		}
//		} else if (!abilitiesActive && haveColorsBeenSet) {
//			GetBuildingColor (true);
//			CmdSetColor (towerNetID, thisBuildingsColor);
//			haveColorsBeenSet = false;
//		}
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
//		towerNetID = gameObject.GetComponent<NetworkBehaviour> ().netId;
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
	void CmdSwitchColor (NetworkInstanceId thisGO, Color col) {
		Debug.Log (NetworkServer.FindLocalObject (thisGO));
		foreach (MeshRenderer x in NetworkServer.FindLocalObject(thisGO).GetComponent<BuildingBase>().coloredMesh) {
			x.material.SetColor ("_Color", col);
			 
		}
	}

	[Command]
	public void CmdSetColor(NetworkInstanceId GOID, Color thisColor) {
//		if (GetComponent<NetworkBehaviour> ().hasAuthority) {
//			CmdTempSwitchColor (GOID, thisColor);
//			Debug.LogWarning (owner + " blah");
//		} else {
			Debug.Log ("ELSE CMD ASSIGN" + owner);
			//gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority (GameObject.Find(owner).GetComponent<NetworkIdentity>().connectionToClient);
			CmdSwitchColor (GOID, thisColor);
			//gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority (GameObject.Find(owner).GetComponent<NetworkIdentity>().connectionToClient);

		//}
	}

}
