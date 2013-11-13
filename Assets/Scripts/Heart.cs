using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Heart : Organ {

	// Heart Stats
	private bool isBeating = false;
	public float heartRate = 0.75f;
	public float heartPressure = 4f; // # of segments per second

	// Animation
	private const float BEAT_FRACTION = 0.5f;
	private Vector3 baseScale;
	private float lastBeatTime;

	// Prefabs
	public GameObject vNodePrefab;
	public GameObject vSegmentPrefab;

	protected override void Awake() {
		__instance = this;
	}

	protected override void Start() {
		// Defaults
		baseScale = transform.localScale;

		StartBeating();
	}

	void Update() {
		// Beating Animation
		if (lastBeatTime + heartRate <= Time.time) {
			Beat();
		}
	}

	/*
	 * Init Functions
	 */
	public void StartBeating() {
		isBeating = true;
		lastBeatTime = 0f;
	}

	/*
	 * Action Functions
	 */
	public void Beat() {
		// Animation
		lastBeatTime = Time.time;
		float halfBeatLength = heartRate * 0.5f * BEAT_FRACTION;
		LeanTween.scale(gameObject, baseScale * 1.5f, halfBeatLength, new object[] {
						"ease",
						LeanTweenType.easeOutElastic
				});
		LeanTween.delayedCall(gameObject, halfBeatLength, "PumpBlood");
		LeanTween.scale(gameObject, baseScale, halfBeatLength, new object[] {
						"delay",
						halfBeatLength,
						"ease",
						LeanTweenType.easeOutElastic
				});
	}

	private void PumpBlood() {
		for (int i = 0; i < attachedNodes.Count; i++) {
			BCell b = new BCell();
			b.nextTarget = attachedNodes[i].node;
			attachedNodes[i].segment.BCellEnter(b);
		}
	}

	/*
	 * Singleton
	 */
	private static Heart __instance;

	public static Heart Instance {
		get { return __instance; }
	}
}
