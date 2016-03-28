using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PlayerStats : NetworkBehaviour {
	[SyncVar]
	float currentEnergy = 100f;
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

	public void IncreaseEnergyUptake (float amount = 1f) {
		energyUptake += amount;
	}

	public bool IsThereEnoughEnergy (float howMuchEnergy) {
		if (howMuchEnergy <= currentEnergy) {
			return true;
		} else {
			return false;
		}
	}
		
	[SerializeField] Text currentEnergyText, energyUptakeText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (currentEnergyText != null && energyUptakeText!= null) {
			currentEnergyText.text = "Energy: " + currentEnergy.ToString ("F0");
			energyUptakeText.text = "Energy Uptake: " + energyUptake + "/sec";
		}
		HandleTimer ();
	}

	void HandleTimer() {
		if (energyTimer >= 1f) {
			currentEnergy += energyUptake;
			energyTimer = 0;
		} 
		currentEnergy += Time.deltaTime;
	}

	void BuildEnergyAssimilator () {

	}
}
