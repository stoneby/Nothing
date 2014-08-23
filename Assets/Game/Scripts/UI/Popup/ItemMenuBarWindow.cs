using Assets.Game.Scripts.Net.handler;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ItemMenuBarWindow : Window
{
    private BarItemControl barItemControl;

    private BarItemType curBarItem = BarItemType.Invalid;

    public enum BarItemType
    {
        ItemSell,
        ItemLevelUp,
        ItemEvolve,
        ItemBuyBack,
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

    private void OnItemList()
    {
        switch(curBarItem)
        {
            case BarItemType.ItemSell:
                {
                    WindowManager.Instance.Show<UIItemCommonWindow>(true, true);
                    //WindowManager.Instance.Show<UISellItemHandler>(true, true);
                    break;
                }
            case BarItemType.ItemLevelUp:
                {
                    WindowManager.Instance.Show<UIItemCommonWindow>(true, true);
                    WindowManager.Instance.Show<UILevelUpItemWindow>(true, true);
                    break;
                }
            case BarItemType.ItemEvolve:
                {
                    WindowManager.Instance.Show<UIItemCommonWindow>(true, true);
                    //WindowManager.Instance.Show<UIEvolveItemHandler>(true, true);
                    break;
                }
            case BarItemType.ItemBuyBack:
                {
                    WindowManager.Instance.Show<UIBuyBackItemsWindow>(true, true); 
                    break;
                }
        }
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        barItemControl = GetComponent<BarItemControl>();
        ItemHandler.ItemListInItemPanel += OnItemList;
    }

    private void OnBarItemClicked(GameObject go)
    {
        WindowManager.Instance.Show<ItemMenuBarWindow>(false);
    }

    #endregion

    #region Public Methods

    public void OnItemSell()
    {
        curBarItem = BarItemType.ItemSell;
        if (ItemModeLocator.AlreadyMainRequest == false)
        {
            ItemModeLocator.Instance.GetItemPos = ItemType.GetItemInPanel;
            var csmsg = new CSQueryAllItems { BagType = 0 };
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show<UIItemCommonWindow>(true, true);
            //WindowManager.Instance.Show<UISellItemHandler>(true, true);
        }
    }

    public void OnItemLevelUp()
    {
        curBarItem = BarItemType.ItemLevelUp;
        if (ItemModeLocator.AlreadyMainRequest == false)
        {
            ItemModeLocator.Instance.GetItemPos = ItemType.GetItemInPanel;
            var csmsg = new CSQueryAllItems { BagType = 0 };
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show<UIItemCommonWindow>(true, true);
            WindowManager.Instance.Show<UILevelUpItemWindow>(true, true);
        }
    }

    public void OnItemEvolve()
    {
        curBarItem = BarItemType.ItemEvolve;
        if (ItemModeLocator.AlreadyMainRequest == false)
        {
            ItemModeLocator.Instance.GetItemPos = ItemType.GetItemInPanel;
            var csmsg = new CSQueryAllItems { BagType = 0 };
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show<UIItemCommonWindow>(true, true);
            //WindowManager.Instance.Show<UIEvolveItemHandler>(true, true);
        }
    }

    public void OnItemBuyBack()
    {
        curBarItem = BarItemType.ItemBuyBack;
        if (ItemModeLocator.AlreadyBuyBackRequest == false)
        {
            ItemModeLocator.Instance.GetItemPos = ItemType.GetItemInPanel;
            var csmsg = new CSQueryAllItems { BagType = 1 };
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show<UIBuyBackItemsWindow>(true, true);
        }
    }

    #endregion
}
