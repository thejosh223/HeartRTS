using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildController : MonoBehaviour {

	protected float minBuildRadius = 2f;
	protected float buildRadius = 8f;
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

					// Get all in radius
					Collider[] hitColliders = Physics.OverlapSphere(v, buildRadius);
					List<Vessel> hitVessels = new List<Vessel>();

					// Add all not yet in radius
					for (int i = 0; i < hitColliders.Length; i++) {
						Vessel hitColliderVessel = hitColliders[i].GetComponent<Vessel>();
						if (hitColliderVessel != null) {
							hitVessels.Add(hitColliderVessel);

							bool found = false;
							for (int o = 0; o < radiusLineRenderers.Count; o++) 
								if (radiusLineRenderers[o].v == hitColliderVessel)
									found = true;
							if (!found) {
								LineRenderer r = LineRendererPool.Instance.InstantiateAt();
								Vector3 radiusLineBasePos = hitColliderVessel.transform.position;
								Vector3 directionToCam = (CameraController.Instance.transform.position - radiusLineBasePos).normalized;
								r.SetPosition(0, radiusLineBasePos + directionToCam * LINE_RENDERER_OFFSET);
								radiusLineRenderers.Add(new VesselLineRenderer(hitColliderVessel, r));
							}
						}
					}

					// Remove all not in radius anymore and/or invalid positions
					for (int i = 0; i < radiusLineRenderers.Count; i++) {
						bool found = false;
						for (int o = 0; o < hitVessels.Count; o++) 
							if (hitVessels[o] == radiusLineRenderers[i].v)
								found = true;
						if (!found) {
							radiusLineRenderers[i].r.gameObject.SetActive(false);
							radiusLineRenderers.RemoveAt(i--);
						}
					}

					for (int i = 0; i < radiusLineRenderers.Count; i++) {
						Vector3 directionToCam = (CameraController.Instance.transform.position - v).normalized;
						radiusLineRenderers[i].r.SetPosition(1, v + directionToCam * LINE_RENDERER_OFFSET);
					}
				} else {
					radiusIdentifier.renderer.material.color = inactiveColor;
					
					for (int i = 0; i < radiusLineRenderers.Count; i++) 
						radiusLineRenderers[i].r.gameObject.SetActive(false);
					radiusLineRenderers.Clear();
				}
			}
		}

		if (Input.GetMouseButtonUp(0)) { 
			if (_sourceVessel != null) {
				// Determine where to instantiate
				Vector3 v = RaycastXYPlane(Input.mousePosition);
				float dist = Vector3.Distance(v, _sourceVessel.transform.position);

				// Instantiate new segment
				if (dist <= buildRadius && dist >= minBuildRadius) {
					// Get all endpoints in radius
					Collider[] hitColliders = Physics.OverlapSphere(v, buildRadius);
					
					Vessel newlyInitted = InstantiateVessel(_sourceVessel, v);
					for (int i = 0; i < hitColliders.Length; i++) {
						Vessel hitColliderVessel = hitColliders[i].GetComponent<Vessel>();
						if (hitColliderVessel != null && hitColliderVessel != _sourceVessel) {
							newlyInitted.AttachVessel(hitColliderVessel);
						}
					}
				}

				// TEST!!!
				if (dist <= minBuildRadius) {
					if (_sourceVessel is Organ) {
						GUICamController.Instance.OpenOrganMenu((Organ)_sourceVessel);
					}

					if (_sourceVessel is Organ && _sourceVessel != Heart.Instance) {
						// Target the Organ
						BCell b = Heart.Instance.GetIdleCell();
						if (b != null) {
							((Organ)_sourceVessel).OnRequestCell(b);
						}
					}
				}
			} 

			SourceVessel = null;
		}
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
				Vector3 directionToCam = (CameraController.Instance.transform.position - radiusLineBasePos).normalized;
				LineRenderer r = LineRendererPool.Instance.InstantiateAt();
				r.SetPosition(0, radiusLineBasePos + directionToCam * LINE_RENDERER_OFFSET);
				radiusLineRenderers.Add(new VesselLineRenderer(_sourceVessel, r));
			}
		}
	}

	public Vessel InstantiateVessel(Vessel attachedTo, Vector3 pos) {
		GameObject g = Instantiate(Heart.Instance.vNodePrefab, pos, Quaternion.identity) as GameObject;
		VesselEndpoint v = g.GetComponent<VesselEndpoint>();
		v.name = "VesselEndpoint_" + GameController.vesselCounter++;
		v.AttachVessel(attachedTo);
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
