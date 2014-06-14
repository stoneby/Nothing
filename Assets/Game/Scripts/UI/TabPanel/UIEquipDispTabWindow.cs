using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIEquipDispTabWindow : Window
{
    private UItemsWindow itemsWindow;

    #region Public Fields

    public List<UIToggle> Toggles;
    public ItemLockHandler ItemLockHandler;
    public ItemSellHandler ItemSellHandler;
    public ItemViewHandler ItemViewHandler;

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
        for (int i = 0; i < Toggles.Count; i++)
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
            var toggleIndex = Toggles.FindIndex(toggle => toggle == UIToggle.current);
            switch (toggleIndex)
            {
                case 0:
                    itemsWindow.RowToShow = 3;
                    itemsWindow.ItemClicked = ItemViewHandler.ItemInfoClicked;
                    break;
                case 1:
                    itemsWindow.RowToShow = 2;
                    itemsWindow.ItemClicked = ItemSellHandler.ItemSellClicked;
                    break;
                case 2:
                    itemsWindow.RowToShow = 3;
                    itemsWindow.ItemClicked = ItemLockHandler.ItemLockClicked;
                    break;
            }
        }
    }

    #endregion
}
