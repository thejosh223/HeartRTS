using UnityEngine;
using System.Collections;

public class BCell {

	public Vessel nextTarget;

	public BCell () {
	}

	public BCell[] Divide(int count) {
		BCell[] b = new BCell[count];
		b[0] = this;
		for (int i = 1; i < b.Length; i++)
			b[i] = new BCell();
		return b;
	}

}
