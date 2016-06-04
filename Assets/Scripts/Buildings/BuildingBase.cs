using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public enum BuildingType {PowerCore, Cannon, Energy, Airport, SnipeCannon};

public class BuildingBase : BaseObject {

	public float actionCooldown;
	public float buildTime;
	bool isWarpingIn;
	public Transform playerCockpit;
	Collider[] allColliders;
	public BuildingType thisBuildingType;
	[SyncVar] bool hasBeenDestroyed;
	[SyncVar] NetworkIdentity linkedEnergyField;
	public GameObject rangeRing;

	[SerializeField][SyncVar] bool isOccupied = false;
	[ClientRpc]
	public void RpcSetIsOccupied (bool val) {
		isOccupied = val;
		if (rangeRing != null ) {
			rangeRing.SetActive (val);
		}
	}
	[SyncVar] bool isFirstBuilding = false;
	[ClientRpc]
	public void RpcSetIsInitBuilding (bool val) {
		isFirstBuilding = val;
	}

	public bool ReturnIsOccupied() {
		return isOccupied;
	}

	public bool ReturnIsInitBuilding() {
		return isFirstBuilding;
	}

	/// <summary>
	/// The colored mesh that switches from player to player.
	/// </summary>

	void Start () {
		CheckIfIsPowered ();
	}

	public void EnableAbilities() {
		if (isServer) {
			GetComponent<BuildingStateController> ().SetMeshRendererColor ();
		}
	}

	public void DisableAbilities () {
		if (isServer) {
			GetComponent<BuildingStateController> ().SetMeshRendererColor ();
		}
	}

	void CheckIfIsPowered() {
		Collider[] nearbyBuildings = Physics.OverlapSphere (transform.position, ConstructionController.CONSTRUCTION_RANGE);
		int totalConstructors = 0;
		foreach (Collider x in nearbyBuildings) {
			if (x.GetComponent<BuildingBase> ()!=null && x.GetComponent<BuildingBase> ().thisBuildingType == BuildingType.PowerCore) {
				totalConstructors++;
			}
		}
		if (totalConstructors > 0) {
			SendMessage ("EnableAbilities");
		} else {
			SendMessage ("DisableAbilities");
		}
	}


	void Awake () {
		allColliders = GetComponents<Collider> ();
		currentHealth = maxHealth;
	}

	public void TakeDamage (float amount) {
		currentHealth -= amount;
		if (isServer) SetDamageState (currentHealth / maxHealth);

		if (currentHealth < 1 && !hasBeenDestroyed) {
			hasBeenDestroyed = true;
			CmdDestroyBuilding (GameManager.players [owner].netId);
			if (isOccupied || isFirstBuilding) {
				NetworkServer.FindLocalObject (GameManager.players [owner].netId).GetComponent<PlayerGameStateHandler> ().CmdPlayerLose ();
				if (owner == 1) {
					NetworkServer.FindLocalObject (GameManager.players [0].netId).GetComponent<PlayerGameStateHandler> ().CmdPlayerWin ();
				} else {
					NetworkServer.FindLocalObject (GameManager.players [1].netId).GetComponent<PlayerGameStateHandler> ().CmdPlayerWin ();
				}

			}
		} else if (isOccupied || isFirstBuilding) {
			NetworkServer.FindLocalObject (GameManager.players [owner].netId).GetComponent<PlayerGameStateHandler> ().CmdPlayerHit();
		}
	}

	void SetDamageState(float val) {
		if (val > .65f && val < 1f) {
			GetComponent<BuildingStateController> ().RpcSetDamageState (0);
		} else if (val > .4f) {
			GetComponent<BuildingStateController> ().RpcSetDamageState (1);
		} else if (val > .1f) {
			GetComponent<BuildingStateController> ().RpcSetDamageState (2);
		}
	}

	public void InitializeBuilding(int thisOwner, NetworkIdentity thisLinkedEnergyField = null, bool isFirst = false) {
		owner = thisOwner;
		if (thisLinkedEnergyField != null) {
			linkedEnergyField = thisLinkedEnergyField;
		}
		EnableAllColliders ();
		DisableMeshRenderers ();
		currentHealth = maxHealth;
		if (isFirst) {
			isFirstBuilding = true;
		}
	}

	public void DisableMeshRenderers () {
		if (GetComponent<MeshRenderer> () != null) {
			GetComponent<MeshRenderer> ().enabled = false;
		}
		foreach (MeshRenderer x in GetComponentsInChildren<MeshRenderer>()) {
			x.enabled = false;
		}
	}
	public void EnableMeshRenderers () {
		if (GetComponent<MeshRenderer> () != null) {
			GetComponent<MeshRenderer> ().enabled = true;
		}
		foreach (MeshRenderer x in GetComponentsInChildren<MeshRenderer>()) {
			x.enabled = true;
		}
	}


	public void DisableAllColliders() {
		SendMessage ("ShowRangeRing", true, SendMessageOptions.DontRequireReceiver);
		foreach (Collider x in allColliders) {
			x.enabled = false;
		}
	}

	public void EnableAllColliders () {
		SendMessage ("ShowRangeRing", false, SendMessageOptions.DontRequireReceiver);

		foreach (Collider x in allColliders) {
			x.enabled = true;
		}
	}

	[Command]
	void CmdDestroyBuilding (NetworkInstanceId thisOwnerId) {
		switch (thisBuildingType) {
		case BuildingType.Energy:
			linkedEnergyField.GetComponent<EnergyField> ().CmdSetIsOccupied(false);
			NetworkServer.FindLocalObject(thisOwnerId).GetComponent<PlayerStats> ().CmdDecreaseEnergyUptake ();
			break;
		}
		GameObject temp = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs[5], transform.position + Vector3.down * -20, Quaternion.identity);
		Destroy (temp, 5f);
		NetworkServer.Spawn (temp);
		Destroy (gameObject);
	}

	void OnWarpInComplete() {
	}
}
