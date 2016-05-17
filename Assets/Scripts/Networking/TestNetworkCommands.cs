using UnityEngine;
using System;
using System.Collections;
using SimpleJson;
using UnityEngine.UI;
public class TestNetworkCommands : MonoBehaviour {
	#region Custom Sentry Wars Commands
	//SW COMMANDS
	string swURL = "sentrywars.herokuapp.com";
	Rester rester;
	JsonObject gameIDObj = new JsonObject();
	public Text infoText;

	public void GetGameID() {
		int thisGameID;
		rester.GetJSON(swURL+"/game/start",( err, result ) => {
			Debug.Log( "GAME ID TEST: " + result ["gameID"]);
			string stringInt = result["gameID"].ToString();
			thisGameID = Int32.Parse(stringInt);
			object getval;
			result.TryGetValue("gameID", out getval);
			infoText.text = getval.ToString();

		});
		//		int thisID;
		//		thisID = Convert.ToInt32 (idObject);


	}

	public void SetGameIDObj(JsonObject thisJO) {
		gameIDObj = thisJO;
		print (gameIDObj.ToString ());
	}

	#endregion
	// Use this for initialization
	void Start () {
		rester = GetComponent<Rester> ();
		GetGameID ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
