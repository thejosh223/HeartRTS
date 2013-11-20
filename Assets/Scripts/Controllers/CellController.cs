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
		cells.Add(b);
		return b;
	}

	public List<BCell> GetCellsAt(Organ org) {
		if (org == Heart.Instance)
			org = null;

		List<BCell> l = new List<BCell>();
		for (int i = 0; i < cells.Count; i++) {
			NodeAction[] actions = cells[i].organPath.path.ToArray();

			if (actions.Length == 0) {
				if (org == null) {
					l.Add(cells[i]);
				}
			} else {
				for (int o = 0; o < actions.Length; o++) {
					if (actions[o].org == org) {
						l.Add(cells[i]);
					}
				}
			}
		}
		return l;
	}

	/*
	 * Singleton
	 */
	private static CellController _instance;
	
	public static CellController Instance {
		get { return _instance; }
	}
}
