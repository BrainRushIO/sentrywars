using UnityEngine;
using System.Collections;

public class VRUIPanel : MonoBehaviour {
	Quaternion lockedRotation;

	void Update () {
		transform.localRotation = lockedRotation;
	}

	void OnEnable() {
		lockedRotation = transform.rotation;
	}
}
