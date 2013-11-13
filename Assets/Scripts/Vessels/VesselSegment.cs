using UnityEngine;
using System.Collections;

public class VesselSegment : Vessel {

	protected BCell currentCell;

	// Animation
	private Vector3 baseScale;

	protected override void Start() {
		baseScale = transform.localScale;
	}

	/*
	 * Cell Movement
	 */
	public override void BCellEnter(BCell b) {
		base.BCellEnter(b);

		currentCell = b;

		float tweenTime = 1f / Heart.Instance.heartPressure;
		LeanTween.scale(gameObject, baseScale * 1.5f, tweenTime, new object[] {
						"ease",
						LeanTweenType.easeOutElastic
				});
		LeanTween.delayedCall(gameObject, tweenTime, "CallCellExit");
	}

	public override void BCellExit(BCell b) {
		base.BCellExit(b);
		LeanTween.scale(gameObject, baseScale, 1f / Heart.Instance.heartPressure, new object[] {
						"ease",
						LeanTweenType.easeOutElastic
				});
	}

	private void CallCellExit() {
		GetImmediateVesselTo(currentCell.nextTarget).BCellEnter(currentCell);
		BCellExit(currentCell);
	}

	/*
	 * Init Functions
	 */
	public void AttachSegment(Vessel node, VesselSegment seg) {
		attachedNodes.Add(new VesselConnection(node, seg));
	}
}
