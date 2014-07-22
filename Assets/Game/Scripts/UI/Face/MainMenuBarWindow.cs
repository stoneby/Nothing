using KXSGCodec;
using UnityEngine;

public class MainMenuBarWindow : Window
{
    #region Private Fields

    private UIEventListener homeLis;
    private UIEventListener heroLis;
    private UIEventListener teamLis;
    private UIEventListener equipLis;
    private UIEventListener summonLis;
    private UIEventListener menuLis;
    private UIEventListener logLis;
    private GameObject inputLog;

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Awake()
    {
        homeLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Home").gameObject);
        heroLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Hero").gameObject);
        teamLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Team").gameObject);
        equipLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Equip").gameObject);
        summonLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Summon").gameObject);
        menuLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Menu").gameObject);
        logLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Log").gameObject);
        inputLog = Utils.FindChild(transform, "Input-Log").gameObject;
        inputLog.SetActive(false);
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        homeLis.onClick = OnHomeClicked;
        heroLis.onClick = OnHeroClicked;
        teamLis.onClick = OnTeamClicked;
        equipLis.onClick = OnEquipClicked;
        summonLis.onClick = OnSummonClicked;
        menuLis.onClick = OnMenuClicked;
        logLis.onClick = OnLogClicked;
    }

    /// <summary>
    /// Uninstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        homeLis.onClick = null;
        heroLis.onClick = null;
        teamLis.onClick = null;
        equipLis.onClick = null;
        summonLis.onClick = null;
    }

    /// <summary>
    /// The callback of clicking log button.
    /// </summary>
    private void OnLogClicked(GameObject go)
    {
        //This is just for testing.
        inputLog.SetActive(!inputLog.gameObject.activeInHierarchy);
    }

    /// <summary>
    /// The callback of clicking menu button.
    /// </summary>
    private void OnMenuClicked(GameObject go)
    {

    }

    /// <summary>
    /// The callback of clicking summon button.
    /// </summary>
    private void OnSummonClicked(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardWindow>(true);
    }

    /// <summary>
    /// The callback of clicking equip button.
    /// </summary>
    private void OnEquipClicked(GameObject go)
    {
        WindowManager.Instance.Show<ItemMenuBarWindow>(true);
    }

    /// <summary>
    /// The callback of clicking team button.
    /// </summary>
    private void OnTeamClicked(GameObject go)
    {
        if (HeroModelLocator.AlreadyRequest == false)
        {
            HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInHeroCreateTeam;
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show<UITeamShowingWindow>(true);
        }  
    }

    /// <summary>
    /// The callback of clicking hero button.
    /// </summary>
    private void OnHeroClicked(GameObject go)
    {
        WindowManager.Instance.Show<HeroMenuBarWindow>(true);
    }

    /// <summary>
    /// The callback of clicking home button.
    /// </summary>
    private void OnHomeClicked(GameObject go)
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
