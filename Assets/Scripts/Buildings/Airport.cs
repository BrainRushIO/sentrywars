using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Airport : TargetingBase {

	public GameObject dronePrefab;
	GameObject droneInstance;
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
		CmdSpawnDrone ();
	}

	[Command]
	void CmdSpawnDrone () {
		GameObject tempDrone = (GameObject)Instantiate (dronePrefab, 
			spawnPos.position, Quaternion.identity);
		NetworkServer.Spawn (tempDrone);
		tempDrone.GetComponent<Drone> ().InitializeUnit (GetComponent<BuildingBase> ().ReturnOwner ());
	}

	void OnDestroy() {
		Destroy (droneInstance);
	}
}
