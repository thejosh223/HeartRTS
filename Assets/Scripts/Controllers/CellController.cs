using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellController : MonoBehaviour {

	public List<BCell> cells = new List<BCell>();

	void Awake() {
		_instance = this;
	}

	public BCell InstantiateNew() {
		BCell b = new BCell();
		b.TargetOrgan = null;
		cells.Add(b);
		return b;
	}

	public BCell[] GetCellsAt(Organ org) {
		if (org == Heart.Instance)
			org = null;

		List<BCell> l = new List<BCell>();
		for (int i = 0; i < cells.Count; i++) {
			if (cells[i].TargetOrgan == org) {
				l.Add(cells[i]);
			}
		}
		return l.ToArray();
	}

	/*
	 * Singleton
	 */
	private static CellController _instance;
	
	public static CellController Instance {
		get { return _instance; }
	}
}
