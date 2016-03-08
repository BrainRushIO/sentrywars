using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	private const string PLAYER_ID_PREFIX = "Player";
	private static Dictionary<string, PlayerController> players = new Dictionary<string, PlayerController>();

	public static void RegisterPlayer(string _netID, PlayerController _player) {
		string _playerID = PLAYER_ID_PREFIX + _netID;
		players.Add (_playerID, _player);
		_player.InitializePlayer(_playerID);
	}

	public static void UnRegisterPlayer(string _playerID) {
		players.Remove (_playerID);
	}

	public static PlayerController GetPlayerManager(string _playerID) {
		return players [_playerID];
	}
}
