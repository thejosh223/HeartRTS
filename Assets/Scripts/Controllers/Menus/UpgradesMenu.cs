using UnityEngine;
using System.Collections;

public class UpgradesMenu : Menu {

	protected Organ activeOrgan;

	public void OpenOrganMenu(Organ org) {
		activeOrgan = org;
	}

	void OnGUI() {
		if (MenuActive) { 
			float w = Screen.width * 0.25f;
			float h = 50f;

			Upgrade[] upgrades = activeOrgan.upgrades;
			for (int i = 0; i < upgrades.Length; i++) {
				string s = "LV: " + upgrades[i].level + " - " + upgrades[i].type.ToString() + " (" + upgrades[i].GetCost() + ")";
				if (GUI.Button(new Rect(Screen.width - w, i * h, w, h), s)) {
					upgrades[i].Activate();
				}
			}

			if (GUI.Button(new Rect(Screen.width - w, Screen.height - h, w, h), "Close Upgrades Menu")) {
				MenuActive = false;
			}
		}
	}
}
