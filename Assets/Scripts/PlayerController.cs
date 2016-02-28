using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum TargetTypes {Building, GUIButton, Floor, EnergyPool, None};
/*
Handle player movement through towers

*/

public class PlayerController : MonoBehaviour {


	[SerializeField] GameObject currentBuilding;
	GameObject currentTarget;
	TargetTypes currentTargetType;
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
		currentTarget = thisHit.collider.gameObject;
		switch(thisHit.transform.tag){
		case "Building":
			isTargetingBuilding = true;
			currentTargetType = TargetTypes.Building;
			break;
		case "GUIButton":
			currentTarget.GetComponent<Button> ().Select ();
			currentTargetType = TargetTypes.GUIButton;
			PressGUIButton ();
			break;
		case "Floor":
			currentTargetType = TargetTypes.Floor;
			break;
		default :
			isTargetingBuilding = false;
			currentTargetType = TargetTypes.None;
			break;
		}
			

	}
		
	void HandleRightTriggerDown() {
		switch (currentTargetType) {
		case TargetTypes.Building:
			PerformActionOnTargetedBuilding ();
			break;
		case TargetTypes.GUIButton:
			PressGUIButton ();
			break;
		}

	}

	void HandleRightTriggerUp() {

	}

	void PerformActionOnTargetedBuilding() {
		if (currentTarget.GetComponent<BuildingBase> ().ReturnOwner () == owner) {
			TeleportToBuilding ();
		}
	}

	void PressGUIButton() {
		currentTarget.GetComponent<Button> ().onClick.Invoke();

	}

	void TeleportToBuilding () {
		
		currentBuilding = currentTarget;
		currentBuilding.GetComponent<BuildingBase> ().isOccupied = false;
		currentTarget.GetComponent<BuildingBase> ().isOccupied = true;
		transform.position = currentTarget.GetComponent<BuildingBase> ().playerCockpit.position;
	}



}
