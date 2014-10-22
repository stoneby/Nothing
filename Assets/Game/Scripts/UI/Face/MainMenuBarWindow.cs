using KXSGCodec;
using UnityEngine;

public class MainMenuBarWindow : Window
{
    public MailAlertChecker NewBehaviourScript;
    #region Window

    public override void OnEnter()
    {
        NewBehaviourScript.OnEnter();
    }

    public override void OnExit()
    {
        NewBehaviourScript.OnExit();
    }

    #endregion

    #region Private Methods

    public void OnEmail()
    {
        var csmsg = new CSMailListMsg();
        if(MailModelLocator.Instance.MailListVersion > 0)
        {
            csmsg.ListVersion = MailModelLocator.Instance.MailListVersion;
        }
        NetManager.SendMessage(csmsg);
    }

    /// <summary>
    /// The callback of clicking log button.
    /// </summary>
    public void OnChatClicked(GameObject go)
    {
        //This is just for testing.
        go.SetActive(!go.gameObject.activeInHierarchy);
    }

    /// <summary>
    /// The callback of clicking menu button.
    /// </summary>
    public void OnBattleClicked()
    {
        MissionModelLocator.Instance.ShowRaidWindow();  
    }

    /// <summary>
    /// The callback of clicking summon button.
    /// </summary>
    public void OnSummonClicked()
    {
        WindowManager.Instance.Show<ChooseCardWindow>(true);
    }

    /// <summary>
    /// The callback of clicking friend button.
    /// </summary>
    public void OnFriendClicked()
    {
        var isOutOfDate = FriendUtils.SendFriendListMessage(FriendModelLocator.FriendListType.LoadingAll);
        if (!isOutOfDate)
        {
            WindowManager.Instance.Show<UIFriendEntryWindow>(true);
        }
    }

    /// <summary>
    /// The callback of clicking equip button.
    /// </summary>
    public void OnEquipClicked()
    {
        //PopTextManager.PopTip("暂未开放该功能！", false);
        OpenEquipWin();
    }

    public static void OpenEquipWin()
    {
        if (ItemModeLocator.AlreadyMainRequest == false)
        {
            ItemModeLocator.Instance.GetItemPos = ItemType.GetItemInPanel;
            var csmsg = new CSQueryAllItems { BagType = ItemType.MainItemBagType };
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show<UIItemCommonWindow>(true);
        }
    }

    /// <summary>
    /// The callback of clicking team button.
    /// </summary>
    public void OnTeamClicked()
    {
        OpenTeamWin();
        //PopTextManager.PopTip("暂未开放该功能！", false);
    }

    public static void OpenTeamWin()
    {
        if (HeroModelLocator.AlreadyRequest == false)
        {
            HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInHeroCreateTeam;
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show<UIBuildingTeamWindow>(true);
        }
    }


    /// <summary>
    /// The callback of clicking hero button.
    /// </summary>
    public void OnHeroClicked()
    {
        //PopTextManager.PopTip("暂未开放该功能！", false);
        //WindowManager.Instance.Show<HeroMenuBarWindow>(true);
        OpenHeroWin();
    }

    public static void OpenHeroWin()
    {
        if (HeroModelLocator.AlreadyRequest == false)
        {
            HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInHeroPanel;
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show<UIHeroCommonWindow>(true);
        }
    }

    /// <summary>
    /// The callback of clicking home button.
    /// </summary>
    public void OnHomeClicked()
    {
        var curMap = WindowManager.Instance.CurrentWindowMap;
        if(curMap.ContainsKey(WindowGroupType.TabPanel))
        {
            var curTabWindow = curMap[WindowGroupType.TabPanel];
            curTabWindow.gameObject.SetActive(false);
        }
    }

    #endregion
}
