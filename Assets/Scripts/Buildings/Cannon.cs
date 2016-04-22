﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Cannon : NetworkBehaviour {
	
	[SyncVar] NetworkInstanceId currentTarget;
	bool isTargetFound, abilitiesActive;
	[SerializeField] GameObject bulletPrefab;
	public const float towerAttackRange = 100;

	float fireCooldown = 3f, cooldownTimer;
	float radarSweepTimer, radarSweepTime = 1f;
	int buildingLayerMask;
	bool changeTarget;

	public void EnableTowerAbilities() {
		abilitiesActive = true;	
	}
	public void DisableTowerAbilities () {
		abilitiesActive = false;	
	}

	void Start () {
		buildingLayerMask = 1 << LayerMask.NameToLayer ("Buildings");
  	}

	void Update () {
		if (abilitiesActive) {
			if (cooldownTimer > 0) {
				cooldownTimer -= Time.deltaTime;
			} else if (cooldownTimer <= 0 && NetworkServer.FindLocalObject(currentTarget)!=null) {
				CmdFireAtTarget (gameObject.GetComponent<BuildingBase> ().playerCockpit.position + new Vector3 (0, -15f, 0),
					currentTarget,
					GetComponent<BuildingBase>().ReturnOwner(), GetComponent<BuildingBase>().ReturnOwnerNetID());
			}
			radarSweepTimer += Time.deltaTime;

			if (radarSweepTimer > radarSweepTime && NetworkServer.FindLocalObject(currentTarget)==null) {
				radarSweepTimer = 0;
				DetectEnemies ();
			}
		}
	}

	[Command]
	void CmdFireAtTarget(Vector3 thisPosition, NetworkInstanceId targetID, int bulletOwner, NetworkIdentity thisOwnerID) {
		GameObject tempBullet = (GameObject)Instantiate (bulletPrefab, 
			thisPosition, Quaternion.identity);
		GameObject target = NetworkServer.FindLocalObject (targetID);
		tempBullet.transform.LookAt (target.transform.position+ new Vector3(0,5f,0));
		tempBullet.GetComponent<Bullet> ().InitializeBullet (bulletOwner, thisOwnerID);
		cooldownTimer = fireCooldown;
		NetworkServer.SpawnWithClientAuthority (tempBullet, GameManager.players[bulletOwner].gameObject);
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
