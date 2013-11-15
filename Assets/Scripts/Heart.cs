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
		List<VesselEndpoint> endpoints = new List<VesselEndpoint>();
		List<Vessel> visited = new List<Vessel>();
		GetEndpoints(endpoints, visited, this);

		for (int i = 0; i < attachedNodes.Count; i++) {
			for (int o = 0; o < 2; o++) {
				Debug.Log("Endpoints Count: " + endpoints.Count);
				int randomEndpoint = endpoints.Count == 1 ? 0 : Random.Range(0, endpoints.Count - 1);

				BCell b = new BCell();
				b.SetTarget(this, endpoints[randomEndpoint]);
				attachedNodes[i].segment.BCellEnter(b);
			}
		}
	}

	private void GetEndpoints(List<VesselEndpoint> list, List<Vessel> visited, Vessel currentTraversal) {
		// TEST ONLY!!!
		Vessel[] nextNodes = currentTraversal.GetNextNodes();
		if (nextNodes.Length == 0)
			return;
		visited.Add(currentTraversal);

		bool added = false;
		for (int i = 0; i < nextNodes.Length; i++) {
			if (!visited.Contains(nextNodes[i])) {
				GetEndpoints(list, visited, nextNodes[i]);
				added = true;
			}
		}
		if (!added) {
			list.Add((VesselEndpoint)currentTraversal);
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
