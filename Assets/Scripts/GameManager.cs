using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	private const string PLAYER_ID_PREFIX = "Player";
	public static List< PlayerController> players = new List<PlayerController>();
	static int playersInMatch = 2;
	public static void RegisterPlayer(PlayerController _player) {
		players.Add (_player);
		if (players.Count == playersInMatch) {
			for (int i = 0; i < playersInMatch; i++) {
				players[i].InitializePlayer (i);
			}
		}
	}

	public static void UnRegisterPlayer(PlayerController _player) {
		players.Remove (_player);
	}
}
