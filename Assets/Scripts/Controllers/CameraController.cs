using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	// Flags
	private bool _allowCamMovement = true;

	// References
	protected Camera cam;

	// Movement Controls
	protected Vector3 initCamDown;
	protected float initOrthoSize;
	protected bool isMouseSet = false;
	protected Vector3 initMouseDown;
	protected bool isMouse2Set = false;
	protected Vector3 initMouse2Down;

	void Awake() {
		_instance = this;
	}

	void Start() {
		cam = GetComponent<Camera>();
	}

	void Update() {
		if (!_allowCamMovement) 
			return;

//		Ray r = cam.ScreenPointToRay(Input.mousePosition);
		if (Input.GetMouseButton(0)) { // At least one button is down
			/*
			 * Data Collection
			 */
			// Variable Inits
			Vector3 viewportPt1 = Vector3.zero;
			Vector3 viewportPt2 = Vector3.zero;
			viewportPt1 = cam.ScreenToViewportPoint(Input.mousePosition);

			// Init [mouseDownAt]
			if (!isMouseSet) {
				isMouseSet = true;
				initCamDown = transform.position;
				initMouseDown = viewportPt1;
				initOrthoSize = cam.orthographicSize;
			}

			// Multitouch Pinch
			if (Input.touchCount >= 2 || Input.GetKey(KeyCode.LeftShift)) {
				// ACTUAL CODE (for Android)
//				Vector2 touch2 = Input.GetTouch(0).position;
//				viewportPt2 = cam.ScreenToViewportPoint(new Vector3(touch2.x, touch2.y, 0));

				// DEBUG FOR PC
				viewportPt2 = new Vector3(0.5f, 0.5f, 0);

				if (!isMouse2Set) {
					isMouse2Set = true;
					initMouse2Down = viewportPt2;
				}
			} else {
				isMouse2Set = false;
			}

			/*
			 * Moving the actual camera
			 */
			// Zoom Camera
//			float newZ = initCamDown.z;
			float orthoMult = 1f;

			if (isMouse2Set) {
				float initDist = Vector3.Distance(initMouseDown, initMouse2Down);
				float newDist = Vector3.Distance(viewportPt1, viewportPt2);
				orthoMult = initDist / newDist;
			}
			
			// Move camera into the proper place
			float frustrumHeight = 2 * cam.orthographicSize;
			float frustrumWidth = frustrumHeight * cam.aspect;

			Vector3 delta = viewportPt1 - initMouseDown;
			delta.x *= frustrumWidth;
			delta.y *= frustrumHeight;
			delta.z = 0;
			Vector3 newPos = initCamDown - delta;
//			newPos.z = newZ;
			cam.orthographicSize = initOrthoSize * orthoMult;
			transform.position = newPos;
		}

		if (Input.GetMouseButtonUp(0)) {
			isMouseSet = false;
		}
	}

	public void SetCamMovable(bool b) {
		_allowCamMovement = b;
		if (b) {
		} else {
			isMouseSet = false;
		}
	}

	public Camera Cam {
		get { return cam; }
	}

	private static CameraController _instance;

	public static CameraController Instance {
		get { return _instance; }
	}
}
