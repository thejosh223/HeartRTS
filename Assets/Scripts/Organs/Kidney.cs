using UnityEngine;
using System.Collections;

public class Kidney : Organ {

	public override void OnRequestCell(BCell b) {
		base.OnRequestCell(b);
		b.MovementMode = MovementType.GatherAtHeart;
	}

	public override void PumpOutCell(BCell b, Organ target) {
		base.PumpOutCell(b, target);
		
		// Set Behaviour
		b.MovementMode = MovementType.GatherAtHeart;
	}

}
