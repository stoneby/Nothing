﻿using System.Collections;
using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;
using System.Linq;

public class UILevelUpItemHandler : MonoBehaviour 
{
    public PropertyUpdater PropertyUpdater;
    public ItemBase ItemBase;
    public GameObject LevelUpEffect;
    public GameObject BaseItemPrefab;
    private bool isReady;
    private UISprite maskBg; 
    private UIGrid matGrid;
    private UIEventListener levelUpLis;
    private UIEventListener cancelLis;
    private short preshowLvl;
    private int cachedContribExp;
    private int expToFull;
    private UILabel fullLvlExpLabel;
    private UILabel canGetExpLabel;
    private UILabel coinsCostLabel;
    private UIItemCommonWindow commonWindow;
    private List<short> bagIndexsCached;
    private const int MaxLevelMatCount = 4;
    private readonly List<short> cachedBagIndexs = new List<short>();
    private readonly Dictionary<Position, GameObject> levelObjects = new Dictionary<Position, GameObject>();
    private Transform cannotLevelMask;

    private ItemInfo MainInfo
    {
        get { return commonWindow.MainInfo; }
    }

    private int coinsCost;
    private int CoinsCost
    {
        get { return coinsCost; }
        set
        {
            coinsCost = value;
            coinsCostLabel.text = value.ToString();
        }
    }

    private int expCanGet;
    private int ExpCanGet
    {
        get { return expCanGet; }
        set
        {
            expCanGet = value;
            var exp = value + MainInfo.CurExp;
            var itemLvlTmpls = ItemModeLocator.Instance.ItemConfig.ItemLvlTmpls;
            var level = MainInfo.Level;
            var maxLevel = MainInfo.MaxLvl;
            while (exp >= itemLvlTmpls[level].MaxExp && level < maxLevel)
            {
                exp -= itemLvlTmpls[level].MaxExp;
                level++;
            }
            preshowLvl = level;
            cachedContribExp = exp;
            canGetExpLabel.text = value.ToString();
            UpdateProperty();
            CoinsCost += ItemModeLocator.Instance.GetLevelCost(MainInfo.CurExp, MainInfo.Level, preshowLvl, exp,
                                                               ItemModeLocator.Instance.GetQuality(MainInfo.TmplId));
        }
    }

    private void Refresh()
    {
        UpdateProperty();
        ItemBase.InitItem(MainInfo.TmplId);
        var isMaterial = ItemModeLocator.Instance.IsMaterial(MainInfo.TmplId);
        if (!isMaterial)
        {
            expToFull = ExpNeededToFull();
            fullLvlExpLabel.text = expToFull.ToString();
        }
        NGUITools.SetActive(cannotLevelMask.gameObject, isMaterial);
    }

    private int ExpNeededToFull()
    {
        var level = MainInfo.Level;
        var maxLevel = MainInfo.MaxLvl;
        var itemLvlTmpls = ItemModeLocator.Instance.ItemConfig.ItemLvlTmpls;
        var totalExp = (level == maxLevel) ?  0 : itemLvlTmpls[level].MaxExp - MainInfo.CurExp;
        for (var i = level + 1; i < maxLevel; i++)
        {
            totalExp += itemLvlTmpls[i].MaxExp;
        }
        return totalExp;
    }

    private void OnEnable()
    {
        commonWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        commonWindow.NormalClicked = OnNormalClicked;
        var selected = commonWindow.MainInfo != null;
        NGUITools.SetActive(ItemBase.gameObject, selected);
        if (commonWindow.MainInfo == null)
        {
            return;
        }
        Refresh();
        RefreshCurScreen();
        InstallHandlers();
    }

    private void OnDisable()
    {
        UnInStallHandlers();
        ResetMasks();
        PropertyUpdater.Reset();
    }

    private void UpdateProperty()
    {
        var level = MainInfo.Level;
        var tempId = MainInfo.TmplId;
        var atk = ItemModeLocator.Instance.GetAttack(tempId, level);
        var hp = ItemModeLocator.Instance.GetHp(tempId, level);
        var recover = ItemModeLocator.Instance.GetRecover(tempId, level);
        var mp = ItemModeLocator.Instance.GetMp(tempId);

        if (preshowLvl > level)
        {
            var changedLevel = preshowLvl - level;
            var attackChanged = ItemModeLocator.Instance.GetAttack(tempId, preshowLvl) -
                                ItemModeLocator.Instance.GetAttack(tempId, level);
            var recoverChanged = ItemModeLocator.Instance.GetRecover(tempId, preshowLvl) -
                                 ItemModeLocator.Instance.GetRecover(tempId, level);
            var hpChanged = ItemModeLocator.Instance.GetAttack(tempId, preshowLvl) -
                            ItemModeLocator.Instance.GetAttack(tempId, level);
            PropertyUpdater.PreShowChangedProperty(changedLevel, attackChanged, recoverChanged, hpChanged, 0);
        }
        else
        {
            PropertyUpdater.UpdateProperty(level, MainInfo.MaxLvl, atk, hp, recover, mp);
        }
    }

