using UnityEngine;
using System.Collections;

public class PlayerColorManager : MonoBehaviour {


	public Color color2;

		
	public Material templateColorRed, templateColorGreen;

	public static Color GetBuildingColor (bool isPowered, string owner) {
		Color tempColor = new Color();
		if (isPowered) {
			switch (GameManager.players.IndexOf(owner)) {
			case 0:
				tempColor = Color.red;

				break;
			case 1:
				tempColor = Color.blue;

				break;

			}
		} else {
			switch (GameManager.players.IndexOf(owner)) {
			case 0:
				tempColor = new Color(0.5f,0f,0f);
				break;
			case 1:
				tempColor = new Color(0f,0f,.5f);
				break;

			}
		}
		return tempColor;


	}

}
