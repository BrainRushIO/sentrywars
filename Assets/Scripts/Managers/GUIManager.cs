﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {
	public HUD overlayHUD, vrHUD, currentHUD;

	float alertTimer, alertDuration = 1.6f;

	public void SetAlert(string message, float duration = alertDuration) {
		currentHUD.alert.text = message;
		currentHUD.alert.enabled = true;
		alertTimer = duration;

	}

	void Update () {
		if (alertTimer >= 0) {
			alertTimer -= Time.deltaTime;
			if (alertTimer < 0) {
				currentHUD.alert.enabled = false;
			}
		}
	}

	public void ActivateOverlayHud() {
		currentHUD = overlayHUD;

		overlayHUD.gameObject.SetActive( true );
		vrHUD.gameObject.SetActive( false );
	}
	
	public void ActivateVrHud() {
		currentHUD = vrHUD;
		
		vrHUD.gameObject.SetActive( true );
		overlayHUD.gameObject.SetActive( false );
	}
}
