using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class BattleLostWindow : BattleResultWindow
{
    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.BattleFailWindow);
        base.OnEnter();
        Logger.Log("I am OnEnter with type - " + GetType().Name);
    }

    public override void OnExit()
    {
        base.OnExit();
        Logger.Log("I am OnExit with type - " + GetType().Name);
        MtaManager.TrackEndPage(MtaType.BattleFailWindow);
    }

    #endregion

    #region Battle Result Window

    protected override void OnBackgroundClick(GameObject sender)
    {
        Close();

        if (OnBattleResult != null)
        {
            OnBattleResult(false);
        }
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
    }

    #endregion
}
