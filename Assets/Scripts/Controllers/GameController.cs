using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static int vesselCounter = 0;
	public GameObject[] lungPrefabs;
	public GameObject[] kidneyPrefabs;
	public GameObject[] lymphnodePrefabs;

	void Awake() {
		LeanTween.init(64);
	}

	void Start() {
		// Level Generation!
		float increment = 10f;
		float startRadius = 10f;
		float maxRadius = startRadius + increment;
		float organDistance = 10f;

		// Layer 1
		// -basic layer: lots of collectibles

		// -> Lung Cells
		int numLungCells = 10;
		int tries = 0;
		int c = 0;
		while (c < numLungCells && tries < 10) {
			float angle = Random.Range(0f, 360f);
			float distFromCenter = Random.Range(startRadius, maxRadius);

			Vector3 pos = new Vector3(distFromCenter * Mathf.Cos(angle), distFromCenter * Mathf.Sin(angle), 0f);
			Vector3 rot = Vector3.zero;

			Collider[] colls = Physics.OverlapSphere(pos, organDistance, BuildController.ORGAN_LAYER);
			if (colls.Length > 0) {
				bool canInstantiate = true;
				for (int i = 0; i < colls.Length; i++) {
					Organ org = colls[i].gameObject.GetComponent<Organ>();
					if (org != null && org != Heart.Instance) {
						canInstantiate = false;
						tries++;
					}
				}
				if (!canInstantiate)
					continue;
			}

			GameObject g = Instantiate(lungPrefabs[Random.Range(0, lungPrefabs.Length)], pos, Quaternion.Euler(rot)) as GameObject;
			c++;
			tries = 0;
		}

		startRadius += increment;
		maxRadius += increment;

		// Layer 2
		int numKidneys = 2;
		c = 0;
		tries = 0;
		float kidneyAngle = Random.Range(0f, 360f);
		while (c < numKidneys && tries < 10) {
			float distFromCenter = Random.Range(startRadius, maxRadius);
			Vector3 pos = new Vector3(distFromCenter * Mathf.Cos(kidneyAngle), distFromCenter * Mathf.Sin(kidneyAngle), 0f);
			Vector3 rot = new Vector3(0, 0, Random.Range(0, 360f));

			Collider[] colls = Physics.OverlapSphere(pos, organDistance, BuildController.ORGAN_LAYER);
			if (colls.Length > 0) {
				bool canInstantiate = true;
				for (int o = 0; o < colls.Length; o++) {
					Organ org = colls[o].gameObject.GetComponent<Organ>();
					if (org != null && org != Heart.Instance) {
						canInstantiate = false;
						tries++;
					}
				}
				if (!canInstantiate) {
					kidneyAngle += Random.Range(-90f, 90f);
					continue;
				}
			}

			GameObject g = Instantiate(kidneyPrefabs[Random.Range(0, kidneyPrefabs.Length)], pos, Quaternion.Euler(rot)) as GameObject;
			tries = 0;
			c++;
			kidneyAngle += 180f;
		}
		
		startRadius += increment;
		maxRadius += increment;

		// Layer 3
		// -Mostly defense layer.
		int numLymphnodes = 8;
		float deltaTheta = 360f / numLymphnodes;
		for (int i = 0; i < numLymphnodes; i++) {
			float angle = i * deltaTheta;
			Vector3 pos = new Vector3(maxRadius * Mathf.Cos(Mathf.Deg2Rad * angle), maxRadius * Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
			Vector3 rot = new Vector3(0, 0, Random.Range(0, 360f));
			GameObject g = Instantiate(lymphnodePrefabs[Random.Range(0, lymphnodePrefabs.Length)], pos, Quaternion.Euler(rot)) as GameObject;
		}
	}
}
