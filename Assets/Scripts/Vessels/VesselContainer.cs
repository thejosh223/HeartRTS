using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VesselContainer : Vessel {
	
	protected List<BCell> currentCells = new List<BCell>();
	
	// Animation
	private Vector3 baseScale;
	
	protected override void Start() {
		base.Start();

		// Base Values
		baseScale = transform.localScale;
	}
	
	/*
	 * Cell Movement
	 */
	public override void BCellEnter(BCell b) {
		base.BCellEnter(b);
		
		currentCells.Add(b);
		b.OnVesselEnter(this);
		
		float tweenTime = 1f / Heart.Instance.heartPressure;
		LeanTween.scale(gameObject, baseScale * 1.5f, tweenTime, 
				new object[] { "ease", LeanTweenType.easeOutBack });
		StartCoroutine(CallCellExit(b, tweenTime));
	}
	
	public override void BCellExit(BCell b) {
		base.BCellExit(b);
		LeanTween.scale(gameObject, baseScale, 1f / Heart.Instance.heartPressure,
		                new object[] { "ease", LeanTweenType.easeOutBack });
	}

	private IEnumerator CallCellExit(BCell b, float delay) {
		yield return new WaitForSeconds(delay);
		Vessel v = GetImmediateVesselTo(b.nextTarget);
		if (v != null) {
			v.BCellEnter(b);
			BCellExit(b);
		}
	}
}
