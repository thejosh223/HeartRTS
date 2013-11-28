using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	private bool _menuActive = false;

	public virtual void OnMenuActivityChange(bool b) {
		BuildController.Instance.isBuildMode = !b;
	}

	public bool MenuActive {
		get {
			return _menuActive;
		}
		set {
			if (value != _menuActive)
				OnMenuActivityChange(value);
			_menuActive = value;
		}
	}
}
