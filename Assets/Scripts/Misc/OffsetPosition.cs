using UnityEngine;
using System.Collections;

public class OffsetPosition : MonoBehaviour {

	public Transform trackedTransform;
	private enum PositionOffsetType { Mirror, Child }
	[Header("Position")]
	[SerializeField] private PositionOffsetType positionOffsetType = PositionOffsetType.Child;
	public Vector3 offset = Vector3.zero;
	public bool useDefaultPosition = false;

	private enum RotationOffsetType { None, Mirror, Child }
	[Header("Rotation")]
	[SerializeField] private RotationOffsetType rotationOffsetType = RotationOffsetType.None;

	private Vector3 cachedPosition;
	private Quaternion cachedRotation;
	private Transform thisTransform;

	void Start() {
		thisTransform = transform;
		cachedPosition = transform.localPosition;
		cachedRotation = transform.localRotation;
	}

	void LateUpdate() {
		if( trackedTransform == null )
			return;

		switch( positionOffsetType )
		{
		case PositionOffsetType.Child:
			Vector3 offsetPosition = offset;

			if( useDefaultPosition )
				offsetPosition = cachedPosition;

			Vector3 x, y, z;
			x = trackedTransform.right * offsetPosition.x;
			y = trackedTransform.up * offsetPosition.y;
			z = trackedTransform.forward * offsetPosition.z;
			
			thisTransform.position = trackedTransform.position + x + y + z;
			break;
		case PositionOffsetType.Mirror:
			thisTransform.position = trackedTransform.position;
			break;
		}

		switch( rotationOffsetType )
		{
		case RotationOffsetType.Mirror:
			thisTransform.rotation = trackedTransform.rotation;
			break;
		case RotationOffsetType.Child:
			thisTransform.rotation = trackedTransform.rotation;
			thisTransform.Rotate( cachedRotation.eulerAngles );
			break;
		}
	}
}
