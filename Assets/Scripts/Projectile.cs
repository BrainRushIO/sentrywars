using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {

	[SerializeField] float damage = 5;

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Building") {
			other.GetComponent<BuildingBase> ().RpcTakeDamage (damage);
		}
	}

	void DestroyProjectile () {
		Destroy (gameObject);
	}


}
