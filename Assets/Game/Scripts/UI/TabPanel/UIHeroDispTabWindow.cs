using System.Collections.Generic;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroDispTabWindow : Window
{
    #region Private Fields

    private UIHerosPageWindow herosWindow;

    #endregion

    #region Public Fields

    public List<UIToggle> Toggles;
    public HeroLockHandler HeroLockHandler;
    public HeroSellHandler HeroSellHandler;
    public HeroViewHandler HeroViewHandler;

    public enum HeroListTabName
    {
        HeroView = 0,
        HeroSell,
        HeroBind
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        herosWindow = WindowManager.Instance.Show<UIHerosPageWindow>(true);
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        if (WindowManager.Instance)
        {
            WindowManager.Instance.Show<UIHerosPageWindow>(false);
        }
    }

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        for (var i = 0; i < Toggles.Count; i++)
        {
            EventDelegate.Add(Toggles[i].onChange, OnToggleChanged);
        }
    }

    private void UnInstallHandlers()
    {
        for (int i = 0; i < Toggles.Count; i++)
        {
            EventDelegate.Remove(Toggles[i].onChange, OnToggleChanged);
        }
    }

    private void OnToggleChanged()
    {
        bool val = UIToggle.current.value;
        if (val)
        {
            herosWindow = herosWindow ?? WindowManager.Instance.GetWindow<UIHerosPageWindow>();
            var curTab = (HeroListTabName)Toggles.FindIndex(toggle => toggle == UIToggle.current);
            switch (curTab)
            {
                case HeroListTabName.HeroView:
                    herosWindow.RowToShow = HeroConstant.ThreeRowsVisble;
                    herosWindow.ItemClicked = HeroViewHandler.OnHeroItemClicked;
                    break;
                case HeroListTabName.HeroSell:
                    herosWindow.RowToShow = HeroConstant.TwoRowsVisible;
                    herosWindow.ItemClicked = HeroSellHandler.HeroSellClicked;
                    break;
                case HeroListTabName.HeroBind:
                    herosWindow.RowToShow = HeroConstant.ThreeRowsVisble;
                    herosWindow.ItemClicked = HeroLockHandler.OnHeroItemClicked;
                    break;
            }
        }
    }

    #endregion
}
