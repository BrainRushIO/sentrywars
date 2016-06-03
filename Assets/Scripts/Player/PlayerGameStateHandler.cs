using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerGameStateHandler : NetworkBehaviour {

	[SerializeField] GameObject loseSphere;


	[Command]
	public void CmdPlayerLose() {
		if (isServer) {
			RpcPlayerLose ();
		}
	}

	[ClientRpc]
	void RpcPlayerLose() {
		if (isLocalPlayer) {
			GetComponent<GUIManager> ().SetAlert("Defeat", 6f);
			loseSphere.SetActive (true);
			GetComponent<ConstructionController> ().enabled = false;
			GetComponent<InputController> ().enabled = false;
			GetComponent<PlayerController> ().EndGame ();
			GameObject.Find ("Soundtrack").SetActive (false);
			GameObject.Find ("loseSound").GetComponent<AudioSource> ().Play ();

		}
	}

	[Command]
	public void CmdPlayerWin () {
		if (isServer) {
			Debug.Log ("PLAYER WIN CMD");
			RpcPlayerWin ();
		}
	}

	[ClientRpc]
	void RpcPlayerWin () {
		if (isLocalPlayer) {
			GetComponent<ConstructionController> ().enabled = false;
			GetComponent<InputController> ().enabled = false;
			GetComponent<GUIManager> ().SetAlert("Victory", 6f);
			GetComponent<PlayerController> ().EndGame ();
			GameObject.Find ("Soundtrack").SetActive (false);
			GameObject.Find ("winSound").GetComponent<AudioSource> ().Play ();
		}
	}

	[Command]
	public void CmdPlayerHit () {
		if (isServer) {
			RpcPlayerHit ();
		}
	}

	[ClientRpc]
	void RpcPlayerHit () {
		if (isLocalPlayer) {
			StartCoroutine ("FlashPlayerScreenRed");
		}
	}
	IEnumerator FlashPlayerScreenRed() {
		loseSphere.SetActive (true);
		GetComponent<SoundtrackManager> ().PlayAudioSource (GetComponent<SoundtrackManager> ().playerHit, false);
		yield return new WaitForSeconds (.12f);
		loseSphere.SetActive (false);
	}
}
