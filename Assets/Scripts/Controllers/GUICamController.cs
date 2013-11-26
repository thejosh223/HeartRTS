using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUICamController : MonoBehaviour {

	//
	protected List<Menu> activeMenus = new List<Menu>();

	// References
	protected Camera _mainCamera;
	protected Camera _guiCamera;
	protected UnitsMenu _unitMenu;
	protected UpgradesMenu _upgradeMenu;

	//
	public GameObject barPrefab;

	void Awake() {
		_instance = this;
	}

	void Start() {
		_mainCamera = CameraController.Instance.GetComponent<Camera>();
		_guiCamera = GetComponent<Camera>();

		_unitMenu = GetComponentInChildren<UnitsMenu>();
		_upgradeMenu = GetComponentInChildren<UpgradesMenu>();
	}

	void LateUpdate() {
	}
	
	public void OpenOrganMenu(Organ org) {
		_unitMenu.OpenOrganMenu(org);
		_unitMenu.MenuActive = org != null;
	}

	public void OpenUpgradeMenu(Organ org) {
		_upgradeMenu.OpenOrganMenu(org);
		_upgradeMenu.MenuActive = org != null;
	}

	/*
	 * Singleton
	 */
	private static GUICamController _instance;

	public static GUICamController Instance {
		get { return _instance; }
	}

}
