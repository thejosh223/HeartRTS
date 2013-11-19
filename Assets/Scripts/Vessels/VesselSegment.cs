﻿using UnityEngine;
using System.Collections;

public class VesselSegment : VesselContainer {

	protected override void Start() {
		base.Start();

		// Rotate based off of 2 attached segments
		Vector3 v = attachedNodes[0].node.transform.position - attachedNodes[1].node.transform.position;
		v = Quaternion.LookRotation(v).eulerAngles;
		v.z = Random.Range(0f, 360f);
		transform.rotation = Quaternion.Euler(v);

		// Disable
		gameObject.SetActive(false);
	}

	/*
	 * Init Functions
	 */
	public void AttachSegment(Vessel node, VesselSegment seg) {
		attachedNodes.Add(new VesselConnection(node, seg));
	}
}
