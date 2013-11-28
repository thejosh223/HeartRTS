using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VesselEndpoint : VesselContainer {

	protected override void Start() {
		base.Start();

		// Disable Visuals
		collider.enabled = false;
		renderer.enabled = false;

		Vector3 v = transform.position;
		v.z = -0.1f;
		transform.position = v;
	}
	
	protected override void OnBuildingConnectionComplete() {
		collider.enabled = true;
		renderer.enabled = true;
	}
}
