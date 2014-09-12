using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class HeroSortControl : MonoBehaviour 
{
    public enum SortType
    {
        Hero,
        Item
    }
    public SortType Type = SortType.Hero;
    public UIEventListener SortBtnLis;
    public UILabel SortLabel;
    public UIToggle DescendToggle;
    private List<HeroInfo> infos; 
    private List<ItemInfo> itemInfos; 

    public delegate void SortOrderChanged();

    /// <summary>
    /// The call back of the sort order changed.
    /// </summary>
    public SortOrderChanged OnSortOrderChangedBefore;
    public SortOrderChanged OnSortOrderChangedAfter;
    public SortOrderChanged OnExcuteAfterSort;

    private bool isDescend;

    public void InstallHandlers()
    {
        SortBtnLis.onClick = OnSort;
        EventDelegate.Add(DescendToggle.onChange, SortTypeChanged);
    }

    public void UnInstallHandlers()
    {
        SortBtnLis.onClick = null;
        EventDelegate.Remove(DescendToggle.onChange, SortTypeChanged);
    }

    public void Init(List<HeroInfo> heroInfos)
    {
        infos = heroInfos;
        var orderType = HeroModelLocator.Instance.OrderType;
        SortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
    }

    public void Init(List<ItemInfo> hInfos)
    {
        itemInfos = hInfos;
        var orderType = ItemModeLocator.Instance.OrderType;
        SortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
    }

    private void OnSort(GameObject go)
    {
        PostSortBeforeEvents();
        ItemHelper.OrderType orderType;
        if (Type == SortType.Hero)
        {
            orderType = HeroModelLocator.Instance.OrderType;
            orderType = (ItemHelper.OrderType)(((int)orderType + 1) % ItemHelper.SortKeys.Count);
            HeroModelLocator.Instance.SortHeroList(orderType, infos, isDescend);
            HeroModelLocator.Instance.OrderType = orderType;
        }
        else
        {
            orderType = ItemModeLocator.Instance.OrderType;
            orderType = (ItemHelper.OrderType)(((int)orderType + 1) % ItemHelper.SortKeys.Count);
            ItemModeLocator.Instance.SortItemList(orderType, itemInfos, isDescend);
            ItemModeLocator.Instance.OrderType = orderType;
        }
        SortLabel.text = LanguageManager.Instance.GetTextValue(ItemHelper.SortKeys[(int)orderType]);
        PostSortAfterEvents();
    }

    private void SortTypeChanged()
    {
        isDescend = DescendToggle.value;
        PostSortBeforeEvents();
        if (Type == SortType.Hero)
        {
            var orderType = HeroModelLocator.Instance.OrderType;
            HeroModelLocator.Instance.SortHeroList(orderType, infos, isDescend);
        }
        else
        {
            var orderType = ItemModeLocator.Instance.OrderType;
            ItemModeLocator.Instance.SortItemList(orderType, itemInfos, isDescend);
        }

        PostSortAfterEvents();
    }

    private void PostSortBeforeEvents()
    {
        if(OnSortOrderChangedBefore != null)
        {
            OnSortOrderChangedBefore();
        }
    }

    private void PostSortAfterEvents()
    {
        if(OnExcuteAfterSort != null)
        {
            OnExcuteAfterSort();
        }
        if(OnSortOrderChangedAfter != null)
        {
            OnSortOrderChangedAfter();
        }
    }
}
