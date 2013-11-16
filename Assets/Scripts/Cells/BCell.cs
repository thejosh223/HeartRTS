using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BCell {

	// Movement Values
	public Vessel finalTarget;
	public Vessel nextTarget;
	public Vessel currentVessel;
	
	//
	public Organ targetOrgan;
	private float _carryingEnergy;
	private float _maxEnergy = 10f;
	public MovementType _movementMode;

	public BCell () {
	}

	public void OnVesselEnter(Vessel v) {
		// Basics
		currentVessel = v;
		if (v == finalTarget) {
			nextTarget = null;
			return;
		}
		if (v == nextTarget) 
			nextTarget = SearchNextTarget();

		// Other
		if (v is Organ) {
			targetOrgan = (Organ)v;
		}
	}

	/*
	 * Movement
	 */
	public Vessel SearchNextTarget() {
		List<Node> openSet = new List<Node>();
		List<Node> closedSet = new List<Node>();

		openSet.Add(new Node(currentVessel, 0f, Vector3.Distance(currentVessel.transform.position, finalTarget.transform.position)));
		while (openSet.Count > 0) {
			// Find the Node with the lowest F
			int lowestIndex = 0;
			for (int i = 1; i < openSet.Count; i++) {
				if (openSet[i].f < openSet[lowestIndex].f) {
					lowestIndex = i;
				}
			}
				
			Node current = openSet[lowestIndex];
			openSet.RemoveAt(lowestIndex);
			closedSet.Add(current);
				
			if (current.pos == finalTarget) {
				// Found!
				while (current.parent.parent != null) 
					current = current.parent;
				return current.pos;
			}

			Vessel[] neighbors = current.pos.GetNextNodes();
			for (int i = 0; i < neighbors.Length; i++) {
				float tentative_g = current.g + Vector3.Distance(neighbors[i].transform.position, current.pos.transform.position);
					
				// Check if the node has already been visited
				bool found = false;
				for (int j = 0; j < closedSet.Count; j++) {
					if (closedSet[j].pos == neighbors[i]) {
						found = true;
						break;
					}
				}
				if (found)
					continue;
					
				// Check if neighbor is in openSet
				bool inOpenSet = false;
				int o = 0;
				for (o = 0; o < openSet.Count; o++) {
					if (openSet[o].pos == neighbors[i]) {
						inOpenSet = true;
						break;
					}
				}
					
				Node n;
				if (!inOpenSet || tentative_g < current.g) {
					if (!inOpenSet) {
						n = new Node(neighbors[i], tentative_g, tentative_g + //
							Vector3.Distance(neighbors[i].transform.position, finalTarget.transform.position));
						openSet.Add(n);
					} else {
						n = openSet[o];
					}
					n.parent = current;
					n.g = tentative_g;
					n.f = tentative_g + Vector3.Distance(neighbors[i].transform.position, finalTarget.transform.position);
				}
			}
		}
		return null;
	}

	public bool SetTarget(Vessel current, Organ v) {
		currentVessel = current;
		finalTarget = v;
		nextTarget = SearchNextTarget();

		if (nextTarget == null)
			return false;
		return true;
	}
	
	public bool IsAtDestination() {
		return currentVessel == finalTarget;
	}

	/*
	 * Setters
	 */
	public float CurrentEnergy {
		get { return _carryingEnergy; }
		set { 
			_carryingEnergy = value; 
		}
	}

	public float MaxEnergy {
		get { return _maxEnergy; }
	}

	public MovementType MovementMode {
		get { return _movementMode; }
		set { _movementMode = value; }
	}

	/*
	 * A Star
	 */
	public static bool HasPathTo(Vessel start, Vessel end) {
		List<Node> openSet = new List<Node>();
		List<Node> closedSet = new List<Node>();
		
		openSet.Add(new Node(start, 0f, Vector3.Distance(start.transform.position, end.transform.position)));
		while (openSet.Count > 0) {
			// Find the Node with the lowest F
			int lowestIndex = 0;
			for (int i = 1; i < openSet.Count; i++) {
				if (openSet[i].f < openSet[lowestIndex].f) {
					lowestIndex = i;
				}
			}
			
			Node current = openSet[lowestIndex];
			openSet.RemoveAt(lowestIndex);
			closedSet.Add(current);
			
			if (current.pos == end) {
				// Found!
				return true;
			}
			
			Vessel[] neighbors = current.pos.GetNextNodes();
			for (int i = 0; i < neighbors.Length; i++) {
				float tentative_g = current.g + Vector3.Distance(neighbors[i].transform.position, current.pos.transform.position);
				
				// Check if the node has already been visited
				bool found = false;
				for (int j = 0; j < closedSet.Count; j++) {
					if (closedSet[j].pos == neighbors[i]) {
						found = true;
						break;
					}
				}
				if (found)
					continue;
				
				// Check if neighbor is in openSet
				bool inOpenSet = false;
				int o = 0;
				for (o = 0; o < openSet.Count; o++) {
					if (openSet[o].pos == neighbors[i]) {
						inOpenSet = true;
						break;
					}
				}
				
				Node n;
				if (!inOpenSet || tentative_g < current.g) {
					if (!inOpenSet) {
						n = new Node(neighbors[i], tentative_g, tentative_g + //
							Vector3.Distance(neighbors[i].transform.position, end.transform.position));
						openSet.Add(n);
					} else {
						n = openSet[o];
					}
					n.parent = current;
					n.g = tentative_g;
					n.f = tentative_g + Vector3.Distance(neighbors[i].transform.position, end.transform.position);
				}
			}
		}
		return false;
	}

	protected class Node {
		public Node parent;
		public Vessel pos;
		public float g;
		public float f;
			
		public Node (Vessel pos) {
			this.pos = pos;
			g = 0;
			f = 0;
			parent = null;
		}
			
		public Node (Vessel pos, float g, float f) {
			this.pos = pos;
			this.g = g;
			this.f = f;
			parent = null;
		}
	}
}
