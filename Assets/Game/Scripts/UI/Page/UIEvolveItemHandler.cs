using System.Collections;
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
    private UISprite evolveSprite;
    private const int MaterialCount = 6;
    private const int MainItemIndex = 0;
    private const int TargetItemIndex = 1;
    private const int CountOfMainAndTarget = 2;
    private Transform arrow;
    private bool containsGreaterStarItem;
    public LevelUpEffectControl LevelUpEffectControl;
    public float ArrowDelay = 1;
    public float ArrowDouration = 1;
    public string EnableEvolveSprite = "SortN";
    public string DisableEvolveSprite = "BtnC";

    #region Window

    private void OnEnable()
    {
        Init();
        commonWindow = WindowManager.Instance.GetWindow<UIItemCommonWindow>();
        commonWindow.NormalClicked = OnNormalClicked;
        var selected = commonWindow.MainInfo != null;
        commonWindow.ShowSelMask(selected);
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
        evolveSprite = evolveLis.GetComponent<UISprite>();
        cannotEvolveMask = transform.Find("CanNotEvolve");
        evolveCost = transform.Find("CostCoins/CostCoinsValue").GetComponent<UILabel>();
        arrow = transform.Find("PreShow/Arrow");
    }

    private void InstallHandlers()
    {
        evolveLis.onClick = OnEvolve;
        commonWindow.Items.OnUpdate += OnUpdate;
        commonWindow.SortControl.OnSortOrderChangedAfter += OnSortAfter;
    }

    private void UnInstallHandlers()
    {
        evolveLis.onClick = null;
        commonWindow.Items.OnUpdate -= OnUpdate;
        commonWindow.SortControl.OnSortOrderChangedAfter -= OnSortAfter;
    }

    private void OnSortAfter()
    {
        RefreshCurScreen();
    }

    private void OnUpdate(GameObject sender, int index)
    {
        CheckCanEvolve(sender.GetComponent<WrapItemContent>());
    }

    private void Init()
    {
        LevelUpEffectControl.Stop();
        NGUITools.SetActive(arrow.gameObject, true);
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
        if(containsGreaterStarItem)
        {
            var assert = WindowManager.Instance.GetWindow<AssertionWindow>();
            assert.AssertType = AssertionWindow.Type.OkCancel;
            assert.Message = "";
            assert.Title = string.Format(LanguageManager.Instance.GetTextValue(ItemType.EvolveConfirmKey), commonWindow.EvolveAndLevelColor, UIItemCommonWindow.ColorEnd);
            assert.OkButtonClicked = OnEvolveOk;
            assert.CancelButtonClicked = OnEvolveCancel;
            WindowManager.Instance.Show(typeof(AssertionWindow), true);
            return;
        }
        SendEvolveMessage();
    }

    private void SendEvolveMessage()
    {
        var msg = new CSEvoluteItem
                      {
                          OperItemIndex = commonWindow.GetInfo(commonWindow.CurSelPos).BagIndex
                      };
        NetManager.SendMessage(msg);
    }

    private void OnEvolveCancel(GameObject sender)
    {
        CleanAssertWindow();
        WindowManager.Instance.Show<AssertionWindow>(false);
    }

    private void OnEvolveOk(GameObject sender)
    {
        CleanAssertWindow();
        SendEvolveMessage();
    }

    private void CleanAssertWindow()
    {
        var assert = WindowManager.Instance.GetWindow<AssertionWindow>();
        assert.OkButtonClicked = null;
        assert.CancelButtonClicked = null;
        WindowManager.Instance.Show<AssertionWindow>(false);
    }

    private void OnNormalClicked(GameObject go)
    {
        CleanUp();
        commonWindow.CurSelPos = UISellItemHandler.GetPosition(go);
        RefreshEvolve(commonWindow.MainInfo);
    }

    private void RefreshEvolve(ItemInfo info)
    {
        if(info == null)
        {
            NGUITools.SetActive(cannotEvolveMask.gameObject, true);
            return;
        }
        containsGreaterStarItem = false;
        var canEvolve = info.Level == info.MaxLvl;
        var baseItemPrefab = commonWindow.BaseItemPrefab;
        var mat = NGUITools.AddChild(evolveBg.gameObject, baseItemPrefab);
        mat.GetComponent<ItemBase>().InitItem(info);
        ItemHelper.InstallLongPress(mat);
        mats[MainItemIndex] = mat;
        var tempId = info.TmplId;
        ItemEvoluteTemplate evoluteTmp;
        ItemModeLocator.Instance.ItemConfig.ItemEvoluteTmpls.TryGetValue(tempId, out evoluteTmp);
        NGUITools.SetActive(cannotEvolveMask.gameObject, evoluteTmp == null);
        if (evoluteTmp != null)
        {
            mat = NGUITools.AddChild(targetBg.gameObject, baseItemPrefab);
            mat.GetComponent<ItemBase>().InitItem(evoluteTmp.TargetItemId);
            mats[TargetItemIndex] = mat;
            ItemHelper.InstallLongPress(mat, null, true);
            var contains = false;
            for (var i = 0; i < evoluteTmp.NeedMaterials.Count; i++)
            {
                var evoluteParam = evoluteTmp.NeedMaterials[i];
                int count, countOfLevelBelow;
                FindMaterialCount(evoluteParam.NeedMaterialId, commonWindow.ConfirmStar, out count, out countOfLevelBelow);
                mat = NGUITools.AddChild(evolveMats.gameObject, baseItemPrefab);
                mat.GetComponent<ItemBase>().InitItem(evoluteParam.NeedMaterialId);
                ItemHelper.InstallLongPress(mat, null, true);
                mats[CountOfMainAndTarget + i] = mat;
                matsOwnCount[i].text = string.Format("{0}/{1}", count, evoluteParam.NeedMaterialCount);
                if(!contains)
                {
                    contains = (countOfLevelBelow < evoluteParam.NeedMaterialCount);
                }
                if(canEvolve)
                {
                    canEvolve = count >= evoluteParam.NeedMaterialCount;
                }
            }
            evolveMats.repositionNow = true;
            evolveCost.text = evoluteTmp.CostGold.ToString();
            if (canEvolve)
            {
                canEvolve = PlayerModelLocator.Instance.Gold >= evoluteTmp.CostGold;
            }
            evolveSprite.spriteName = canEvolve ? EnableEvolveSprite : DisableEvolveSprite;
            containsGreaterStarItem = canEvolve && contains;
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

    private void FindMaterialCount(int id, int star, out int count, out int countOfStarBelow)
    {
        var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.Where(
                 t => (t != commonWindow.MainInfo && t.TmplId == id && t.EquipStatus == 0 && t.BindStatus == 0)).ToList();
        count =  infos.Count();
        countOfStarBelow = infos.Count(info => ItemHelper.GetStarCount(ItemModeLocator.Instance.GetQuality(info.TmplId)) <= star);
    }

    public void ShowEvolveOver(ItemInfo evolveInfo)
    {
        StartCoroutine("PlayEffect", evolveInfo);
    }

    private IEnumerator PlayEffect(object evolveInfo)
    {
        LevelUpEffectControl.Play();
        yield return new WaitForSeconds(ArrowDelay);
        NGUITools.SetActive(arrow.gameObject, false);
        yield return new WaitForSeconds(ArrowDouration);
        NGUITools.SetActive(arrow.gameObject, true);
        CleanUp();
        RefreshEvolve(evolveInfo as ItemInfo);
    }

    #endregion
}
