using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Input controller.
/// </summary>
public class InputController : NetworkBehaviour {

	public bool steamVrRunning = false;
	public bool isMouseKeyboardDebug;
	public Transform rightControllerRaycastOrigin, leftControllerRaycastOrigin;
	public Camera VRCamera;
	public GameObject VRHandlers;
	public WandController rightController, leftController;

	Camera playerCamera;
	float raycastDistance = 1000;
	[SerializeField] GameObject targetBubble;

	// Use this for initialization
	void Start () {
		playerCamera = GetComponentInChildren<Camera> ();
		
		steamVrRunning = ( SteamVR.active && SteamVR.instance != null ) ? true : false;
		VRCamera.gameObject.SetActive( steamVrRunning );
		VRHandlers.SetActive( steamVrRunning );
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.gameHasStarted) {
			if( steamVrRunning ) {
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

		if( steamVrRunning ) {
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

	void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawRay( rightControllerRaycastOrigin.position, rightControllerRaycastOrigin.forward );
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
