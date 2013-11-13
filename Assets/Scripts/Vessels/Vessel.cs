using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vessel : MonoBehaviour {

	// Graph
	protected List<VesselConnection> attachedNodes = new List<VesselConnection>();

	/*
	 * Init Functions
	 */
	protected virtual void Awake() {
	}

	protected virtual void Start() {
	}

	/*
	 * Get Vessel Endpoints
	 * -aka. the graph nodes
	 */
	public virtual Vessel[] GetNextNodes() {
		Vessel[] v = new Vessel[attachedNodes.Count];
		for (int i = 0; i < v.Length; i++) 
			v[i] = attachedNodes[i].node;
		return v;
	}

	/*
	 * Helper Functions
	 */
	public Vessel GetImmediateVesselTo(Vessel v) {
		for (int i = 0; i < attachedNodes.Count; i++) 
			if (attachedNodes[i].node == v)
				return attachedNodes[i].NextImmediateVessel();
		return null;
	}

	public virtual void AttachVessel(Vessel v) {
		// Create Segments
		GameObject vesselPrefab = Heart.Instance.vSegmentPrefab;

		Vector3 prevPos = v.transform.position;
		VesselSegment vFirst = null; 
		VesselSegment vTemp = null;

		float dist = Vector3.Distance(transform.position, prevPos);
		int numVessels = (int)(dist / vesselPrefab.transform.localScale.x); // Note: localscale for prefab == lossyscale in game
		float deltaX = (transform.position.x - prevPos.x) / numVessels;
		float deltaY = (transform.position.y - prevPos.y) / numVessels;
		for (int i = 0; i < numVessels; i++) {
			// Instantiate
			GameObject g = Instantiate(vesselPrefab, //
				new Vector3(prevPos.x + deltaX * i + deltaX / 2, prevPos.y + deltaY * i + deltaY / 2, 0), //
				Quaternion.identity) as GameObject;
			g.transform.parent = transform.parent;
			VesselSegment v2 = g.GetComponent<VesselSegment>();

			// point current segment (newly instantiated) backward
			v2.AttachSegment(v, vTemp);

			// point previous segment forward
			if (vTemp != null)
				vTemp.AttachSegment(this, v2);

			// move the list
			vTemp = v2;

			// save the first segment!
			if (vFirst == null) 
				vFirst = v2;
		}
		// point final segment to target destination
		vTemp.AttachSegment(this, null);

		// Add to List
		attachedNodes.Add(new VesselConnection(v, vTemp)); // null -> first one from this node to v
		v.attachedNodes.Add(new VesselConnection(this, vFirst)); // null -> last one from this node to v
	}

	/*
	 * Cell Entering / Exiting Functions
	 * -call this when you want a cell to enter/exit the vessel
	 */
	public virtual void BCellEnter(BCell b) {
	}

	public virtual void BCellExit(BCell b) {
	}

	protected class VesselConnection {
		public Vessel node;
		public VesselSegment segment;

		public VesselConnection (Vessel v, VesselSegment vs) {
			node = v;
			segment = vs;
		}

		public Vessel NextImmediateVessel() {
			return segment == null ? node : segment;
		}

		public bool IsTerminalConnection() {
			return segment == null;
		}
	}
}
