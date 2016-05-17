using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour {
	// Grip Button
	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
	public bool gripButtonDown = false;
	public bool gripButtonUp = false;
	public bool gripButtonPressed = false;


	// Trigger
	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	public bool triggerButtonDown = false;
	public bool triggerButtonUp = false;
	public bool triggerButtonPressed = false;


	// TouchPad
	private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
	public Vector2 touchPadTouchPosition = Vector2.zero;
	public bool touchPadTouchDown = false;
	public bool touchPadTouchUp = false;
	public bool touchPadButtonUp = false;
	public bool touchPadButtonDown = false;
	public bool touchPadButtonPress = false;


	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
	private SteamVR_TrackedObject trackedObj;

	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
		
	void Update () {
		if (controller == null) {
			Debug.Log("Controller not initialized");
			return;
		}

		gripButtonDown = controller.GetPressDown(gripButton);
		gripButtonUp = controller.GetPressUp(gripButton);
		gripButtonPressed = controller.GetPress(gripButton);

		triggerButtonDown = controller.GetPressDown(triggerButton);
		triggerButtonUp = controller.GetPressUp(triggerButton);
		triggerButtonPressed = controller.GetPress(triggerButton);


		touchPadTouchDown = controller.GetTouchDown(touchPad);
		touchPadTouchUp = controller.GetTouchUp(touchPad);
		touchPadButtonDown = controller.GetPressDown( touchPad );
		touchPadButtonUp = controller.GetPressUp( touchPad );
		touchPadButtonPress = controller.GetPress( touchPad );

		if( controller.GetTouch(touchPad) ) {
			touchPadTouchPosition = controller.GetAxis(touchPad);
		} else {
			touchPadTouchPosition = Vector3.zero;
		}

		if (gripButtonDown) {
			Debug.Log("Grip Button was just pressed");
		}
		if (gripButtonUp) {
			Debug.Log("Grip Button was just unpressed");
		}
		if (triggerButtonDown) {
			Debug.Log("Trigger Button was just pressed");
		}
		if (triggerButtonUp) {
			Debug.Log("Trigger Button was just unpressed");
		}
	}
}
