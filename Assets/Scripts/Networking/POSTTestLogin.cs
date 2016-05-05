using UnityEngine;
using UnityEngine.UI;
using SimpleJson;
using System.Collections;

/*
	Tests POST request from Unity to server
*/

public class POSTTestLogin : MonoBehaviour {
	
	public string extension;
	public InputField emailInputField;
	public InputField passwordInputField;

	private const string url = "asaghostmatch.herokuapp.com/";
	private Rester _Rester;
	
	void Start () {
		_Rester = this.GetComponent<Rester>();

		if( emailInputField == null ) {
			Debug.LogError("Couldn't find Email Input Field");
		}
		if( passwordInputField == null ) {
			Debug.LogError("Couldn't find Email Input Field");
		}
	}

	public void POSTToDataBase() {
		JsonObject newUser = new JsonObject();
		newUser.Add( "email", emailInputField.text);
		newUser.Add( "password", passwordInputField.text );

		_Rester.PostJSON( url + "register", newUser, ( err, result ) => {
			Debug.LogWarning( "Error: " + err );
			Debug.LogWarning( "Result: " + result );
		});
	}
}
