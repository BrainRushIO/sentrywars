using UnityEngine;
using System.Collections;

public class SoundtrackManager : MonoBehaviour {
	
	public AudioSource oceanBreeze; //soundtrack files
	public static SoundtrackManager s_instance;
	
	void Awake () {
		s_instance = this; 
		DontDestroyOnLoad (gameObject); //persist through scenes
	}
	
	IEnumerator FadeOutAudioSource(AudioSource x) { //call from elsewhere
		while (x.volume > 0.0f) {					//where x is sound track file
			x.volume -= 0.01f;
			yield return new WaitForSeconds(0.03f);
		}
		x.Stop ();
	}
	
	public void PlayAudioSource(AudioSource x) { //call from elsewhere
		x.volume = 1;
		x.Play ();
	}
}