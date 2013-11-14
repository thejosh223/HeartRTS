using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BCell {

	public Vessel finalTarget;
	public Vessel nextTarget;
	public Vessel currentVessel;

	public BCell () {
	}

	public void OnVesselEnter(Vessel v) {
		currentVessel = v;
		if (v == finalTarget) {
			nextTarget = null;
			return;
		}
		if (v == nextTarget) 
			nextTarget = SearchNextTarget();
	}

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

	public void SetTarget(Vessel current, Vessel v) {
		currentVessel = current;
		finalTarget = v;
		nextTarget = SearchNextTarget();

//		Debug.Log("Final Target: " + finalTarget.name);
//		Debug.Log("Next Target: " + nextTarget.name);
	}

	public BCell[] Divide(int count) {
		BCell[] b = new BCell[count];
		b[0] = this;
		for (int i = 1; i < b.Length; i++)
			b[i] = new BCell();
		return b;
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
