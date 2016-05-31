using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Drone : UnitBase {

	[SyncVar]
	int owner;

	public enum DroneStates {LiftOff, Search, Attack};
	DroneStates thisDroneState = DroneStates.LiftOff;
	float liftOffSpeed = .2f, liftOffTimer, liftOffTime = 3f;
	NetworkIdentity currentTarget;




	bool switchToSearch, switchToAttack;

	// Use this for initialization
	void Start () {

	}

	public void InitializeDrone (int thisOwner) {
		owner = thisOwner;
	}

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
			break;

		case DroneStates.Search:
			if (switchToAttack) {
				switchToAttack = false;
				thisDroneState = DroneStates.Attack;
			}

			break;

		case DroneStates.Attack:
			if (switchToSearch) {
				switchToSearch = false;
				thisDroneState = DroneStates.Search;

			}

			break;


		}
	}
}
