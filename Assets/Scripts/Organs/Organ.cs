using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Organ : Vessel {
	
	public const int MAX_CELLS = 8;
	public const float ENERGY_TRANSFER_RATE = 10f;
	public const string CONNECTION_NAME = "Connection";

	//
	protected GameObject _model;
	protected Transform[] connectionPoints;
	protected List<BCell> pumpOutQueue = new List<BCell>();
	protected List<BCell> currentCells = new List<BCell>();
	protected float energyTransferRate = ENERGY_TRANSFER_RATE;
	public float energy = 0;

	protected override void Start() {
		base.Start();
		Transform t = transform.FindChild("Model");
		if (t != null)
			_model = t.gameObject;

		// Get connection points
		List<Transform> children = new List<Transform>();
		foreach (Transform child in transform)
			if (child.name == CONNECTION_NAME) 
				children.Add(child);
		connectionPoints = children.ToArray();
		if (connectionPoints.Length == 0)
		connectionPoints = new Transform[] { transform };
	}

	protected override void Update() {
		base.Update();

		for (int i = 0; i < currentCells.Count; i++) {
			BCell b = currentCells[i];

			if (b.finalTarget != null) {
				if (b.finalTarget == this) 
					MovementTypeCall(currentCells[i], b.MovementMode);
				else 
					QueuePumpOutCell(currentCells[i]);
			}
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

	public void PumpOutCell(BCell b) {
		if (b.finalTarget == this) 
			b.ExecutePath();
		BCellExit(b);
		
		Vessel v = GetImmediateVesselTo(b.nextTarget);
		v.BCellEnter(b);
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
