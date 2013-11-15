using UnityEngine;
using System.Collections;

public class BuildController : MonoBehaviour {

	protected float buildRadius = 8f;
	private Vessel _sourceVessel;
	protected Plane xyZeroPlane;

	// Build Guidelines
	private GameObject radiusIdentifier;
	private Color activeColor = new Color(109 / 255f, 194 / 255f, 255 / 255f);
	private Color inactiveColor = new Color(255 / 255f, 131 / 255f, 119 / 255f);

	void Start() {
		xyZeroPlane = new Plane(Vector3.forward, Vector3.zero);

		radiusIdentifier = transform.FindChild("RadiusMarker").gameObject;
		radiusIdentifier.SetActive(false);
	}
	
	void Update() {
		if (Input.GetMouseButton(0)) {
			if (Input.GetMouseButtonDown(0)) {
				// Raycast
				RaycastHit hit = Raycast(Input.mousePosition);
				if (hit.transform != null) {
					// It hit something!
					SourceVessel = hit.transform.GetComponent<Vessel>();
				} else {
					// Nothing hit.
					SourceVessel = null;
				}
			}

			if (_sourceVessel != null) {
				// Check supposed distance
				Vector3 v = RaycastXYPlane(Input.mousePosition);
				float dist = Vector3.Distance(v, _sourceVessel.transform.position);
				radiusIdentifier.renderer.material.color = dist <= buildRadius ? activeColor : inactiveColor;
			}
		}

		if (Input.GetMouseButtonUp(0)) { 
			if (_sourceVessel != null) {
				// Determine where to instantiate
				Vector3 v = RaycastXYPlane(Input.mousePosition);
				float dist = Vector3.Distance(v, _sourceVessel.transform.position);

				// Instantiate new segment
				if (dist <= buildRadius)
					InstantiateVessel(_sourceVessel, v);
			} 

			SourceVessel = null;
		}
	}

	private Vessel SourceVessel {
		set {
			_sourceVessel = value;
			CameraController.Instance.SetCamMovable(value == null);
			radiusIdentifier.SetActive(value != null);

			if (value != null) {
				radiusIdentifier.transform.position = _sourceVessel.transform.position;
				radiusIdentifier.transform.localScale = new Vector3(buildRadius * 2f, buildRadius * 2f, buildRadius * 2f);
			}
		}
	}

	public void InstantiateVessel(Vessel attachedTo, Vector3 pos) {
		GameObject g = Instantiate(Heart.Instance.vNodePrefab, pos, Quaternion.identity) as GameObject;
		VesselEndpoint v = g.GetComponent<VesselEndpoint>();
		v.name = "VesselEndpoint_" + GameController.vesselCounter++;
		v.AttachVessel(attachedTo);
	}

	protected RaycastHit Raycast(Vector3 v) {
		Ray ray = Camera.main.ScreenPointToRay(v);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			return hit;
		return new RaycastHit();
	}

	protected Vector3 RaycastXYPlane(Vector3 target) {
		Vector3 v = Vector3.zero;
		Ray ray = Camera.main.ScreenPointToRay(target);
		float distance = 0;
		if (xyZeroPlane.Raycast(ray, out distance)) 
			v = ray.GetPoint(distance);
		return v;
	}

	/*
	 * Singleton
	 */
	private static BuildController _instance;

	public static BuildController Instance {
		get { return _instance; }
	}
}
