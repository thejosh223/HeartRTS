using UnityEngine;
using System.Collections;

public class Kidney : Organ {

	public const float MULTIPLIER_TIME = 1f;
	public const float MULTIPLIER_AMT = 2f;

	protected override void MovementTypeCall(BCell b, MovementType mov) {
		if (mov == MovementType.Wait) {
			if (b.cellTimer == -1) {
				b.cellTimer = Time.time + MULTIPLIER_TIME;
			} else {
				if (b.cellTimer <= Time.time) {
					b.energyMultiplier = MULTIPLIER_AMT;
					QueuePumpOutCell(b);
				}
			}
		}
	}

	public override MovementType GetDefaultBehaviour() {
		return MovementType.Wait;
	}
}
