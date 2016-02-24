using UnityEngine;
using System.Collections;

/*
Sets up events for different 





*/


public class InputController : MonoBehaviour {

	public bool isMouseKeyboardDebug;

	float raycastDistance = 1000;
	[SerializeField] GameObject targetBubble;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}

	void CastRayFromDebugReticle () {
		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, raycastDistance)) {
			

		}

	}


	public delegate void RightTriggerFingerDownAction();
	public static event RightTriggerFingerDownAction OnRightTriggerFingerDown;
	public delegate void RightTriggerFingerReleaseAction();
	public static event RightTriggerFingerReleaseAction OnRightTriggerFingerRelease;
	public delegate void CycleUpAction();
	public static event CycleUpAction OnCycleUp;
	public delegate void CycleDownAction();
	public static event CycleDownAction OnCycleDown;

	public delegate void SendPointerInfoAction(RaycastHit thisHit);
	public static event SendPointerInfoAction OnSendPointerInfo;


	void SwitchInputMode() {

	}


}
