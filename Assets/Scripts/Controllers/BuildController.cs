using UnityEngine;
using System.Collections;

public class BuildController : MonoBehaviour {

	protected Vessel sourceVessel;
	protected Plane xyZeroPlane;

	void Start() {
		xyZeroPlane = new Plane(Vector3.forward, Vector3.zero);
	}
	
	void Update() {

		if (Input.GetMouseButton(0)) {
			if (Input.GetMouseButtonDown(0)) {
				// Raycast
				RaycastHit hit = Raycast(Input.mousePosition);
				if (hit.transform != null) {
					// It hit something!
					CameraController.Instance.SetCamMovable(false);
					sourceVessel = hit.transform.GetComponent<Vessel>();
				} else {
					// Nothing hit.
					CameraController.Instance.SetCamMovable(true);
					sourceVessel = null;
				}
			}

			if (sourceVessel != null) {
				Debug.Log("Vessel: " + sourceVessel.name);
			}
		}

		if (Input.GetMouseButtonUp(0)) { 
			if (sourceVessel != null) {
				// Determine where to instantiate
				Vector3 v = Vector3.zero;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				float distance = 0;
				if (xyZeroPlane.Raycast(ray, out distance)) 
					v = ray.GetPoint(distance);

				// Instantiate new segment
				InstantiateVessel(sourceVessel, v);
			} 

			CameraController.Instance.SetCamMovable(true);
			sourceVessel = null;
		}
	}

	public void InstantiateVessel(Vessel attachedTo, Vector3 pos) {
		GameObject g = Instantiate(Heart.Instance.vNodePrefab, pos, Quaternion.identity) as GameObject;
		VesselEndpoint v = g.GetComponent<VesselEndpoint>();
		v.name = "VesselEndpoint_" + GameController.vesselCounter++;
		v.AttachVessel(attachedTo);
	}

	protected RaycastHit Raycast(Vector3 v) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			return hit;
		return new RaycastHit();
	}

	/*
	 * Singleton
	 */
	private static BuildController _instance;

	public static BuildController Instance {
		get { return _instance; }
	}
}
