using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerScript : NetworkBehaviour {

	public GameObject bulletPrefab;

	void Update () {
		if (!isLocalPlayer) {
			return;
		}
		var x = Input.GetAxis("Horizontal")*0.1f;
		var z = Input.GetAxis("Vertical")*0.1f;

		transform.Translate(x, 0, z);
		if (Input.GetKeyDown (KeyCode.C)) {
			//firebullet
			CmdFire();
		}
	}


	[Command]
	void CmdFire() {
		var bullet = (GameObject)Instantiate(
			GetComponent<ConstructionController>().buildingPrefabs[0],
			GetComponent<ConstructionController>().currentBuildingToConstruct.transform.position,
			Quaternion.identity);
		// make the bullet move away in front of the player
//		bullet.GetComponent<Rigidbody>().velocity = -transform.forward*10f;
		NetworkServer.Spawn (bullet);
		// make bullet disappear after 2 seconds
//		Destroy(bullet, 10.0f);   
	}
	void OnEnable() {
//		InputController.OnRightTriggerFingerDown += CmdFire;
	}
	void OnDisable() {
//		InputController.OnRightTriggerFingerDown -= CmdFire;
	}

	public override void OnStartLocalPlayer()
	{
//		GetComponent<MeshRenderer>().material.color = Color.red;
	}
}
