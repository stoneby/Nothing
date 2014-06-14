﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KXSGCodec;

public class ItemLockHandler : MonoBehaviour
{
    private readonly List<short> lockList = new List<short>();
    public static List<short> ChangedLockList { get; private set; }
    private readonly List<short> LastLockList = new List<short>();
    private UILabel bindCount;
    private int cachedLockCount;
    private short lockCountLimit;
    private UItemsWindow itemsWindow;

    public void ItemLockClicked(GameObject go)
    {
        var equipItem = go.GetComponent<EquipItem>();
        var bagIndex = equipItem.BagIndex;
        if (!lockList.Contains(bagIndex))
        {
            if (cachedLockCount >= lockCountLimit)
            {
                return;
            }
            lockList.Add(bagIndex);
            equipItem.ShowLockMask(true);
            cachedLockCount ++;
        }
        else
        {
            lockList.Remove(bagIndex);
            equipItem.ShowLockMask(false);
            cachedLockCount --;
        }
        var capacity = ItemModeLocator.Instance.ServerConfigMsg.BindItemLimit;
        bindCount.text = string.Format("{0}/{1}", cachedLockCount, capacity);
    }

    private void OnDisable()
    {
        var changed = LockStateChanged();
        if (changed)
        {
            ChangedLockList = lockList.Union(LastLockList).Except(lockList.Intersect(LastLockList)).ToList();
            var msg = new CSItemLockOper { OperItemIndex = ChangedLockList };
            NetManager.SendMessage(msg);
        }
        for (int i = 0; i < lockList.Count; i++)
        {
            ShowLockMask(lockList[i], false);
        }
    }

    private void OnEnable()
    {
        itemsWindow = WindowManager.Instance.GetWindow<UItemsWindow>();
        InitLockData();
        var capacity = ItemModeLocator.Instance.ServerConfigMsg.BindItemLimit;
        bindCount.text = string.Format("{0}/{1}", cachedLockCount, capacity);
    }

    private void Awake()
    {
        bindCount = Utils.FindChild(transform, "BindNum").GetComponent<UILabel>();
        lockCountLimit = ItemModeLocator.Instance.ServerConfigMsg.BindItemLimit;
    }

    private void InitLockData()
    {
        lockList.Clear();
        LastLockList.Clear();
        var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
        cachedLockCount = 0;
        if (infos == null)
        {
            return;
        }
        for (int i = 0; i < infos.Count; i++)
        {
            var info = infos[i];
            if(info.BindStatus == 1)
            {
                cachedLockCount++;
                var bagIndex = info.BagIndex;
                LastLockList.Add(bagIndex);
                lockList.Add(bagIndex);
                ShowLockMask(bagIndex, true);
            }
        }
    }

    private void ShowLockMask(int bagIndex, bool show)
    {
        var items = itemsWindow.Items.transform;
        for (int i = 0; i < items.childCount; i++)
        {
            var item = items.GetChild(i).GetComponent<EquipItem>();
            if(item.BagIndex == bagIndex)
            {
                item.ShowLockMask(show);
                break;
            }
        }
    }

    private bool LockStateChanged()
    {
        if(lockList.Count != LastLockList.Count)
        {
            return true;
        }
        if(lockList.Count == 0)
        {
            return false;
        }
        var lockListTemp = new List<short>(lockList);
        lockListTemp.Sort();
        LastLockList.Sort();
        for (int i = 0; i < lockList.Count; i++)
        {
            if(lockListTemp[i] != LastLockList[i])
            {
                return true;
            }
        }
        return false;
    }
}
