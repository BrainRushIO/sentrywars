using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConstructionController : NetworkBehaviour {
	enum ConstructionState {Inactive, PlacingBuilding, SpawnBuilding};
	ConstructionState currConstructionState = ConstructionState.Inactive;
	[SerializeField] GameObject[] buildingPrefabs;
	BuildingType currentBuildingToConstructType;
	GameObject currentBuildingToConstruct;
	bool isBuildingTemplateInstantiated;

	Vector3 buildingPlacementPosition;

	private const float GRID_SPACING = 10f;
	public bool isTargetingEnergy;

	//State Machine Switches
	bool switchToInactive, switchToPlacingBuilding, switchToSpawnBuilding;
	public void SwitchToPlacingBuilding() {
		switchToPlacingBuilding = true;
	}
	public void SwitchToInactive() {
		switchToInactive = true;
	}
	
	[SerializeField] Text constructBuildingType, constructBuildingCost;

	[SerializeField]
	Dictionary<BuildingType, float> buildingCosts = new Dictionary<BuildingType, float>();

	// Use this for initialization
	void Start () {
		buildingCosts.Add (BuildingType.Constructor, 20);
		buildingCosts.Add (BuildingType.Canon, 10);
		buildingCosts.Add (BuildingType.Defense, 15);
		buildingCosts.Add (BuildingType.Shield, 50);
		buildingCosts.Add (BuildingType.Energy, 20);
		buildingCosts.Add (BuildingType.Tactical, 80);
	}


	// Update is called once per frame
	void Update () {
		//temp construction selection
		if (Input.GetKeyDown(KeyCode.Q)) {
			SelectConstructBuildingType(BuildingType.Constructor);
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			SelectConstructBuildingType(BuildingType.Canon);
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			SelectConstructBuildingType(BuildingType.Energy);
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			SelectConstructBuildingType(BuildingType.Defense);
		}
		if (Input.GetKeyDown(KeyCode.T)) {
			SelectConstructBuildingType(BuildingType.Shield);
		}
		if (Input.GetKeyDown(KeyCode.Y)) {
			SelectConstructBuildingType(BuildingType.Tactical);
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			CmdSpawnBuilding ();
		}
			
		//temp UI
		if(constructBuildingCost!=null)constructBuildingCost.text = "Construction Cost: " + buildingCosts[currentBuildingToConstructType].ToString();
		if(constructBuildingType!=null)constructBuildingType.text = "Construction Type: " + currentBuildingToConstructType.ToString ();

//		switch (currConstructionState) {
//		case ConstructionState.Inactive:
//			if (switchToPlacingBuilding) {
//				switchToPlacingBuilding = false;
//				currConstructionState = ConstructionState.PlacingBuilding;
//			}
//			break;
//		case ConstructionState.PlacingBuilding:
//			//have only one building template at a time
//			if (!isBuildingTemplateInstantiated) {
//				InstantiateBuildingTemplate ();
//				isBuildingTemplateInstantiated = true;
//			}
//			if (switchToInactive) {
//				Destroy (currentBuildingToConstruct);
//				switchToInactive = false;
//				currConstructionState = ConstructionState.Inactive;
//				isBuildingTemplateInstantiated = false;
//			} else if (switchToSpawnBuilding) {
//				switchToSpawnBuilding = false;
//				currConstructionState = ConstructionState.SpawnBuilding;
//			}
//			break;
//
//		case ConstructionState.SpawnBuilding:
//			print ("INIT BUILDING");
//			currentBuildingToConstruct.GetComponent<BuildingBase> ().InitializeBuilding (transform.gameObject.name);
//			RenderCurrentBuildingAsBuilt ();
//			isBuildingTemplateInstantiated = false;
////			CmdSpawnBuilding ();
//			currConstructionState = ConstructionState.Inactive;
//			break;
//		}
	}

	void OnEnable() {

		InputController.OnRightTriggerFingerDown += SwitchToSpawnBuilding;
		InputController.OnSendPointerInfo += PlaceBuildingTemplate;
	}

	void OnDisable () {
		InputController.OnRightTriggerFingerDown -= SwitchToSpawnBuilding;
		InputController.OnSendPointerInfo -= PlaceBuildingTemplate;
	}

	void PlaceBuildingTemplate (RaycastHit hit) {
		buildingPlacementPosition = ConvertVector3ToGridPoint (hit.point);
		if (currentBuildingToConstruct != null) {
			currentBuildingToConstruct.transform.position = buildingPlacementPosition;
		}
	}

	Vector3 ConvertVector3ToGridPoint(Vector3 thisPoint) {
		float xCoordinate = thisPoint.x;
		float zCoordinate = thisPoint.z;
		xCoordinate = RoundToGridSpacing (xCoordinate);
		zCoordinate = RoundToGridSpacing (zCoordinate);
		Vector3 output = new Vector3 (xCoordinate, thisPoint.y, zCoordinate);
		return output;
	}

	float RoundToGridSpacing (float val) {
		float remainder = val % GRID_SPACING;
		if (remainder >= GRID_SPACING / 2) {
			//ie 57 -> r = 7 add GS-r or  (10-7) to round up
			//ie -57 -> r = -7 subtract (GS+r) or (10-7) to round down

			if (val >= 0) {
				val += (GRID_SPACING - remainder);
			} else {
				val -= (GRID_SPACING + remainder);
			}
		} else {
			//ie 53 -> r = 3, sub 3 to round down
			//ie -53 -> r = -3 sub -3 to round up
			val -= remainder;
		}
		return val;
	}


	[Command]
	void CmdSpawnBuilding() {
		print ("command called");
		GameObject newBuilding = (GameObject)Instantiate (currentBuildingToConstruct, currentBuildingToConstruct.transform.position, Quaternion.identity);
		NetworkServer.Spawn (newBuilding);

	}

	void SwitchToSpawnBuilding() {
		if (currConstructionState == ConstructionState.PlacingBuilding &&
		    GetComponent<PlayerStats> ().TryToSpendEnergy (buildingCosts [currentBuildingToConstructType])) { 
			switchToSpawnBuilding = true;
		} else {
			//throw some NOT ENOUGH ENERGY MESSAGE
		}
	}

	void SelectConstructBuildingType(BuildingType thisBuildingType) {
		currentBuildingToConstructType = thisBuildingType;
		SwitchBuildingTemplate ();
	}

	void InstantiateBuildingTemplate () {
		currentBuildingToConstruct = (GameObject)Instantiate (buildingPrefabs [(int)currentBuildingToConstructType], buildingPlacementPosition, Quaternion.identity);
		currentBuildingToConstruct.GetComponentInChildren<BuildingBase> ().DisableAllColliders ();
		RenderCurrentBuildingAsTemplate ();

	}
	void RenderCurrentBuildingAsBuilt() {
		print ("render current building as built");
		if (currentBuildingToConstruct.GetComponentsInChildren<MeshRenderer> () == null) {
			Debug.LogError ("MeshRenderers not Registering");
		}
		MeshRenderer[] allMaterialsOnBuilding = currentBuildingToConstruct.GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer x in allMaterialsOnBuilding) {
			x.material = GetComponent<PlayerColorManager> ().ReturnPlayerColorMaterial (gameObject.name, 0);
			print ("set mesh renderers");
		}
		if (currentBuildingToConstruct.GetComponent<MeshRenderer> () != null) {
			currentBuildingToConstruct.GetComponent<MeshRenderer> ().material = GetComponent<PlayerColorManager> ().ReturnPlayerColorMaterial (gameObject.name, 1);
		}
	}

	void RenderCurrentBuildingAsTemplate() {
		MeshRenderer[] allMaterialsOnBuilding = currentBuildingToConstruct.GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer x in allMaterialsOnBuilding) {
			x.material = GetComponent<PlayerColorManager> ().templateColor;
		}
		if (currentBuildingToConstruct.GetComponent<MeshRenderer> () != null) {
			currentBuildingToConstruct.GetComponent<MeshRenderer> ().material = GetComponent<PlayerColorManager> ().templateColor;
		}
	}

	void SwitchBuildingTemplate() {
		if (currentBuildingToConstruct != null) {
			Destroy (currentBuildingToConstruct);
		}
		InstantiateBuildingTemplate ();
	}
}
