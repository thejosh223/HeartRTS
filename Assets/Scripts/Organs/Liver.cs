using UnityEngine;
using System.Collections;

public class Liver : Organ {
	
	void OnMouseDown() {
		Heart.Instance.TargetCell(this);
		Debug.Log("Target Liver!");
	}

	public override void PumpOutCell(BCell b, Organ target) {
		base.PumpOutCell(b, target);
		
		// Set Behaviour
		b.MovementMode = MovementType.GatherAtHeart;
	}

}
