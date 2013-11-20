using UnityEngine;
using System.Collections;

public class Lymphnode : Organ {
	
	public override MovementType GetDefaultBehaviour() {
		// This is redundant because base() returns the same value.
		return MovementType.Deposit;
	}

}
