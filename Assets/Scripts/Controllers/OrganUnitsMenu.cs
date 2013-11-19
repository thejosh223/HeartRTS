using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitsMenu : MonoBehaviour {

	protected Organ _activeOrgan;
	protected GameObject[] unSelectedObjects;

	//
	protected GUICamController _guiController;
	protected SelectedUnitsMenu _selectedUnitsMenu;

	void Start() {
		// References
		_guiController = transform.root.GetComponentInChildren<GUICamController>();
		_selectedUnitsMenu = transform.root.GetComponentInChildren<SelectedUnitsMenu>();

		// Instantiate lots of stuff.
		GameObject gUnselected = transform.FindChild("UnitUnselected").gameObject;
		unSelectedObjects = new GameObject[Organ.MAX_CELLS];
		int c = 0;
		for (int o = 0; o < 2; o++) {
			for (int i = 0; i < 4; i++) {
				Vector3 pos = gUnselected.transform.position + new Vector3(gUnselected.transform.localScale.x * i, gUnselected.transform.localScale.y * -o, 0);
				GameObject tg = Instantiate(gUnselected, pos, Quaternion.identity) as GameObject;
				tg.name = "BCellIdentifier_" + (o * 4 + i);
				tg.transform.parent = gUnselected.transform.parent;
				tg.SetActive(false);

				unSelectedObjects[c++] = tg;
			}
		}
		
		Destroy(gUnselected);
	}

	public void HitObject(GameObject g) {
		if (g != null) {
			for (int i = 0; i < unSelectedObjects.Length; i++) {
				if (unSelectedObjects[i] == g) {
					_selectedUnitsMenu.SetCellSelected(_cells[i]);
					_cells.Remove(_cells[i]);
				}
			}
		}
		UpdateUI();
	}

	public void SetOrgan(Organ org) {
		gameObject.SetActive(org != null);

		_activeOrgan = org;
		_cells = CellController.Instance.GetCellsAt(org);
		_selectedUnitsMenu.Clear();
		UpdateUI();
	}
	
	public void SetCellUnSelected(BCell b) {
		_cells.Add(b);
		UpdateUI();
	}

	public void UpdateUI() {
		// Disable all objects
		for (int i = 0; i < unSelectedObjects.Length; i++) 
			unSelectedObjects[i].SetActive(false);

		// Enable select objects
		for (int i = 0; i < _cells.Count; i++) 
			unSelectedObjects[i].SetActive(true);
	}
}
