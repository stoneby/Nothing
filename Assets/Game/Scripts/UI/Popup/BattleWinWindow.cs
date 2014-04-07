
using UnityEngine;

public class BattleWinWindow : Window
{
    #region Window

    public override void OnEnter()
    {
        Debug.Log("I am OnEnter with type - " + GetType().Name);
    }

    public override void OnExit()
    {
        Debug.Log("I am OnExit with type - " + GetType().Name);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {

    }

    #endregion
}
