using UnityEngine;
using System.Collections;

public class VesselSegment : VesselContainer {

	protected override void Start() {
		base.Start();

		// Disable
		gameObject.SetActive(false);
	}

	public void RotateTowards(Vector3 direction) {
		direction = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
		transform.rotation = Quaternion.Euler(direction);
	}

	/*
	 * Init Functions
	 */
	public void AttachSegment(Vessel node, VesselSegment seg) {
		attachedNodes.Add(new VesselConnection(node, seg));
	}
}
