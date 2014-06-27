using System.Collections.Generic;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIEquipDispTabWindow : Window
{
    #region Private Fields

    private UItemsWindow itemsWindow;

    #endregion

    #region Public Fields

    public List<UIToggle> Toggles;
    public ItemLockHandler ItemLockHandler;
    public ItemSellHandler ItemSellHandler;
    public ItemViewHandler ItemViewHandler;
    public enum ItemListTabName
    {
        ItemView = 0,
        ItemSell,
        ItemBind
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        itemsWindow = WindowManager.Instance.Show<UItemsWindow>(true);
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        if (WindowManager.Instance)
        {
            WindowManager.Instance.Show<UItemsWindow>(false);
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
            itemsWindow = itemsWindow ?? WindowManager.Instance.GetWindow<UItemsWindow>();
            var curTab = (ItemListTabName)Toggles.FindIndex(toggle => toggle == UIToggle.current);
            switch (curTab)
            {
                case ItemListTabName.ItemView:
                    itemsWindow.RowToShow = HeroConstant.ThreeRowsVisble;
                    itemsWindow.ItemClicked = ItemViewHandler.ItemInfoClicked;
                    break;
                case ItemListTabName.ItemSell:
                    itemsWindow.RowToShow = HeroConstant.TwoRowsVisible;
                    itemsWindow.ItemClicked = ItemSellHandler.ItemSellClicked;
                    break;
                case ItemListTabName.ItemBind:
                    itemsWindow.RowToShow = HeroConstant.ThreeRowsVisble;
                    itemsWindow.ItemClicked = ItemLockHandler.ItemLockClicked;
                    break;
            }
        }
    }

    #endregion
}
