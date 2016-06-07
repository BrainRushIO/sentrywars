using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncRotation : NetworkBehaviour {

	[SyncVar] Quaternion syncObjRotation;

	[SerializeField] Transform objTransform;
	[SerializeField] float lerpRate = 15;

	void LerpRotations() {
		if (!isLocalPlayer) {
			syncObjRotation = Quaternion.Lerp (objTransform.rotation, syncObjRotation, Time.deltaTime * lerpRate);
		}
	}

	[Command]
	void CmdProvideRotationsToServer(Quaternion objRotation) {
		syncObjRotation = objRotation;
	}

	[Client]
	void TransmitRotations() {
		if (isLocalPlayer) {
			CmdProvideRotationsToServer (objTransform.rotation);
		}

	}
}
