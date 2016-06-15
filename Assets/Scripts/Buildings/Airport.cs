using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Airport : TargetingBase {

	public GameObject dronePrefab;
	[SyncVar] NetworkIdentity droneInstance;
	public Transform spawnPos;
	bool isDroneBuilt;

	// Update is called once per frame
	void Update () {
		//has drone been destroyed
		//if so, build a new one
		if (isServer && isDroneBuilt) {
			RebuildDrone ();
		}

	}

	public override void Start ()
	{
	}


	void OnWarpInComplete () {
		CmdSpawnDrone (GetComponent<BuildingBase>().ReturnOwner(), GetComponent<NetworkIdentity>().netId);
	}

	public bool RebuildDrone() {
		if (droneInstance == null) {
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
		GameObject tempSplash = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs[7],
			spawnPos.position, Quaternion.identity);
		NetworkServer.Spawn (tempSplash);
		droneInstance = tempDrone.GetComponent<NetworkIdentity> ();
		tempDrone.GetComponent<Drone> ().InitializeUnit (GetComponent<BuildingBase> ().ReturnOwner (), thisBuildingId);
		isDroneBuilt = true;
	}

	[Command]
	public override void CmdOnChangeTarget(NetworkInstanceId thisId) {
		droneInstance.GetComponent<Drone> ().CmdSetCurrentTarget (thisId);
	}

	void OnBuildingDeath() {
		droneInstance.gameObject.GetComponent<Drone> ().TakeDamage (10000000f);
	}
}