    private void Awake()
    {
        NGUITools.SetActive(PropertyUpdater.gameObject, true);
        NGUITools.SetActive(ItemBase.gameObject, true);
        maskBg = transform.Find("Sprite bg").GetComponent<UISprite>();
        maskBg.enabled = false;
        const string matBgPrefix = "Mats/Mat";
        for(var i = 0; i < MaxLevelMatCount; i++)
        {
            var child = transform.Find(matBgPrefix + i).gameObject;
            UIEventListener.Get(child).onClick = OnLevelMat;
        }
        matGrid = transform.Find("Mats/Grid").GetComponent<UIGrid>();
        levelUpLis = UIEventListener.Get(transform.Find("Buttons/Button-LvlUp").gameObject);
        cancelLis = UIEventListener.Get(transform.Find("Buttons/Button-Cancel").gameObject);
        canGetExpLabel = transform.Find("CanGetExp/CanGetExpValue").GetComponent<UILabel>();
        fullLvlExpLabel = transform.Find("FullExp/FullExpValue").GetComponent<UILabel>();
        coinsCostLabel = transform.Find("CostCoins/CostCoinsValue").GetComponent<UILabel>();
        cannotLevelMask = transform.Find("CanNotLevel");
    }

    private void OnLevelMat(GameObject go)
    {
        var tweener = go.GetComponent<TweenAlpha>();
        tweener.value = 1;
        tweener.enabled = false;
        SetMask();  
    }

    private void SetMask(bool showMask = true)
    {
        maskBg.enabled = showMask;
        isReady = showMask;
        RefreshCurScreen();
    }

    private void OnNormalClicked(GameObject go)
    {
        if(isReady)
        {
            var pos = UISellItemHandler.GetPosition(go);
            if(pos == commonWindow.CurSelPos)
            {
                return;
            }
            var containsKey = levelObjects.ContainsKey(pos);
            if (!containsKey && levelObjects.Count >= MaxLevelMatCount)
            {
                return;
            }
            var bagIndex = go.GetComponent<ItemBase>().BagIndex;
            var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
            var mainType = ItemModeLocator.Instance.GetItemType(MainInfo.TmplId);
            var flag = containsKey ? -1 : 1;
            ExpCanGet += (flag * itemInfo.ContribExp * (ItemHelper.IsSameJobType(mainType, MainInfo.TmplId, bagIndex) ? 5 : 1));
            if (containsKey)
            {
                RemoveSellObject(pos);
                RefreshCurScreen();
            }
            else
            {
                var child = NGUITools.AddChild(matGrid.gameObject, BaseItemPrefab);
                var heroBase = child.GetComponent<ItemBase>();
                heroBase.InitItem(itemInfo);   
                levelObjects.Add(pos, child);     
            }
            var item = NGUITools.FindInParents<WrapItemContent>(go);
            item.ShowSellMask(pos.Y, !containsKey);
            matGrid.repositionNow = true;
        }
        else
        {
            commonWindow.CurSelPos = UISellItemHandler.GetPosition(go);
            commonWindow.ShowSelMask(go.transform.position);
            Refresh();
        }
    }

    private void RemoveSellObject(Position pos)
    {
        var clone = levelObjects[pos];
        NGUITools.Destroy(clone);
        levelObjects.Remove(pos);
    }

