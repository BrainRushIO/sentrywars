using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	string owner;
	float bulletSpeed = 1f;
	float bulletDamage = 5f;
	bool initialized;
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
	}

	void OnTriggerEnter(Collider other) {
		print (other.name + owner +other.GetComponent<BuildingBase>().ReturnOwner() );

		if (other.tag == "Building" && other.GetComponent<BuildingBase>().ReturnOwner()!=owner && initialized) {
			other.GetComponent<BuildingBase> ().RpcTakeDamage (bulletDamage);
			Destroy (gameObject);
		}
	}
}
