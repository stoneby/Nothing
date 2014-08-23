using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class WrapItemContent : WrapItemBase
{
    public List<Transform> Children;

    private readonly List<Transform> sellMasks = new List<Transform>();

    public int Row;
    
    private void Awake()
    {
        foreach (var child in Children)
        {
            var sellMask = child.Find("SellMask");
            sellMasks.Add(sellMask);
            NGUITools.SetActive(sellMask.gameObject, false);
        }
    }

    public void ShowSellMask(int col, bool show)
    {
        if (col < 0 || col >= sellMasks.Count)
        {
            Logger.LogError("The column index is out of range.");
            return;
        }
        NGUITools.SetActive(sellMasks[col].gameObject, show);
    }

    public void ShowSellMasks(bool show)
    {
        foreach (var mask in sellMasks)
        {
            NGUITools.SetActive(mask.gameObject, show);
        }
    }

    public override void SetData(object data, int index)
    {
        Row = index;
        var itemDatas = data as List<ItemInfo>;
        for (var i = 0; i < itemDatas.Count; i++)
        {
            var child = Children[i];
            NGUITools.SetActive(child.gameObject, true);
            var item = child.GetComponentInChildren<NewEquipItem>();
            var info = itemDatas[i];
            item.InitItem(info);
            ItemHelper.ShowItem(ItemModeLocator.Instance.OrderType, item, info);
        }
        for (int i = itemDatas.Count; i < Children.Count; i++)
        {
            NGUITools.SetActive(Children[i].gameObject, false);
        }    
    }
}
