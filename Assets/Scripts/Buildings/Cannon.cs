﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Cannon : TargetingBase  {
	
	[SerializeField] GameObject bulletPrefab, rangeRing;
	public const float towerAttackRange = 200;
	float detectionRange = 200f;
	float fireCooldown = 3f, cooldownTimer;
	float radarSweepTimer = .8f, radarSweepTime = 1f;

	void ShowRangeRing(bool show) {
		rangeRing.SetActive (show);
	}

	void Start () {
		rangeRing.transform.localScale = new Vector3 (towerAttackRange/10, 1, towerAttackRange/10);
  	}

	void Update () {
		if (abilitiesActive) {
			if (cooldownTimer > 0) {
				cooldownTimer -= Time.deltaTime;
			} else if (cooldownTimer <= 0 && NetworkServer.FindLocalObject(currentTarget)!=null) {
				CmdFireAtTarget (gameObject.GetComponent<BuildingBase> ().playerCockpit.position + new Vector3 (0, -10f, 0),
					currentTarget,
					GetComponent<BuildingBase>().ReturnOwner());
			}
		
			radarSweepTimer += Time.deltaTime;

			if (radarSweepTimer > radarSweepTime && NetworkServer.FindLocalObject(currentTarget)==null) {
				radarSweepTimer = 0;
				DetectEnemyBuildings();
			}
		}
	}

	[Command]
	void CmdFireAtTarget(Vector3 thisPosition, NetworkInstanceId targetID, int bulletOwner) {
		GameObject tempBullet = (GameObject)Instantiate (bulletPrefab, 
			thisPosition, Quaternion.identity);
		GameObject target = NetworkServer.FindLocalObject (targetID);
		float randVerticality = Random.Range (0, 35);
		tempBullet.transform.LookAt (target.transform.position+ new Vector3(0,5f+randVerticality,0));
		tempBullet.GetComponent<Bullet> ().InitializeBullet (bulletOwner);
		cooldownTimer = fireCooldown;
		NetworkServer.Spawn (tempBullet);
	}

	[Command]
	public override void CmdOnChangeTarget(NetworkInstanceId thisId) {
		currentTarget = thisId;
	}
}
