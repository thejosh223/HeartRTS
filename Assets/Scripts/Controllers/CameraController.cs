using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	// Flags
	private bool _allowCamMovement = true;

	// References
	protected Camera cam;

	// Screen Data
	private float frustrumWidth;
	private float frustrumHeight;

	// Movement Controls
	protected bool isMouseSet = false;
	protected Vector3 initCamDown;
	protected Vector3 initMouseDown;

	void Awake() {
		_instance = this;
	}

	void Start() {
		cam = GetComponent<Camera>();
		frustrumHeight = 2f * Mathf.Abs(transform.position.z) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		frustrumWidth = frustrumHeight * cam.aspect;
		Debug.Log("Frustrum = (" + frustrumWidth + ", " + frustrumHeight + ")");
	}

	void Update() {
		if (!_allowCamMovement) 
			return;

		if (Input.GetMouseButton(0)) {			
			Vector3 viewportPt = cam.ScreenToViewportPoint(Input.mousePosition);

			// Init [mouseDownAt]
			if (!isMouseSet) {
				isMouseSet = true;
				initCamDown = transform.position;
				initMouseDown = viewportPt;
			}

			// Move camera into the proper place
			Vector3 delta = viewportPt - initMouseDown;
			delta = -new Vector3(delta.x * frustrumWidth, delta.y * frustrumHeight, 0);
//			Debug.Log("Delta: " + delta);
			transform.position = initCamDown + delta;
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

	private static CameraController _instance;

	public static CameraController Instance {
		get { return _instance; }
	}
}
