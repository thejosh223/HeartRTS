using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Organ : Vessel {
	
	public const int MAX_CELLS = 8;
	public const float ENERGY_TRANSFER_RATE = 10f;

	//
	protected List<BCell> currentCells = new List<BCell>();
	protected float energyTransferRate = ENERGY_TRANSFER_RATE;
	protected float energy = 0;

	protected override void Update() {
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
								PumpOutCell(currentCells[i]);
							}
							break;
						case MovementType.Gather:
							energyTransfer = -Mathf.Min(currentCells[i].MaxEnergy - currentCells[i].CurrentEnergy, Time.deltaTime * ENERGY_TRANSFER_RATE);
							currentCells[i].CurrentEnergy -= energyTransfer;
							energy += energyTransfer;

							if (currentCells[i].CurrentEnergy == currentCells[i].MaxEnergy) 
								PumpOutCell(currentCells[i]);
							break;
						case MovementType.Wait:
							MovementTypeCall(currentCells[i], MovementType.Wait);
							break;
					}
				} else {
					PumpOutCell(currentCells[i]);
				}
			}
		}
	}

	protected virtual void MovementTypeCall(BCell b, MovementType mov) {
	}

	public void PumpOutCell(BCell b) {
		if (b.finalTarget == this) {
			b.ExecutePath();
		}
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
