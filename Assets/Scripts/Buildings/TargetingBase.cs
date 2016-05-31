using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TargetingBase : NetworkBehaviour {

	[SyncVar] protected NetworkInstanceId currentTarget;
	protected bool isTargetFound, abilitiesActive;
	protected float buildingDetectionRange;
	public int buildingLayerMask;
	protected bool changeTarget;


	public void EnableAbilities() {
		abilitiesActive = true;
	}
	public void DisableAbilities () {
		abilitiesActive = false;	
	}

	void Start () {
		buildingLayerMask = 1 << LayerMask.NameToLayer ("Buildings");
	}


	[Command]
	public void CmdOnChangeTarget(NetworkInstanceId thisId) {
		currentTarget = thisId;
	}

	public void DetectEnemyBuildings () {
		Collider[] collidersInRange = Physics.OverlapSphere (transform.position, buildingDetectionRange, buildingLayerMask);
		foreach (Collider x in collidersInRange) {
			if (x.GetComponent<BuildingBase> ().ReturnOwner () != GetComponent<BuildingBase> ().ReturnOwner ()) {
				currentTarget = x.GetComponent<NetworkIdentity>().netId;
				break;
			}
		}
	}
}
