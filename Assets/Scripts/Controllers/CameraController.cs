using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	protected Camera cam;
	protected float dragSpeed = 22;
	protected bool isMouseSet = false;
	protected Vector3 prevMouseDown = Vector3.zero;
	protected Plane xyZeroPlane;

	// Screen Data
	private float frustrumWidth;
	private float frustrumHeight;

	// TEST
	protected Vector3 initCamDown;
	protected Vector3 initMouseDown;

	void Start() {
		xyZeroPlane = new Plane(Vector3.forward, Vector3.zero);

		cam = GetComponent<Camera>();
		frustrumHeight = 2f * Mathf.Abs(transform.position.z) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		frustrumWidth = frustrumHeight * cam.aspect;
		Debug.Log("Frustrum = (" + frustrumWidth + ", " + frustrumHeight + ")");
	}

	void Update() {
		if (Input.GetMouseButton(0)) {
//			if (!isMouseSet) {
//				isMouseSet = true;
//				prevMouseDown = Input.mousePosition;
//			}
			
//			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - prevMouseDown);
//			Vector3 move = - new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
//			transform.Translate(move, Space.World);  
//			prevMouseDown = Input.mousePosition;


			// The right method!!!
//			Vector3 v = Vector3.zero;
//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			float distance = 0;
//			if (xyZeroPlane.Raycast(ray, out distance)) 
//				v = ray.GetPoint(distance);
//			Debug.Log("V: " + v);
			
			Vector3 viewportPt = cam.ScreenToViewportPoint(Input.mousePosition);
			Debug.Log("Viewport: " + viewportPt);

			// Init [mouseDownAt]
			if (!isMouseSet) {
				isMouseSet = true;
//				prevMouseDown = v;

				initCamDown = transform.position;
				initMouseDown = viewportPt;
			}

			// Move camera into the proper place
			Vector3 delta = viewportPt - initMouseDown;
			delta = -new Vector3(delta.x * frustrumWidth, delta.y * frustrumHeight, 0);
			Debug.Log("Delta: " + delta);
			transform.position = initCamDown + delta;
		}

		if (Input.GetMouseButtonUp(0)) {
			isMouseSet = false;
		}
	}
}
