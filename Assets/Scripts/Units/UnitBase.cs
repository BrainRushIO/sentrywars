using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class UnitBase : BaseObject {


	public virtual void TakeDamage (float amount) {
		currentHealth -= amount;

		if (currentHealth < 1) {
			Destroy (gameObject);
		}
	}

	public virtual void InitializeUnit (int thisOwner) {
		owner = thisOwner;
	}
}
