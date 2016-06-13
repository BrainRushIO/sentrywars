using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public abstract class TargetingBase : NetworkBehaviour {

	[SyncVar] protected NetworkInstanceId currentTarget;
	protected bool isTargetFound, abilitiesActive;
	protected float buildingDetectionRange;
	protected int targetLayerMask;
	protected bool changeTarget;


	public void EnableAbilities() {
		abilitiesActive = true;
	}
	public void DisableAbilities () {
		abilitiesActive = false;	
	}

	public abstract void Start ();


	[Command]
	public abstract void CmdOnChangeTarget (NetworkInstanceId thisId);

	public void DetectEnemyBuildings () {
		Collider[] collidersInRange = Physics.OverlapSphere (transform.position, buildingDetectionRange, targetLayerMask);
		foreach (Collider x in collidersInRange) {
			if (x.GetComponent<BuildingBase> ().ReturnOwner () != GetComponent<BuildingBase> ().ReturnOwner ()) {
				currentTarget = x.GetComponent<NetworkIdentity>().netId;
				break;
			}
		}
	}
}
