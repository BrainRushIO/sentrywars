using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Input controller.
/// </summary>
public class InputController : NetworkBehaviour {
	private static InputController s_instance;
	public static InputController instance {get {return s_instance;}}

	public bool playInVR = false, isMouseKeyboardDebug;
	public Transform rightControllerRaycastOrigin, leftControllerRaycastOrigin;
	public Camera VRCamera;
	public GameObject VRHandlers;
	public WandController rightController, leftController;

	Camera playerCamera;
	float raycastDistance = 1000;
	[SerializeField] GameObject targetBubble;

	void Awake() {
		if( s_instance == null ) {
			s_instance = this;
		} else {
			Debug.Log( "Destroying additional InputController in object: " +gameObject.name );
			DestroyImmediate( this.gameObject );
		}
	}

	// Use this for initialization
	void Start () {
		playerCamera = GetComponentInChildren<Camera> ();
		
//		bool steamVrRunning = false;
//		steamVrRunning = ( !SteamVR.active && playInVR ) ? true : false;
		if( playInVR ) {
			VRCamera.gameObject.SetActive( true );
			VRHandlers.SetActive( true );
		} else {
			DestroyImmediate( VRCamera.gameObject );
			DestroyImmediate( VRHandlers );
		}
	}
	
	// Update is called once per frame
	void Update () {
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
			Debug.Log( "Getting ray from Controller" );
			ray = new Ray( rightControllerRaycastOrigin.position, rightControllerRaycastOrigin.forward );
			Debug.Log( "Ray: " + ray );
		} else {
			ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
		}

		if (Physics.Raycast (ray, out hit, raycastDistance)) {
			OnSendPointerInfo (hit);
		}
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
