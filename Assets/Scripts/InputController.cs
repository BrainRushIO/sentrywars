using UnityEngine;
using System.Collections;

/*






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
			if (hit.collider.tag == "Floor") {
				targetBubble.transform.position = hit.point;
			} else {

			}

		}

	}

	void SwitchInputMode() {

	}
}
