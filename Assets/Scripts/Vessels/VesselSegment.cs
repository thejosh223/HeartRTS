using UnityEngine;
using System.Collections;

public class VesselSegment : VesselContainer {

	/*
	 * Init Functions
	 */
	public void AttachSegment(Vessel node, VesselSegment seg) {
		attachedNodes.Add(new VesselConnection(node, seg));
	}
}
