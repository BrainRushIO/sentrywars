using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class UserSelectionInterface : MonoBehaviour {

	//TODO make this automated
	string[] imageURLs, usernames;
	List<Texture> profileImages;
	float verticalAngle = 15f;
	float horizontalAngle = 15f;
	[SerializeField] Rester thisRester;
	string profileInfoListURL = "sw-tournament.herokuapp.com/gamers";

	public List<GameObject> userProfilePivots;
	// Use this for initialization
	void Start () {
		for(int i = 0; i < transform.childCount; i++) {
			userProfilePivots.Add (transform.GetChild(i).gameObject);
		}
		thisRester = GameObject.FindObjectOfType <Rester> ();
		GetProfileInfoList ();
	}
		

	// Update is called once per frame
	void Update () {
	
	}

	void GetProfileInfoList() {
		thisRester.GetJSON (profileInfoListURL, ( err, result) => {
			InterpretJSON(result);// = result;
		});
	}

	void InterpretJSON (JsonObject thisJSON) {
		object tempUsernamesObj, tempImageURLsObj;
		thisJSON.TryGetValue ("playerUsernames", out tempUsernamesObj);
		thisJSON.TryGetValue ("playerProfileURLs", out tempImageURLsObj);
		usernames = tempUsernamesObj.ToString ().Split (',');
		imageURLs = tempImageURLsObj.ToString ().Split (',');
		for (int i = 0; i < usernames.Length; i++) {
			userProfilePivots [i].transform.GetChild (0).GetComponent<ProfileCanvas> ().usernameText.text = usernames [i];
			StartCoroutine(SetProfileImage (i, imageURLs [i]));
		}
	}


	IEnumerator SetProfileImage(int index, string url) {
		// Start a download of the given URL
		WWW www = new WWW(url);

		// Wait for download to complete
		yield return www;

		// assign texture
		Texture2D temp = www.texture;
//		Image thisProfileImage = userProfilePivots [index].transform.GetChild (0).GetComponent<ProfileCanvas> ().profileImage;
		Sprite thisSprite = Sprite.Create (temp, new Rect(0,0, temp.width, temp.height), new Vector2(.5f,.5f));

		userProfilePivots [index].transform.GetChild (0).GetComponent<ProfileCanvas> ().profileImage.sprite = thisSprite;
	}
}
