using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncTransform : NetworkBehaviour {

	[SyncVar] Vector3 syncPos;
	[SerializeField] Transform myTransform;
	[SerializeField] float lerpRate = 15;

	void Update () {
		if (isServer) {
			RpcProvidePositionToClient (myTransform.position, myTransform.rotation);
		}
//		LerpPosition();
	}

	void LerpPosition() {
//		if (isLocalPlayer) {
//			myTransform.position = syncPos;
//			myTransform.position = Vector3.Lerp (myTransform.position, syncPos, Time.deltaTime * lerpRate);
//		}
	}

	[ClientRpc]
	void RpcProvidePositionToClient(Vector3 pos, Quaternion rot) {
		if (!isServer) {
			myTransform.position = pos;
			myTransform.rotation = rot;
		}
	}

}
