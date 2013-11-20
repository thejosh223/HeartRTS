using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrganPath {

	private BCell _cell;
	public List<NodeAction> path = new List<NodeAction>();
	public int currentIndex = 0;

	public OrganPath (BCell b) {
		_cell = b;
	}

	public void AddOrgan(Organ org, MovementType mov) {
		// 1) Check if Valid Organ

		// 2) Add
		path.Add(new NodeAction(org, mov));
	}

	public bool FinalizePath() {
		if (path.Count == 0)
			return false;

		// Insert the heart
		if (path[0].movementType == MovementType.Deposit) {
			// This means that the heart is the first destination
			// ie. Collect resources at Heart then transfer to another organ.
			path.Insert(0, new NodeAction(Heart.Instance, MovementType.Gather));
		} else if (path[0].movementType == MovementType.Gather) {
			// This means that the heart is NOT the first destination, but a target organ
			// ie. Collect resources at the given organ then eventually transfer to heart
			path.Add(new NodeAction(Heart.Instance, MovementType.Deposit));
		}
		return true;
	}

	public void Reset() {
		currentIndex = 0;
	}

	public void SetNextTarget() {
		NodeAction action = path[currentIndex++];

		_cell.SetTarget(null, action.org);
		_cell.MovementMode = action.movementType;
		
		// Reset path if over.
		if (currentIndex == path.Count)
			currentIndex = 0;
	}
}
