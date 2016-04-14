using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BuildingStateController : NetworkBehaviour {

	public MeshRenderer[] coloredMesh;
	[SyncVar] Color thisBuildingColor = new Color();
	[SyncVar] NetworkInstanceId towerNetID;

	public void SetMeshRendererColor(bool isPowered) {
		GetBuildingColor (isPowered);
		RpcSwitchColor (thisBuildingColor);
	}

	[ClientRpc]
	void RpcSwitchColor (Color col) {
		foreach (MeshRenderer x in coloredMesh) {
			x.material.SetColor ("_Color", col);
		}
	}

	void GetBuildingColor (bool isPowered) {
		PlayerController owner = GetComponent<PlayerController> ();
		if (isPowered) {
			switch (owner.playerInt) {
			case 0:
				thisBuildingColor = Color.red;
				break;
			case 1:
				thisBuildingColor = Color.blue;
				break;
			}
		} else {
			switch (owner.playerInt) {
			case 0:
				thisBuildingColor = new Color(0.5f,0f,0f);
				break;
			case 1:
				thisBuildingColor = new Color(0f,0f,.5f);
				break;

			}
		}
	}

}
