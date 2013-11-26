using UnityEngine;
using System.Collections;

public class Bar : MonoBehaviour {

	// Monitor
	public Transform followTrans;
	public Vector3 offset;

	//
	private Camera mainCam;
	private Camera guiCam;
	private Transform subBar;

	void Start() {
		mainCam = CameraController.Instance.camera;
		guiCam = GUICamController.Instance.camera;
		subBar = transform.FindChild("SubBar");
	}

	void Update() {
		transform.position = guiCam.ViewportToWorldPoint(mainCam.WorldToViewportPoint(followTrans.transform.position)) + offset;
	}

	public void SetValue(float val) {
		Vector3 v = subBar.localScale;
		v.x = val;
		subBar.localScale = v;
	}
}
