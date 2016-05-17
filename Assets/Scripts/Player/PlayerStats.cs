using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PlayerStats : NetworkBehaviour {
	
	[SyncVar]
	float currentEnergy = 40f;
	[SyncVar]
	float energyUptake = 1f;
	[SyncVar]
	float energyTimer = 0;

	public float GetCurrentEnergy() {
		return currentEnergy;
	}

	public void SpendEnergy (float howMuchEnergy) {
		currentEnergy -= howMuchEnergy;
	}

	public void IncreaseEnergyUptake () {
		energyUptake += 1;
	}

//	void AlterEnergyUptake(float amount) {
//		energyUptake += amount;
//	}
		
	void GatherEnergy() {
		currentEnergy += energyUptake;
	}

	[Command]
	public void CmdDecreaseEnergyUptake () {
		if (isServer) {
			RpcDecreaseEnergyUptake ();
		}
	}

	[ClientRpc]
	void RpcDecreaseEnergyUptake() {
		if (isLocalPlayer) {
			energyUptake -= 1;
		}
	}

	public bool IsThereEnoughEnergy (float howMuchEnergy) {
		if (howMuchEnergy <= currentEnergy) {
			return true;
		} else {
			return false;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<GUIManager>().currentHUD.currentEnergyText.text = "Energy: " + currentEnergy.ToString ("F0");
		GetComponent<GUIManager>().currentHUD.energyUptakeText.text = "Energy Uptake: " + energyUptake + "/sec";
		if (GameManager.gameHasStarted) {
			HandleTimer ();
		}
	}

	void HandleTimer() {
		if (energyTimer >= 1f) {
			GatherEnergy ();
			energyTimer = 0;
		} 
		energyTimer += Time.deltaTime;
	}


}
