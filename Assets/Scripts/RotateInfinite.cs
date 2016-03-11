using UnityEngine;
using System.Collections;

public class RotateInfinite : MonoBehaviour {

	public float xAxis, yAxis, zAxis;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (xAxis, yAxis, zAxis);
	}
}
