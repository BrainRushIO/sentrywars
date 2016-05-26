using UnityEngine;
using System.Collections;

public class VRUIObject : MonoBehaviour {

	public Material unselected, selected;
	float timer, unselectTime=.2f;
	bool isSelected;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (timer > 0) {
			timer -= Time.deltaTime;
		}
		if (timer <= 0 && unselected) {
			Unselect ();
		}
	}

	public void HoverOver () {
		if (!isSelected) {
			GetComponent<MeshRenderer> ().material = selected;
			isSelected = true;
		}
		timer = .1f;


	}

	public void Select () {

	}

	void Unselect() {
		GetComponent<MeshRenderer> ().material = unselected;

		isSelected = false;
	}
}
