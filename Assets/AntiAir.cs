using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AntiAir : TargetingBase {
	float detectionRange = 200f;
	float fireCooldown = .3f, cooldownTimer;
	float radarSweepTimer = .8f, radarSweepTime = 1f;
	[SerializeField] Transform turret;
	float flakDamage = 4f;
	GameObject currentTargetGO;

	bool isTargetingEnemy;

	// Update is called once per frame
	void Update () {
		if (abilitiesActive) {
			if (NetworkServer.FindLocalObject (currentTarget) == null) {
				isTargetingEnemy = false;
			}
			if (cooldownTimer > 0) {
				cooldownTimer -= Time.deltaTime;
			} else if (cooldownTimer <= 0 && currentTargetGO!=null) {
				CmdFireAtTarget (currentTarget);
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
		print ("Attacking drone");
		float rx = Random.Range (-40, 40);
		float ry = Random.Range (-40, 40);
		float rz = Random.Range (-40, 40);
		Vector3 displace = new Vector3 (rx, ry, rz);
		GameObject temp = (GameObject)Instantiate (NetworkManager.singleton.spawnPrefabs[11],
			NetworkServer.FindLocalObject(thisTarget).transform.position + displace,
			Quaternion.identity);
		NetworkServer.FindLocalObject (thisTarget).GetComponent<UnitBase> ().TakeDamage (flakDamage);
	}

	[Command]
	public override void CmdOnChangeTarget (NetworkInstanceId thisId) {
		//dont do shit
	}

	public void DetectEnemyAirUnits () {
		Collider[] collidersInRange = Physics.OverlapSphere (transform.position, detectionRange, targetLayerMask);
		if (collidersInRange.Length > 0) {
			foreach (Collider x in collidersInRange) {
				currentTarget = x.GetComponent<NetworkIdentity> ().netId;
				currentTargetGO = NetworkServer.FindLocalObject (currentTarget);
				return;
			}
		} else {
			isTargetingEnemy = false;
		}

	}
}