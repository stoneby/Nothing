using UnityEngine;
using System.Collections;

public class AnimationSpeed : MonoBehaviour {

//	public string _name = "";
//	public float _speed = 2.0f;

	// Use this for initialization
	void Start () {
//		if (_name != "") {
//			animation[_name].speed = _speed;
//		}
		animation["Take run01"].speed = 2.0f;
		animation["Take attack01"].speed = 2.0f;
		animation["Take idle01"].speed = 1.4f;
	}
	
	// Update is called once per frame
	void Update () {
		AnimationPlay ();
	}

	void AnimationPlay () {
		if (Input.GetMouseButton (1)) {
			animation.CrossFade ("Take run01");
		} else if (Input.GetMouseButton (0)) {
			animation.CrossFade ("Take attack01");		
		}else{
			animation.CrossFade ("Take idle01");
		}
	}
}
