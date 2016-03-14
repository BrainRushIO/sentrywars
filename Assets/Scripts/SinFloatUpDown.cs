using UnityEngine;
using System.Collections;

public class SinFloatUpDown : MonoBehaviour {

	Vector3 initPos;
	float speedScalar = 1f, translationScalar = .1f;
	float val;
	// Use this for initialization
	void Start () {
		initPos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		val += Time.deltaTime * speedScalar;
		float sin = Mathf.Sin (val);
		transform.localPosition = new Vector3 (initPos.x, initPos.y + sin*translationScalar, initPos.z);


	}
}
