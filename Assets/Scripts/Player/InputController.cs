using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Input controller.
/// </summary>
public class InputController : NetworkBehaviour {
	private static InputController s_instance;

	public bool playInVR = false, isMouseKeyboardDebug;
	public Transform rightControllerRaycastOrigin, leftControllerRaycastOrigin;
	public GameObject firstPersonCharacter, VRCameraRig;
	public WandController rightController, leftController;

	private ConstructionController constructionController;
	private Camera playerCamera;
	private float raycastDistance = 1000;
	[SerializeField] private GameObject targetBubble;
	private int buildingTypeSelected = 0;

//	void Awake() {
//		if( s_instance == null ) {
//			s_instance = this;
//		} else {
//			Debug.Log( "Destroying additional InputController in object: " +gameObject.name );
//			DestroyImmediate( this.gameObject );
//		}
//	}

	// Use this for initialization
	void Start () {
		playerCamera = GetComponentInChildren<Camera> ();
		constructionController = GetComponent<ConstructionController>();
		
//		bool steamVrRunning = false;
//		steamVrRunning = ( !SteamVR.active && playInVR ) ? true : false;
		if( playInVR ) {
			VRCameraRig.SetActive( true );
			firstPersonCharacter.SetActive( false );

			GetComponent<GUIManager>().ActivateVrHud();
			//Disable currentEventSystem and activate the one on the VR CameraRig
			UnityEngine.EventSystems.EventSystem.current.enabled = false;
			GameObject.FindObjectOfType<ViveControllerInput>().GetComponent<UnityEngine.EventSystems.EventSystem>().enabled = true;
			//Disable First Person Controller so we can't move the player with a mouse drag when in vr.
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
		} else {
			DestroyImmediate( VRCameraRig );
		}
	}
	
	// Update is called once per frame
	void Update () {
		CheckWeaponChange();
		if (GameManager.gameHasStarted) {
			if( SteamVR.active && playInVR ) {

				if( rightController.triggerButtonDown == true ) {
					Debug.LogWarning( "Right Controller trigger down" );
					OnRightTriggerFingerDown();
				}
			} else {
				if(Input.GetKeyDown (KeyCode.Space)) {
					OnRightTriggerFingerDown ();
				}
			}
//		if (Input.GetKeyUp (KeyCode.Space)) {
//			OnRightTriggerFingerUp ();
//		}
//		if (Input.GetKeyDown (KeyCode.X)) {
//			OnLeftTriggerFingerDown ();
//		}
//		if (Input.GetKeyUp (KeyCode.X)) {
//			OnLeftTriggerFingerUp ();
//		}
		}
	}

	void FixedUpdate() {
		if (GameManager.gameHasStarted) {
			CastRayFromDebugReticle ();
		}
	}

	void CastRayFromDebugReticle () {
		Ray ray = new Ray();
		RaycastHit hit = new RaycastHit();

		if( SteamVR.active && playInVR ) {
			ray = new Ray( rightControllerRaycastOrigin.position, rightControllerRaycastOrigin.forward );
		} else {
			ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
		}

		if (Physics.Raycast (ray, out hit, raycastDistance)) {
			OnSendPointerInfo (hit);
		}
	}

	private void CheckWeaponChange() {
		if( rightController.touchPadButtonDown ) {
			// Cycle through building types. 3 is the number of items in the public BuildingType enum.
			if( buildingTypeSelected+1 < 3 )
				buildingTypeSelected++;
			else
				buildingTypeSelected = 0;

			SelectNewBuilding();
			return;
		}

//		if( rightController.touchPadTouchPosition.magnitude == 0f )
//			return;
//
//		Vector2 normalizedDir = rightController.touchPadTouchPosition.normalized;
//		float deg = Mathf.Atan2( normalizedDir.y, normalizedDir.x ) * Mathf.Rad2Deg;
//		if( deg < 0f ) {
//			deg += 360f;
//		}
//
//		float numOptions = 3f; // Number of BuildingTypes in Constructor.cs
//		float startPoint =  (0.5f-(1f / numOptions)) / 2f; // We want th first option to take up the top hemisphere (0.5f) 
//		deg /= 360f;
//
//		buildingTypeSelected = ( deg >= startPoint ) ? Mathf.FloorToInt((deg)*numOptions) : (int)numOptions-1;
	}

	private void SelectNewBuilding() {
		constructionController.SelectConstructBuildingType((BuildingType)buildingTypeSelected);
	}

	public delegate void RightTriggerFingerDownAction();
	public static event RightTriggerFingerDownAction OnRightTriggerFingerDown;
	public delegate void RightTriggerFingerUpAction();
	public static event RightTriggerFingerUpAction OnRightTriggerFingerUp;
	public delegate void LeftTriggerFingerDownAction();
	public static event LeftTriggerFingerDownAction OnLeftTriggerFingerDown;
	public delegate void LeftTriggerFingerUpAction();
	public static event LeftTriggerFingerUpAction OnLeftTriggerFingerUp;

	public delegate void SendPointerInfoAction(RaycastHit thisHit);
	public static event SendPointerInfoAction OnSendPointerInfo;

}
