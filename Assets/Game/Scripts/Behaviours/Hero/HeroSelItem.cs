using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class HeroSelItem : MonoBehaviour 
{
    private UIEventListener sortBtnLis;
    private UILabel sortLabel;
    private UILabel equipNum;
    private List<ItemInfo> canEquipItems;

    public GameObject ItemSnapShot;

    /// <summary>
    /// The prefab of the hero.
    /// </summary>
    public GameObject ItemPrefab;

    [HideInInspector]
    public UIGrid Items;

    public void Refresh()
    {
        canEquipItems = FilterItems();
        Refresh(canEquipItems);
    }

    private void Awake()
    {
        sortBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sort").gameObject);
        sortBtnLis.onClick += OnSortClicked;
        sortLabel = sortBtnLis.GetComponentInChildren<UILabel>();
        equipNum = Utils.FindChild(transform, "EquipNumValue").GetComponent<UILabel>();
        Items = GetComponentInChildren<UIGrid>();
    }

    private void OnSortClicked(GameObject go)
    {
        var orderType = ItemModeLocator.Instance.OrderType;
        orderType = (ItemHelper.OrderType)(((int)orderType + 1) % (StringTable.SortStrings.Count - 1));
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        ItemModeLocator.Instance.OrderType = orderType;
        Refresh(canEquipItems);
    }

    /// <summary>
    /// Filter all items to exclude material items.
    /// </summary>
    /// <returns>The item info list of all items after filtering.</returns>
    private List<ItemInfo> FilterItems()
    {
        if (ItemModeLocator.Instance.ScAllItemInfos.ItemInfos != null)
        {
            return
                ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.FindAll(
                    item => ItemModeLocator.Instance.GetItemType(item.TmplId) != ItemHelper.EquipType.Material);
        }
        return new List<ItemInfo>();
    }

    /// <summary>
    /// Fill in the equip item game objects.
    /// </summary> 
    private void UpdateItemList(int itemCounts)
    {
        var childCount = Items.transform.childCount;
        if (childCount != itemCounts)
        {
            var isAdd = childCount < itemCounts;
            HeroUtils.AddOrDelItems(Items.transform, ItemPrefab.transform, isAdd, Mathf.Abs(itemCounts - childCount),
                                HeroConstant.HeroPoolName,
                                OnNormalPress,
                                OnLongPress);
        }
        Items.repositionNow = true;
    }

    private void OnNormalPress(GameObject go)
    {
        var itemSnapShot = NGUITools.AddChild(gameObject, ItemSnapShot);
        var info = ItemModeLocator.Instance.FindItem(go.GetComponent<NewEquipItem>().BagIndex);
        itemSnapShot.GetComponent<ItemSnapShot>().Init(info);
    }

    private void OnLongPress(GameObject go)
    {
        ItemModeLocator.Instance.GetItemDetailPos = ItemType.GetItemDetailInHeroInfo;
        var bagIndex = go.GetComponent<NewEquipItem>().BagIndex;
        var csmsg = new CSQueryItemDetail { BagIndex = bagIndex };
        NetManager.SendMessage(csmsg);
    }

    public void Refresh(List<ItemInfo> newInfos)
    {
        var capacity = ItemModeLocator.Instance.ScAllItemInfos.Capacity;
        equipNum.text = string.Format("{0}/{1}", newInfos.Count, capacity);
        UpdateItemList(newInfos.Count);
        var orderType = ItemModeLocator.Instance.OrderType;
        sortLabel.text = StringTable.SortStrings[(int)orderType];
        ItemModeLocator.Instance.SortItemList(orderType, newInfos);
        for (var i = 0; i < newInfos.Count; i++)
        {
            var equipItem = Items.transform.GetChild(i).GetComponent<NewEquipItem>();
            equipItem.InitItem(newInfos[i]);
            ItemHelper.ShowItem(orderType, equipItem, newInfos[i]);
        }
    }
}
