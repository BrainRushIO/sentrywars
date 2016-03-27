using UnityEngine;
using System.Collections;

public class PlayerColorManager : MonoBehaviour {

	public Material[] player1, player2;
		
	public Material templateColorRed, templateColorGreen;

	public Material ReturnPlayerColorMaterial(string pID, int shade) {
//		if (pID == "Player2") {
//			return (player2 [shade]);
//		} else if (pID == "Player1") {
			return (player1 [shade]);
//		} else {
//			return null;
//		}
	}
}
