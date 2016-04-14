using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/*
Sets up events for different 
*/


public class InputController : NetworkBehaviour {

	public bool isMouseKeyboardDebug;
	Camera playerCamera;
	float raycastDistance = 1000;
	[SerializeField] GameObject targetBubble;

	// Use this for initialization
	void Start () {
		playerCamera = GetComponentInChildren<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.gameHasStarted) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				OnRightTriggerFingerDown ();
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
		Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
		RaycastHit hit;
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
