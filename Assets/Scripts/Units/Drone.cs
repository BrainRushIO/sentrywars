using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Drone : UnitBase {

	public enum DroneStates {InitLiftOff, Idle, FlyToTarget, Fire, RotateTowardBase, ReturnToBase, RotateTowardTarget};
	DroneStates thisDroneState = DroneStates.InitLiftOff;
	float liftOffSpeed = .5f, liftOffTimer, liftOffTime = 3.5f;
	float droneFlySpeed = 2f;
	NetworkInstanceId currentTarget;
	GameObject currentTargetGO;
	float attackRange = 200f;
	float reloadRange = 100f;
	bool switchToIdle, switchToAttack;
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] Transform bulletSpawnPoint;
	Vector3 homeBasePosition;

	Quaternion startLerp, endLerp;
	float lerpVal;


	// STATE MACHINE
	void Update () {
		if (isServer) {
			switch (thisDroneState) {
			case DroneStates.InitLiftOff:
				homeBasePosition = NetworkServer.FindLocalObject (homeBuilding).transform.position;
				transform.Translate (Vector3.up * liftOffSpeed);
				liftOffTimer += Time.deltaTime;
				if (liftOffTimer > liftOffTime) {
					switchToIdle = true;
				}
				if (switchToIdle) {
					switchToIdle = false;
					thisDroneState = DroneStates.Idle;
				}
				break;

			case DroneStates.Idle:
				if (switchToAttack) {
					switchToAttack = false;
					thisDroneState = DroneStates.FlyToTarget;

				}
				break;

			case DroneStates.FlyToTarget:
				FaceTarget ();
				transform.Translate (Vector3.forward * droneFlySpeed);
				if (switchToIdle) {
					switchToIdle = false;
					thisDroneState = DroneStates.Idle;
				}
				if (currentTargetGO == null) {
					thisDroneState = DroneStates.RotateTowardBase;
				}
				if (Vector3.Distance (transform.position, currentTargetGO.transform.position) < attackRange) {
					thisDroneState = DroneStates.Fire;
				}
				break;

			case DroneStates.Fire:
				transform.Translate (Vector3.forward * droneFlySpeed);

				CmdFireAtTarget (currentTarget, bulletSpawnPoint.position, owner);
				thisDroneState = DroneStates.RotateTowardBase;
				break;

			case DroneStates.RotateTowardBase:
				transform.Translate (Vector3.forward * droneFlySpeed);
				transform.Rotate (Vector3.up, 1f);
				if (Vector3.Angle (transform.forward, new Vector3(homeBasePosition.x, 0, homeBasePosition.z) - new Vector3(transform.position.x, 0, transform.position.z)) < 2) {
					transform.LookAt (new Vector3 (homeBasePosition.x, transform.position.y, homeBasePosition.z));
					thisDroneState = DroneStates.ReturnToBase;
				}

				break;

			case DroneStates.ReturnToBase:
				transform.Translate (Vector3.forward * droneFlySpeed);
				if (Vector3.Distance (new Vector3 (transform.position.x, 0, transform.position.z), new Vector3 (homeBasePosition.x, 0, homeBasePosition.z)) < 2f) {
					if (currentTargetGO == null) {
						thisDroneState = DroneStates.Idle;
					} else {
						thisDroneState = DroneStates.RotateTowardTarget;
					}

				}
				break;

			case DroneStates.RotateTowardTarget:
				transform.Translate (Vector3.forward * droneFlySpeed);
				transform.Rotate (Vector3.up, 1f);
				if (Vector3.Angle (transform.forward, new Vector3(currentTargetGO.transform.position.x, 0, currentTargetGO.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)) < 2) {
					thisDroneState = DroneStates.FlyToTarget;
				}
				break;
			}


		}
	}
	[Command]
	public void CmdSetCurrentTarget(NetworkInstanceId thisTarget) {
		currentTarget = thisTarget;
		switchToAttack = true;
		currentTargetGO= NetworkServer.FindLocalObject (currentTarget);
	}

	[Command]
	public void CmdSetReturnHome() {
		if (thisDroneState == DroneStates.FlyToTarget) {
			thisDroneState = DroneStates.RotateTowardBase;
		}
	}

	void FaceTarget() {
		if (currentTargetGO == null) {
			currentTargetGO= NetworkServer.FindLocalObject (currentTarget);
		}
		transform.LookAt (new Vector3 (currentTargetGO.transform.position.x, transform.position.y, currentTargetGO.transform.position.z));
	}

	[Command]
	void CmdFireAtTarget(NetworkInstanceId thisTarget, Vector3 spawnPosition, int bulletOwner) {
		GameObject temp = (GameObject) Instantiate (bulletPrefab, spawnPosition, Quaternion.identity);
		GameObject target = NetworkServer.FindLocalObject (thisTarget);
		temp.transform.LookAt (target.transform.position + Vector3.up*40f);
		temp.GetComponent<Bullet> ().InitializeBullet (bulletOwner);
		NetworkServer.Spawn (temp);
	}
}
