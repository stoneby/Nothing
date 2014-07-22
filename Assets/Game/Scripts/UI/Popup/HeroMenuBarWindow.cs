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
        barItemControl.ItemClicked = OnBarItemClicked;
    }

    public override void OnExit()
    {
        barItemControl.CleanUp();
    }

    private void OnHeroList()
    {
        WindowManager.Instance.Show<UIHeroCommonWindow>(true, true);
        switch (curBarItem)
        {
            case  BarItemType.HeroTeam:
                {
                    WindowManager.Instance.Show<UIBuildingTeamWindow>(true, true);
                    break;
                }
                case BarItemType.HeroLevelUp:
                {
                    WindowManager.Instance.Show<UILevelUpHeroWindow>(true, true);
                    break;
                }
                case BarItemType.HeroBreak:
                {
                    break;
                }
                case BarItemType.HeroSell:
                {
                    WindowManager.Instance.Show<UISellHeroWindow>(true, true);
                    break;
                }
        }
    }

    #endregion

    #region Private Methods
    
    private void Awake()
    {
        barItemControl = GetComponent<BarItemControl>();
        HeroHandler.HeroListInHeroPanel += OnHeroList;
    }

    private void OnBarItemClicked(GameObject go)
    {
        WindowManager.Instance.Show<HeroMenuBarWindow>(false);
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
            WindowManager.Instance.Show<UIHeroCommonWindow>(true, true);
            WindowManager.Instance.Show<UIBuildingTeamWindow>(true, true);
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
            WindowManager.Instance.Show<UIHeroCommonWindow>(true, true);
            WindowManager.Instance.Show<UILevelUpHeroWindow>(true, true);
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
            WindowManager.Instance.Show<UIHeroCommonWindow>(true, true);
            WindowManager.Instance.Show<UISellHeroWindow>(true, true);
        }  
    }

    #endregion
}
