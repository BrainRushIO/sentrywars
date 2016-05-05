using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

/*
	This class stores the vector3s for position and rotation values and sends them up to server in a json object
*/

public class GhostPathRecorder : MonoBehaviour {

	public float sampleRate = 0.25f;
	public bool isRecording = false;
	public bool exportJSON = false;
	public bool fetch = false;

	public Transform transformToTrack;
	public List<Vector3> recordedPositions;
	public List<float> recordedYRotations;
	private float timer;
	private Rester _Rester;
	private JsonObject GETOBJ;

	void Start () {
		transformToTrack = transform;

		recordedPositions = new List<Vector3>();
		recordedYRotations = new List<float>();

		_Rester = GameObject.FindObjectOfType<Rester>();
		if( _Rester == null ) {
			Debug.LogError( "No rester." );
		}

		// TODO Get sampleRate from external script
	}

	void Update () {
		if( fetch ) {
			fetch = false;
			_Rester.GetJSON( "asaghostmatch.herokuapp.com/data/" + "5653e00e617b660300ccd226", ( string err, JsonObject result ) => {
			});
		}

		if( exportJSON ) {
			exportJSON = false;
			if( recordedPositions.Count < 2 || recordedYRotations.Count < 2 ) {
				Debug.LogError( "GhostPathRecorder hasn't recorded anything yet so there is nothing to export." );
			} else {
				ExportRecordedDataToJSON();
			}
		}

		if( isRecording ) {
			if( timer >= sampleRate ) {
				// Record position and rotation
				recordedPositions.Add( transformToTrack.position );
				recordedYRotations.Add( transformToTrack.rotation.eulerAngles.y );

				// Add the left over faction of time to the timer after reset
				timer = timer % sampleRate;
			}

			timer += Time.deltaTime;
		}
	}

	public void CheckIfUploadData() {
		// TODO Get data from database
	}

	private void ExportRecordedDataToJSON() {
		JsonObject recordedData = new JsonObject();
		JsonArray posRotArray = new JsonArray();

		for( int i = 0; i < recordedPositions.Count; i++ ) {
			// Get reference to current position/rotation in our recorded data
			Vector3 currentPos = recordedPositions[i];
			float currentYRot = recordedYRotations[i];

			// Build out position and rotation objects
			JsonObject posV = new JsonObject();
			posV.Add( "x", currentPos.x );
			posV.Add( "y", currentPos.y );
			posV.Add( "z", currentPos.z );
			
//			JsonObject rotQ = new JsonObject();
//			rotQ.Add( "w", currentYRot.w );
//			rotQ.Add( "x", currentYRot.x );
//			rotQ.Add( "y", currentYRot.y );
//			rotQ.Add( "z", currentYRot.z );

			// Create and populate object that will encapsulate position and rotation at this index
			JsonObject posRotPair = new JsonObject();
			posRotPair.Add( "posV", posV );
			posRotPair.Add( "yRot", currentYRot );

			// Add new object to array of positions
			posRotArray.Add( posRotPair );
		}
		// Add array of positions to recorded data
		recordedData.Add( "frames", posRotArray );

		_Rester.PostJSON( "asaghostmatch.herokuapp.com/data", recordedData, ( string err, JsonObject retJO ) => {
			Debug.Log( err );
		});
		Debug.LogWarning( "Exporting complete." );
	}

	public void StartRecording() {
		isRecording = true;
		recordedPositions.Add( transformToTrack.position );
		recordedYRotations.Add( transformToTrack.rotation.eulerAngles.y );
	}

	public void StopRecording() {
		isRecording = false;
	}
}
