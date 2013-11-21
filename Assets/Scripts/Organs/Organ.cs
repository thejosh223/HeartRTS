using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Organ : Vessel {
	
	public const int MAX_CELLS = 8;
	public const float ENERGY_TRANSFER_RATE = 10f;

	//
	protected List<BCell> pumpOutQueue = new List<BCell>();
	protected List<BCell> currentCells = new List<BCell>();
	protected float energyTransferRate = ENERGY_TRANSFER_RATE;
	public float energy = 0;

	protected override void Update() {
		base.Update();

		for (int i = 0; i < currentCells.Count; i++) {
			BCell b = currentCells[i];

			if (b.finalTarget != null) {
				if (b.finalTarget == this) {
					float energyTransfer = 0;
					switch (b.MovementMode) {
						case MovementType.Deposit:
							energyTransfer = Mathf.Min(currentCells[i].CurrentEnergy, Time.deltaTime * ENERGY_TRANSFER_RATE);
							currentCells[i].CurrentEnergy -= energyTransfer;
							energy += energyTransfer * currentCells[i].energyMultiplier;

							if (currentCells[i].CurrentEnergy == 0) {
								currentCells[i].energyMultiplier = 1f;
								QueuePumpOutCell(currentCells[i]);
							}
							break;
						case MovementType.Gather:
							energyTransfer = -Mathf.Min(currentCells[i].MaxEnergy - currentCells[i].CurrentEnergy, Time.deltaTime * ENERGY_TRANSFER_RATE);
							currentCells[i].CurrentEnergy -= energyTransfer;
							energy += energyTransfer;

							if (currentCells[i].CurrentEnergy == currentCells[i].MaxEnergy) {
								QueuePumpOutCell(currentCells[i]);
							}
							break;
						case MovementType.Wait:
							MovementTypeCall(currentCells[i], MovementType.Wait);
							break;
					}
				} else {
					Debug.Log("final Target: " + b.finalTarget.name);
					QueuePumpOutCell(currentCells[i]);
				}
			}
		}
	}

	protected virtual void MovementTypeCall(BCell b, MovementType mov) {
	}

	public void QueuePumpOutCell(BCell b) {
		pumpOutQueue.Add(b);
	}

	public void OnHeartPump() {
		Debug.Log("C: " + name + " > " + currentCells.Count);

		if (pumpOutQueue.Count > 0) {
			BCell b = pumpOutQueue[0];
			pumpOutQueue.RemoveAt(0);
			currentCells.Remove(b);
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
}
