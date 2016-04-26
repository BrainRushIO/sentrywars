using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {
	public Text constructBuildingType, constructBuildingCost;
	public Text thisBuildingHP, endMatch;
	public Text currentEnergyText, energyUptakeText;
	public Text alert;

	float alertTimer, alertDuration = 3f;

	public void SetAlert(string message) {
		alert.text = message;
		alert.enabled = true;
		alertTimer = alertDuration;

	}

	void Update () {
		if (alertTimer >= 0) {
			alertTimer -= Time.deltaTime;
			if (alertTimer < 0) {
				alert.enabled = false;
			}
		}
	}
	
}
