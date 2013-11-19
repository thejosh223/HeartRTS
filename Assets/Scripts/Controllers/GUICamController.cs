using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUICamController : MonoBehaviour {

	//
	protected Organ _activeOrgan;

	// References
	protected Camera _mainCamera;
	protected Camera _guiCamera;
	protected OrganUnitsMenu _organUnitsMenu;
	protected SelectedUnitsMenu _selUnitsMenu;

	//
	protected List<BCell> _selectedCells = new List<BCell>();
	protected List<BCell> _cells = new List<BCell>();

	void Awake() {
		_instance = this;
	}

	void Start() {
		_mainCamera = CameraController.Instance.GetComponent<Camera>();
		_guiCamera = GetComponent<Camera>();
		
		_organUnitsMenu = transform.FindChild("OrganUnitsMenu").GetComponent<OrganUnitsMenu>();
		_selUnitsMenu = transform.FindChild("SelectedUnitsMenu").GetComponent<SelectedUnitsMenu>();
	}

	// Note: LateUpdate() is used so that BuildMode isnt set as TRUE when BuildController.Update() is called.
	void LateUpdate() {
		if (GUICamController.Instance.IsOpen()) {
			if (Input.GetMouseButtonDown(0)) {
				// Raycast
				Ray ray = _guiCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit rayHit;
				if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GUI"))) {
					string name = rayHit.transform.name;

					if (name.StartsWith(OrganUnitsMenu.PREFIX)) {
						// Clicked on Organ Units Menu
						int index = _organUnitsMenu.IndexOfHit(rayHit.transform.gameObject);

						// Open selected menu
						if (_selectedCells.Count == 0) 
							_selUnitsMenu.gameObject.SetActive(true);

						_selectedCells.Add(_cells[index]);
						_cells.Remove(_cells[index]);

						_organUnitsMenu.UpdateUI(_cells.ToArray());
						_selUnitsMenu.UpdateUI(_selectedCells.ToArray());
					} else if (name.StartsWith(SelectedUnitsMenu.PREFIX)) {
						// Clicked on Selected Units Menu
						int index = _selUnitsMenu.IndexOfHit(rayHit.transform.gameObject);
						_cells.Add(_selectedCells[index]);
						_selectedCells.Remove(_selectedCells[index]);

						// Close selected menu
						if (_selectedCells.Count == 0) 
							_selUnitsMenu.gameObject.SetActive(false);

						_organUnitsMenu.UpdateUI(_cells.ToArray());
						_selUnitsMenu.UpdateUI(_selectedCells.ToArray());
					} else if (name.StartsWith("ExitMenu")) {

					}
				} else {
					bool keepOpen = false;

					ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out rayHit)) {
						Organ org = rayHit.transform.GetComponent<Organ>();

						if (org != null) {
							if (org != _activeOrgan && org != Heart.Instance) {
								if (BCell.HasPathTo(Heart.Instance, org)) 
									SendUnitsTo(org);
							}
						} else {
							// Hit Vessel
							if (_selectedCells.Count > 0) 
								keepOpen = true;
						}
					} else {
						// Hit Nothing.
						if (_selectedCells.Count > 0) 
							keepOpen = true;
					}
					
					if (keepOpen) {
						// 1) Close OrganUnitsMenu()
						_organUnitsMenu.gameObject.SetActive(false);
						// 2) Don't Close SelectedUnitsMenu()
					} else {
						// Close Menu
						_organUnitsMenu.gameObject.SetActive(false);
						_selUnitsMenu.gameObject.SetActive(false);
						
						BuildController.Instance.isBuildMode = true;
					}
				}
			}

			if (_activeOrgan != null) {
				Vector3 viewportPoint = _mainCamera.WorldToViewportPoint(_activeOrgan.transform.position);
				Ray r = _guiCamera.ViewportPointToRay(viewportPoint);
				_organUnitsMenu.transform.position = r.GetPoint(1f); 
			}
		}
	}

	public void SendUnitsTo(Organ org) {
		for (int i = 0; i < _selectedCells.Count; i++) 
			org.OnRequestCell(_selectedCells[i]);
		_selectedCells.Clear();
	}

	public void OpenOrganMenu(Organ org) {
		BuildController.Instance.isBuildMode = false;

		_activeOrgan = org;
		_cells = CellController.Instance.GetCellsAt(org);
		_selectedCells.Clear();

		if (org != null) {
			_organUnitsMenu.gameObject.SetActive(true);
			_organUnitsMenu.UpdateUI(_cells.ToArray());
		} else {
		}
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
