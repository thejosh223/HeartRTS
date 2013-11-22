using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static int vesselCounter = 0;

	void Awake() {
		LeanTween.init(64);
	}

	void Start() {
	}
	
	private static Vector3 RandomDelta() {
		return new Vector3(Random.Range(4f, 8f), Random.Range(-6f, 6f), 0);
	}
}
