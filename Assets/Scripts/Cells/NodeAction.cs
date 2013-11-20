using UnityEngine;
using System.Collections;

public class NodeAction {
	public Organ org;
	public MovementType movementType;

	public NodeAction (Organ org, MovementType move) {
		this.org = org;
		this.movementType = move;
	}
}