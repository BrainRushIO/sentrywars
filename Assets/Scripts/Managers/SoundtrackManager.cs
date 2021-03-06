﻿using UnityEngine;
using System.Collections;

public class SoundtrackManager : MonoBehaviour {
	
	public AudioSource warp, error, constructBuilding, cooldownOver, selectTarget, changeTarget, playerHit; //soundtrack files

	
//	IEnumerator FadeOutAudioSource(AudioSource x) { //call from elsewhere
//		while (x.volume > 0.0f) {					//where x is sound track file
//			x.volume -= 0.01f;
//			yield return new WaitForSeconds(0.03f);
//		}
//		x.Stop ();
//	}
	
	public void PlayAudioSource(AudioSource x, bool overrideIsPlaying = true) { //call from elsewhere
		if (overrideIsPlaying) {
			x.Play ();
		} else if (!overrideIsPlaying && !x.isPlaying) {
			x.Play ();

		}
	}
}