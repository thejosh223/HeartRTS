using UnityEngine;
using System.Collections;

public class LungCell : Organ {

	public override MovementType GetDefaultBehaviour() {
		return MovementType.Gather;
	}

}
