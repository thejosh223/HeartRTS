using UnityEngine;
using System.Collections;

public class OrganUnitsMenu : MonoBehaviour {

	public const string PREFIX = "OrganUnit_";
	protected Organ _activeOrgan;
	protected GameObject[] cellObjects;
	public GameObject bAddCell;

	void Start() {
		// References
		bAddCell = transform.FindChild("bAddCell").gameObject;
		bAddCell.SetActive(false);

		// Instantiate lots of stuff.
		GameObject prefab = transform.FindChild("UnitUnselected").gameObject;
		cellObjects = new GameObject[Organ.MAX_CELLS];
		int c = 0;
		for (int o = 0; o < 2; o++) {
			for (int i = 0; i < 4; i++) {
				Vector3 pos = prefab.transform.position + new Vector3(prefab.transform.localScale.x * i, prefab.transform.localScale.y * -o, 0);
				GameObject tg = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
				tg.name = PREFIX + (o * 4 + i);
				tg.transform.parent = prefab.transform.parent;
				tg.SetActive(false);

				cellObjects[c++] = tg;
			}
		}
		
		Destroy(prefab);
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
		for (int i = 0; i < cellObjects.Length; i++) 
			cellObjects[i].SetActive(false);

		// Enable select objects
		for (int i = 0; i < cells.Length; i++) 
			cellObjects[i].SetActive(true);
	}
}
