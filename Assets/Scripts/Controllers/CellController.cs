using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellController : MonoBehaviour {

	public List<BCell> cells = new List<BCell>();

	public BCell InstantiateNew() {
		BCell b = new BCell();
		b.TargetOrgan = null;
		cells.Add(b);
	}

	/*
	 * Singleton
	 */
	private static CellController _instance;
	
	public static CellController Instance {
		get { return _instance; }
	}
}
