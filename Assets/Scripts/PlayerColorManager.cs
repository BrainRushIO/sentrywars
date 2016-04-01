using UnityEngine;
using System.Collections;

public class PlayerColorManager : MonoBehaviour {

	public Material player2;
		
	public Material templateColorRed, templateColorGreen;

	public Material ReturnPlayerColorMaterial() {
//		if (pID == "Player2") {
//			return (player2 [shade]);
//		} else if (pID == "Player1") {
			return (player2);
//		} else {
//			return null;
//		}
	}
}
