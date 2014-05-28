using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIEquipDispTabWindow : Window
{
    private UItemsWindow itemsWindow;
    private List<ItemInfo> infos;
    private readonly CSHeroSell csHeroSell = new CSHeroSell { SellList = new List<long>() };
    private const int MaxHeroCountCanSell = 10;
    private int capacity;
    private Transform viewObjectsTran;

    #region Public Fields

    [HideInInspector]
    public int ToggleIndex = 0;

    public List<UIToggle> Toggles;

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

    #region Mono

    // Use this for initialization
    void Awake()
    {
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();

        viewObjectsTran = Utils.FindChild(transform, "ViewObjects");
        capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
    }

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
            ToggleIndex = Toggles.FindIndex(toggle => toggle == UIToggle.current);
            switch (ToggleIndex)
            {
                case 0:
                    itemsWindow.RowToShow = 3;
                    itemsWindow.ItemClicked = OnItemInfoClicked;
                    UpdateViewOjects();
                    break;
                case 1:
                    itemsWindow.RowToShow = 2;
                    itemsWindow.ItemClicked = OnItemSellClicked;
                    UpdateSellObjects();
                    break;
                case 2:
                    itemsWindow.RowToShow = 3;
                    itemsWindow.ItemClicked = OnItemBindClicked;
                    UpdateBindObjects();
                    break;
            }
        }
    }

    private void OnItemBindClicked(GameObject go)
    {
        
    }

    private void OnItemSellClicked(GameObject go)
    {
        
    }

    private void UpdateBindObjects()
    {
        
    }

    private void UpdateSellObjects()
    {
        
    }

    private void UpdateViewOjects()
    {
        if (viewObjectsTran.gameObject.activeInHierarchy == false)
        {
            NGUITools.SetActive(viewObjectsTran.gameObject, true);
        }
        var equipNums = Utils.FindChild(viewObjectsTran, "EquipNums").GetComponent<UILabel>();
        equipNums.text = string.Format("{0}/{1}", infos.Count, capacity);
    }

    /// <summary>
    /// The callback of clicking each item.
    /// </summary>
    private void OnItemInfoClicked(GameObject go)
    {
        if(ToggleIndex == 0)
        {
            var bagIndex = go.GetComponent<EquipItemInfoPack>().BagIndex;
            var csmsg = new CSQueryItemDetail { BagIndex = bagIndex };
            NetManager.SendMessage(csmsg);
        }
        else if (ToggleIndex == 1)
        {
           
        }
        else if (ToggleIndex == 2)
        {
            
        }

    }

    #endregion
}
