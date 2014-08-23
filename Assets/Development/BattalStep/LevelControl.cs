using UnityEngine;
using System.Collections;

public class LevelControl : MonoBehaviour {
	public GameObject[] characters;

	enum State{attack,run,idle,next};
	State state = State.idle;

	public GameObject g_btn_attack;
	//public GameObject g_btn_run;

	// Use this for initialization
	void Start () {
		UIEventListener.Get (g_btn_attack).onClick = AttackPress;

		characters = GameObject.FindGameObjectsWithTag ("character");
	}

	void FixedUpdate () {
		if (Input.GetKey(KeyCode.Escape)) Application.Quit();		
	}

	void AttackPress (GameObject btn) {
		StartCoroutine ("AttackProgram");
	}

	IEnumerator AttackProgram (){
		int menber = Random.Range (0,6);
		while (true) {
			for (int i = 0 ; i< menber ; i++){
				characters[i].transform.localPosition += Vector3.right*Time.deltaTime*1000;
			}	
			yield return 0;		
		}
	}

	// Update is called once per frame
	void Update () {
		if (state == State.idle) {
			foreach (GameObject g in characters) {
				PlayAnimation (g,"Idle");
			}
		}
	}

	void RandAnimation () {
		foreach (GameObject g in characters) {
			g.animation["Attack"].time = Random.Range(0,6);	
			g.animation["Idle"].time = Random.Range(0,6);	
			g.animation["Run"].time = Random.Range(0,10);		
		}
	}

	void PlayAnimation (GameObject g , string a) {
		switch (a) {
			case "Idle": 
				g.animation[a].speed = 1f;
				break;
			case "Run": 
				g.animation[a].speed = 1f;
				break;
			case "Attack": 
				g.animation[a].speed = 1f;
				break;

			if (g.animation[a].time == 1 || g.animation[a].time == 28 || g.animation[a].time == 43 || g.animation[a].time == 20)
				RandAnimation ();
			g.animation.CrossFade (a);		
		}
	}
}
