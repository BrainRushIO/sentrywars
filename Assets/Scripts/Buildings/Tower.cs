using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Tower : NetworkBehaviour {
	public GameObject currentTarget;
	bool isTargetFound, abilitiesActive;
	[SerializeField] GameObject bulletPrefab;
	float towerFireRadius = 1000f;

	float fireCooldown = 3f, cooldownTimer;
	float radarSweepTimer, radarSweepTime = 1f;

	int buildingLayerMask;

	public void EnableTowerAbilities() {
		abilitiesActive = true;	
	}

	void Start () {
		buildingLayerMask = 1 << LayerMask.NameToLayer ("Buildings");
//		buildingLayerMask = ~buildingLayerMask;
	}

	// Update is called once per frame
	void Update () {
		if (abilitiesActive) {
			if (cooldownTimer > 0) {
				cooldownTimer -= Time.deltaTime;
			} else if (cooldownTimer <= 0 && currentTarget != null) {
				CmdFireAtTarget (gameObject.GetComponent<BuildingBase> ().playerCockpit.position + new Vector3 (0, -5f, 0), currentTarget.transform.position + new Vector3 (0, 15f, 0), GetComponent<BuildingBase>().ReturnOwner());
			}
			radarSweepTimer += Time.deltaTime;
			if (radarSweepTimer > radarSweepTime) {
				radarSweepTimer = 0;
				DetectEnemies ();
			}
		}

	}

	[Command]
	void CmdFireAtTarget(Vector3 thisPosition, Vector3 targetPosition, string bulletOwner) {
		GameObject tempBullet = (GameObject)Instantiate (bulletPrefab, 
			thisPosition, Quaternion.identity);
		tempBullet.transform.LookAt (targetPosition);
		tempBullet.GetComponent<Bullet> ().InitializeBullet (bulletOwner);
		cooldownTimer = fireCooldown;
		NetworkServer.Spawn (tempBullet);
	}

	public void ManualSelectTarget(GameObject thisTarget) {
		currentTarget = thisTarget;
	}

	void DetectEnemies () {
		Collider[] collidersInRange = Physics.OverlapSphere (transform.position, towerFireRadius, buildingLayerMask);
		foreach (Collider x in collidersInRange) {
			print (x.name);
			if (x.GetComponent<BuildingBase> ().ReturnOwner () != GetComponent<BuildingBase> ().ReturnOwner ()) {
				currentTarget = x.gameObject;
				break;
			}
		}
	}
}
