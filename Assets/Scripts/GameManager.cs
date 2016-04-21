using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {
	public static bool gameHasStarted = false;
	private const string PLAYER_ID_PREFIX = "Player";
	public static List< NetworkIdentity> players = new List<NetworkIdentity>();
	static int playersInMatch = 2;
	public static void RegisterPlayer(NetworkIdentity _player) {
			players.Add (_player);
			if (players.Count == playersInMatch) {
				for (int i = 0; i < playersInMatch; i++) {
					players [i].GetComponent<PlayerController>().InitializePlayer (i);
					gameHasStarted = true;
				}
			}
	}

	public static void UnRegisterPlayer(NetworkIdentity _player) {
		players.Remove (_player);
	}
	void Update() {
//		if (isServer) {
			print ("GM");
//		}
	}
}
