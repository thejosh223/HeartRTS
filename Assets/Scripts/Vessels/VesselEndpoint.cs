using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VesselEndpoint : VesselContainer {

	protected override void Start() {
		base.Start();

		// Disable Visuals
		collider.enabled = false;
		renderer.enabled = false;

		// Start building!
//		segmentList = new Vessel[attachedNodes.Count];
//		for (int i = 0; i < attachedNodes.Count; i++) 
//			segmentList[i] = attachedNodes[i].node;
//		_lastBuildTime = Time.time;
//		_isBuilding = true;
	}
	
	protected override void OnBuildingConnectionComplete() {
		collider.enabled = true;
		renderer.enabled = true;
	}
}
