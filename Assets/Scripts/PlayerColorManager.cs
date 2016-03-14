using UnityEngine;
using System.Collections;

public class PlayerColorManager : MonoBehaviour {


	public Material[] player1, player2;
		
	public Material templateColor;


	public Material ReturnPlayerColorMaterial(string pID, int shade) {
		if (pID == "Player4") {
			return (player2 [shade]);
		} else if (pID == "Player3") {
			return (player1 [shade]);
		} else {
			return null;
		}
	}
		

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
