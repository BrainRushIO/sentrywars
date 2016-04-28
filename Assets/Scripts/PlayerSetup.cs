using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;
	Camera sceneCamera;

	void Start () {
		if (!isLocalPlayer) {
			DisableComponents ();
			AssignRemotePlayer ();
		} else {
//			sceneCamera = Camera.main;
			sceneCamera = GameObject.Find( "IntroCamera" ).GetComponent<Camera>();
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
		NetworkIdentity _player = GetComponent<NetworkIdentity> ();
		GameManager.RegisterPlayer (_player);
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
		GameManager.UnRegisterPlayer (GetComponent<NetworkIdentity>());
	}
}
