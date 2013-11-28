using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Heart : Organ {

	public static float STARTING_ENERGY = 100f;

	// Heart Stats
	private bool isBeating = false;
	public float heartRate = 0.75f; // # of beats per second
	public float heartPressure = 4f; // # of segments per second
	public float energyGeneration = 1f;

	// Animation
	private const float BEAT_FRACTION = 0.5f;
	private float lastBeatTime;
	private Vector3 deltaScale;

	// Prefabs
	public GameObject vNodePrefab;
	public GameObject vSegmentPrefab;

	protected override void Awake() {
		__instance = this;
	}

	protected override void Start() {
		base.Start();
		// Defaults
		buildRadius = 10f;

		// Upgrades
		upgrades = new Upgrade[] {
			new Upgrade(UpgradeType.HeartRate, 100f), 
			new Upgrade(UpgradeType.HeartPressure, 100f) };

		// Energy
		energy = STARTING_ENERGY;

		// Starting Cells
		for (int i = 0; i < 6; i++) {
			BCell b = CellController.Instance.InstantiateNew();
			b.currentVessel = this;
			BCellEnter(b);
		}

		// Start the heart
		StartBeating();

		// Animation
		animator.hasScaleAnimation = false;
	}

	void OnGUI() {
		GUI.Box(new Rect(0, 0, 200, 50), "Energy: " + energy);
	}

	protected override void Update() {
		base.Update();

		// Beating Animation
		if (isBeating) {
			if (lastBeatTime + heartRate <= Time.time) {
				Beat();
			}
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
	private void BeatUpdate(Vector3 v) {
		animator.deltaScale += v;
	}

	public void Beat() {
		// Animation
		lastBeatTime = Time.time;
		float halfBeatLength = heartRate * 0.5f * BEAT_FRACTION;

		// Beat Up
//		LeanTween.value(gameObject, "BeatUpdate", Vector3.zero, animator.baseScale * 0.5f, halfBeatLength, new object[] {
//						"ease",
//						LeanTweenType.easeOutElastic
//				});
		LeanTween.scale(gameObject, animator.baseScale * 1.5f, halfBeatLength, new object[] {
						"ease",
						LeanTweenType.easeOutElastic
				});
		AudioController.Instance.Play(AudioController.Instance.heartbeatUp, 1f);

		// Beat Down
		LeanTween.delayedCall(gameObject, halfBeatLength, "PumpBlood");
//		LeanTween.value(gameObject, "BeatUpdate", animator.baseScale * 0.5f, Vector3.zero, halfBeatLength, new object[] {
//					"ease",
//					LeanTweenType.easeOutElastic
//				});
		LeanTween.scale(gameObject, animator.baseScale, halfBeatLength, new object[] {
						"delay",
						halfBeatLength,
						"ease",
						LeanTweenType.easeOutElastic
				});
	}

	private void PumpBlood() {
		energy += energyGeneration;
		Organ[] o = FindObjectsOfType<Organ>();
		for (int i = 0; i < o.Length; i++) 
			o[i].OnHeartPump();

		AudioController.Instance.Play(AudioController.Instance.heartbeatDown, 1f);
	}

	/*
	 * Search Functions
	 */
	private void GetOrgans(List<Organ> list, List<Vessel> visited, Vessel currentTraversal) {
		if (currentTraversal is Organ) 
			list.Add((Organ)currentTraversal);

		Vessel[] nextNodes = currentTraversal.GetNextNodes();
		if (nextNodes.Length == 0)
			return;

		visited.Add(currentTraversal);
		for (int i = 0; i < nextNodes.Length; i++) 
			if (!visited.Contains(nextNodes[i])) 
				GetOrgans(list, visited, nextNodes[i]);
	}

	/*
	 * Singleton
	 */
	private static Heart __instance;

	public static Heart Instance {
		get { return __instance; }
	}
}
