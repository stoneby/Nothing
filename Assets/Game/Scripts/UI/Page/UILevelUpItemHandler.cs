using System.Collections;
using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;
using System.Linq;

public class UILevelUpItemHandler : MonoBehaviour 
{
    public PropertyUpdater PropertyUpdater;
    public ItemBase ItemBase;
    public EffectSequnce LevelUpEffectSeq;
    private bool isReady;
    private UISprite maskBg; 
    private UIGrid matGrid;
    private UIEventListener levelUpLis;
    private UIEventListener cancelLis;
    private short preshowLvl;
    private int curExpAfter;
    private int expToFull;
    private UILabel fullLvlExpLabel;
    private UILabel canGetExpLabel;
    private UILabel coinsCostLabel;
    private UIItemCommonWindow commonWindow;
    private const int MaxLevelMatCount = 4;
    private readonly List<short> cachedBagIndexs = new List<short>();
    private readonly List<short> cachedMatBagIndexs = new List<short>();
    private readonly Dictionary<Position, GameObject> levelObjects = new Dictionary<Position, GameObject>();
    private List<Position> materialsPosList = new List<Position>(); 
    private Transform cannotLevelMask;
    private List<short> choiceIndexs = new List<short>();

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
            if (MainInfo == null)
            {
                return;
            }
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
            curExpAfter = exp;
            canGetExpLabel.text = value.ToString();
            UpdateProperty();
            CoinsCost = ItemModeLocator.Instance.GetLevelCost(MainInfo.CurExp, MainInfo.Level, preshowLvl, exp,
                                                               ItemModeLocator.Instance.GetQuality(MainInfo.TmplId));
        }
    }

    private void Refresh()
    {
        if(MainInfo == null)
        {
            return;
        }
        UpdateProperty();
        ItemBase.InitItem(MainInfo);
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
        if (MainInfo == null)
        {
            return 0;
        }
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
        MtaManager.TrackBeginPage(MtaType.LevelUpItemWindow);
        LevelUpEffectSeq.Stop();
        commonWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        commonWindow.NormalClicked = OnNormalClicked;
        var selected = MainInfo != null;
        NGUITools.SetActive(ItemBase.gameObject, selected);
        if (MainInfo == null)
        {
            return;
        }
        commonWindow.ShowSelMask(true);
        Refresh();
        ItemHelper.InstallLongPress(ItemBase.gameObject);
        GetMaterialPosList();
        RefreshCurScreen();
        InstallHandlers();
    }

    private void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.LevelUpItemWindow);
        UnInStallHandlers();
        PropertyUpdater.Reset();
        Clean();
    }

    private void GetMaterialPosList()
    {
        materialsPosList.Clear();
        var index = 0;
        foreach(var info in commonWindow.Infos)
        {
            var isMaterial = (ItemModeLocator.Instance.GetItemType(info.TmplId) == ItemHelper.EquipType.Material);
            if(isMaterial)
            {
                materialsPosList.Add(Utils.OneDimToTwo(index, commonWindow.CountOfOneGroup));
            }
            index++;
        }
    }

    private void UpdateProperty()
    {
        if(MainInfo == null)
        {
            return;
        }
        var level = MainInfo.Level;
        var tempId = MainInfo.TmplId;
        var atk = ItemModeLocator.Instance.GetAttack(tempId, level);
        var hp = ItemModeLocator.Instance.GetHp(tempId, level);
        var recover = ItemModeLocator.Instance.GetRecover(tempId, level);
        var mp = ItemModeLocator.Instance.GetMp(tempId);

        if (preshowLvl > level)
        {
            var changedLevel = (sbyte)(preshowLvl - level);
            var attackChanged = ItemModeLocator.Instance.GetAttackLvlParms(tempId, changedLevel);
            var recoverChanged = ItemModeLocator.Instance.GetRecoverLvlParms(tempId, changedLevel);
            var hpChanged = ItemModeLocator.Instance.GetHpLvlParms(tempId, changedLevel);
            PropertyUpdater.PreShowChangedProperty(changedLevel, attackChanged, hpChanged, recoverChanged, 0);
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
        var pos = UISellItemHandler.GetPosition(go);
        var bonus = (int)ItemModeLocator.Instance.ServerConfigMsg.SameTypeExpTimes;
        var containsKey = levelObjects.ContainsKey(pos);
        if(isReady)
        {
            if(pos == commonWindow.CurSelPos)
            {
                return;
            }

            if (!containsKey && levelObjects.Count >= MaxLevelMatCount)
            {
                return;
            }
            var flag = containsKey ? -1 : 1;
            var bagIndex = go.GetComponent<ItemBase>().BagIndex;
            var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
            if(itemInfo.EquipStatus == 1 || itemInfo.BindStatus == 1)
            {
                return;
            }
            var mainType = ItemModeLocator.Instance.GetItemType(MainInfo.TmplId);
            ExpCanGet += (flag * itemInfo.ContribExp * (ItemHelper.IsSameJobType(mainType, MainInfo.TmplId, bagIndex) ? bonus : 1));
            if (containsKey)
            {
                RemoveSellObject(pos);
                RefreshCurScreen();
            }
            else
            {
                var child = NGUITools.AddChild(matGrid.gameObject, commonWindow.BaseItemPrefab);
                var heroBase = child.GetComponent<ItemBase>();
                heroBase.InitItem(itemInfo);
                ItemHelper.InstallLongPress(child);
                levelObjects.Add(pos, child);     
            }
            var item = NGUITools.FindInParents<WrapItemContent>(go);
            item.ShowSellMask(pos.Y, !containsKey);
            matGrid.repositionNow = true;
        }
        else
        {
            if(containsKey)
            {
                RemoveSellObject(pos);
                var wrapItem = NGUITools.FindInParents<WrapItemContent>(go);
                wrapItem.ShowSellMask(wrapItem.Children.IndexOf(go.transform), false);
                var bagIndex = go.GetComponent<ItemBase>().BagIndex;
                var itemInfo = ItemModeLocator.Instance.FindItem(bagIndex);
                var mainType = ItemModeLocator.Instance.GetItemType(MainInfo.TmplId);
                ExpCanGet -= (itemInfo.ContribExp * (ItemHelper.IsSameJobType(mainType, MainInfo.TmplId, bagIndex) ? bonus : 1));
                matGrid.repositionNow = true;
            }
            else
            {
                RefreshChangeMain(go);
            }
        }
    }

    private void RefreshChangeMain(GameObject go)
    {
        commonWindow.CurSelPos = UISellItemHandler.GetPosition(go);
        ExpCanGet = 0;
        foreach(var levelObj in levelObjects)
        {
            var pos = levelObj.Key;
            var info = commonWindow.GetInfo(pos);
            var mainType = ItemModeLocator.Instance.GetItemType(MainInfo.TmplId);
            ExpCanGet += (info.ContribExp * (ItemHelper.IsSameJobType(mainType, MainInfo.TmplId, info.BagIndex) ? 5 : 1));
        }
        Refresh();
    }

    private void RemoveSellObject(Position pos)
    {
        var clone = levelObjects[pos];
        NGUITools.Destroy(clone);
        levelObjects.Remove(pos);
    }

    private void RefreshCurScreen()
    {
        if(MainInfo == null)
        {
            return;
        }
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
                    var isMainInfo = theItemInfo.BagIndex == MainInfo.BagIndex;
                    var pos = new Position {X = wrapItem.Row, Y = j};
                    
                    if(isReady)
                    {
                        wrapItem.ShowMask(j, isMainInfo);
                    }
                    else
                    {
                        wrapItem.ShowMask(j, materialsPosList.Contains(pos));
                    }
                    var showSellMask = levelObjects.ContainsKey(pos);
                    wrapItem.ShowSellMask(j, showSellMask);
                }
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
        commonWindow.SortControl.OnSortOrderChangedBefore += OnSortOrderChangedBefore;
        commonWindow.SortControl.OnSortOrderChangedAfter += OnSortOrderChangedAfter;
        levelUpLis.onClick = OnLevelUp;
        cancelLis.onClick = OnCancel;
    }

    private void UnInStallHandlers()
    {
        commonWindow.Items.OnUpdate -= OnUpdate;
        commonWindow.SortControl.OnSortOrderChangedBefore -= OnSortOrderChangedBefore;
        commonWindow.SortControl.OnSortOrderChangedAfter -= OnSortOrderChangedAfter;
        levelUpLis.onClick = null;
        cancelLis.onClick = null;
    }

    private void OnLevelUp(GameObject go)
    {
        choiceIndexs = levelObjects.Keys.Select(key => commonWindow.GetInfo(key).BagIndex).ToList();
        var containsGreatStar =
            choiceIndexs.Any(
                chiceIndex =>
                ItemHelper.GetStarCount(
                    ItemModeLocator.Instance.GetQuality(ItemModeLocator.Instance.FindItem(chiceIndex).TmplId)) >
                commonWindow.ConfirmStar);
        if(containsGreatStar)
        {
            var assert = WindowManager.Instance.GetWindow<AssertionWindow>();
            assert.AssertType = AssertionWindow.Type.OkCancel;
            assert.Message = "";
            assert.Title = string.Format(LanguageManager.Instance.GetTextValue(ItemType.LvlUpConfirmKey),
                                         commonWindow.EvolveAndLevelColor, UIItemCommonWindow.ColorEnd);
            assert.OkButtonClicked = OnEvolveOk;
            assert.CancelButtonClicked = OnEvolveCancel;
            WindowManager.Instance.Show(typeof(AssertionWindow), true);
            return;
        }
        SendLvlUpMessage();
    }

    private void OnEvolveCancel(GameObject sender)
    {
        CleanAssertWindow();
        WindowManager.Instance.Show<AssertionWindow>(false);
    }

    private void OnEvolveOk(GameObject sender)
    {
        CleanAssertWindow();
        SendLvlUpMessage();
    }

    private void SendLvlUpMessage()
    {
        var msg = new CSStrengthItem { OperItemIndex = MainInfo.BagIndex, ChoiceItemIndexes = choiceIndexs };
        NetManager.SendMessage(msg);
    }

    private void CleanAssertWindow()
    {
        var assert = WindowManager.Instance.GetWindow<AssertionWindow>();
        assert.OkButtonClicked = OnEvolveOk;
        assert.CancelButtonClicked = OnEvolveCancel;
        WindowManager.Instance.Show<AssertionWindow>(false);
    }

    private void OnCancel(GameObject go)
    {
        SetMask(false);
    }

    private void OnSortOrderChangedAfter()
    {
        var hinfos = commonWindow.Infos;
        var countPerGroup = commonWindow.CountOfOneGroup;
        var posList = UISellItemHandler.GetPositionsViaBagIndex(cachedBagIndexs, hinfos, countPerGroup);
        var values = levelObjects.Values.ToList();
        levelObjects.Clear();
        for (var i = 0; i < posList.Count; i++)
        {
            levelObjects.Add(posList[i], values[i]);
        }
        materialsPosList = UISellItemHandler.GetPositionsViaBagIndex(cachedMatBagIndexs, hinfos, countPerGroup);
        RefreshCurScreen();
    }

    private void OnSortOrderChangedBefore()
    {
        CacheBagIndexsFromPos(levelObjects.Keys.ToList(), cachedBagIndexs);
        CacheBagIndexsFromPos(materialsPosList, cachedMatBagIndexs);
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
            if(!isReady)
            {
                show = materialsPosList.Contains(pos);
                wrapItem.ShowMask(i, show);
            }
        }
    }

    public void ShowLevelOver()
    {
        LevelUpEffectSeq.Play();
        UpdateLevelUp(preshowLvl);
        choiceIndexs.Clear();
    }

    private void UpdateLevelUp(short level)
    {
        var cachedBagIndex = MainInfo.BagIndex;
        MainInfo.Level = level;
        MainInfo.CurExp = curExpAfter;
        commonWindow.Refresh();
        var index = commonWindow.Infos.FindIndex(info => info.BagIndex == cachedBagIndex);
        commonWindow.CurSelPos = Utils.OneDimToTwo(index, commonWindow.CountOfOneGroup);
        SetMask(false);
        CleanMasks();
        ExpCanGet = 0;
        Refresh();
    }

    /// <summary>
    /// Set color back to normal and destory mask game objects.
    /// </summary>
    private void CleanMasks(bool includedMaterials = false)
    {
        var items = commonWindow.Items.transform;
        for (var i = 0; i < items.childCount; i++)
        {
            var wrapItem = items.GetChild(i).GetComponent<WrapItemContent>();
            if (wrapItem)
            {
                wrapItem.ShowSellMasks(false);
                if (includedMaterials)
                {
                    wrapItem.ShowMasks(false);
                }
            }
        }
        var clones = levelObjects.Select(sellObj => sellObj.Value).ToList();
        for (var i = clones.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(clones[i]);
        }
        levelObjects.Clear();
        cachedBagIndexs.Clear();
    }

   private void Clean()
   {
       SetMask(false);
       CleanMasks(true);
       ExpCanGet = 0;
       CoinsCost = 0;
       cachedMatBagIndexs.Clear();
       choiceIndexs.Clear();
   }
}
