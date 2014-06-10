
using UnityEngine;

public class BattleWinWindow : BattleResultWindow
{
    #region Window

    public override void OnEnter()
    {
        base.OnEnter();

        Logger.Log("I am OnEnter with type - " + GetType().Name);
    }

    public override void OnExit()
    {
        base.OnExit();
    
        Logger.Log("I am OnExit with type - " + GetType().Name);
    }

    #endregion

    #region Battle Result Window

    protected override void OnBackgroundClick(GameObject sender)
    {
        Close();

        if (OnBattleResult != null)
        {
            OnBattleResult(true);
        }
    }

    #endregion
}
