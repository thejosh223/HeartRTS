using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VesselEndpoint : Vessel {

//	protected BCell currentCell;
	protected List<BCell> currentCells = new List<BCell>();

	// Animation
	private Vector3 baseScale;

	protected override void Start() {
		// Base Values
		baseScale = transform.localScale;
	}

	/*
	 * Cell Movement
	 */
	public override void BCellEnter(BCell b) {
		base.BCellEnter(b);

		currentCells.Add(b);

		float tweenTime = 1f / Heart.Instance.heartPressure;
		LeanTween.scale(gameObject, baseScale * 1.5f, tweenTime, new object[] {
			"ease",
			LeanTweenType.easeOutElastic
		});
//		LeanTween.delayedCall(gameObject, tweenTime, "CallCellExit");
	}

	public override void BCellExit(BCell b) {
		base.BCellExit(b);
		LeanTween.scale(gameObject, baseScale, 1f / Heart.Instance.heartPressure, new object[] {
			"ease",
			LeanTweenType.easeOutElastic
		});
	}

	private void CallCellExit() {
//		if (nextVessels.Count > 0) {
//			BCell[] cells = currentCell.Divide(nextVessels.Count);
//			for (int i = 0; i < nextVessels.Count; i++)
//				nextVessels[i].BCellEnter(cells[i]);
//		}
//		BCellExit(currentCell);
	}
}
