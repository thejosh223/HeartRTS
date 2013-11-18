using UnityEngine;
using System.Collections;

public class GUICamController : MonoBehaviour {

	//
	protected Organ _activeOrgan;

	// References
	protected Camera _mainCamera;
	protected Camera _guiCamera;
	protected Transform _unitsMenu;

	void Awake() {
		_instance = this;
	}

	void Start() {
		_mainCamera = CameraController.Instance.GetComponent<Camera>();
		_guiCamera = GetComponent<Camera>();

		_unitsMenu = transform.FindChild("UnitsMenu");
	}

	void Update() {
		if (_activeOrgan != null) {
			Vector3 viewportPoint = _mainCamera.WorldToViewportPoint(_activeOrgan.transform.position);
			Ray r = _guiCamera.ViewportPointToRay(viewportPoint);
			_unitsMenu.transform.position = r.GetPoint(1f); 
		}
	}

	public void OpenOrganMenu(Organ org) {
		_activeOrgan = org;
	}

	/*
	 * Singleton
	 */
	private static GUICamController _instance;

	public static GUICamController Instance {
		get { return _instance; }
	}

}
