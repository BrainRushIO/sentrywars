using UnityEngine;
using System.Collections;

public class TempGUICanvas : MonoBehaviour {


	public Transform followThisTransform;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position = followThisTransform.position + new Vector3 (0, 0, 25f);
	}
}
