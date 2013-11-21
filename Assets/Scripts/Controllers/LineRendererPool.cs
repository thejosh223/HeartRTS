using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineRendererPool : MonoBehaviour {
	
	private List<LineRenderer> _objects;
	public LineRenderer objectPrefab;
	public int instancesOfObjects;
	
	void Awake() {
		_instance = this;
	}
	
	void Start() {
		_objects = new List<LineRenderer>();
		
		for (int i = 0; i < instancesOfObjects; i++) {
			LineRenderer r = Instantiate(objectPrefab, new Vector3(0, 100, 0), Quaternion.identity) as LineRenderer;
			r.gameObject.SetActive(false);
			r.transform.parent = transform;
			_objects.Add(r);
		}
	}
	
	public LineRenderer InstantiateAt() {
		for (int i = 0; i < _objects.Count; i++) {
			if (_objects[i].gameObject.activeSelf)
				continue;
			
			_objects[i].gameObject.SetActive(true);
			return _objects[i];
		}
		
		LineRenderer r = Instantiate(objectPrefab, new Vector3(0, 100, 0), Quaternion.identity) as LineRenderer;
		r.gameObject.SetActive(false);
		r.transform.parent = transform;
		_objects.Add(r);
		return r;
	}
	
	/*
	 * Singleton
	 */
	private static LineRendererPool _instance;
	
	public static LineRendererPool Instance {
		get { return _instance; }
	}
}
