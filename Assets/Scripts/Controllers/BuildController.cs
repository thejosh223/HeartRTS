using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildController : MonoBehaviour {

	public const float SNAP_RADIUS = 1f;

	//
	public bool isBuildMode = true;
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
				// Get the transform of the closest ConnectionPoint
				Vector3 mousePos = RaycastXYPlane(Input.mousePosition);

				// Set connection point from the sourceVessel
				Transform closestConnnectionPoint = ClosestConnectionPoint(_sourceVessel, mousePos);
				radiusIdentifier.transform.position = closestConnnectionPoint.position;

				// Compute Distance
				float dist = Vector3.Distance(closestConnnectionPoint.position, mousePos);
				Vessel snapToVessel = ClosestVessel(mousePos, SNAP_RADIUS, _sourceVessel.transform.position, buildRadius);

				if (dist <= buildRadius) {
					if (snapToVessel != null) 
						mousePos = ClosestConnectionPoint(snapToVessel, mousePos).position;

					radiusIdentifier.renderer.material.color = activeColor;
					for (int i = 0; i < radiusLineRenderers.Count; i++) {
						Vector3 srcPos = ClosestConnectionPoint(_sourceVessel, mousePos).position;
						radiusLineRenderers[i].r.SetPosition(0, srcPos - Vector3.forward * LINE_RENDERER_OFFSET);
						radiusLineRenderers[i].r.SetPosition(1, mousePos - Vector3.forward * LINE_RENDERER_OFFSET);
						radiusLineRenderers[i].r.enabled = true;
					}
				} else {
					radiusIdentifier.renderer.material.color = inactiveColor;
					for (int i = 0; i < radiusLineRenderers.Count; i++)
						radiusLineRenderers[i].r.enabled = false;
				}
			}
		}

		if (Input.GetMouseButtonUp(0)) { 
			if (_sourceVessel != null) {
				// Get the transform of the closest ConnectionPoint
				Vector3 mousePos = RaycastXYPlane(Input.mousePosition);
				Transform closestConnnectionPoint = ClosestConnectionPoint(_sourceVessel, mousePos);
				
				// Set closestConnectionPoint
				radiusIdentifier.transform.position = closestConnnectionPoint.position;
				
				// Compute Distance
				float dist = Vector3.Distance(closestConnnectionPoint.position, mousePos);

				// Instantiate new segment
				if (dist <= buildRadius) {
					if (Heart.Instance.energy >= Vessel.VESSELSEGMENT_COST) {
						Vessel hitVessel = null;
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						RaycastHit hit;
						if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
							hitVessel = hit.transform.GetComponent<Vessel>();

						if (hitVessel != null) {
							if (hitVessel != _sourceVessel && hitVessel != _sourceVessel && !hitVessel.IsConnectedTo(_sourceVessel)) {
								// Vessel or Organ Hit
								// -No need to instantiate.
								hitVessel.AttachVessel(_sourceVessel, //
										ClosestConnectionPoint(_sourceVessel, mousePos), //
										ClosestConnectionPoint(hitVessel, mousePos));
								OnBuild();
							}
						} else {
							Vessel snapToVessel = ClosestVessel(mousePos, SNAP_RADIUS, _sourceVessel.transform.position, buildRadius);

							if (snapToVessel != null) {
								snapToVessel.AttachVessel(_sourceVessel, //
								                          ClosestConnectionPoint(_sourceVessel, mousePos),//
								                          ClosestConnectionPoint(snapToVessel, mousePos));
								OnBuild();
							} else {
								// Nothing Hit.
								Vessel newlyInitted = InstantiateVessel(_sourceVessel, mousePos);
								newlyInitted.AttachVessel(_sourceVessel, //
								                          ClosestConnectionPoint(_sourceVessel, mousePos), //
								                          ClosestConnectionPoint(newlyInitted, mousePos));
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
				radiusIdentifier.transform.localScale = new Vector3(buildRadius * 2f, buildRadius * 2f, buildRadius * 2f);
				// radiusIdentifier.transform.position = _sourceVessel.transform.position;

				// Add one line renderer
				radiusLineRenderers.Add(new VesselLineRenderer(_sourceVessel, LineRendererPool.Instance.InstantiateAt()));
			}
		}
	}

	public Vessel InstantiateVessel(Vessel attachedTo, Vector3 pos) {
		GameObject g = Instantiate(Heart.Instance.vNodePrefab, pos, Quaternion.identity) as GameObject;
		VesselEndpoint v = g.GetComponent<VesselEndpoint>();
		v.name = "VesselEndpoint_" + GameController.vesselCounter++;
		return v;
	}

	/*
	 * Casting Functions
	 */
	protected Vessel ClosestVessel(Vector3 v, float radius, Vector3 center, float centerMaxDist) {
		Collider[] c = Physics.OverlapSphere(v, radius);
		Vessel minVessel = null;
		float minDist = Mathf.Infinity;
		for (int i = 0; i < c.Length; i++) {
			Vessel ves = c[i].GetComponent<Vessel>();
			if (ves != null && ves != _sourceVessel && !ves.IsConnectedTo(_sourceVessel)) {
				float dist = Vector3.Distance(ves.transform.position, v);
				float centerDist = Vector3.Distance(ves.transform.position, center);
				if (centerDist < centerMaxDist && dist < minDist) {
					minVessel = ves;
					minDist = dist;
				}
			}
		}
		return minVessel;
	}

	protected Transform ClosestConnectionPoint(Vessel ves, Vector3 v) {
		Transform[] t = ves.GetConnectionPoints();
		Transform closestTrans = t[0];
		float closestDist = Vector3.Distance(t[0].position, v);

		if (t.Length > 1) {
			for (int i = 1; i < t.Length; i++) {
				float d = Vector3.Distance(t[i].position, v);
				if (d < closestDist) {
					closestDist = d;
					closestTrans = t[i];
				}
			}
		}
		return closestTrans;
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
