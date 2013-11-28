using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	public AudioClip[] pops;
	public AudioClip heartbeatUp;
	public AudioClip heartbeatDown;

	void Awake() {
		_instance = this;
	}

	public void Play(AudioClip a, float v) {
		audio.PlayOneShot(a, v);
	}

	public void PlayPop() {
		audio.PlayOneShot(pops[Random.Range(0, pops.Length)], 0.8f);
	}

	private static AudioController _instance;
	
	public static AudioController Instance {
		get { return _instance; }
	}
}
