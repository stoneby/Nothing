using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class HeroMenuBarWindow : Window
{
    private BarItemControl barItemControl;

    #region Window

    public override void OnEnter()
    {
        barItemControl.Init();
    }

    public override void OnExit()
    {
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
        if (HeroModelLocator.AlreadyRequest == false)
        {
            HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInHeroPanel;
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            Utils.ShowWithoutDestory(typeof(UIHeroCommonWindow));
        }
    }

    public void OnHeroLevelUp()
    {
    }

    public void OnHeroBreak()
    {
    }

    public void OnHeroSell()
    {
    }

    #endregion
}
