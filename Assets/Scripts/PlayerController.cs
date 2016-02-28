using UnityEngine;
using System.Collections;


/*
Handle player movement through towers

*/

public class PlayerController : MonoBehaviour {


	[SerializeField] GameObject currentBuilding;
	GameObject currentTargetedBuilding;
	bool isTargetingBuilding;

	private int owner;
	public int ReturnOwner(){return owner;}

	// Use this for initialization
	void Start () {
		currentBuilding.GetComponent<BuildingBase> ().isOccupied = true;
	}

	void OnEnable() {
		InputController.OnRightTriggerFingerDown += HandleRightTriggerDown;
		InputController.OnRightTriggerFingerUp += HandleRightTriggerDown;

		InputController.OnSendPointerInfo += HandleRightHandTargeting;
	}

	void OnDisable () {
		InputController.OnRightTriggerFingerDown -= HandleRightTriggerDown;
		InputController.OnRightTriggerFingerUp -= HandleRightTriggerDown;

		InputController.OnSendPointerInfo -= HandleRightHandTargeting;
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void HandleRightHandTargeting(RaycastHit thisHit) {

		print (thisHit.transform.tag);
		switch(thisHit.transform.tag){

		case "Building":
			isTargetingBuilding = true;
			currentTargetedBuilding = thisHit.collider.gameObject;
			break;
		case "GUIButton":
			break;
		default :
			isTargetingBuilding = false;
			currentTargetedBuilding = null;
			break;
		
		}





	}
		
	void HandleRightTriggerDown() {
		if (isTargetingBuilding) {
			PerformActionOnTargetedBuilding ();
		}
	}

	void HandleRightTriggerUp() {

	}

	void PerformActionOnTargetedBuilding() {
		if (currentTargetedBuilding.GetComponent<BuildingBase> ().ReturnOwner () == owner) {
			TeleportToBuilding ();
		}
	}

	void TeleportToBuilding () {
		
		currentBuilding = currentTargetedBuilding;
		currentBuilding.GetComponent<BuildingBase> ().isOccupied = false;
		currentTargetedBuilding.GetComponent<BuildingBase> ().isOccupied = true;
		transform.position = currentTargetedBuilding.GetComponent<BuildingBase> ().playerCockpit.position;
	}



}
