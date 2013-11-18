using UnityEngine;
using System.Collections;

public class LungCell : Organ {

	public override void OnRequestCell(BCell b) {
		base.OnRequestCell(b);
		b.MovementMode = MovementType.DepositAtHeart;
	}

	public override void PumpOutCell(BCell b, Organ target) {
		base.PumpOutCell(b, target);
		
		// Set Behaviour
		b.MovementMode = MovementType.DepositAtHeart;
	}

}
