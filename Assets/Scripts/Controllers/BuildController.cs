using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildController : MonoBehaviour {

	public const float SNAP_RADIUS = 0.5f;
	public static int ORGAN_LAYER;
	public static int OBSTACLE_LAYER;

	//
	public bool isBuildMode = true;
	protected float minBuildRadius = 2f;
	protected Plane xyZeroPlane;
	private Vessel _sourceVessel;

	// Build Guidelines
	private static float LINE_RENDERER_OFFSET = 2f;
	private GameObject radiusIdentifier;
	private List<VesselLineRenderer> radiusLineRenderers = new List<VesselLineRenderer>();
	private Color activeColor = new Color(109 / 255f, 194 / 255f, 255 / 255f);
	private Color inactiveColor = new Color(255 / 255f, 131 / 255f, 119 / 255f);

	void Awake() {
		_instance = this;
		
		ORGAN_LAYER = 1 << LayerMask.NameToLayer("Organ");
		OBSTACLE_LAYER = 1 << LayerMask.NameToLayer("Obstacles");
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
				Transform closestConnectionPt = ClosestConnectionPoint(_sourceVessel, mousePos);
				radiusIdentifier.transform.position = _sourceVessel.transform.position;

				// Compute Distance
				float dist = Vector3.Distance(_sourceVessel.transform.position, mousePos);
				Vessel snapToVessel = ClosestVessel(mousePos, SNAP_RADIUS, _sourceVessel.transform.position, _sourceVessel.buildRadius);

				if (dist <= _sourceVessel.buildRadius && !IsObstructed(closestConnectionPt.position, mousePos)) {
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
				Transform closestConnectionPt = ClosestConnectionPoint(_sourceVessel, mousePos);
				
				// Set closestConnectionPoint
				radiusIdentifier.transform.position = _sourceVessel.transform.position;
				
				// Compute Distance
				float dist = Vector3.Distance(_sourceVessel.transform.position, mousePos);

				// Instantiate new segment
				if (dist <= _sourceVessel.buildRadius) {
					if (Heart.Instance.energy >= Vessel.VESSELSEGMENT_COST) {
						Vessel hitVessel = null;
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						RaycastHit hit;
						if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
							hitVessel = hit.transform.GetComponent<Vessel>();

						if (hitVessel != null) {
							if (hitVessel != _sourceVessel && !hitVessel.IsConnectedTo(_sourceVessel)) {
								// Build to a raycasted Organ/Node
								Transform closestTargetConnectionPt = ClosestConnectionPoint(hitVessel, mousePos);
								if (!IsObstructed(closestConnectionPt.position, closestTargetConnectionPt.position)) {
									hitVessel.AttachVessel(_sourceVessel, closestConnectionPt, closestTargetConnectionPt);
									OnBuild();
								}
							} else {
							}
						} else {
							Vessel snapToVessel = ClosestVessel(mousePos, SNAP_RADIUS, _sourceVessel.transform.position, _sourceVessel.buildRadius);
							if (snapToVessel != null) {
								// Snap to closest visible organ/node
								Transform closestTargetConnectionPt = ClosestConnectionPoint(snapToVessel, mousePos);
								if (!IsObstructed(closestConnectionPt.position, closestTargetConnectionPt.position)) {
									snapToVessel.AttachVessel(_sourceVessel, closestConnectionPt, closestTargetConnectionPt);
									OnBuild();
								}
							} else { 
								// Nothing hit, instantiate new vessel
								if (!IsObstructed(closestConnectionPt.position, mousePos)) {
									Vessel newlyInitted = InstantiateVessel(_sourceVessel, mousePos); 
									newlyInitted.AttachVessel(_sourceVessel, closestConnectionPt, ClosestConnectionPoint(newlyInitted, mousePos));
									OnBuild();
								} else {
									Debug.Log("Obstructed!");
								}
							}
						}
					}
				}

				if (dist <= minBuildRadius) {
					if (_sourceVessel is Organ) {
						((Organ)_sourceVessel).OpenOrganMenu();
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
				float f = _sourceVessel.buildRadius * 2f;
				radiusIdentifier.transform.localScale = new Vector3(f, f, f);
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
		Collider[] c = Physics.OverlapSphere(v, radius, ORGAN_LAYER);
		Vessel minVessel = null;
		float minDist = Mathf.Infinity;
		for (int i = 0; i < c.Length; i++) {
			Vessel ves = c[i].GetComponent<Vessel>();
			if (ves != null && ves != _sourceVessel && !ves.IsConnectedTo(_sourceVessel)) {
				float dist = Vector3.Distance(ves.transform.position, v);
//				float centerDist = Vector3.Distance(ves.transform.position, center);
//				if (centerDist < centerMaxDist && dist < minDist) {
				if (dist < minDist) {
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

	protected bool IsObstructed(Vector3 v1, Vector3 v2) {
		Debug.DrawLine(v1, v2);
		Vector3 v = v2 - v1;
		if (Physics.Raycast(v1, v, v.magnitude, OBSTACLE_LAYER)) 
			return true;
		return false;
	}

	protected RaycastHit Raycast(Vector3 v) {
		Ray ray = Camera.main.ScreenPointToRay(v);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, ORGAN_LAYER))
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
