using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[Client]
	void Shoot () {
		
	}

	[Command]
	void CmdPlayerShot(string _ID) {

	}
}
