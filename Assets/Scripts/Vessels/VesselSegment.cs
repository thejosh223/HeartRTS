using UnityEngine;
using System.Collections;

public class VesselSegment : VesselContainer {

	public const float ANIM_TIME = 4f;
	public const float ANIM_SCALE = 0.025f;

	//
	protected Transform[] endPts; // This is for quick access
	protected float startTime = 0;
	protected Vector3 basePosition;

	protected override void Start() {
		base.Start();

		endPts = new Transform[] {
			attachedNodes[0].node.transform,
			attachedNodes[1].node.transform
		};

		basePosition = transform.position;

		// Disable
		gameObject.SetActive(false);
	}

	protected override void Update() {
		base.Update();

		float t = (((Time.time - startTime) % ANIM_TIME) / ANIM_TIME) * 2 * Mathf.PI;
		transform.position = basePosition + transform.up * ANIM_SCALE * (Mathf.Sin(t) * 2 - 1f);
	}

	public override void OnSetActive() {
		startTime = Time.time;
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
