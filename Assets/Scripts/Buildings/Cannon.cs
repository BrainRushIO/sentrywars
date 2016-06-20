using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Cannon : TargetingBase  {
	
	[SerializeField] GameObject bulletPrefab, rangeRing;
	public const float towerAttackRange = 200;
	float fireCooldown = 3f, cooldownTimer;
	float radarSweepTimer = .8f, radarSweepTime = 1f;


	void ShowRangeRing(bool show) {
		rangeRing.SetActive (show);
	}

	public override void Start () {
		targetLayerMask = 1 << LayerMask.NameToLayer ("Buildings");

		rangeRing.transform.localScale = new Vector3 (towerAttackRange/10, 1, towerAttackRange/10);
  	}

	void Update () {
		if (abilitiesActive) {
			if (cooldownTimer > 0) {
				cooldownTimer -= Time.deltaTime;
			} else if (cooldownTimer <= 0 && currentTargetGO!=null) {
				print ("FIRE");
				CmdFireAtTarget (gameObject.GetComponent<BuildingBase> ().playerCockpit.position + new Vector3 (0, -10f, 0),
					currentTargetID,
					GetComponent<BuildingBase>().ReturnOwner());
			}
			radarSweepTimer += Time.deltaTime;

			if (radarSweepTimer > radarSweepTime && currentTargetGO==null) {
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
		currentTargetID = thisId;
		currentTargetGO = NetworkServer.FindLocalObject (thisId);
	}

	[ClientRpc]
	void RpcOnChangeTarget(NetworkInstanceId thisId) {
		currentTargetGO = NetworkServer.FindLocalObject (thisId);

	}
}
