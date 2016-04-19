using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConstructionController : NetworkBehaviour {
	enum ConstructionState {Inactive, PlacingBuilding, SpawnBuilding};
	ConstructionState currConstructionState = ConstructionState.Inactive;
	BuildingType currentBuildingToConstructType;


	public GameObject[] buildingPrefabs;
	public GameObject currentBuildingToConstruct;
	Camera playerCamera;
	Vector3 buildingPlacementPosition;
	NetworkIdentity currentEnergyFieldTargeted;
	public bool isInConstructor = true, isTargetingEnergyField;
	bool isBuildingTemplateInstantiated, isBuildingTemplateGreen, canBuild;

	const float GRID_SPACING = 10f;
	public const float CONSTRUCTION_RANGE = 100f;

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

	void OnEnable() {
		InputController.OnSendPointerInfo += PlaceBuildingTemplate;
		InputController.OnRightTriggerFingerDown += SwitchToSpawnBuilding;
	}

	void OnDisable() {
		InputController.OnSendPointerInfo -= PlaceBuildingTemplate;
		InputController.OnRightTriggerFingerDown -= SwitchToSpawnBuilding;
	}

	// Use this for initialization
	void Start () {
		playerCamera = GetComponentInChildren<Camera> ();
		buildingCosts.Add (BuildingType.Constructor, 20);
		buildingCosts.Add (BuildingType.Cannon, 10);
		buildingCosts.Add (BuildingType.Energy, 20);
	}

	public void BuildInitialBuilding() {
		buildingPlacementPosition = new Vector3 (transform.position.x, 0, transform.position.z);
		currConstructionState = ConstructionState.SpawnBuilding;

	}

	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}
		//temp construction selection
		if (Input.GetKeyDown(KeyCode.Q)) {
			SelectConstructBuildingType(BuildingType.Constructor);
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			SelectConstructBuildingType(BuildingType.Cannon);
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			SelectConstructBuildingType(BuildingType.Energy);
		}
			
		//temp UI
		if(constructBuildingCost!=null)constructBuildingCost.text = "Construction Cost: " + buildingCosts[currentBuildingToConstructType].ToString();
		if(constructBuildingType!=null)constructBuildingType.text = "Construction Type: " + currentBuildingToConstructType.ToString ();


		if (isInConstructor) {
			switch (currConstructionState) {
			case ConstructionState.Inactive:
				if (switchToPlacingBuilding) {
					switchToPlacingBuilding = false;
					currConstructionState = ConstructionState.PlacingBuilding;
				}
				break;
			case ConstructionState.PlacingBuilding:
				//have only one building template at a time
				if (!isBuildingTemplateInstantiated) {
					InstantiateBuildingTemplate ();
					isBuildingTemplateInstantiated = true;
				} else
					CheckIfCanBuild ();

				if (switchToInactive) {
					Destroy (currentBuildingToConstruct);
					switchToInactive = false;
					currConstructionState = ConstructionState.Inactive;
					isBuildingTemplateInstantiated = false;
					isBuildingTemplateGreen = false;

				} else if (switchToSpawnBuilding) {
					Destroy (currentBuildingToConstruct);
					switchToSpawnBuilding = false;
					currConstructionState = ConstructionState.SpawnBuilding;
				}
				break;
			case ConstructionState.SpawnBuilding:
				CmdSpawnBuilding (buildingPlacementPosition, GetComponent<PlayerController>().playerInt, currentBuildingToConstructType, currentEnergyFieldTargeted, isTargetingEnergyField);
				currConstructionState = ConstructionState.Inactive;
				break;
			}
		} 
	}
	void PlaceBuildingTemplate (RaycastHit hit) {
		if (currentBuildingToConstruct != null) {
			buildingPlacementPosition = ConvertVector3ToGridPoint (hit.point);
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
	public void CmdSpawnBuilding(Vector3 placementPos, int thisPlayerID, BuildingType thisType, NetworkIdentity thisEnergyPool, bool isEnergy) {
		isBuildingTemplateInstantiated = false;
		GameObject temp = (GameObject)Instantiate (buildingPrefabs [(int)thisType], placementPos, Quaternion.identity);
		if (isEnergy) {
			temp.GetComponent<BuildingBase> ().InitializeBuilding (thisPlayerID, thisEnergyPool);
		} else {
			temp.GetComponent<BuildingBase> ().InitializeBuilding (thisPlayerID);
		}
		NetworkServer.SpawnWithClientAuthority (temp, gameObject);

	}

	void SwitchToSpawnBuilding() {
		if (currConstructionState == ConstructionState.PlacingBuilding &&
		    GetComponent<PlayerStats> ().IsThereEnoughEnergy (buildingCosts [currentBuildingToConstructType]) &&
		    canBuild) { 
			HandleSpendEnergyOnBuilding ();
		}
	}

	void HandleSpendEnergyOnBuilding() {
		GetComponent<PlayerStats> ().SpendEnergy (buildingCosts [currentBuildingToConstructType]);
		if (currentBuildingToConstructType == BuildingType.Energy) {
			GetComponent<PlayerStats> ().IncreaseEnergyUptake ();
			GetComponent<PlayerController> ().ReturnCurrentTarget ().GetComponent<EnergyField> ().RpcSetIsOccupied(true);
			currentEnergyFieldTargeted = GetComponent<PlayerController> ().ReturnCurrentTarget ().GetComponent<NetworkIdentity> ();

		} else {
			//throw some NOT ENOUGH ENERGY MESSAGE
		}
		switchToSpawnBuilding = true;
	}

	void SelectConstructBuildingType(BuildingType thisBuildingType) {
		if (currConstructionState == ConstructionState.PlacingBuilding) {
			currentBuildingToConstructType = thisBuildingType;
			SwitchBuildingTemplate ();
		}
	}

	void InstantiateBuildingTemplate () {
		currentBuildingToConstruct = (GameObject)Instantiate (buildingPrefabs [(int)currentBuildingToConstructType], buildingPlacementPosition, Quaternion.identity);
		currentBuildingToConstruct.GetComponentInChildren<BuildingBase> ().DisableAllColliders ();
		CheckIfCanBuild ();

	}

	void RenderCurrentBuildingAsTemplate(bool isBuildable) {
		if (isBuildable && !isBuildingTemplateGreen) {
			isBuildingTemplateGreen = true;
			RenderMeshGreenRed (true);

		} else if (!isBuildable && isBuildingTemplateGreen) {
			isBuildingTemplateGreen = false;
			RenderMeshGreenRed (false);
		} else if (!isBuildingTemplateGreen) {
			RenderMeshGreenRed (false);
		} else {
			RenderMeshGreenRed (true);
		}
	}

	void RenderMeshGreenRed(bool green) {
		MeshRenderer[] allMaterialsOnBuilding = currentBuildingToConstruct.GetComponentsInChildren<MeshRenderer> ();
		if (green) {
			foreach (MeshRenderer x in allMaterialsOnBuilding) {
				x.material = GetComponent<PlayerColorManager> ().templateColorGreen;
			}
			if (currentBuildingToConstruct.GetComponent<MeshRenderer> () != null) {
				currentBuildingToConstruct.GetComponent<MeshRenderer> ().material = GetComponent<PlayerColorManager> ().templateColorGreen;
			}
		} else {
			foreach (MeshRenderer x in allMaterialsOnBuilding) {
				x.material = GetComponent<PlayerColorManager> ().templateColorRed;
			}
			if (currentBuildingToConstruct.GetComponent<MeshRenderer> () != null) {
				currentBuildingToConstruct.GetComponent<MeshRenderer> ().material = GetComponent<PlayerColorManager> ().templateColorRed;
			}
		}
	}

	void SwitchBuildingTemplate() {
		if (currentBuildingToConstruct != null) {
			Destroy (currentBuildingToConstruct);
		}
		isBuildingTemplateInstantiated = false;
	}

	void CheckIfCanBuild () {
		if (Vector3.Distance (buildingPlacementPosition, gameObject.transform.position) < CONSTRUCTION_RANGE && currentBuildingToConstruct!=null) {
			canBuild = true;
			if (currentBuildingToConstructType == BuildingType.Energy && !isTargetingEnergyField) {
				RenderCurrentBuildingAsTemplate (false);
				canBuild = false;
			} else if (currentBuildingToConstructType == BuildingType.Energy && isTargetingEnergyField) {
				RenderCurrentBuildingAsTemplate (true);
			} else if (currentBuildingToConstructType != BuildingType.Energy && isTargetingEnergyField) {
				RenderCurrentBuildingAsTemplate (false);
				canBuild = false;
			} else {
				RenderCurrentBuildingAsTemplate (true);
			}
		} else if (currentBuildingToConstruct!=null){
			RenderCurrentBuildingAsTemplate (false);
			canBuild = false;
		}
	}
}
