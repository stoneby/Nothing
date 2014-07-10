using Assets.Game.Scripts.Net.handler;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class HeroMenuBarWindow : Window
{
    private BarItemControl barItemControl;

    private BarItemType curBarItem = BarItemType.Invalid;

    public enum BarItemType
    {
        HeroTeam,
        HeroLevelUp,
        HeroBreak,
        HeroSell,
        Invalid
    }

    #region Window

    public override void OnEnter()
    {
        barItemControl.Init();
        HeroHandler.HeroListInHeroPanel += OnHeroList;
    }

    public override void OnExit()
    {
        HeroHandler.HeroListInHeroPanel -= OnHeroList;
    }

    private void OnHeroList()
    {
        Utils.ShowWithoutDestory(typeof(UIHeroCommonWindow));
        switch (curBarItem)
        {
            case  BarItemType.HeroTeam:
                {
                    WindowManager.Instance.Show<UIBuildingTeamWindow>(true);
                    break;
                }
                case BarItemType.HeroLevelUp:
                {
                    WindowManager.Instance.Show<UILevelUpHeroWindow>(true);
                    break;
                }
                case BarItemType.HeroBreak:
                {
                    break;
                }
                case BarItemType.HeroSell:
                {
                    WindowManager.Instance.Show<UISellHeroWindow>(true);
                    break;
                }
        }
    }

    #endregion

    #region Private Methods
    
    private void Awake()
    {
        barItemControl = GetComponent<BarItemControl>();
    }

    #endregion

    #region Public Methods

    public void OnHeroTeam()
    {
        curBarItem = BarItemType.HeroTeam;
        if (HeroModelLocator.AlreadyRequest == false)
        {
            HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInHeroPanel;
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            Utils.ShowWithoutDestory(typeof(UIHeroCommonWindow));
            WindowManager.Instance.Show<UIBuildingTeamWindow>(true);
        }    
    }

    public void OnHeroLevelUp()
    {
        curBarItem = BarItemType.HeroLevelUp;
        if (HeroModelLocator.AlreadyRequest == false)
        {
            HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInHeroPanel;
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            Utils.ShowWithoutDestory(typeof(UIHeroCommonWindow));
            WindowManager.Instance.Show<UILevelUpHeroWindow>(true);
        }    
    }

    public void OnHeroBreak()
    {
        curBarItem = BarItemType.HeroBreak;
    }

    public void OnHeroSell()
    {
        curBarItem = BarItemType.HeroSell;
        if (HeroModelLocator.AlreadyRequest == false)
        {
            HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInHeroPanel;
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            Utils.ShowWithoutDestory(typeof(UIHeroCommonWindow));
            WindowManager.Instance.Show<UISellHeroWindow>(true);
        }  
    }

    #endregion
}
