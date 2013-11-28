using UnityEngine;
using System.Collections;

public class BackgroundRepeater : MonoBehaviour {

	public Camera c;

	//
	Vector3 prevDeltaCam;
	float xBound, yBound;
	Transform[] bgs;

	void Start() {
		bgs = new Transform[transform.childCount];
		int c = 0;
		foreach (Transform t in transform) 
			bgs[c++] = t;
		
		xBound = bgs[0].transform.lossyScale.x;
		yBound = bgs[0].transform.lossyScale.y;
		prevDeltaCam = this.c.transform.position - transform.position;
	}
	
	void Update() {
		// Find out the center
		Vector3 vCam = c.transform.position;
		Vector3 vBGPos = transform.position;

		Vector3 bgPos = vCam - vBGPos;
		if (Mathf.FloorToInt(bgPos.x / xBound) != Mathf.FloorToInt(prevDeltaCam.x / xBound)) {
			if (Mathf.FloorToInt(bgPos.x / xBound) < Mathf.FloorToInt(prevDeltaCam.x / xBound)) {
				for (int i = 0; i < bgs.Length; i++) {
					bgs[i].position -= new Vector3(xBound, 0, 0);
				}
			} else {
				for (int i = 0; i < bgs.Length; i++) {
					bgs[i].position += new Vector3(xBound, 0, 0);
				}
			}
		}
		if (Mathf.FloorToInt(bgPos.y / yBound) != Mathf.FloorToInt(prevDeltaCam.y / yBound)) {
			if (Mathf.FloorToInt(bgPos.y / yBound) < Mathf.FloorToInt(prevDeltaCam.y / yBound)) {
				for (int i = 0; i < bgs.Length; i++) {
					bgs[i].position -= new Vector3(0, yBound, 0);
				}
			} else {
				for (int i = 0; i < bgs.Length; i++) {
					bgs[i].position += new Vector3(0, yBound, 0);
				}
			}
		}

		prevDeltaCam = bgPos;
	}
}
