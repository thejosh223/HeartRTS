using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrganDamagePool : MonoBehaviour {
	
	private List<GameObject> _objects;
	public GameObject objectPrefab;
	public int instancesOfObjects;

	void Awake() {
		_instance = this;
	}
	
	void Start() {
		_objects = new List<GameObject>();
		
		for (int i = 0; i < instancesOfObjects; i++) {
			GameObject g = Instantiate(objectPrefab, new Vector3(0, 100, 0), Quaternion.identity) as GameObject;
			g.gameObject.SetActive(false);
			g.transform.parent = transform;
			_objects.Add(g);
		}
	}
	
	public GameObject InstantiateAt(Vector3 v, Quaternion q) {
		for (int i = 0; i < _objects.Count; i++) {
			if (_objects[i].gameObject.activeSelf)
				continue;
			
			_objects[i].gameObject.SetActive(true);
			_objects[i].transform.position = v;
			_objects[i].transform.rotation = q;
			_objects[i].transform.parent = transform;
			_objects[i].GetComponent<ShakerAnimation>().StartAnimation();
			return _objects[i];
		}

		// Instantiate New
		GameObject g = Instantiate(objectPrefab, v, q) as GameObject;
		g.gameObject.SetActive(true);
		g.transform.parent = transform;
		g.GetComponent<ShakerAnimation>().StartAnimation();
		_objects.Add(g);
		return g;
	}
	
	/*
	 * Singleton
	 */
	private static OrganDamagePool _instance;
	
	public static OrganDamagePool Instance {
		get { return _instance; }
	}
}
