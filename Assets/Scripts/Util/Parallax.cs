using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	public Transform[] backgrounds;
	public float parallaxScale = 0.5f;
	public float smoothing = 8;

	//
	private Transform cam;
	private Vector3 previousCamPos;

	void Start() {
		cam = Camera.main.transform;
		previousCamPos = cam.position;

		backgrounds = GetComponentsInChildren<Transform>();
	}
	
	void LateUpdate() {
		Vector3 parallax = (previousCamPos - cam.position) * parallaxScale;

		// For each successive background...
		for (int i = 0; i < backgrounds.Length; i++) {
			if (backgrounds[i].transform.position.z > 1)
				continue;

			Vector3 bgTargetPos = backgrounds[i].position + parallax * (1f - backgrounds[i].localPosition.z);
			backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, bgTargetPos, smoothing * Time.deltaTime);
		}
		
		previousCamPos = cam.position;
	}
}
