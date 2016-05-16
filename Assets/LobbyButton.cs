using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class LobbyButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartLobby() {
		NetworkLobbyManager.singleton.StartHost ();

	}

	public void JoinLobby () {
		
		NetworkLobbyManager.singleton.StartClient ();
	}
}
