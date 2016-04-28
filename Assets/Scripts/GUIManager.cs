using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {
	public HUD overlayHUD, vrHUD, currentHUD;

	float alertTimer, alertDuration = 3f;

	void Start() {
		currentHUD = overlayHUD;
	}

	public void SetAlert(string message) {
		currentHUD.alert.text = message;
		currentHUD.alert.enabled = true;
		alertTimer = alertDuration;

	}

	void Update () {
		if (alertTimer >= 0) {
			alertTimer -= Time.deltaTime;
			if (alertTimer < 0) {
				currentHUD.alert.enabled = false;
			}
		}
	}
	
}
