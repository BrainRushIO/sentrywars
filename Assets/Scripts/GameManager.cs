using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	private const string PLAYER_ID_PREFIX = "Player";
	public static List< PlayerController> players = new List<PlayerController>();

	public static void RegisterPlayer(PlayerController _player) {
		players.Add (_player);
		if (players.Count > 1) {
			foreach (PlayerController x in players) {
				x.InitializePlayer ();
			}
		}
	}

	public static void UnRegisterPlayer(PlayerController _player) {
		players.Remove (_player);
	}
}
