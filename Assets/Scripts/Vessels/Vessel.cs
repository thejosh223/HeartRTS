using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vessel : MonoBehaviour {
	
	protected static float SEGMENT_BUILDTIME = 0.2f;
//	protected static float SEGMENT_BUILDTIME = 0.0625f;
//	protected static float SEGMENT_BUILDTIME = 0f;
	public const float VESSELSEGMENT_COST = 10f;
	public static int VESSELCOUNTER = 0;

	// Graph
	protected List<VesselConnection> attachedNodes = new List<VesselConnection>();

	// For building connectinos
	protected bool _isBuilding = false;
	protected float _lastBuildTime;
	protected Vessel[] segmentList;

	/*
	 * Init Functions
	 */
	protected virtual void Awake() {
	}

	protected virtual void Start() {
	}

	protected virtual void Update() {
	}

	protected virtual void LateUpdate() {
		if (_isBuilding) {
			if (_lastBuildTime <= Time.time) {
				_lastBuildTime = _lastBuildTime + SEGMENT_BUILDTIME;
				bool builded = false;
				for (int i = 0; i < segmentList.Length; i++) {
					if (segmentList[i] != null) {
						Vessel v = segmentList[i].GetImmediateVesselTo(this);
						if (v != this) {
							v.gameObject.SetActive(true);
							v.OnSetActive();
							segmentList[i] = v;
							builded = true;
						} else {
							segmentList[i] = null;
						}
					}
				}
				
				if (!builded) {
					_isBuilding = false;
					OnBuildingConnectionComplete();
				}
			}
		}
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
		for (int i = 0; i < attachedNodes.Count; i++) {
			if (attachedNodes[i].node == v) {
				return attachedNodes[i].NextImmediateVessel();
			}
		}
		return null;
	}

	public virtual Transform[] GetConnectionPoints() {
		return new Transform[] { transform };
	}
	
	public virtual void AttachVessel(Vessel v, Transform connectFrom, Transform connectTo) {
		// Create Segments
		GameObject vesselPrefab = Heart.Instance.vSegmentPrefab;

		Vector3 prevPos = connectFrom.position;
		VesselSegment vFirst = null; 
		VesselSegment vTemp = null;

		float dist = Vector3.Distance(connectTo.position, prevPos);
		int numVessels = (int)(dist / vesselPrefab.transform.localScale.x); // Note: localscale for prefab == lossyscale in game
		float deltaX = (connectTo.position.x - prevPos.x) / numVessels;
		float deltaY = (connectTo.position.y - prevPos.y) / numVessels;
		for (int i = 0; i < numVessels; i++) {
			// Instantiate
			GameObject g = Instantiate(vesselPrefab, //
				new Vector3(prevPos.x + deltaX * i + deltaX / 2, prevPos.y + deltaY * i + deltaY / 2, 0), //
				Quaternion.identity) as GameObject;
			g.transform.parent = transform.parent;
			g.name = "Segment: " + (VESSELCOUNTER++);
			VesselSegment v2 = g.GetComponent<VesselSegment>();
			v2.RotateTowards(connectTo.position - connectFrom.position);

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

		// Animate building
		BuildTo(v);

		// Add to List
		// TODO: move this to OnBuildingConnectionComplete()
		attachedNodes.Add(new VesselConnection(v, vTemp)); // null -> first one from this node to v
		v.attachedNodes.Add(new VesselConnection(this, vFirst)); // null -> last one from this node to v
	}

	public void BuildTo(Vessel v) {
		_isBuilding = true;
		_lastBuildTime = Time.time;
		segmentList = new Vessel[] { v };
	}

	protected virtual void OnBuildingConnectionComplete() {
	}

	public bool IsConnectedTo(Vessel v) {
		for (int i = 0; i < attachedNodes.Count; i++) 
			if (attachedNodes[i].node == v || attachedNodes[i].segment == v) 
				return true;
		return false;
	}

	/*
	 * Cell Entering / Exiting Functions
	 * -call this when you want a cell to enter/exit the vessel
	 */
	public virtual void BCellEnter(BCell b) {
		b.OnVesselEnter(this);
	}

	public virtual void BCellExit(BCell b) {
	}

	public virtual void OnSetActive() {
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
