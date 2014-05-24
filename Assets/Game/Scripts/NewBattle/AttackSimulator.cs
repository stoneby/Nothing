using UnityEngine;

public abstract class AttackSimulator : MonoBehaviour
{
    [HideInInspector]
    public TeamSelectController TeamController;
    
    public float RunTime;
    public float ReturnTime;
    
    public abstract void Attack(GameObject target);
    
    #region Mono

    protected virtual void Start()
    {
    }

    #endregion
}
