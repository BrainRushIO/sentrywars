using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public abstract class TargetingBase : NetworkBehaviour {

	[SyncVar] protected NetworkInstanceId currentTargetID;
	protected bool isTargetFound, abilitiesActive;
	protected float buildingDetectionRange = 200f;
	protected int targetLayerMask;
	protected bool changeTarget;
	protected GameObject currentTargetGO;

	public void OnWarpInComplete () {
		print ("abilities active");
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
		print (collidersInRange.Length + " big length");
		foreach (Collider x in collidersInRange) {
			if (x.GetComponent<BuildingBase> ().ReturnOwner () != GetComponent<BuildingBase> ().ReturnOwner ()) {
				print ("found enemy");

				currentTargetID = x.GetComponent<NetworkIdentity>().netId;
				currentTargetGO = NetworkServer.FindLocalObject (currentTargetID);
				print ("found bldgs");

				break;
			}
		}
	}
}
