using UnityEngine;

public class AttackSimulator2D : AttackSimulator
{

    // Use this for initialization
    public override void Attack(GameObject target)
    {
        Debug.LogWarning("Attack - " + target.name);
    }
}
