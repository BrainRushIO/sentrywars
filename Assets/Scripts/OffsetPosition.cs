using UnityEngine;
using System.Collections;

public class OffsetPosition : MonoBehaviour {

	public Transform trackedTransform;
	public Vector3 offset = Vector3.zero;
	public bool mirrorRotation = true;

	private Transform thisTransform;

	void Start() {
		thisTransform = transform;
	}

	void LateUpdate() {
		if( trackedTransform == null )
			return;

		if( mirrorRotation )
			thisTransform.rotation = trackedTransform.rotation;

		Vector3 x, y, z;
		x = trackedTransform.right * offset.x;
		y = trackedTransform.up * offset.y;
		z = trackedTransform.forward * offset.z;

		thisTransform.position = trackedTransform.position + x + y + z;
	}
}
