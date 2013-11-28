using UnityEngine;
using System.Collections;

public enum UpgradeType {
	// Heart
	HeartRate,
	HeartPressure,
	// Cells (probably at the heart too)
	CellCapacity, 
	// Lungs
	LungRegenRate,
	LungTransferRate,
	// Kidney
	KidneyMultiplier,
	KidneyTransferRate
}

public static class ExtensionMethods {
	public static string GetDescription(this UpgradeType u) {
		switch (u) {
			case UpgradeType.HeartRate:
				return "Increase Heart Rate.";
			default:
				return "";
		}
	}
}