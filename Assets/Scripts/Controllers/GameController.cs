using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static int vesselCounter = 0;
	public GameObject[] LungPrefabs;

	void Awake() {
		LeanTween.init(64);
	}

	void Start() {
		// Level Generation!
		float increment = 6f;
		float startRadius = 5f;
		float maxRadius = startRadius + increment;

		// Layer 1
		// -basic layer: lots of collectibles

		// -> Lung Cells
		int numLungCells = 4;
		float organDistance = 6f;
		GameObject[] lungCells = new GameObject[numLungCells];
		int c = 0;
		int tries = 0;
		while (c < numLungCells || tries > 4) {
			float angle = Random.Range(0f, 360f);
			float distFromCenter = Random.Range(startRadius, maxRadius);

			Vector3 pos = new Vector3(distFromCenter * Mathf.Cos(angle), distFromCenter * Mathf.Sin(angle), 0f);
			Vector3 rot = Vector3.zero;

			bool posOkay = true;
			for (int i = 0; i < c; i++) {
				if (Vector3.Distance(pos, lungCells[i].transform.position) < organDistance) {
					tries++;
					posOkay = false;
					break;
				}
			}
			if (!posOkay)
				continue;

			lungCells[c++] = Instantiate(LungPrefabs[Random.Range(0, LungPrefabs.Length)], pos, Quaternion.Euler(rot)) as GameObject;
			tries = 0;
		}

		// Layer 2

		// Layer 3
		// -Mostly defense layer.
	}
}
