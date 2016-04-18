﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Cannon : NetworkBehaviour {
	[SyncVar] NetworkInstanceId currentTarget;
	bool isTargetFound, abilitiesActive;
	[SerializeField] GameObject bulletPrefab;
	float towerFireRadius = 100;

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

	// Update is called once per frame
	void Update () {
		if (abilitiesActive) {
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
		if (changeTarget) {
			ChangeTarget(GameManager.players [GetComponent<BuildingBase> ().ReturnOwner ()].ReturnCurrentTarget().GetComponent<NetworkIdentity>());
			changeTarget = false;
		}

	}

	[Command]
	void CmdFireAtTarget(Vector3 thisPosition, NetworkInstanceId targetID, int bulletOwner) {
		GameObject tempBullet = (GameObject)Instantiate (bulletPrefab, 
			thisPosition, Quaternion.identity);
		GameObject target = NetworkServer.FindLocalObject (targetID);
		tempBullet.transform.LookAt (target.transform.position+ new Vector3(0,5f,0));
		tempBullet.GetComponent<Bullet> ().InitializeBullet (bulletOwner);
		cooldownTimer = fireCooldown;
		NetworkServer.Spawn (tempBullet);
	}

	void ChangeTarget(NetworkIdentity thisId) {
		Debug.Log ("stuck");
		currentTarget = thisId.netId;
	}
	public void TargetNewBuilding() {
		changeTarget = true;
	}
				
	void DetectEnemies () {
		Collider[] collidersInRange = Physics.OverlapSphere (transform.position, towerFireRadius, buildingLayerMask);
		foreach (Collider x in collidersInRange) {
			if (x.GetComponent<BuildingBase> ().ReturnOwner () != GetComponent<BuildingBase> ().ReturnOwner ()) {
				currentTarget = x.GetComponent<NetworkIdentity>().netId;
				break;
			}
		}
	}
}
