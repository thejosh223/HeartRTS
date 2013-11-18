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

			if (b.TargetOrgan == null)
				continue;

			float energyTransfer = 0;
			switch (b.MovementMode) {
				case MovementType.DepositAtHeart:
					energyTransfer = Mathf.Min(currentCells[i].CurrentEnergy, Time.deltaTime * ENERGY_TRANSFER_RATE);
					break;
				case MovementType.DepositAtOrgan:
					energyTransfer = Mathf.Min(currentCells[i].CurrentEnergy, Time.deltaTime * ENERGY_TRANSFER_RATE);
					break;
				case MovementType.GatherAtHeart:
					energyTransfer = -Mathf.Min(currentCells[i].MaxEnergy - currentCells[i].CurrentEnergy, Time.deltaTime * ENERGY_TRANSFER_RATE);
					break;
				case MovementType.GatherAtOrgan:
					energyTransfer = -Mathf.Min(currentCells[i].MaxEnergy - currentCells[i].CurrentEnergy, Time.deltaTime * ENERGY_TRANSFER_RATE);
					break;
			}

			currentCells[i].CurrentEnergy -= energyTransfer;
			energy += energyTransfer;

			switch (b.MovementMode) {
				case MovementType.DepositAtHeart:
					if (b.CurrentEnergy == 0)
						PumpOutCell(b, b.TargetOrgan);
					break;
				case MovementType.DepositAtOrgan:
					if (b.CurrentEnergy == 0)
						PumpOutCell(b, Heart.Instance);
					break;
				case MovementType.GatherAtHeart:
					if (b.CurrentEnergy == b.MaxEnergy)
						PumpOutCell(b, b.TargetOrgan);
					break;
				case MovementType.GatherAtOrgan:
					if (b.CurrentEnergy == b.MaxEnergy)
						PumpOutCell(b, Heart.Instance);
					break;
			}
		}
	}

	public virtual void PumpOutCell(BCell b, Organ target) {
		b.SetTarget(this, target);
		BCellExit(b);
		GetImmediateVesselTo(b.nextTarget).BCellEnter(b);
	}

	public virtual void OnRequestCell(BCell b) {
		b.TargetOrgan = this;
	}

	public override void BCellEnter(BCell b) {
		currentCells.Add(b);
	}
	
	public override void BCellExit(BCell b) {
		currentCells.Remove(b);
	}
}
