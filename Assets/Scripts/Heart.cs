using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Heart : Organ {

	public static float STARTING_ENERGY = 1000f;

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
		base.Start();

		// Defaults
		baseScale = transform.localScale;

		// Energy
		energy = STARTING_ENERGY;

		// Starting Cells
		for (int i = 0; i < 6; i++) {
			BCellEnter(CellController.Instance.InstantiateNew());
		}

		// Start the heart
		StartBeating();
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
//		for (int i = 0; i < currentCells.Count; i++) {
//			if (currentCells[i].targetOrgan != null) {
//				if (BCell.HasPathTo(this, currentCells[i].targetOrgan))
//					PumpOutCell(currentCells[i], currentCells[i].targetOrgan);
//			}
//		}
	}

	public override void PumpOutCell(BCell b, Organ target) {
		base.PumpOutCell(b, target);

		// Set Behaviour			
		switch (b.MovementMode) {
			case MovementType.DepositAtHeart:
				b.MovementMode = MovementType.GatherAtOrgan;
				break;
			case MovementType.GatherAtHeart:
				b.MovementMode = MovementType.DepositAtOrgan;
				break;
			default:
				Debug.LogError("It Defaulted! This should not happen.");
				break;
		}
	}

	public BCell GetIdleCell() {
		for (int i = 0; i < currentCells.Count; i++) {
			if (currentCells[i].TargetOrgan == null) {
				return currentCells[i];
			}
		}
		return null;
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
