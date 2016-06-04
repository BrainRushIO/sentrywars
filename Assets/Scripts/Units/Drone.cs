using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Drone : UnitBase {

	public enum DroneStates {LiftOff, Search, Attack};
	DroneStates thisDroneState = DroneStates.LiftOff;
	float liftOffSpeed = .2f, liftOffTimer, liftOffTime = 3.5f;
	[SyncVar] NetworkInstanceId currentTarget;
	GameObject currentTargetGO;
	float attackRange = 200f;
	bool switchToSearch, switchToAttack;



	// STATE MACHINE
	void Update () {
		switch (thisDroneState) {
		case DroneStates.LiftOff:
			transform.Translate (Vector3.up * liftOffSpeed);
			liftOffTimer += Time.deltaTime;
			if (liftOffTimer > liftOffTime) {
				switchToSearch = true;
			}
			if (switchToSearch) {
				switchToSearch = false;
				thisDroneState = DroneStates.Search;
			}
			if (switchToAttack) {
				switchToAttack = false;
				thisDroneState = DroneStates.Attack;
			}
			break;

		case DroneStates.Search:
			if (switchToAttack) {
				switchToAttack = false;
				thisDroneState = DroneStates.Attack;
			}
			break;

		case DroneStates.Attack:
			FaceTarget ();
			transform.Translate (Vector3.forward * 4f);
			if (switchToSearch) {
				switchToSearch = false;
				thisDroneState = DroneStates.Search;

			}
			break;


		}
	}
	[Command]
	public void CmdSetCurrentTarget(NetworkInstanceId thisTarget) {
		currentTarget = thisTarget;
		switchToAttack = true;
	}

	void FaceTarget() {
		if (currentTargetGO == null) {
			currentTargetGO= NetworkServer.FindLocalObject (currentTarget);
		}
		transform.LookAt (new Vector3 (currentTargetGO.transform.position.x, transform.position.y, currentTargetGO.transform.position.z));
	}
}
