using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class BuildingStateController : NetworkBehaviour {

	public MeshRenderer[] coloredMesh;
	[SyncVar] Color thisBuildingColor = new Color();
	[SyncVar] NetworkInstanceId towerNetID;
	[SyncVar] public int damageState;
	[SyncVar] bool isWarpingIn = true;
	public GameObject[] stage1damage, stage2damage, stage3damage;
	List<GameObject[]> damageStages;

	[SerializeField] GameObject warpEffect;

	public void SetMeshRendererColor() {
		StartCoroutine (WarpInComplete ());
	}


	IEnumerator WarpInComplete() {
		yield return new WaitForSeconds (3);
		isWarpingIn = false;
		GetBuildingColor ();
		RpcSwitchColor (thisBuildingColor);
		SendMessage ("OnWarpInComplete", SendMessageOptions.DontRequireReceiver);
	}
	void Start () {
		damageStages = new List<GameObject[]> ();
		if (stage1damage.Length > 0) {
			damageStages.Add (stage1damage);
			damageStages.Add (stage2damage);
			damageStages.Add (stage3damage);
		}
	}

	public bool ReturnIsWarpingIn() {
		return isWarpingIn;
	}

	[ClientRpc]
	void RpcSwitchColor (Color col) {
		GetComponent<BuildingBase> ().enabled = true;
		GetComponent<BuildingBase> ().EnableMeshRenderers ();
		foreach (MeshRenderer x in coloredMesh) {
			x.material.SetColor ("_EmissionColor", col);
		}
	}
//	[Command]
//	public void CmdSetDamageState(int thisStage) {
//		RpcSetDamageState (thisStage);
//	}
//
	[ClientRpc]
	public void RpcSetDamageState(int stage) {
		damageState = stage;
		foreach (GameObject x in damageStages[stage]) {
			x.SetActive (true);
		}
	}

	void GetBuildingColor () {
		PlayerController owner = GameManager.players[GetComponent<BuildingBase> ().ReturnOwner ()].GetComponent<PlayerController>();
		switch (owner.playerInt) {
			case 0:
				thisBuildingColor = Color.red;
				break;
			case 1:
				thisBuildingColor = Color.blue;
				break;
			}
		}
}
