using UnityEngine;
using System.Collections;

public class AttackSimulator2D : AttackSimulator {

	// Use this for initialization
    public override void Attack(GameObject target)
    {
        Debug.LogWarning("Attack - " + target.name);
    }

    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
