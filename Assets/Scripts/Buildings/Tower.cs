using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Tower : NetworkBehaviour {
	public List<GameObject> currentTargetsInRange = new List<GameObject>();
	GameObject currentTarget;
	bool isTargetFound, abilitiesActive;
	[SerializeField] GameObject bulletPrefab;

	float fireCooldown = 3f, cooldownTimer;

	public void EnableTowerAbilities() {
		abilitiesActive = true;	
	}

	// Update is called once per frame
	void Update () {
		if (abilitiesActive) {
			if (cooldownTimer > 0) {
				cooldownTimer -= Time.deltaTime;
			} else if (cooldownTimer <= 0 && currentTarget != null) {
				CmdFireAtTarget (gameObject.GetComponentInParent<BuildingBase> ().playerCockpit.position + new Vector3 (0, -5f, 0), currentTarget.transform.position + new Vector3 (0, 15f, 0));
			}
			AutoSelectTarget ();
		}
	}

	[Command]
	void CmdFireAtTarget(Vector3 thisPosition, Vector3 targetPosition) {
		CleanTargetList ();
		GameObject tempBullet = (GameObject)Instantiate (bulletPrefab, 
			thisPosition, Quaternion.identity);
		tempBullet.transform.LookAt (targetPosition);
		tempBullet.GetComponent<Bullet> ().InitializeBullet (GetComponentInParent<BuildingBase>().ReturnOwner());
		cooldownTimer = fireCooldown;
		NetworkServer.Spawn (tempBullet);
	}

	void CleanTargetList() {
		if (currentTargetsInRange.Count > 0 && currentTargetsInRange [0] == null) {
			currentTargetsInRange.Remove (currentTargetsInRange [0]);
		}
	}

	void AutoSelectTarget() {
		if (currentTargetsInRange.Count!=0) {
			currentTarget = currentTargetsInRange [0];
		}
	}

	public void ManualSelectTarget(GameObject thisTarget) {

	}


	void OnTriggerStay(Collider other) {
		if (other.GetComponentInParent<BuildingBase> () != null) {
			if (other.GetComponentInParent<BuildingBase> ().ReturnOwner() != GetComponentInParent<BuildingBase> ().ReturnOwner()) {
				if (!currentTargetsInRange.Contains(other.gameObject)) {
					currentTargetsInRange.Add (other.gameObject);
				}
			}
		}
	}
}
