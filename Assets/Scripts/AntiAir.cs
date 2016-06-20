using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AntiAir : TargetingBase {
	float detectionRange = 200f;
	float fireCooldown = .1f, cooldownTimer;
	float radarSweepTimer = .8f, radarSweepTime = 1f;
	[SerializeField] Transform turret;
	float flakDamage = 1f;
	public Transform muzzleFlash1, muzzleFlash2;

	bool isTargetingEnemy;

	// Update is called once per frame
	void Update () {
		if (abilitiesActive) {
			if (NetworkServer.FindLocalObject (currentTargetID) == null) {
				isTargetingEnemy = false;
			}
			if (cooldownTimer > 0) {
				cooldownTimer -= Time.deltaTime;
			} else if (cooldownTimer <= 0 && currentTargetGO!=null) {
				CmdFireAtTarget (currentTargetID);
				isTargetingEnemy = true;
				cooldownTimer = fireCooldown;
			}
			if (isTargetingEnemy == true ) {
				turret.transform.LookAt (currentTargetGO.transform);
			} 
			radarSweepTimer += Time.deltaTime;

			if (radarSweepTimer > radarSweepTime && currentTargetGO==null) {
				radarSweepTimer = 0;
				DetectEnemyAirUnits();

			}
		}
	}

	public override void Start () {
		targetLayerMask = 1 << LayerMask.NameToLayer ("AirUnit");
	}

	[Command]
	void CmdFireAtTarget(NetworkInstanceId thisTarget) {
		float rx = Random.Range (-10, 10);
		float ry = Random.Range (-10, 10);
		float rz = Random.Range (-10, 10);
		Vector3 displace = new Vector3 (rx, ry, rz);
		GameObject temp = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs[11],
			NetworkServer.FindLocalObject(thisTarget).transform.position + displace,
			Quaternion.identity);
		NetworkServer.FindLocalObject (thisTarget).GetComponent<UnitBase> ().TakeDamage (flakDamage);
		GameObject mzFlash1 = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs [12],
			                      muzzleFlash1.position, Quaternion.identity);
		GameObject mzFlash2 = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs [12],
			muzzleFlash2.position, Quaternion.identity);
		NetworkServer.Spawn (mzFlash1);
		NetworkServer.Spawn (mzFlash2);
		NetworkServer.Spawn (temp);

	}

	[Command]
	public override void CmdOnChangeTarget (NetworkInstanceId thisId) {
		//dont do shit
	}

	public void DetectEnemyAirUnits () {
		Collider[] collidersInRange = Physics.OverlapSphere (transform.position, detectionRange, targetLayerMask);
		if (collidersInRange.Length > 0) {
			foreach (Collider x in collidersInRange) {
				if (x.gameObject.GetComponent<Drone> ().ReturnOwner () != GetComponent<BuildingBase> ().ReturnOwner ()) {
					currentTargetID = x.GetComponent<NetworkIdentity> ().netId;
					currentTargetGO = NetworkServer.FindLocalObject (currentTargetID);
					return;
				}
			}
		} else {
			isTargetingEnemy = false;
		}

	}
}