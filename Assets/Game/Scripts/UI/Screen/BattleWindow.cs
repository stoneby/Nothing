
using UnityEngine;

public class BattleWindow : Window
{
    #region Public Fields

    public InitBattleField Battle;

    #endregion

    #region Window

    public override void OnEnter()
    {
        Logger.Log("I am OnEnter with type - " + GetType().Name);

        Battle.Init();
        Battle.StartBattle();
    }

    public override void OnExit()
    {
        Logger.Log("I am OnExit with type - " + GetType().Name);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {

    }

    #endregion
}

