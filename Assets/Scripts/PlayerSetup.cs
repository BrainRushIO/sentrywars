﻿using UnityEngine;
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

		string _netID = GetComponent<NetworkIdentity> ().netId.ToString();
		PlayerController _player = GetComponent<PlayerController> ();
		GameManager.RegisterPlayer (_netID, _player);
	}

	void DisableComponents() {
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;
		}
	}

	void AssignRemotePlayer () {
		gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
	}

	void OnDisable() {
		if (sceneCamera != null) {
			sceneCamera.gameObject.SetActive (true);
		}
		GameManager.UnRegisterPlayer (transform.name);
	}
}
