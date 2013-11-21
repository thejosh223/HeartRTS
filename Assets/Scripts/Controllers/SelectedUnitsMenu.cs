using UnityEngine;
using System.Collections;

public class SelectedUnitsMenu : MonoBehaviour {

	public const string PREFIX = "SelectedCell_";
	protected GameObject[] cellObjects;

	void Start() {
		GameObject gSelected = transform.FindChild("UnitSelected").gameObject;
		cellObjects = new GameObject[Organ.MAX_CELLS];
		int c = 0;
		for (int i = 0; i < 8; i++) {
			Vector3 pos = gSelected.transform.position + new Vector3(gSelected.transform.localScale.x * i, 0, 0);
			GameObject tg = Instantiate(gSelected, pos, Quaternion.identity) as GameObject;
			tg.name = PREFIX + i;
			tg.transform.parent = gSelected.transform.parent;
			tg.SetActive(false);
			
			cellObjects[c++] = tg;
		}
		
		Destroy(gSelected);
		gameObject.SetActive(false);
	}

	public int IndexOfHit(GameObject g) {
		if (g != null) {
			for (int i = 0; i < cellObjects.Length; i++) {
				if (cellObjects[i] == g) {
					return i;
				}
			}
		}
		return -1;
	}
	
	public void UpdateUI(BCell[] cells) {
		// Disable all objects
		for (int i =0; i < cellObjects.Length; i++) 
			cellObjects[i].SetActive(false);

		// Enable select objects
		for (int i =0; i < cells.Length; i++) 
			cellObjects[i].SetActive(true);
	}
}
