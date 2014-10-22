using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Template.Auto.Hero;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UISellHeroHandler : MonoBehaviour
{
    private UIHeroCommonWindow commonWindow;
    private UILabel selCount;
    private UILabel soulCount;
    private SCHeroList scHeroList;
    private List<long> teamMembers = new List<long>();
    private const int MaxSellCount = 12;
    private UIEventListener sellLis;
    private Hero hero;
    private long totalSoul;

    private readonly List<long> cachedSelUuids = new List<long>(); 
    private readonly List<long> cachedCanNotSellUuids = new List<long>();
    private long curSelUuidCache;

    private readonly Dictionary<Position, GameObject> sellObjects = new Dictionary<Position, GameObject>();
    private List<Position> cannotSellPosList = new List<Position>(); 

    private UIGrid grid;

    public AudioSource SellHeroSound;

    #region Private Methods

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.SellHeroWindow);
        commonWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
        commonWindow.ShowSelMask(false);
        commonWindow.NormalClicked = OnNormalClick;
        RefreshSelAndSoul();
        GetTeamMembers();
        CacheCannotSellList();
        UpdateSelAndCanNotSellMasks();
        InstallHandlers();
        HeroConstant.EnterType = HeroConstant.HeroDetailEnterType.SellHero;
        sellLis.GetComponent<UIButton>().isEnabled = false;
    }

    private void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.SellHeroWindow);
        UnInstallHandlers();
        CleanUp(true);
    }

    // Use this for initialization
    private void Awake()
    {
        selCount = Utils.FindChild(transform, "SelValue").GetComponent<UILabel>();
        soulCount = Utils.FindChild(transform, "SoulValue").GetComponent<UILabel>();
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        hero = HeroModelLocator.Instance.HeroTemplates;
        sellLis = UIEventListener.Get(transform.Find("Button-Sell").gameObject);
        grid = transform.Find("Container/Grid").GetComponent<UIGrid>();
    }

    private void InstallHandlers()
    {
        sellLis.onClick = OnSell;
        commonWindow.Heros.OnUpdate += OnUpdate;
        commonWindow.HeroSortControl.OnSortOrderChangedBefore += OnSortOrderChangedBefore;
        commonWindow.HeroSortControl.OnSortOrderChangedAfter += OnSortOrderChangedAfter;
    }

    private void UnInstallHandlers()
    {
        sellLis.onClick = null;
        commonWindow.Heros.OnUpdate -= OnUpdate;
        commonWindow.HeroSortControl.OnSortOrderChangedBefore -= OnSortOrderChangedBefore;
        commonWindow.HeroSortControl.OnSortOrderChangedAfter -= OnSortOrderChangedAfter;
    }

    private void OnSortOrderChangedAfter()
    {
        var hInfos = commonWindow.Infos;
        var countPerGroup = commonWindow.CountOfOneGroup;
        var posList = GetPositionsViaUuids(cachedSelUuids, hInfos, countPerGroup);
        var values = sellObjects.Values.ToList();
        sellObjects.Clear();
        for (var i = 0; i < posList.Count; i++)
        {
            sellObjects.Add(posList[i], values[i]);
        }
        cannotSellPosList = GetPositionsViaUuids(cachedCanNotSellUuids, hInfos, countPerGroup);
        UpdateSelAndCanNotSellMasks();
    }

    public static List<Position> GetPositionsViaUuids(IEnumerable<long> uuids, List<HeroInfo> hInfos, int countPerGroup)
    {
        return uuids.Select(uuid => hInfos.FindIndex(info => info.Uuid == uuid)).Select(
            index => Utils.OneDimToTwo(index, countPerGroup)).ToList();
    }

    private void OnSortOrderChangedBefore()
    {
        CacheUuidsFromPos(sellObjects.Keys.ToList(), cachedSelUuids);
        CacheUuidsFromPos(cannotSellPosList, cachedCanNotSellUuids);
    }

    private void CacheUuidsFromPos(IEnumerable<Position> list, List<long> uuids)
    {
        uuids.Clear();
        uuids.AddRange(list.Select(pos => commonWindow.GetInfo(pos).Uuid));
    }

    private void UpdateSelAndCanNotSellMasks()
    {
        var childCount = commonWindow.Heros.transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapHero = commonWindow.Heros.transform.GetChild(i).GetComponent<WrapHerosItem>();
            if(wrapHero != null)
            {
                for (var j = 0; j < wrapHero.Children.Count; j++)
                {
                    var pos = new Position { X = wrapHero.Row, Y = j };
                    var showSellMask = sellObjects.ContainsKey(pos);
                    wrapHero.ShowSellMask(j, showSellMask);
                    var showCanNotSellMask = cannotSellPosList.Contains(pos);
                    wrapHero.ShowCanNotSellMask(j, showCanNotSellMask);
                }
            }
        }
    }

    private void OnUpdate(GameObject sender, int index)
    {
        UpdateSelAndCanNotSellMasks();
    }

    private void OnSell(GameObject go)
    {
        curSelUuidCache = commonWindow.GetInfo(commonWindow.CurSelPos).Uuid;
        CacheUuidsFromPos(cannotSellPosList, cachedCanNotSellUuids);
        var containHighStars =
            sellObjects.Any(
                sellObj =>
                HeroModelLocator.Instance.GetHeroByTemplateId(commonWindow.GetInfo(sellObj.Key).TemplateId).Star >= 4);
        if(containHighStars)
        {
            var assert = WindowManager.Instance.GetWindow<AssertionWindow>();
            assert.Title = string.Format(LanguageManager.Instance.GetTextValue("UISellHandler.ConfirmLabel"),
                             "[ffff00]", "[-]");
            assert.Message = "";
            assert.AssertType = AssertionWindow.Type.OkCancel;
            assert.OkButtonClicked = OnSellConfirm;
            assert.CancelButtonClicked = OnSellCancel;
            WindowManager.Instance.Show<AssertionWindow>(true);
        }
        else
        {
            SendSellMessage();
        }
    }

    private void OnSellCancel(GameObject sender)
    {
        var assert = WindowManager.Instance.GetWindow<AssertionWindow>();
        assert.OkButtonClicked = null;
        assert.CancelButtonClicked = null;
        WindowManager.Instance.Show<AssertionWindow>(false);
    }

    private void OnSellConfirm(GameObject sender)
    {
        var assert = WindowManager.Instance.GetWindow<AssertionWindow>();
        assert.OkButtonClicked = null;
        SendSellMessage();
        WindowManager.Instance.Show<AssertionWindow>(false);
    }

    private void SendSellMessage()
    {
        var sellList = sellObjects.Select(item => commonWindow.GetInfo(item.Key).Uuid).ToList();
        if(sellList.Count > 0)
        {
            var csHeroSell = new CSHeroSell {SellList = sellList};
            NetManager.SendMessage(csHeroSell);
        }
    }

    private void OnNormalClick(GameObject go)
    {
        var pos = GetPosition(go);
        var isToSell = !sellObjects.ContainsKey(pos);
        if ((isToSell && sellObjects.Count >= MaxSellCount) || !CanSell(pos))
        {
            return;
        }
        if (sellObjects.Count == 0)
        {
            sellLis.GetComponent<UIButton>().isEnabled = true;
        }
        if(isToSell)
        {
            var child = NGUITools.AddChild(grid.gameObject, commonWindow.BaseHeroPrefab);
            var heroBase = child.GetComponent<HeroItemBase>();
            var heroInfo = go.GetComponent<HeroItemBase>().HeroInfo;
            heroBase.InitItem(heroInfo);
            var longPressHandler = child.GetComponent<HeroLongPressHandler>();
            if(longPressHandler)
            {
                longPressHandler.InstallLongPress();
            }
            sellObjects.Add(pos, child);
        }
        else
        {
            RemoveSellObject(pos);
        }
        var item = NGUITools.FindInParents<WrapHerosItem>(go);
        item.ShowSellMask(pos.Y, isToSell);
        RefreshSellList(commonWindow.GetInfo(pos).Uuid, isToSell);
        grid.repositionNow = true;
    }

    private void CacheCannotSellList()
    {
        cannotSellPosList.Clear();
        var infos = commonWindow.Infos;
        var count = infos.Count;
        var countPerGroup = commonWindow.CountOfOneGroup;
        for(var i = 0; i < count; i++)
        {
            var info = infos[i];
            if(teamMembers.Contains(info.Uuid)|| info.Bind)
            {
                cannotSellPosList.Add(Utils.OneDimToTwo(i, countPerGroup));
            }
        }
    }

    private void RemoveSellObject(Position pos)
    {
        var clone = sellObjects[pos];
        NGUITools.Destroy(clone);
        sellObjects.Remove(pos);
    }

    public static Position GetPosition(GameObject go)
    {
        var item = NGUITools.FindInParents<WrapHerosItem>(go);
        var row = item.Row;
        var col = item.Children.IndexOf(go.transform);
        return new Position{X = row, Y = col};
    }

     private void RefreshSellList(long uuid, bool isToSell)
     {
         var heroInfo = HeroModelLocator.Instance.FindHero(uuid);
         var baseSoul = hero.HeroTmpls[heroInfo.TemplateId].Price;
         var level = heroInfo.Lvl;
         var stars = hero.HeroTmpls[heroInfo.TemplateId].Star;
         var costSoul = GetCostSoul(stars, level);
         if (isToSell)
         {
             totalSoul += (long)(baseSoul + costSoul * hero.BaseTmpls[1].SellCoeff);
         }
         else
         {
             totalSoul -= (long)(baseSoul + costSoul * hero.BaseTmpls[1].SellCoeff);
         }
         RefreshSelAndSoul();
     }

    /// <summary>
    /// Refresh some ui items when we needed.
    /// </summary>
    private void RefreshSelAndSoul()
    {
        selCount.text = sellObjects.Count.ToString(CultureInfo.InvariantCulture);
        soulCount.text = totalSoul.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Get the cost soul of special star and level.
    /// </summary>
    /// <param name="stars">The star of the hero.</param>
    /// <param name="level">The level of the hero.</param>
    /// <returns></returns>
    private long GetCostSoul(sbyte stars, short level)
    {
        long costSoul = 0;
        for (int index = 1; index < level; index++)
        {
            costSoul += hero.LvlUpTmpls[index].CostSoul[stars - 1];
        }
        return costSoul;
    }


    /// <summary>
    /// Get the the team members.
    /// </summary>
    private void GetTeamMembers()
    {
        teamMembers.Clear();
        var count = scHeroList.TeamList.Count;
        for (int teamIndex = 0; teamIndex < count; teamIndex++)
        {
            for (int uUidIndex = 0; uUidIndex < scHeroList.TeamList[teamIndex].ListHeroUuid.Count; uUidIndex++)
            {
                if (scHeroList.TeamList[teamIndex].ListHeroUuid[uUidIndex] != HeroConstant.NoneInitHeroUuid)
                {
                    teamMembers.Add(scHeroList.TeamList[teamIndex].ListHeroUuid[uUidIndex]);
                }
            }
        }
        teamMembers = teamMembers.Distinct().ToList();
    }

    /// <summary>
    /// Check the hero with special uuid can be sold.
    /// </summary>
    /// <param name="pos">The uuid of the hero to be checked.</param>
    /// <returns>If true, the hero with the special uuid can be sold.</returns>
    private bool CanSell(Position pos)
    {
        return !cannotSellPosList.Contains(pos);
    }

    #endregion

    public void SellOverUpdate()
    {
        SellHeroSound.Play();
        UpdateCurSelPosition();
        cannotSellPosList = GetPositionsViaUuids(cachedCanNotSellUuids, commonWindow.Infos, commonWindow.CountOfOneGroup);
        UpdateSelAndCanNotSellMasks();
        CleanUp();
    }

    private void UpdateCurSelPosition()
    {
        if(sellObjects.ContainsKey(commonWindow.CurSelPos))
        {
            commonWindow.CurSelPos = HeroConstant.FirstPos;
        }
        else
        {
            var index = commonWindow.Infos.FindIndex(info => info.Uuid == curSelUuidCache);
            commonWindow.CurSelPos = Utils.OneDimToTwo(index, commonWindow.CountOfOneGroup);
        }
    }

    public void CleanUp(bool includeCanNotSellMask = false)
    { 
        CleanSellMasks(includeCanNotSellMask);
        totalSoul = 0;
        RefreshSelAndSoul();
        if (includeCanNotSellMask)
        {
            cannotSellPosList.Clear();
            cachedCanNotSellUuids.Clear();
        }
    }

    private void CleanSellMasks(bool includeCanNotSellMask = false)
    {
        var heros = commonWindow.Heros.transform;
        for (var i = 0; i < heros.childCount; i++)
        {
            var wrapHerosItem = heros.GetChild(i).GetComponent<WrapHerosItem>();
            if (wrapHerosItem != null)
            {
                wrapHerosItem.ShowSellMasks(false);
                if (includeCanNotSellMask)
                {
                    wrapHerosItem.ShowCanNotSellMasks(false);
                }
            }
        }
        var clones = sellObjects.Select(sellObj => sellObj.Value).ToList();
        for (var i = clones.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(clones[i]);
        }
        sellObjects.Clear();
        cachedSelUuids.Clear();
    }
}
