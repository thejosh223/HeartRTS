using UnityEngine;
using System.Collections;

public class GUICamController : MonoBehaviour {

	//
	protected Organ _activeOrgan;

	// References
	protected Camera _mainCamera;
	protected Camera _guiCamera;
	protected UnitsMenu _unitsMenu;

	void Awake() {
		_instance = this;
	}

	void Start() {
		_mainCamera = CameraController.Instance.GetComponent<Camera>();
		_guiCamera = GetComponent<Camera>();

		_unitsMenu = transform.FindChild("UnitsMenu").GetComponent<UnitsMenu>();
		_unitsMenu.gameObject.SetActive(false);
	}

	void Update() {
		if (GUICamController.Instance.IsOpen()) {
			if (Input.GetMouseButtonDown(0)) {
				// Raycast
				Ray ray = _guiCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit rayHit;
				if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GUI"))) {
					if (rayHit.transform.name.StartsWith("BCellIdentifier_")) {
						_unitsMenu.HitObject(rayHit.transform.gameObject);
					}
				} else {
					ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out rayHit)) {
						Organ org = rayHit.transform.GetComponent<Organ>();

						if (org != null) {
							if (org != _activeOrgan && org != Heart.Instance) {
								if (BCell.HasPathTo(Heart.Instance, org)) {
									_unitsMenu.SendUnitsTo(org);
								}
							}
						} else {
							SetUnitsMenu(false);
						}
					} else {
						SetUnitsMenu(false);
					}
				}
			}

			if (_activeOrgan != null) {
				Vector3 viewportPoint = _mainCamera.WorldToViewportPoint(_activeOrgan.transform.position);
				Ray r = _guiCamera.ViewportPointToRay(viewportPoint);
				_unitsMenu.transform.position = r.GetPoint(1f); 
			}
		}
	}

	public void OpenOrganMenu(Organ org) {
		_activeOrgan = org;
		_unitsMenu.SetOrgan(org);

		if (org != null) {
			SetUnitsMenu(true);
		} else {
		}
	}

	public void SetUnitsMenu(bool b) {
		_unitsMenu.gameObject.SetActive(b);
		BuildController.Instance.isBuildMode = !b;
	}

	public bool IsOpen() {
		return _activeOrgan != null;
	}

	/*
	 * Singleton
	 */
	private static GUICamController _instance;

	public static GUICamController Instance {
		get { return _instance; }
	}

}
