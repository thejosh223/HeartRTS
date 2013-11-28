using UnityEngine;
using System.Collections;

public class Animatulator : MonoBehaviour {

//	public bool isModelBased = true;

	// Enablers for the animation types
	public bool hasPositionAnimation = false;
	public bool hasScaleAnimation = false;

	//
	public Vector3 basePosition;
	public Vector3 baseScale;

	//
	public Vector3 deltaPosition;
	public Vector3 deltaScale;

	void Start() {
		basePosition = transform.position;
		baseScale = transform.localScale;
	}

	void LateUpdate() {
		if (hasPositionAnimation) {
			transform.position = basePosition + deltaPosition;
			deltaPosition = Vector3.zero;
		}

		if (hasScaleAnimation) {
			transform.localScale = baseScale + deltaScale;
			deltaScale = Vector3.zero;
		}
	}
}
