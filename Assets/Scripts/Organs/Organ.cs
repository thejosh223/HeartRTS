using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Organ : Vessel {

	public const float ANIM_TIME = 4f;
	public const float ANIM_SCALE = 0.2f;
	public const int MAX_CELLS = 8;
	public const float ENERGY_TRANSFER_RATE = 10f;
	public const string CONNECTION_NAME = "Connection";

	// 
	public Upgrade[] upgrades; // Crunch time. Make EVERYTHING public.
	protected Transform[] connectionPoints;
	protected List<BCell> pumpOutQueue = new List<BCell>(); // queue for cells leaving the organ. pump out one per heartbeat.
	protected List<BCell> currentCells = new List<BCell>(); // cells currently inside the organ

	// Organ Stats
	protected float energyTransferRate = ENERGY_TRANSFER_RATE;
	public float lifeRegen = 0.1f; // (ie. 10 seconds to revive)
	protected float life = 1;
	public float energy = 0;

	// Animation variables
	protected GameObject model; // for animation purposes
	protected float startTime = 0; // animation constant

	protected override void Start() {
		base.Start();
		buildRadius = 6f;

		Transform t = transform.FindChild("Model");
		if (t != null)
			model = t.gameObject;

		// Get connection points
		List<Transform> children = new List<Transform>();
		foreach (Transform child in transform)
			if (child.name == CONNECTION_NAME) 
				children.Add(child);
		connectionPoints = children.ToArray();
		if (connectionPoints.Length == 0)
			connectionPoints = new Transform[] { transform };

		// Animation
		startTime = Time.time - Random.Range(0f, 2 * Mathf.PI);
		animator.hasScaleAnimation = true;
	}

	protected override void Update() {
		base.Update();

		// Pumping out of cells
		for (int i = 0; i < currentCells.Count; i++) {
			BCell b = currentCells[i];

			if (b.finalTarget != null) {
				if (b.finalTarget == this) 
					MovementTypeCall(currentCells[i], b.MovementMode);
				else 
					QueuePumpOutCell(currentCells[i]);
			}
		}

		// Animation
		if (this != Heart.Instance) {
			float t = (((Time.time - startTime) % ANIM_TIME) / ANIM_TIME) * 2 * Mathf.PI;
			animator.deltaScale += new Vector3(Mathf.Sin(t) * ANIM_SCALE, Mathf.Cos(t) * ANIM_SCALE, 0f);
		}

		if (Life < 1) {
			Life += lifeRegen * Time.deltaTime;
		}
	}

	protected virtual void MovementTypeCall(BCell b, MovementType mov) {
		float energyTransfer = 0;
		switch (mov) {
			case MovementType.Deposit:
				energyTransfer = Mathf.Min(b.CurrentEnergy, Time.deltaTime * ENERGY_TRANSFER_RATE);
				b.CurrentEnergy -= energyTransfer;
				energy += energyTransfer * b.energyMultiplier;
				
				if (b.CurrentEnergy == 0) {
					b.energyMultiplier = 1f;
					QueuePumpOutCell(b);
				}
				break;
			case MovementType.Gather:
				energyTransfer = Mathf.Min(b.MaxEnergy - b.CurrentEnergy, Time.deltaTime * ENERGY_TRANSFER_RATE);
				energyTransfer = Mathf.Min(energyTransfer, energy);
				b.CurrentEnergy += energyTransfer;
				energy -= energyTransfer;
				
				if (b.CurrentEnergy == b.MaxEnergy || energy == 0) {
					QueuePumpOutCell(b);
				}
				break;
			default:
				break;
		}
	}

	public void QueuePumpOutCell(BCell b) {
		currentCells.Remove(b);
		pumpOutQueue.Add(b);
	}

	public void OnHeartPump() {
		if (pumpOutQueue.Count > 0) {
			BCell b = pumpOutQueue[0];
			pumpOutQueue.RemoveAt(0);
			PumpOutCell(b);
		}
	}

	public virtual void OpenOrganMenu() {
		GUICamController.Instance.OpenOrganMenu(this);
		if (upgrades != null && upgrades.Length > 0) 
			GUICamController.Instance.OpenUpgradeMenu(this);
	}

	public void PumpOutCell(BCell b) {
		if (b.finalTarget == this) 
			b.ExecutePath();
		BCellExit(b);
		
		Vessel v = GetImmediateVesselTo(b.nextTarget);
		v.BCellEnter(b);
	}

	public float Life {
		get { return life; }
		set {
			life = value;

			if (life <= 0) {
				life = 0;

				// Disable the organ

				// Animate it's death or something
			}
		}
	}

	public virtual MovementType GetDefaultBehaviour() {
		return MovementType.Deposit;
	}

	public override void BCellEnter(BCell b) {
		base.BCellEnter(b);
		currentCells.Add(b);
	}
	
	public override void BCellExit(BCell b) {
		base.BCellExit(b);
		currentCells.Remove(b);
	}

	public override Transform[] GetConnectionPoints() {
		return connectionPoints;
	}
}
