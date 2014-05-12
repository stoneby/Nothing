using KXSGCodec;
using UnityEngine;
using System.Collections;

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

    private void InstallHandlers()
    {
        homeLis.onClick += OnHomeClicked;
        heroLis.onClick += OnHeroClicked;
        teamLis.onClick += OnTeamClicked;
        equipLis.onClick += OnEquipClicked;
        summonLis.onClick += OnSummonClicked;
        menuLis.onClick += OnMenuClicked;
        logLis.onClick += OnLogClicked;
    }

    private void UnInstallHandlers()
    {
        homeLis.onClick -= OnHomeClicked;
        heroLis.onClick -= OnHeroClicked;
        teamLis.onClick -= OnTeamClicked;
        equipLis.onClick -= OnEquipClicked;
        summonLis.onClick -= OnSummonClicked;
    }

    private void OnLogClicked(GameObject go)
    {
        //This is just for testing.
        inputLog.SetActive(!inputLog.gameObject.activeInHierarchy);
    }

    private void OnMenuClicked(GameObject go)
    {

    }

    private void OnSummonClicked(GameObject go)
    {
        
    }

    private void OnEquipClicked(GameObject go)
    {
       
    }

    private void OnTeamClicked(GameObject go)
    {
        
    }

    private void OnHeroClicked(GameObject go)
    {
        if (HeroModelLocator.Instance.SCHeroList == null || UIHerosDisplayWindow.IsCreateOne)
        {
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show(typeof(UIHerosDisplayWindow), true);
        }
    }

    private void OnHomeClicked(GameObject go)
    {
    }

    #endregion
}
