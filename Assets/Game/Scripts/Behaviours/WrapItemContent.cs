using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class WrapItemContent : WrapItemBase
{
    public List<Transform> Children;

    private readonly List<Transform> sellMasks = new List<Transform>();
    private readonly List<Transform> masks = new List<Transform>();
    private readonly List<Transform> equipMasks = new List<Transform>();

    public int Row;
    
    private void Awake()
    {
        foreach (var child in Children)
        {
            var sellMask = child.Find("SellMask");
            sellMasks.Add(sellMask);
            var mask = child.Find("Mask");
            masks.Add(mask);     
            var equipMask = child.Find("EquipMask");
            equipMasks.Add(equipMask);
            NGUITools.SetActive(equipMask.gameObject, false);
            NGUITools.SetActive(sellMask.gameObject, false);
            NGUITools.SetActive(mask.gameObject, false);
        }
    }

    public GameObject ShowSellMask(int col, bool show)
    {
        if (col < 0 || col >= sellMasks.Count)
        {
            Logger.LogError("The column index is out of range.");
            return null;
        }
        NGUITools.SetActive(sellMasks[col].gameObject, show);
        return sellMasks[col].gameObject;
    }

    public void ShowSellMasks(bool show)
    {
        foreach (var mask in sellMasks)
        {
            NGUITools.SetActive(mask.gameObject, show);
        }
    } 
    
    public GameObject ShowMask(int col, bool show)
    {
        if (col < 0 || col >= masks.Count)
        {
            Logger.LogError("The column index is out of range.");
            return null;
        }
        masks[col].parent.GetComponent<BoxCollider>().enabled = !show;
        NGUITools.SetActive(masks[col].gameObject, show);
        return sellMasks[col].gameObject;
    }

    public void ShowMasks(bool show)
    {
        foreach (var mask in masks)
        {
            mask.parent.GetComponent<BoxCollider>().enabled = !show;
            NGUITools.SetActive(mask.gameObject, show);
        }
    }  
    
    public GameObject ShowEquipMask(int col, bool show)
    {
        if (col < 0 || col >= equipMasks.Count)
        {
            Logger.LogError("The column index is out of range.");
            return null;
        }
        NGUITools.SetActive(equipMasks[col].gameObject, show);
        return equipMasks[col].gameObject;
    }

    public void ShowEquipMasks(bool show)
    {
        foreach (var mask in equipMasks)
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
