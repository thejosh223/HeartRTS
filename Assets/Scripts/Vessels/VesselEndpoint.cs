using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VesselEndpoint : VesselContainer {
	
//	private static float SEGMENT_BUILDTIME = 0.0625f;
	private static float SEGMENT_BUILDTIME = 0f;
	//
	private Vessel buildToVessel;

	//
	private bool _isBuilding = true;
	private float _lastBuildTime;
	Vessel[] segmentList;

	protected override void Start() {
		base.Start();

		// Disable Visuals
		collider.enabled = false;
		renderer.enabled = false;

		// Start building!
		segmentList = new Vessel[attachedNodes.Count];
		for (int i = 0; i < attachedNodes.Count; i++) 
			segmentList[i] = attachedNodes[i].node;

		_lastBuildTime = Time.time;
	}

	protected override void Update() {
		base.Update();

		if (_isBuilding) {
			if (_lastBuildTime <= Time.time) {
				_lastBuildTime = _lastBuildTime + SEGMENT_BUILDTIME;
				bool builded = false;
				for (int i = 0; i < segmentList.Length; i++) {
					if (segmentList[i] != null) {
						Vessel v = segmentList[i].GetImmediateVesselTo(this);
						if (v != this) {
							v.gameObject.SetActive(true);
							segmentList[i] = v;
							builded = true;
						} else {
							segmentList[i] = null;
						}
					}
				}
				if (!builded) {
					_isBuilding = false;
					collider.enabled = true;
					renderer.enabled = true;
				}
			}
		}
	}
//
//	public void BuildTo(Vessel v) {
//		_isBuilding = true;
//	}
}