    private void RefreshCurScreen()
    {
        var items = commonWindow.Items.transform;
        var childCount = items.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapItem = items.GetChild(i).GetComponent<WrapItemContent>();
            if (wrapItem != null)
            {
                for (var j = 0; j < wrapItem.Children.Count; j++)
                {
                    var theItemInfo = wrapItem.Children[j].GetComponent<ItemBase>().TheItemInfo;
                    var isMaterial = (ItemModeLocator.Instance.GetItemType(theItemInfo.TmplId) ==ItemHelper.EquipType.Material);
                    var isMainInfo = theItemInfo.BagIndex == MainInfo.BagIndex;
                    if(isReady)
                    {
                        wrapItem.ShowMask(j, isMainInfo);
                    }
                    else
                    {
                        wrapItem.ShowMask(j, isMaterial);
                    }

                    var pos = new Position { X = wrapItem.Row, Y = j };
                    var showSellMask = levelObjects.ContainsKey(pos);
                    wrapItem.ShowSellMask(j, showSellMask);
                }
            }
        }
    }

    private void ResetMasks()
    {
        var items = commonWindow.Items.transform;
        var childCount = items.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapItem = items.GetChild(i).GetComponent<WrapItemContent>();
            if (wrapItem != null)
            {
                wrapItem.ShowMasks(false);
            }
        }
    }

    private void CacheBagIndexsFromPos(IEnumerable<Position> list, List<short> bagIndexs)
    {
        bagIndexs.Clear();
        bagIndexs.AddRange(list.Select(pos => commonWindow.GetInfo(pos).BagIndex));
    }

    private void InstallHandlers()
    {
        commonWindow.Items.OnUpdate += OnUpdate;
        commonWindow.OnSortOrderChangedBefore += OnSortOrderChangedBefore;
        commonWindow.OnSortOrderChangedAfter += OnSortOrderChangedAfter;
        levelUpLis.onClick = OnLevelUp;
        cancelLis.onClick = OnCancel;
    }

    private void UnInStallHandlers()
    {
        commonWindow.Items.OnUpdate -= OnUpdate;
        commonWindow.OnSortOrderChangedBefore -= OnSortOrderChangedBefore;
        commonWindow.OnSortOrderChangedAfter -= OnSortOrderChangedAfter;
        levelUpLis.onClick = null;
        cancelLis.onClick = null;
    }

    private void OnLevelUp(GameObject go)
    {
        var choiceIndexs = levelObjects.Keys.Select(key => commonWindow.GetInfo(key).BagIndex).ToList();
        var msg = new CSStrengthItem {OperItemIndex = MainInfo.BagIndex, ChoiceItemIndexes = choiceIndexs};
        NetManager.SendMessage(msg);
    }

    private void OnCancel(GameObject go)
    {
        SetMask(false);
        CleanMasks();
        ExpCanGet = 0;
        CoinsCost = 0;
    }

    private void OnSortOrderChangedAfter(List<ItemInfo> hinfos)
    {
        var countPerGroup = commonWindow.CountOfOneGroup;
        var posList = UISellItemHandler.GetPositionsViaBagIndex(cachedBagIndexs, hinfos, countPerGroup);
        var values = levelObjects.Values.ToList();
        levelObjects.Clear();
        for (var i = 0; i < posList.Count; i++)
        {
            levelObjects.Add(posList[i], values[i]);
        }
        UpdateSellMasks();
        RefreshCurScreen();
    }

    private void UpdateSellMasks()
    {
        var items = commonWindow.Items.transform;
        var childCount = items.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapitem = items.GetChild(i).GetComponent<WrapItemContent>();
            if (wrapitem)
            {
                for (var j = 0; j < wrapitem.Children.Count; j++)
                {
                    var pos = new Position { X = wrapitem.Row, Y = j };
                    var showSellMask = levelObjects.ContainsKey(pos);
                    wrapitem.ShowSellMask(j, showSellMask);
                }
            }
        }
    }

    private void OnSortOrderChangedBefore(List<ItemInfo> hinfos)
    {
        CacheBagIndexsFromPos(levelObjects.Keys.ToList(), cachedBagIndexs);
    }

    private void OnUpdate(GameObject sender, int index)
    {
        var wrapItem = sender.GetComponent<WrapItemContent>();
        var col = wrapItem.Children.Count;
        for (var i = 0; i < col; i++)
        {
            var pos = new Position { X = index, Y = i };
            var show = levelObjects.Keys.Contains(pos);
            wrapItem.ShowSellMask(i, show);
        }
    }

    private IEnumerator PlayEffect(float time)
    {
        var pss = LevelUpEffect.GetComponents<ParticleSystem>();
        foreach (var system in pss)
        {
            system.Play();
        }
        yield return new WaitForSeconds(time);
        foreach (var system in pss)
        {
            system.Stop();
        }
    }

    public void ShowLevelOver()
    {
        StartCoroutine("PlayEffect", 1.5f);
        UpdateLevelUp(preshowLvl);
    }

    private void UpdateLevelUp(short level)
    {
        MainInfo.Level = level;
        MainInfo.ContribExp = cachedContribExp;
        commonWindow.Refresh();
        SetMask(false);
        CleanMasks();
        ExpCanGet = 0;
        Refresh();
    }

    /// <summary>
    /// Set color back to normal and destory mask game objects.
    /// </summary>
    private void CleanMasks()
    {
        var items = commonWindow.Items.transform;
        for (var i = 0; i < items.childCount; i++)
        {
            var wrapItem = items.GetChild(i).GetComponent<WrapItemContent>();
            if (wrapItem)
            {
                wrapItem.ShowSellMasks(false);
            }
        }
        var clones = levelObjects.Select(sellObj => sellObj.Value).ToList();
        for (var i = clones.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(clones[i]);
        }
        levelObjects.Clear();
    }
}
