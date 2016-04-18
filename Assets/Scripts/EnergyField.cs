using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnergyField : NetworkBehaviour {
	[SyncVar] bool isOccupied = false;

	[ClientRpc]
	public void RpcSetIsOccupied(bool val) {
		isOccupied = val;
	}
	public bool ReturnIsOccupied() {
		return isOccupied;
	}
}
