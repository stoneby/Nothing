using System.Collections.Generic;
using KXSGCodec;
using Template.Auto.Item;
using UnityEngine;
using System.Linq;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIEvolveItemHandler : MonoBehaviour
{
    private readonly List<GameObject> mats = new List<GameObject>();
    private readonly List<UILabel> matsOwnCount = new List<UILabel>(); 
    private Transform evolveBg;
    private Transform targetBg;
    private UIGrid evolveMats;
    private UIEventListener evolveLis;
    private UIItemCommonWindow commonWindow;
    private Transform cannotEvolveMask;
    private UILabel evolveCost;
    private const int MaterialCount = 6;
    private const int MainItemIndex = 0;
    private const int TargetItemIndex = 1;
    private const int CountOfMainAndTarget = 2;

    /// <summary>
    /// The item which will show on the right.
    /// </summary>
    public GameObject BaseItemPrefab;

    #region Window

    private void OnEnable()
    {
        Init();
        commonWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        commonWindow.NormalClicked = OnNormalClicked;
        RefreshEvolve(commonWindow.GetInfo(commonWindow.CurSelPos));
        InstallHandlers();
    }

    private void OnDisable()
    {
        evolveLis.onClick = null;
        CleanUp();
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        evolveBg = transform.Find("PreShow/Evolve");
        targetBg = transform.Find("PreShow/Target");
        evolveMats = transform.Find("EvolveMats").GetComponent<UIGrid>();
        var matBgs = transform.Find("MatBgs");
        for (var i = 1; i <= matBgs.childCount; i++)
        {
            matsOwnCount.Add(matBgs.Find("Mat"+i+"/Label").GetComponent<UILabel>());
        }
        evolveLis = UIEventListener.Get(transform.Find("Button-Evolve").gameObject);
        cannotEvolveMask = transform.Find("CanNotEvolve");
        evolveCost = transform.Find("CostCoins/CostCoinsValue").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        evolveLis.onClick = OnEvolve;
        commonWindow.Items.OnUpdate += OnUpdate;
        commonWindow.OnSortOrderChangedAfter += OnSortAfter;
    }

    private void UnInstallHandlers()
    {
        evolveLis.onClick = null;
        commonWindow.Items.OnUpdate -= OnUpdate;
        commonWindow.OnSortOrderChangedAfter -= OnSortAfter;
    }

    private void OnSortAfter(List<ItemInfo> hinfos)
    {
        RefreshCurScreen();
    }

    private void OnUpdate(GameObject sender, int index)
    {
        CheckCanEvolve(sender.GetComponent<WrapItemContent>());
    }

    private void Init()
    {
        if(mats.Count == 0)
        {
            for (var i = 0; i < MaterialCount; i++)
            {
                mats.Add(null);
            }
        }
        else
        {
            for (var i = 0; i < MaterialCount; i++)
            {
                mats[i] = null;
            }
        }
    }

    private void OnEvolve(GameObject go)
    {
        var msg = new CSEvoluteItem
        {
            OperItemIndex = commonWindow.GetInfo(commonWindow.CurSelPos).BagIndex
        };
        NetManager.SendMessage(msg);
    }

    private void OnNormalClicked(GameObject go)
    {
        CleanUp();
        commonWindow.CurSelPos = UISellItemHandler.GetPosition(go);
        commonWindow.ShowSelMask(go.transform.position);
        RefreshEvolve(commonWindow.MainInfo);
    }

    private void RefreshEvolve(ItemInfo info)
    {
        if(info == null)
        {
            NGUITools.SetActive(cannotEvolveMask.gameObject, true);
            return;
        }
        var mat = NGUITools.AddChild(evolveBg.gameObject, BaseItemPrefab);
        mat.GetComponent<ItemBase>().InitItem(info);
        mats[MainItemIndex] = mat;
        var tempId = info.TmplId;
        ItemEvoluteTemplate evoluteTmp;
        ItemModeLocator.Instance.ItemConfig.ItemEvoluteTmpls.TryGetValue(tempId, out evoluteTmp);
        NGUITools.SetActive(cannotEvolveMask.gameObject, evoluteTmp == null);
        if (evoluteTmp != null)
        {
            mat = NGUITools.AddChild(targetBg.gameObject, BaseItemPrefab);
            mat.GetComponent<ItemBase>().InitItem(evoluteTmp.TargetItemId);
            mats[TargetItemIndex] = mat;
            for(var i = 0; i < evoluteTmp.NeedMaterials.Count; i++)
            {
                var evoluteParam = evoluteTmp.NeedMaterials[i];
                var count = FindMaterialCount(evoluteParam.NeedMaterialId);
                mat = NGUITools.AddChild(evolveMats.gameObject, BaseItemPrefab);
                mat.GetComponent<ItemBase>().InitItem(evoluteParam.NeedMaterialId);
                mats[CountOfMainAndTarget + i] = mat;
                matsOwnCount[i].text = string.Format("{0}/{1}", count, evoluteParam.NeedMaterialCount);
                evolveMats.repositionNow = true;
            }
            evolveCost.text = evoluteTmp.CostGold.ToString();
        }
    }

    private void RefreshCurScreen()
    {
        var items = commonWindow.Items.transform;
        var childCount = items.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var wrapItem = items.GetChild(i).GetComponent<WrapItemContent>();
            CheckCanEvolve(wrapItem);
        }
    }

    private void CheckCanEvolve(WrapItemContent wrapItem)
    {
        if(wrapItem != null)
        {
            for(var j = 0; j < wrapItem.Children.Count; j++)
            {
                var theItemInfo = wrapItem.Children[j].GetComponent<ItemBase>().TheItemInfo;
                wrapItem.ShowMask(j, ItemModeLocator.Instance.IsMaterial(theItemInfo.TmplId));
            }
        }
    }

    private void CleanUp()
    {
        for(var i = mats.Count - 1; i >= 0; i--)
        {
            var mat = mats[i];
            if(mat != null)
            {
                NGUITools.Destroy(mat);
                mats[i] = null;
            }
        }

        for (var i = 0; i < matsOwnCount.Count; i++)
        {
            matsOwnCount[i].text = "";
        }
    }

    private int FindMaterialCount(int id)
    {
        return ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.Count(t => t.TmplId == id);
    }

    public void ShowEvolveOver()
    {
        CleanUp();
    }

    #endregion
}
