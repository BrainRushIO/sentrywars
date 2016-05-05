using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
	This class moves the ghost boat across the map during ghostmatch play back
*/
public class TestLerp : MonoBehaviour {

	public bool getRecordedData = false;

	private List<Vector3> positions;
	private List<float> yRotations;
	private Vector3 currentStartPos, currentEndPos;
	private Quaternion currentStartRot, currentEndRot;
	private int currentIndex = 0;
	private float sampleRate;
	private float timer = 0f;
	private bool isMoving = false;

	private Transform thisTransform;

	void Start() {
		thisTransform = GetComponent<Transform>();
	}

	void Update () {
		if( getRecordedData ) {
			FindAttributes();
		}
		if( isMoving ) {
			// Check if we should go to next segment
			if( timer >= sampleRate ) {
				if( currentIndex+2 < positions.Count ) {
					IterateToNextSegment();
				} else {
					isMoving = false;
				}
			}
			// Lerp Positions
			thisTransform.position = Vector3.Lerp( currentStartPos, currentEndPos, timer/sampleRate );
			thisTransform.rotation = Quaternion.Lerp( currentStartRot, currentEndRot, timer/sampleRate );

			timer += Time.deltaTime;
		}
	}

	void FindAttributes() {
		getRecordedData = false;
		GhostPathRecorder temp = GameObject.FindObjectOfType<GhostPathRecorder>();
		if( temp == null ) {
			Debug.LogError( "TestLerp coulnd't find a GhostPathRecorder in he scene." );
			return;
		}
		if( temp.recordedPositions.Count < 1 || temp.recordedYRotations.Count < 1 ) {
			Debug.LogError( "The GhostPathRecorder that TestLerp found has too little sampled positions/rotations to work." );
			return;
		}

		positions = temp.recordedPositions;
		yRotations = temp.recordedYRotations;
		sampleRate = temp.sampleRate;

		thisTransform.position = positions[0];
		thisTransform.rotation = Quaternion.Euler( new Vector3( 0f, yRotations[0], 0f ) );

		currentStartPos = positions[0];
		currentEndPos = positions[1];
		currentStartRot = Quaternion.Euler( new Vector3( 0f, yRotations[0], 0f ) );
		currentEndRot = Quaternion.Euler( new Vector3( 0f, yRotations[1], 0f ) );

		isMoving = true;
		getRecordedData = false;
	}

	void IterateToNextSegment() {
		currentIndex++;
		currentStartPos = positions[currentIndex];
		currentEndPos = positions[currentIndex+1];
		currentStartRot = Quaternion.Euler( new Vector3( 0f, yRotations[currentIndex], 0f ) );
		currentEndRot = Quaternion.Euler( new Vector3( 0f, yRotations[currentIndex+1], 0f ) );

		timer = timer%sampleRate;
	}
}
