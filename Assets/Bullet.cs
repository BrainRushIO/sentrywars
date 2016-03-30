﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {
	string owner;
	float bulletSpeed = 1f;
	float bulletDamage = 5f;
	bool initialized;
	[SerializeField] GameObject explosionPrefab;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.forward * bulletSpeed);
	}

	public void InitializeBullet (string thisOwner) {
		owner = thisOwner;
		initialized = true;
		Destroy (gameObject, 5f);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Building" && other.GetComponent<BuildingBase>().ReturnOwner()!=owner && initialized) {
			other.GetComponent<BuildingBase> ().RpcTakeDamage (bulletDamage);
			CmdSpawnExplosion (gameObject.transform.position);
			Destroy (gameObject);
		}
	}

	[Command]
	void CmdSpawnExplosion(Vector3 thisPosition) {
		GameObject temp = (GameObject)Instantiate (explosionPrefab, thisPosition, Quaternion.identity);
		Destroy (temp, 5f);
		NetworkServer.Spawn (temp);
	}

}
