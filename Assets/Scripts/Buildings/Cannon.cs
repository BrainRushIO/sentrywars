using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Cannon : NetworkBehaviour {
	
	[SyncVar] NetworkInstanceId currentTarget;
	bool isTargetFound, abilitiesActive;
	[SerializeField] GameObject bulletPrefab, rangeRing;
	public const float towerAttackRange = 100;

	float fireCooldown = 3f, cooldownTimer;
	float radarSweepTimer = .8f, radarSweepTime = 1f;
	int buildingLayerMask;
	bool changeTarget;

	void ShowRangeRing(bool show) {
		rangeRing.SetActive (show);
	}

	public void EnableTowerAbilities() {
		abilitiesActive = true;
	}
	public void DisableTowerAbilities () {
		abilitiesActive = false;	
	}

	void Start () {
		rangeRing.transform.localScale = new Vector3 (towerAttackRange/10, 1, towerAttackRange/10);
		buildingLayerMask = 1 << LayerMask.NameToLayer ("Buildings");
  	}

	void Update () {
		if (abilitiesActive &&hasAuthority) {
			if (cooldownTimer > 0) {
				cooldownTimer -= Time.deltaTime;
			} else if (cooldownTimer <= 0 && NetworkServer.FindLocalObject(currentTarget)!=null) {
				CmdFireAtTarget (gameObject.GetComponent<BuildingBase> ().playerCockpit.position + new Vector3 (0, -15f, 0),
					currentTarget,
					GetComponent<BuildingBase>().ReturnOwner());
			}
		
			radarSweepTimer += Time.deltaTime;

			if (radarSweepTimer > radarSweepTime && NetworkServer.FindLocalObject(currentTarget)==null) {
				radarSweepTimer = 0;
				DetectEnemies ();
			}
		}
	}

	[Command]
	void CmdFireAtTarget(Vector3 thisPosition, NetworkInstanceId targetID, int bulletOwner) {
		GameObject tempBullet = (GameObject)Instantiate (bulletPrefab, 
			thisPosition, Quaternion.identity);
		GameObject target = NetworkServer.FindLocalObject (targetID);
		float randVerticality = Random.Range (0, 20);
		tempBullet.transform.LookAt (target.transform.position+ new Vector3(0,5f+randVerticality,0));
		tempBullet.GetComponent<Bullet> ().InitializeBullet (bulletOwner);
		cooldownTimer = fireCooldown;
		NetworkServer.Spawn (tempBullet);
	}

	[Command]
	public void CmdOnChangeTarget(NetworkInstanceId thisId) {
		Debug.Log ("CHANGE TARGET " + thisId);
		currentTarget = thisId;
	}
				
	void DetectEnemies () {
		Collider[] collidersInRange = Physics.OverlapSphere (transform.position, towerAttackRange, buildingLayerMask);
		foreach (Collider x in collidersInRange) {
			if (x.GetComponent<BuildingBase> ().ReturnOwner () != GetComponent<BuildingBase> ().ReturnOwner ()) {
				currentTarget = x.GetComponent<NetworkIdentity>().netId;
				break;
			}
		}
	}
}
