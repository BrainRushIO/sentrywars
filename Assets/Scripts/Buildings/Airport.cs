using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Airport : TargetingBase {

	public GameObject dronePrefab;
	[SyncVar] NetworkIdentity droneInstance;
	public Transform spawnPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//has drone been destroyed
		//if so, build a new one
	}

	void OnWarpInComplete () {
		CmdSpawnDrone (GetComponent<BuildingBase>().ReturnOwner(), GetComponent<NetworkIdentity>().netId);
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
		print ("SETTING TARGET FROM AIRPORT");
		droneInstance.GetComponent<Drone> ().CmdSetCurrentTarget (thisId);
	}

	void OnDestroy() {
		Destroy (droneInstance);
	}
}
