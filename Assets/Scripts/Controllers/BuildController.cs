using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildController : MonoBehaviour {

	public bool isBuildMode = true;

	//
	protected float minBuildRadius = 2f;
	protected float buildRadius = 8f; // TODO: Set this to 6f
	private Vessel _sourceVessel;
	protected Plane xyZeroPlane;

	// Build Guidelines
	private static float LINE_RENDERER_OFFSET = 2f;
	private GameObject radiusIdentifier;
	private List<VesselLineRenderer> radiusLineRenderers = new List<VesselLineRenderer>();
	private Color activeColor = new Color(109 / 255f, 194 / 255f, 255 / 255f);
	private Color inactiveColor = new Color(255 / 255f, 131 / 255f, 119 / 255f);

	void Awake() {
		_instance = this;
	}

	void Start() {
		xyZeroPlane = new Plane(Vector3.forward, Vector3.zero);

		radiusIdentifier = transform.FindChild("RadiusMarker").gameObject;
		radiusIdentifier.SetActive(false);
	}
	
	void Update() {
		if (!isBuildMode)
			return;

		if (Input.GetMouseButton(0)) {
			if (Input.GetMouseButtonDown(0)) {
				// Raycast
				RaycastHit hit = Raycast(Input.mousePosition);
				if (hit.transform != null) {
					// It hit something!
					Vessel hitVessel = hit.transform.GetComponent<Vessel>();
					if (BCell.HasPathTo(Heart.Instance, hitVessel)) 
						SourceVessel = hitVessel;
				} else {
					// Nothing hit.
					SourceVessel = null;
				}
			}

			if (_sourceVessel != null) {
				// Check supposed distance
				Vector3 v = RaycastXYPlane(Input.mousePosition);
				float dist = Vector3.Distance(v, _sourceVessel.transform.position);

				if (dist <= buildRadius && dist >= minBuildRadius) {
					radiusIdentifier.renderer.material.color = activeColor;
					for (int i = 0; i < radiusLineRenderers.Count; i++) 
						radiusLineRenderers[i].r.SetPosition(1, v - Vector3.forward * LINE_RENDERER_OFFSET);
				} else {
					radiusIdentifier.renderer.material.color = inactiveColor;
				}
			}
		}

		if (Input.GetMouseButtonUp(0)) { 
			if (_sourceVessel != null) {
				// Determine where to instantiate
				Vector3 v = RaycastXYPlane(Input.mousePosition);
				float dist = Vector3.Distance(v, _sourceVessel.transform.position);

				// Instantiate new segment
				if (dist <= buildRadius) {
					if (Heart.Instance.energy >= Vessel.VESSELSEGMENT_COST) {
						Vessel hitVessel = null;

						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						RaycastHit hit;
						if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
							hitVessel = hit.transform.GetComponent<Vessel>();

						if (hitVessel != _sourceVessel) {
							if (hitVessel != null) {
								// Vessel or Organ Hit
								// -No need to instantiate.
								hitVessel.AttachVessel(_sourceVessel);
								OnBuild();
							} else {
								// Nothing Hit.
								Vessel newlyInitted = InstantiateVessel(_sourceVessel, v);
								newlyInitted.AttachVessel(_sourceVessel);
								OnBuild();
							}
						}
					}
				}

				if (dist <= minBuildRadius) {
					if (_sourceVessel is Organ) {
						GUICamController.Instance.OpenOrganMenu((Organ)_sourceVessel);
					}
				}
			} 

			SourceVessel = null;
		}
	}

	private void OnBuild() {
		Heart.Instance.energy -= Vessel.VESSELSEGMENT_COST;
	}

	private Vessel SourceVessel {
		set {
			_sourceVessel = value;
			CameraController.Instance.SetCamMovable(value == null);
			radiusIdentifier.SetActive(value != null);

			for (int i = 0; i < radiusLineRenderers.Count; i++) 
				radiusLineRenderers[i].r.gameObject.SetActive(false);
			radiusLineRenderers.Clear();

			if (value != null) {
				radiusIdentifier.transform.position = _sourceVessel.transform.position;
				radiusIdentifier.transform.localScale = new Vector3(buildRadius * 2f, buildRadius * 2f, buildRadius * 2f);

				Vector3 radiusLineBasePos = _sourceVessel.transform.position;
				LineRenderer r = LineRendererPool.Instance.InstantiateAt();
				r.SetPosition(0, radiusLineBasePos - Vector3.forward * LINE_RENDERER_OFFSET);
				radiusLineRenderers.Add(new VesselLineRenderer(_sourceVessel, r));
			}
		}
	}

	public Vessel InstantiateVessel(Vessel attachedTo, Vector3 pos) {
		GameObject g = Instantiate(Heart.Instance.vNodePrefab, pos, Quaternion.identity) as GameObject;
		VesselEndpoint v = g.GetComponent<VesselEndpoint>();
		v.name = "VesselEndpoint_" + GameController.vesselCounter++;
		return v;
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

	/*
	 * For use with the line renderer
	 */
	public class VesselLineRenderer {
		public Vessel v;
		public LineRenderer r;

		public VesselLineRenderer (Vessel v, LineRenderer r) {
			this.v = v;
			this.r = r;
		}
	}
}
