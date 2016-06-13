using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class UnitBase : BaseObject {
	
	[SyncVar] Color thisUnitColor = new Color();
	public MeshRenderer[] coloredMesh;
	[SyncVar] protected NetworkInstanceId homeBuilding;

	public virtual void TakeDamage (float amount) {
		if (isServer) {
			currentHealth -= amount;

			if (currentHealth < 1) {
				CmdSpawnDeathExplosion ();
				Destroy (gameObject);
			}
		}
	}

	public virtual void InitializeUnit (int thisOwner, NetworkInstanceId thisHomeBuilding) {
		owner = thisOwner;
		currentHealth = maxHealth;
		homeBuilding = thisHomeBuilding;
		GetUnitColor ();
		RpcSwitchColor (thisUnitColor);
	}

	[ClientRpc]
	void RpcSwitchColor (Color col) {
		print ("SET COLOR " + col);
		foreach (MeshRenderer x in coloredMesh) {
			x.material.SetColor ("_Color", col);
		}
	}
	[Command]
	public void CmdSpawnDeathExplosion() {
		GameObject temp = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs[5], transform.position, Quaternion.identity);
		Destroy (temp, 5f);
		NetworkServer.Spawn (temp);
	}
	


	void GetUnitColor () {
		PlayerController thisPlayerController = GameManager.players[owner].GetComponent<PlayerController>();
		switch (thisPlayerController.playerInt) {
		case 0:
			thisUnitColor = Color.red;
			break;
		case 1:
			thisUnitColor = Color.blue;
			break;
		}
	}
}
