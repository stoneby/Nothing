using UnityEngine;
using System.Collections;

public class AnimationSpeed : MonoBehaviour {

	public string _name = "";
	public float _speed = 2.0f;

	// Use this for initialization
	void Start () {
		if (_name != "") {
			animation[_name].speed = _speed;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
