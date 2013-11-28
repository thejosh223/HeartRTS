using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		List<Transform> childrens = new List<Transform>();
		foreach (Transform t in transform) 
			childrens.Add(t);
		backgrounds = childrens.ToArray();
//		Transform[] t = transform;
//		backgrounds = new Transform[t.Length - 1];
//		for (int i = 0, o = 0; i < t.Length; i++) 
//			if (t[i] != this.transform)
//				backgrounds[o++] = t[i];
	}
	
	void LateUpdate() {
		Vector3 parallax = (previousCamPos - cam.position) * parallaxScale;

		// For each successive background...
		for (int i = 0; i < backgrounds.Length; i++) {
			Vector3 bgTargetPos = backgrounds[i].position + parallax * (1f - backgrounds[i].localPosition.z);
			backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, bgTargetPos, smoothing * Time.deltaTime);
		}
		
		previousCamPos = cam.position;
	}
}
