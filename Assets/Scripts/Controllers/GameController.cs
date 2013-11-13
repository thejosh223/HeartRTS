using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	void Awake() {
		LeanTween.init(64);
	}

	void Start() {
		GameObject g = Instantiate(Heart.Instance.vNodePrefab, new Vector3(8, 2, 0), Quaternion.identity) as GameObject;
		VesselEndpoint v = g.GetComponent<VesselEndpoint>();
		v.AttachVessel(Heart.Instance);

		g = Instantiate(Heart.Instance.vNodePrefab, new Vector3(7, -4, 0), Quaternion.identity) as GameObject;
		VesselEndpoint v2 = g.GetComponent<VesselEndpoint>();
		v2.AttachVessel(v);
	}
}
