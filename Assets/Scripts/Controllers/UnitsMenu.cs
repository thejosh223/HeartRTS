﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitsMenu : MonoBehaviour {

	protected Organ _activeOrgan;
	protected GameObject[] unitObjects;

	//
	protected List<BCell> _cells = new List<BCell>();
	protected List<BCell> _selectedCells = new List<BCell>();

	void Start() {
		// Instantiate lots of stuff.
		GameObject g = transform.FindChild("Unit").gameObject;
		unitObjects = new GameObject[Organ.MAX_CELLS];
		int c = 0;
		for (int o = 0; o < 2; o++) {
			for (int i = 0; i < 4; i++) {
				Vector3 pos = g.transform.position + new Vector3(g.transform.localScale.x * i, g.transform.localScale.y * -o, 0);
				GameObject tg = Instantiate(g, pos, Quaternion.identity) as GameObject;
				tg.name = "BCellIdentifier_" + (o * 4 + i);
				tg.transform.parent = g.transform.parent;
				tg.SetActive(false);

				unitObjects[c++] = tg;
			}
		}
		Destroy(g);
	}

	public void SendUnitsTo(Organ org) {
		for (int i = 0; i < _selectedCells.Count; i++) {
			org.OnRequestCell(_selectedCells[i]);
		}
		_selectedCells.Clear();
	}

	public void HitObject(GameObject g) {
		if (g != null) {
			for (int i = 0; i < unitObjects.Length; i++) {
				if (unitObjects[i] == g) {
					_selectedCells.Add(_cells[i]);
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
		_selectedCells.Clear();
		UpdateUI();
	}

	public void UpdateUI() {
		for (int i = 0; i < unitObjects.Length; i++) 
			unitObjects[i].SetActive(false);
		for (int i = 0; i < _cells.Count; i++) 
			unitObjects[i].SetActive(true);
	}
}
