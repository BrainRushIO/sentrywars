using UnityEngine;
using System.Collections;

public class RangeRing : MonoBehaviour {
	float val, speedScalar = 1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		val += Time.deltaTime * speedScalar;
		float sin = Mathf.Abs (Mathf.Sin (val));
		GetComponent<MeshRenderer> ().material.color = new Color (1, 1, 1, sin);
	}
}
