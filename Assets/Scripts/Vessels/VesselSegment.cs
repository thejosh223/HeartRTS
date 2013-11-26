using UnityEngine;
using System.Collections;

public class VesselSegment : VesselContainer {

	public const float ANIM_TIME = 4f;
	public const float ANIM_SCALE = 0.025f;

	//
	protected Transform[] endPts; // This is for quick access
	public float ratioDistanceToPrev; // [0, 1] distance from one endpoint to the next

	// Animation
	protected float startTime = 0; // animation constant

	protected override void Start() {
		base.Start();

		endPts = new Transform[] {
			attachedNodes[0].nodeTransform,
			attachedNodes[1].nodeTransform
		};
		ratioDistanceToPrev = Vector3.Distance(transform.position, endPts[0].position) / Vector3.Distance(endPts[0].position, endPts[1].position);

		// Animation
		animator.hasPositionAnimation = true;

		// Disable
		gameObject.SetActive(false);
	}

	protected override void Update() {
		base.Update();

		// set animator's base position.
		animator.basePosition = endPts[0].position + (endPts[1].position - endPts[0].position).normalized * ratioDistanceToPrev * 
			Vector3.Distance(endPts[0].position, endPts[1].position);

		// move up and down slowly
		float t = (((Time.time - startTime) % ANIM_TIME) / ANIM_TIME) * 2 * Mathf.PI;
		animator.deltaPosition += transform.up * ANIM_SCALE * (Mathf.Sin(t) * 2 - 1f);

		// rotate towards position
		RotateTowards(endPts[1].position - endPts[0].position);
	}

	protected override void LateUpdate() {
	}

	public override void OnSetActive() {
		startTime = Time.time;
	}

	public void RotateTowards(Vector3 direction) {
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan(direction.y / direction.x) * Mathf.Rad2Deg));
//		direction = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
//		transform.rotation = Quaternion.Euler(direction);
	}

	/*
	 * Init Functions
	 */
	public void AttachSegment(Vessel node, VesselSegment seg, Transform t) {
		attachedNodes.Add(new VesselConnection(node, seg, t));
	}
}
