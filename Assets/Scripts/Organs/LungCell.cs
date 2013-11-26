using UnityEngine;
using System.Collections;

public class LungCell : Organ {
	// Constants
	public const float MAX_ENERGY = 100f;
	public const float ENERGY_REPLACE_RATE = 2f;

	//
	protected Bar monitor;

	// Animation
	protected Vector3 baseScale;

	protected override void Start() {
		base.Start();
		GameObject g = Instantiate(GUICamController.Instance.barPrefab) as GameObject;
		monitor = g.GetComponent<Bar>();
		monitor.transform.parent = GUICamController.Instance.transform;
		monitor.followTrans = transform;
		monitor.offset = new Vector3(0, -0.1f, 0f);

		baseScale = transform.localScale;
	}

	protected override void Update() {
		base.Update();
		energy += ENERGY_REPLACE_RATE * Time.deltaTime;
		if (energy > MAX_ENERGY)
			energy = MAX_ENERGY;

		float percentage = energy / MAX_ENERGY;
		monitor.SetValue(percentage);

		// Animation
		transform.localScale = baseScale * Mathf.Pow(2, (percentage * 2f) - 1f);
	}

	public override MovementType GetDefaultBehaviour() {
		return MovementType.Gather;
	}

}
