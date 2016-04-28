using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class BuildingStateController : NetworkBehaviour {

	public MeshRenderer[] coloredMesh;
	[SyncVar] Color thisBuildingColor = new Color();
	[SyncVar] NetworkInstanceId towerNetID;
	public GameObject[] stage1damage, stage2damage, stage3damage;
	List<GameObject[]> damageStages;
	public void SetMeshRendererColor(bool isPowered) {
		GetBuildingColor (isPowered);
		RpcSwitchColor (thisBuildingColor);
	}
	void Start () {
		damageStages = new List<GameObject[]> ();
		if (stage1damage.Length > 0) {
			damageStages.Add (stage1damage);
			damageStages.Add (stage2damage);
			damageStages.Add (stage3damage);
		}
	}

	[ClientRpc]
	void RpcSwitchColor (Color col) {
		foreach (MeshRenderer x in coloredMesh) {
			x.material.SetColor ("_Color", col);
		}
	}

	[Command]
	public void CmdSetDamageState(int thisStage) {
		RpcSetDamageState (thisStage);
	}

	[ClientRpc]
	void RpcSetDamageState(int stage) {
			foreach (GameObject x in damageStages[stage]) {
				x.SetActive (true);
			}
	}

	void GetBuildingColor (bool isPowered) {
		PlayerController owner = GameManager.players[GetComponent<BuildingBase> ().ReturnOwner ()].GetComponent<PlayerController>();
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
