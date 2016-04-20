﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnergyField : NetworkBehaviour {
	[SerializeField] [SyncVar] bool isOccupied = false;

	[Command]
	public void CmdSetIsOccupied(bool val) {
		isOccupied = val;
	}

	public bool ReturnIsOccupied() {
		return isOccupied;
	}
}
