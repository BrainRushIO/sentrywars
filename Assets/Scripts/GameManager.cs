using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	private const string PLAYER_ID_PREFIX = "Player";
	public static List< string> players = new List<string>();

	public static void RegisterPlayer(PlayerController _player) {
		players.Add (_player.playerID);
		_player.InitializePlayer();
		Debug.Log (_player.playerID + " has joined the game.");
	}

	public static void UnRegisterPlayer(PlayerController _player) {
		players.Remove (_player.playerID);
	}
}
