using UnityEngine;
using System.Collections;

public class GameWorldControl : MonoBehaviour {
	public GameObject openBtnSprite;
	// Use this for initialization
	void Start () {
		UISprite btn = openBtnSprite.GetComponent<UISprite>();
		btn.alpha = 0.1f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
