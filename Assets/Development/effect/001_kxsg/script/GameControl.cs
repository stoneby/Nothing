using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		EffectManager.InitEffects (gameObject);
		EffectManager.PlayEffect ("particle/yanhuo_001", 500, 0, 0, new Vector3 (0, 0, 0));
		EffectManager.PlayEffect ("particle/baozha_001", 500, 100, 0, new Vector3 (0, 0, 0));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
