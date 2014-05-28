using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemLevelUpSelWindow : Window
{
    private List<ItemInfo> infos;
    private UILabel itemNums;
    private int capacity;
    private int cachedRow;
    private UItemsWindow cachedItemsWindow;
    private UIEventListener.VoidDelegate itemClickDelegate;
    private static bool isShuttingDown;

    #region Public Fields

    /// <summary>
    /// The prefab of the equip item.
    /// </summary>
    public GameObject ItemPrefab;

    #endregion

    #region Window

    public override void OnEnter()
    {
        cachedItemsWindow = WindowManager.Instance.GetWindow<UItemsWindow>();
        cachedRow = cachedItemsWindow.RowToShow;
        cachedItemsWindow.RowToShow = 2;
        itemClickDelegate = cachedItemsWindow.ItemClicked;
        cachedItemsWindow.ItemClicked = OnItemClicked;

        Refresh();
        InstallHandlers();
    }

    public override void OnExit()
    {
        if (!isShuttingDown)
        {
            cachedItemsWindow.RowToShow = cachedRow;
        }
        cachedItemsWindow.ItemClicked = itemClickDelegate;
        UnInstallHandlers();
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        itemNums = Utils.FindChild(transform, "EquipNums").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        
    }

    private void UnInstallHandlers()
    {
    
    }

    private void OnItemClicked(GameObject go)
    {
        
    }

    public void Refresh()
    {
        itemNums.text = string.Format("{0}/{1}", infos.Count, capacity);
    }

    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    #endregion
}
