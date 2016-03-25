using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;
	Camera sceneCamera;

	[SerializeField]
	string remoteLayerName = "RemoteLayer";

	void Start () {
		if (!isLocalPlayer) {
			DisableComponents ();
			AssignRemotePlayer ();
		} else {
			sceneCamera = Camera.main;
			if (sceneCamera != null) {
				sceneCamera.gameObject.SetActive (false);
			}
		}

	}

	public override void OnStartClient() {
		base.OnStartClient ();
		int tempID = (int)GetComponent<NetworkIdentity> ().netId.Value;
//		if (tempID > 2 && tempID % 2 == 0) {
//			tempID = 2;
//		} else {
//			tempID = 1;
//		}
		string _netID = tempID.ToString ();
		PlayerController _player = GetComponent<PlayerController> ();
		GameManager.RegisterPlayer (_netID, _player);
	}

	void DisableComponents() {
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;
		}
	}

	void AssignRemotePlayer () {
		gameObject.layer = 9;
	}

	void OnDisable() {
		if (sceneCamera != null) {
			sceneCamera.gameObject.SetActive (true);
		}
		GameManager.UnRegisterPlayer (transform.name);
	}
}
