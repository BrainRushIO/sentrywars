using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Drone : UnitBase {

	public enum DroneStates {LiftOff, Search, Attack, Fire, RotateTowardBase, ReturnToBase, RotateTowardTarget};
	DroneStates thisDroneState = DroneStates.LiftOff;
	float liftOffSpeed = .2f, liftOffTimer, liftOffTime = 3.5f;
	float droneFlySpeed = 1f;
	[SyncVar] NetworkInstanceId currentTarget;
	GameObject currentTargetGO;
	float attackRange = 200f;
	float reloadRange = 100f;
	bool switchToSearch, switchToAttack;
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] Transform bulletSpawnPoint;
	Vector3 homeBasePosition;

	Quaternion startLerp, endLerp;
	float lerpVal;


	// STATE MACHINE
	void Update () {
		if (isServer) {
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
				transform.Translate (Vector3.forward * droneFlySpeed);
				if (switchToSearch) {
					switchToSearch = false;
					thisDroneState = DroneStates.Search;

				}
				if (Vector3.Distance (transform.position, currentTargetGO.transform.position) < attackRange) {
					thisDroneState = DroneStates.Fire;
				}
				break;

			case DroneStates.Fire:
				transform.Translate (Vector3.forward * droneFlySpeed);

				CmdFireAtTarget (currentTarget, bulletSpawnPoint.position, owner);
				homeBasePosition = NetworkServer.FindLocalObject (homeBuilding).transform.position;
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
				break;
			}
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

	[Command]
	void CmdFireAtTarget(NetworkInstanceId thisTarget, Vector3 spawnPosition, int bulletOwner) {
		GameObject temp = (GameObject) Instantiate (bulletPrefab, spawnPosition, Quaternion.identity);
		GameObject target = NetworkServer.FindLocalObject (thisTarget);
		temp.transform.LookAt (target.transform.position);
		temp.GetComponent<Bullet> ().InitializeBullet (bulletOwner);
		NetworkServer.Spawn (temp);

	}
}
