using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static int vesselCounter = 0;

	void Awake() {
		LeanTween.init(64);
	}

	void Start() {
		GameObject g = Instantiate(Heart.Instance.vNodePrefab, new Vector3(Random.Range(6f, 8f), Random.Range(-2f, 2f), 0), Quaternion.identity) as GameObject;
		VesselEndpoint v = g.GetComponent<VesselEndpoint>();
		v.AttachVessel(Heart.Instance);
		v.name = "VesselEndpoint_" + vesselCounter++;
		TestInitFunction(v, RandomDelta(), true, 0);
	}

	private void TestInitFunction(Vessel v, Vector3 deltaPos, bool keepRecursion, int depth) {
		if (!keepRecursion)
			return;

		GameObject g = Instantiate(Heart.Instance.vNodePrefab, v.transform.position + deltaPos, Quaternion.identity) as GameObject;
		VesselEndpoint v2 = g.GetComponent<VesselEndpoint>();
		v2.name = "VesselEndpoint_" + vesselCounter++;
		v2.AttachVessel(v);

		depth++;
		for (int i = 0; i < 2; i++)
			TestInitFunction(v2, RandomDelta(), depth < 5, depth);
	}
	
	private static Vector3 RandomDelta() {
		return new Vector3(Random.Range(4f, 8f), Random.Range(-6f, 6f), 0);
	}
}
