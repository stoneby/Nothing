using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIMainScreenWindow : Window
{
    private UIEventListener addMoneyLis;
    private UIEventListener addMp;
    private UIEventListener startGameLis;
    private UIEventListener filpLeftLis;
    //private UIEventListener flipRightLis;

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        addMoneyLis = UIEventListener.Get(Utils.FindChild(transform, "Button-AddMoney").gameObject);
        addMp = UIEventListener.Get(Utils.FindChild(transform, "Button-AddMp").gameObject);
        startGameLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Start").gameObject);
        filpLeftLis = UIEventListener.Get(Utils.FindChild(transform, "Button-FlipL").gameObject);
        //flipRightLis = UIEventListener.Get(Utils.FindChild(transform, "Button-FlipR").gameObject);
    }

    private void InstallHandlers()
    {
        addMoneyLis.onClick += OnAddMoneyClicked;
        addMp.onClick += OnAddMpClicked;
        startGameLis.onClick += OnStartGameClicked;
        filpLeftLis.onClick += OnFlipLeftClicked;
        //flipRightLis.onClick += OnFlipRightClicked;
    }

    private void UnstallHandlers()
    {
        addMoneyLis.onClick -= OnAddMoneyClicked;
        addMp.onClick -= OnAddMpClicked;
        startGameLis.onClick -= OnStartGameClicked;
        filpLeftLis.onClick -= OnFlipLeftClicked;
        //flipRightLis.onClick -= OnFlipRightClicked;
    }

    private void OnAddMoneyClicked(GameObject go)
    {
        
    }

    private void OnAddMpClicked(GameObject go)
    {
        
    }

    private void OnStartGameClicked(GameObject go)
    {
        var csMsg = new CSRaidBattleStartMsg();
        csMsg.RaidId = 1;
        csMsg.FriendId = 1;
        NetManager.SendMessage(csMsg);
    }

    private void OnFlipLeftClicked(GameObject go)
    {
        
    }

    private void OnFlipRightClicked(GameObject go)
    {
        
    }

    #endregion
}
