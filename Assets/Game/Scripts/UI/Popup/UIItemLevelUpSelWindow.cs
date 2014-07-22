using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemLevelUpSelWindow : Window
{
    #region Private Fields

    private List<ItemInfo> infos;
    private UILabel itemNums;
    private int capacity;
    private int cachedRow;
    private UItemsWindow cachedItemsWindow;
    private UIEventListener.VoidDelegate itemClickDelegate;
    private static bool isShuttingDown;

    private UILabel costExpValue;
    private UILabel selCountValue;
    private UILabel getExpValue;
    private GameObject selMask;

    private UIEventListener cancelLis;
    private UIEventListener okLis;
    private UIEventListener backLis;
    private readonly List<short> choiceItemIndexes = new List<short>();
    private readonly List<GameObject> selMasks = new List<GameObject>();
    private ItemHelper.EquipType mainType;
    private ItemInfo mainItemInfo;
    private int expCanGet;

    #endregion

    #region Public Fileds

    public Vector3 MaskOffset = new Vector3(0, 13, 0);

    /// <summary>
    /// The depth of the items window's panel. 
    /// </summary>
    public int ItemsWindowDepth = 10;

    #endregion

    #region Window

    public override void OnEnter()
    {
        Init();
        Refresh();
        InstallHandlers();
    }

    public override void OnExit()
    {
        CleanUp();
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos ?? new List<ItemInfo>();
        capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        itemNums = Utils.FindChild(transform, "EquipNums").GetComponent<UILabel>();
        costExpValue = Utils.FindChild(transform, "CostExpValue").GetComponent<UILabel>();
        selCountValue = Utils.FindChild(transform, "SelCountValue").GetComponent<UILabel>();
        getExpValue = Utils.FindChild(transform, "GetExpValue").GetComponent<UILabel>();
        selMask = Utils.FindChild(transform, "SelMask").gameObject;
        selMask.SetActive(false);
        cancelLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Cancel").gameObject);
        okLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Ok").gameObject);
        backLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        mainItemInfo = ItemModeLocator.Instance.FindItem(ItemBaseInfoWindow.ItemDetail.BagIndex);
        mainType = ItemModeLocator.Instance.GetItemType(mainItemInfo.TmplId);
    }

    private void InstallHandlers()
    {
        cancelLis.onClick = OnCancel;
        okLis.onClick = OnOk;
        backLis.onClick = OnBack;
    }

    private void UnInstallHandlers()
    {
        cancelLis.onClick = null;
        okLis.onClick = null;
        backLis.onClick = null;
    }

    private void OnCancel(GameObject go)
    {
        for (int i = 0; i < selMasks.Count; i++)
        {
            selMasks[i].transform.parent = null;
            Destroy(selMasks[i]);
        }
        selMasks.Clear();
        choiceItemIndexes.Clear();
    }

    private void OnOk(GameObject go)
    {
        var indexs = new List<short>(choiceItemIndexes);
        var exp = expCanGet;
        var lvlUpWindow = WindowManager.Instance.Show<UIItemLevelUpWindow>(true);
        lvlUpWindow.UpdateSelect(indexs, exp);
        WindowManager.Instance.Show<ItemBaseInfoWindow>(true);
    }

    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UIItemLevelUpWindow>(true);
        WindowManager.Instance.Show<ItemBaseInfoWindow>(true);
    }

    private void OnItemClicked(GameObject go)
    {
        var bagIndex = go.GetComponent<EquipItem>().BagIndex;
        //如果点击的是主装备，不响应
        if (ItemBaseInfoWindow.ItemDetail.BagIndex == bagIndex)
        {
            return;
        }
        var info = ItemModeLocator.Instance.FindItem(bagIndex);
        //已装备或已绑定的话，不响应
        if(info.BindStatus == 1 || info.EquipStatus == 1)
        {
            return;
        }
        if(!choiceItemIndexes.Contains(bagIndex))
        {
            if (choiceItemIndexes.Count >= UIItemLevelUpWindow.MaxSelCount)
            {
                return;
            }
            choiceItemIndexes.Add(bagIndex);
            var child = NGUITools.AddChild(go, selMask);
            child.transform.localPosition = MaskOffset;
            child.SetActive(true);
            selMasks.Add(child);
            expCanGet += info.ContribExp * (ItemHelper.IsSameJobType(mainType, mainItemInfo.TmplId, bagIndex) ? 5 : 1);
        }
        else
        {
            choiceItemIndexes.Remove(bagIndex);
            var child = go.transform.FindChild("SelMask(Clone)");
            selMasks.Remove(child.gameObject);
            child.parent = null;
            Destroy(child.gameObject);
            expCanGet -= info.ContribExp * (ItemHelper.IsSameJobType(mainType, mainItemInfo.TmplId, bagIndex) ? 5 : 1);
        }
        selCountValue.text = string.Format("{0}/{1}", choiceItemIndexes.Count, UIItemLevelUpWindow.MaxSelCount);
        getExpValue.text = expCanGet.ToString(CultureInfo.InvariantCulture);
    }

    public void Refresh()
    {
        itemNums.text = string.Format("{0}/{1}", infos.Count, capacity);
        costExpValue.text = UIItemLevelUpWindow.ExpToFullLvl.ToString(CultureInfo.InvariantCulture);
        selCountValue.text = string.Format("{0}/{1}", choiceItemIndexes.Count, UIItemLevelUpWindow.MaxSelCount);
        getExpValue.text = expCanGet.ToString(CultureInfo.InvariantCulture);
    }

    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    private void Init()
    {
        cachedItemsWindow = WindowManager.Instance.Show<UItemsWindow>(true);
        cachedRow = cachedItemsWindow.RowToShow;
        cachedItemsWindow.RowToShow = HeroConstant.TwoRowsVisible;
        itemClickDelegate = cachedItemsWindow.ItemClicked;
        cachedItemsWindow.ItemClicked = OnItemClicked;
        cachedItemsWindow.Depth = ItemsWindowDepth;
    }

    private void CleanUp()
    {
        if (!isShuttingDown)
        {
            cachedItemsWindow.RowToShow = cachedRow;
        }
        cachedItemsWindow.ItemClicked = itemClickDelegate;
        cachedItemsWindow.ResetDepth();
        for(var i = 0; i < selMasks.Count; i++)
        {  
            selMasks[i].transform.parent = null;
            Destroy(selMasks[i]);
        }
        selMasks.Clear();
        expCanGet = 0;
        choiceItemIndexes.Clear();
        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.Show<UItemsWindow>(false);
        }
    }

    public void UpdateSelects(List<short> choiceIndexes, int getExp)
    {
        if (choiceIndexes == null || choiceIndexes.Count == 0)
        {
            return;
        }
        var itemsTrans = cachedItemsWindow.Items.transform;
        for (int i = 0; i < itemsTrans.childCount; i++)
        {
            var item = itemsTrans.GetChild(i);
            var bagIndex = item.GetComponent<EquipItem>().BagIndex; 
            if(choiceIndexes.Contains(bagIndex))
            {
                choiceItemIndexes.Add(bagIndex);
                var child = NGUITools.AddChild(item.gameObject, selMask);
                child.transform.localPosition = MaskOffset;
                child.SetActive(true);
                selMasks.Add(child);
            }
        }
        selCountValue.text = string.Format("{0}/{1}", choiceItemIndexes.Count, UIItemLevelUpWindow.MaxSelCount);
        expCanGet = getExp;
        getExpValue.text = expCanGet.ToString(CultureInfo.InvariantCulture);
    }

    #endregion
}
