using UnityEngine;
using System.Collections;

public class Upgrade {

	public Organ owner;
	public UpgradeType type;
	public int level;
	protected float[] cost;

	public Upgrade (UpgradeType type, float cost) : this(type, new float[] {cost}) {
	}

	public Upgrade (UpgradeType type, float[] cost) {
		this.type = type;
		this.cost = cost;
		level = 0;
	}

	public float GetCost() {
		if (cost.Length == 1)
			return cost[0];
		if (level >= cost.Length)
			return cost[cost.Length - 1];
		return cost[level];
	}

	public virtual void Activate() {
		level ++;
		switch (type) {
			case UpgradeType.HeartRate:
				Heart.Instance.heartRate *= 0.5f;
				break;
			case UpgradeType.HeartPressure:
				Heart.Instance.heartPressure *= 1.5f;
				break;
			default:
				break;
		}
	}
}
