using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrganPath {

	private BCell _cell;
	public List<Organ> path = new List<Organ>();
	public int currentIndex = 0;

	public OrganPath (BCell b) {
		_cell = b;
	}

	public void AddOrgan(Organ org) {
		// 1) Check if Valid Organ

		// 2) Add
		path.Add(org);
	}
}
