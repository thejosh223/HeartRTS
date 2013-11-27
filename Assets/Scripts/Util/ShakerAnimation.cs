using UnityEngine;
using System.Collections;

public class ShakerAnimation : MonoBehaviour {

	public float animationDuration = 4f;
	public float animationScale = 0.25f;
	Vector3 baseScale;
	float startTime;

	void Start() {
		StartAnimation();
	}

	void Update() {
		float t = Mathf.Sin((((Time.time - startTime) % animationDuration) / animationDuration) * 2 * Mathf.PI);
		transform.localScale = baseScale + new Vector3(t, t, 0f) * animationScale;
	}

	public void StartAnimation() {
		baseScale = transform.localScale;
		startTime = Time.time;
	}

	public void Deactivate() {
		gameObject.SetActive(false);
		gameObject.transform.parent = OrganDamagePool.Instance.transform;
		gameObject.transform.localScale = baseScale;
	}
}
