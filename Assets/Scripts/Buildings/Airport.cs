﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Airport : TargetingBase {

	public GameObject dronePrefab;
	[SyncVar] NetworkIdentity droneInstance;
	public Transform spawnPos;


	// Update is called once per frame
	void Update () {
		//has drone been destroyed
		//if so, build a new one
	}

	public override void Start ()
	{
		//dont do shit again
	}


	void OnWarpInComplete () {
		CmdSpawnDrone (GetComponent<BuildingBase>().ReturnOwner(), GetComponent<NetworkIdentity>().netId);
	}

	public bool RebuildDrone() {
		if (droneInstance.gameObject == null) {
			CmdSpawnDrone (GetComponent<BuildingBase> ().ReturnOwner (), GetComponent<NetworkIdentity> ().netId);
			return true;
		} else {
			return false;
		}
	}

	public void ReturnDrone() {
		CmdReturnDrone ();
	}

	[Command]
	public void CmdReturnDrone() {
		droneInstance.GetComponent<Drone> ().CmdSetReturnHome ();
	}

	[Command]
	void CmdSpawnDrone (int thisOwner, NetworkInstanceId thisBuildingId) {
		GameObject tempDrone = (GameObject)Instantiate (dronePrefab, 
			spawnPos.position, Quaternion.identity);
		NetworkServer.Spawn (tempDrone);
		droneInstance = tempDrone.GetComponent<NetworkIdentity> ();
		tempDrone.GetComponent<Drone> ().InitializeUnit (GetComponent<BuildingBase> ().ReturnOwner (), thisBuildingId);

	}

	[Command]
	public override void CmdOnChangeTarget(NetworkInstanceId thisId) {
		droneInstance.GetComponent<Drone> ().CmdSetCurrentTarget (thisId);
	}

	void OnDestroy() {
		Destroy (droneInstance);
	}
}
