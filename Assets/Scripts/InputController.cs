using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	float raycastDistance = 1000;
	[SerializeField] GameObject targetBubble;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, raycastDistance)) {
			if (hit.collider.tag == "Floor") {
				targetBubble.transform.position = hit.point;
//				targetBubble.SetActive (true);

			} else {
//				targetBubble.SetActive (false);

			}

		}

	}
}
