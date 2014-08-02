using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using Template;
using Template.Auto.Item;
using UnityEngine;

/// <summary>
/// The window to show the item evolve operation.
/// </summary>
public class UIItemEvolveWindow : Window
{
    #region Private Fields

    private UIEventListener backBtnLis;
    private UIEventListener evolveBtnLis;
    private Transform leftItem;
    private Transform rightItem;
    private UIGrid evolveMats;
    private const int MaterialCount = 4;
    private short operItemIndex;
    private ItemEvoluteTemplate curEvoluteTmp;
    private ItemInfo itemInfo;
    private readonly List<int> curCounts = new List<int>();
    private readonly List<int> needsCounts = new List<int>();
    private EvolveState evState;
    private EvolveState EvState
    {
        get { return evState; }
        set
        {
            evState = value;
            evolveBtnLis.GetComponent<UISprite>().color = value == EvolveState.EvolveOk ? Color.white : Color.gray;
        }
    }

    #endregion

    #region Public Fields

    public Transform ItemPrefab;
    public Transform EvolveMatPrefab;

    public enum EvolveState
    {
        MoneyNotEnough,
        MatNotEnough,
        ItemNotFullLevel,
        EvolveOk,
        Invalid
    }

    #endregion 

    #region Window

    public override void OnEnter()
    {
        Init();
        InstallHandlers();
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        evolveBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Evolve").gameObject);
        leftItem = Utils.FindChild(transform, "LeftItem");
        rightItem = Utils.FindChild(transform, "RightItem");
        evolveMats = Utils.FindChild(transform, "EvolveMats").GetComponent<UIGrid>();
        FillItems();
    }

    private void InstallHandlers()
    {
        backBtnLis.onClick = OnBack;
        evolveBtnLis.onClick = OnEvolve;
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick = null;
        evolveBtnLis.onClick = null;
    }


    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UIItemEvolveWindow>(false);
        WindowManager.Instance.Show<UIItemInfoWindow>(true);
    }

    private void OnEvolve(GameObject go)
    {
        var msg = new CSEvoluteItem
                      {
                          OperItemIndex = operItemIndex
                      };
        NetManager.SendMessage(msg);
    }

    private void Refresh()
    {
        var orderType = ItemModeLocator.Instance.OrderType;
        var equipItem = leftItem.GetChild(0).GetComponent<EquipItem>();
        var info = ItemModeLocator.Instance.FindItem(operItemIndex);
        ItemHelper.ShowItem(orderType, equipItem, info);

        equipItem = rightItem.GetChild(0).GetComponent<EquipItem>();
        ItemHelper.ShowItem(orderType, equipItem, curEvoluteTmp.TargetItemId, itemInfo.Level);

        var item = evolveMats.GetChild(0);
        var desc = Utils.FindChild(item, "Desc").GetComponent<UILabel>();
        var ownCount = item.FindChild("OwnCount").GetComponent<UILabel>();

        var needMaterials = curEvoluteTmp.NeedMaterials;
        for (var i = 0; i < needMaterials.Count; i++)
        {
            var needMaterial = needMaterials[i];
            var count = FindMaterialCount(needMaterial.NeedMaterialId);
            ownCount.text = string.Format("{0}/{1}", count, needMaterial.NeedMaterialCount);
            ShowRarity(item);
            if (count > 0)
            {
                desc.text = ItemModeLocator.Instance.GetName(needMaterial.NeedMaterialId);
            } 
        }
        EvState = CanStartEvolve();
    }

    private int FindMaterialCount(int id)
    {
        return ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.Count(t => t.TmplId == id);
    }

    private void FillItems()
    {
        var item = PoolManager.Pools[HeroConstant.HeroPoolName].Spawn(ItemPrefab);
        Utils.MoveToParent(leftItem, item);
        NGUITools.SetActive(item.gameObject, true);

        item = PoolManager.Pools[HeroConstant.HeroPoolName].Spawn(ItemPrefab);
        Utils.MoveToParent(rightItem, item);
        NGUITools.SetActive(item.gameObject, true);

        for (int i = 0; i < MaterialCount; i++)
        {
            NGUITools.AddChild(evolveMats.gameObject, EvolveMatPrefab.gameObject);
        }
        evolveMats.repositionNow = true;
    }

    private void ShowRarity(Transform parent)
    {
        var stars = Utils.FindChild(parent, "Rarity");
        var quality = ItemModeLocator.Instance.GetQuality(itemInfo.TmplId);
        for (int index = 0; index < quality; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = quality; index < stars.transform.childCount; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
    }

    private EvolveState CanStartEvolve()
    {
        if(itemInfo.Level != itemInfo.MaxLvl)
        {
            return EvolveState.ItemNotFullLevel;
        }
        if(curCounts.Count != needsCounts.Count)
        {
            return EvolveState.Invalid;
        }
        if (curCounts.Where((t, i) => t < needsCounts[i]).Any())
        {
            return EvolveState.MatNotEnough;
        }
        if (PlayerModelLocator.Instance.Gold < curEvoluteTmp.CostGold)
        {
            return EvolveState.MoneyNotEnough;
        }
        return EvolveState.EvolveOk;
    }

    private void Init()
    {
        operItemIndex = ItemBaseInfoWindow.ItemDetail.BagIndex;
        itemInfo = ItemModeLocator.Instance.FindItem(operItemIndex);
        curEvoluteTmp = ItemModeLocator.Instance.ItemConfig.ItemEvoluteTmpls[itemInfo.TmplId];
        curCounts.Clear();
        needsCounts.Clear();
    }

    #endregion
}
