using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemLevelUpWindow : Window
{
    #region Private Fields

    private UIEventListener backBtnLis;
    private UIEventListener cancelBtnLis;
    private UIEventListener lvlUpBtnLis;
    private UIEventListener oneKeyAddLis;
    private readonly List<Transform> lvlUpSelBtns = new List<Transform>();
    private ItemInfo itemInfo;
    private UILabel costExpValue;
    private UILabel selCountValue;
    private List<short> choiceItemIndexes;
    public const int MaxSelCount = 6;
    private readonly List<Transform> itemsToUp = new List<Transform>();

    private short cacheLevel;
    private int cachedCurExp;
    private ItemBaseInfoWindow cachedBaseWindow;
    private bool isPreShow;
    private ItemInfo mainItemInfo;
    private ItemHelper.EquipType mainType;
    private int getExp;

    #endregion

    #region Public Fields

    public int GetExp
    {
        get { return getExp;}
        set
        {
            if(value != getExp)
            {
                getExp = value;
                var exp = getExp + itemInfo.CurExp;
                var itemLvlTmpls = ItemModeLocator.Instance.ItemConfig.ItemLvlTmpls;
                var lvl = itemInfo.Level;
                while (exp >= itemLvlTmpls[lvl].MaxExp)
                {
                    exp -= itemLvlTmpls[lvl].MaxExp;
                    lvl++;
                }
                cachedCurExp = exp;
                cacheLevel = lvl;
                if (lvl > itemInfo.Level && isPreShow == false)
                {
                    cachedBaseWindow.gameObject.SetActive(true);
                    cachedBaseWindow.GetComponent<ItemBaseInfoWindow>().StartCoroutine("PreShowLevelUp", lvl);
                    isPreShow = true;
                }
                if(isPreShow && lvl == itemInfo.Level)
                {
                    cachedBaseWindow.gameObject.SetActive(true);
                    cachedBaseWindow.GetComponent<ItemBaseInfoWindow>().StopCoroutine("PreShowLevelUp");
                    cachedBaseWindow.RefreshChangedData();
                    isPreShow = false;
                }
            }
        }
    }

    public static int ExpToFullLvl { get; private set; }
    public GameObject LvlUpItem;
    public GameObject ItemUpConfirm;

    #endregion

    #region Window

    public override void OnEnter()
    {
        itemInfo = ItemModeLocator.Instance.FindItem(ItemBaseInfoWindow.ItemDetail.BagIndex);
        cachedBaseWindow = WindowManager.Instance.GetWindow<ItemBaseInfoWindow>();
        InstallHandlers();
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        CleanUp();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        cancelBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-CancelAll").gameObject);
        lvlUpBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-LevelUp").gameObject);
        oneKeyAddLis = UIEventListener.Get(Utils.FindChild(transform, "Button-OneKeyAdd").gameObject);
        costExpValue = Utils.FindChild(transform, "CostExpValue").GetComponent<UILabel>();
        selCountValue = Utils.FindChild(transform, "SelCountValue").GetComponent<UILabel>();
        var levelUpSelBtns = Utils.FindChild(transform, "LevelUpSelBtns");
        for (int i = 1; i <= MaxSelCount; i++)
        {
            itemsToUp.Add(Utils.FindChild(transform, "Btn" + i));
        }
        for (int i = 0; i < levelUpSelBtns.childCount; i++)
        {
            lvlUpSelBtns.Add(levelUpSelBtns.GetChild(i));  
        }
        mainItemInfo = ItemModeLocator.Instance.FindItem(ItemBaseInfoWindow.ItemDetail.BagIndex);
        mainType = ItemModeLocator.Instance.GetItemType(mainItemInfo.TmplId);
    }

    private void InstallHandlers()
    {
        backBtnLis.onClick = OnBack;
        cancelBtnLis.onClick = OnCancel;
        lvlUpBtnLis.onClick = OnLevelUp;
        oneKeyAddLis.onClick = OnOneKeyAdd;
        for (int i = 0; i < lvlUpSelBtns.Count; i++)
        {
            UIEventListener.Get(lvlUpSelBtns[i].gameObject).onClick = OnLevelUpSel;
        }
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick = null;
        cancelBtnLis.onClick = null;
        lvlUpBtnLis.onClick = null;
        oneKeyAddLis.onClick = null;
        for (int i = 0; i < lvlUpSelBtns.Count; i++)
        {
            UIEventListener.Get(lvlUpSelBtns[i].gameObject).onClick = null;
        }
    }

    private void OnOneKeyAdd(GameObject go)
    {
        var infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos.FindAll(
            info => (info.BagIndex != mainItemInfo.BagIndex && itemInfo.BindStatus == 0 &&
                     itemInfo.EquipStatus == 0 &&
                     (info.Level < 10 && ItemModeLocator.Instance.GetQuality(info.TmplId) > 3 &&
                      ItemModeLocator.Instance.GetQuality(info.TmplId) <= 6 ||
                      ItemModeLocator.Instance.GetQuality(info.TmplId) >= 1 &&
                      ItemModeLocator.Instance.GetQuality(info.TmplId) <= 3)));
        if(infos.Count < MaxSelCount)
        {
            UpdateSelect(infos);
        }
        else
        {
            var equipTemplate = ItemModeLocator.Instance.ItemTemplates.EquipTmpls;
            var materialTempl = ItemModeLocator.Instance.ItemTemplates.MaterialTmpls;
            var materials =
           infos.FindAll(
               info =>
               ItemModeLocator.Instance.GetItemType(info.TmplId) == ItemHelper.EquipType.Material);
            List<ItemInfo> equips = infos.FindAll(
                    info => ItemModeLocator.Instance.GetItemType(info.TmplId) == ItemHelper.EquipType.Equip);
            var armors = infos.FindAll(
                info => ItemModeLocator.Instance.GetItemType(info.TmplId) == ItemHelper.EquipType.Armor);
            var selInfos = new List<ItemInfo>();
            if (mainType == ItemHelper.EquipType.Equip)
            {
                var sameMaterials =
                    materials.FindAll(
                        info =>
                        materialTempl[info.TmplId].FitType == 1 &&
                        equipTemplate[mainItemInfo.TmplId].JobType == materialTempl[info.TmplId].FitJobType);
                if (sameMaterials.Count >= MaxSelCount)
                {
                    selInfos.AddRange(sameMaterials.GetRange(0, MaxSelCount));
                }
                else
                {
                    var sameEquips = equips.FindAll(
                        info =>
                        equipTemplate[mainItemInfo.TmplId].JobType == equipTemplate[info.TmplId].JobType);
                    if (sameMaterials.Count + sameEquips.Count >= MaxSelCount)
                    {
                        selInfos.AddRange(sameMaterials);
                        selInfos.AddRange(sameEquips.GetRange(0, MaxSelCount - sameMaterials.Count));
      
                    }
                    else
                    {
                        if (sameMaterials.Count + equips.Count >= MaxSelCount)
                        {
                            selInfos.AddRange(sameMaterials);
                            selInfos.AddRange(sameEquips);
                            selInfos.AddRange(equips.Except(sameEquips).ToList().GetRange(0,
                                                                                          MaxSelCount -
                                                                                          sameMaterials.Count -
                                                                                          sameEquips.Count));

                        }
                        else
                        {
                            if (sameMaterials.Count + equips.Count + armors.Count >= MaxSelCount)
                            {
                                selInfos.AddRange(sameMaterials);
                                selInfos.AddRange(equips);
                                selInfos.AddRange(armors.GetRange(0, MaxSelCount - sameMaterials.Count - equips.Count));
                            }
                            else
                            {
                                selInfos.AddRange(sameMaterials);
                                selInfos.AddRange(equips);
                                selInfos.AddRange(armors);
                                selInfos.AddRange(materials.Except(sameMaterials).ToList().GetRange(0, MaxSelCount - sameMaterials.Count - equips.Count - armors.Count));
                            }
                        }
                    }
                }
            }
            else
            {
                var sameMaterials = materials.FindAll(info => materialTempl[info.TmplId].FitType == 2);
                if (sameMaterials.Count >= MaxSelCount)
                {
                    selInfos.AddRange(sameMaterials.GetRange(0, MaxSelCount));
                }
                else
                {
                    if (materials.Count + armors.Count >= MaxSelCount)
                    {
                        selInfos.AddRange(sameMaterials);
                        selInfos.AddRange(armors.GetRange(0, MaxSelCount - sameMaterials.Count));
                    }
                    else
                    {
                        if (materials.Count + armors.Count + equips.Count >= MaxSelCount)
                        {
                            selInfos.AddRange(sameMaterials);
                            selInfos.AddRange(armors);
                            selInfos.AddRange(equips.GetRange(0, MaxSelCount - sameMaterials.Count - armors.Count));
                        }
                        else
                        {
                            selInfos.AddRange(sameMaterials);
                            selInfos.AddRange(equips);
                            selInfos.AddRange(armors);
                            selInfos.AddRange(materials.Except(sameMaterials).ToList().GetRange(0, MaxSelCount - sameMaterials.Count - equips.Count - armors.Count));
                        }
                    }
                }
            }
            UpdateSelect(selInfos);
        }
    }

    private void OnLevelUp(GameObject go)
    {
        var itemUpConfirm = NGUITools.AddChild(transform.gameObject, ItemUpConfirm);
        var selCount = Utils.FindChild(itemUpConfirm.transform, "SelCountValue").GetComponent<UILabel>();
        selCount.text = selCountValue.text;
        var expLabel = Utils.FindChild(itemUpConfirm.transform, "GetExpValue").GetComponent<UILabel>();
        expLabel.text = GetExp.ToString(CultureInfo.InvariantCulture);
        var cancel = UIEventListener.Get(Utils.FindChild(transform, "Cancel").gameObject);
        var ok = UIEventListener.Get(Utils.FindChild(transform, "OK").gameObject);
        cancel.onClick += OnConfirmCancel;
        ok.onClick += OnConfirmOk;
    }

    private void OnConfirmOk(GameObject go)
    {
        var msg = new CSStrengthItem
                      {
                          OperItemIndex = ItemBaseInfoWindow.ItemDetail.BagIndex,
                          ChoiceItemIndexes = choiceItemIndexes
                      };
        NetManager.SendMessage(msg);
        Destroy(go.transform.parent.gameObject);
    }

    private void OnConfirmCancel(GameObject go)
    {
        Destroy(go.transform.parent.gameObject);
    }

    private void OnCancel(GameObject go)
    {
        for (int i = 0; i < choiceItemIndexes.Count; i++)
        {
            var itemFrame = itemsToUp[i];
            Destroy(itemFrame.FindChild(LvlUpItem.name + "(Clone)").gameObject);
        }
        choiceItemIndexes.Clear();
        selCountValue.text = string.Format("{0}/{1}", choiceItemIndexes.Count, MaxSelCount);
        GetExp = 0;
    }

    private int GetExpToFullLvl()
    {
        var curLvl = itemInfo.Level;
        var maxLvl = itemInfo.MaxLvl;
        var itemLvlTmpls = ItemModeLocator.Instance.ItemConfig.ItemLvlTmpls;
        var result = itemLvlTmpls[curLvl].MaxExp - itemInfo.CurExp;
        for (int i = curLvl + 1; i < maxLvl; i++)
        {
            result += itemLvlTmpls[i].MaxExp;
        }
        return result;
    }

    private void Refresh()
    {
        ExpToFullLvl = GetExpToFullLvl();
        costExpValue.text = ExpToFullLvl.ToString(CultureInfo.InvariantCulture);
    }

    private void OnLevelUpSel(GameObject go)
    {
        var indexs = (choiceItemIndexes != null) ? new List<short>(choiceItemIndexes) : null;
        var upSelWin = WindowManager.Instance.Show<UIItemLevelUpSelWindow>(true);
        upSelWin.UpdateSelects(indexs, GetExp);
        WindowManager.Instance.Show<ItemBaseInfoWindow>(false);
    }

    private void OnBack(GameObject go)
    {
        cachedBaseWindow.RefreshChangedData();
        WindowManager.Instance.Show<UIItemInfoWindow>(true);
    }

    private void CleanUp()
    {
        if (choiceItemIndexes != null)
        {
            for (int i = 0; i < choiceItemIndexes.Count; i++)
            {
                var itemFrame = itemsToUp[i];
                var child = itemFrame.FindChild(LvlUpItem.name + "(Clone)");
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
            choiceItemIndexes.Clear();
        }
        GetExp = 0;
    }

    public void UpdateSelect(List<short> choiceIndexes, int exp)
    {
        choiceItemIndexes = choiceIndexes;
        selCountValue.text = string.Format("{0}/{1}", choiceItemIndexes.Count, MaxSelCount);
        GetExp = exp;
        for (int i = 0; i < choiceItemIndexes.Count; i++)
        {
            var itemFrame = itemsToUp[i];
            var child = itemFrame.FindChild(LvlUpItem.name + "(Clone)") ??
                        NGUITools.AddChild(itemFrame.gameObject, LvlUpItem).transform;
            var levelUpItem = child.GetComponent<LevelUpItem>();
            var info = ItemModeLocator.Instance.FindItem(choiceItemIndexes[i]);
            levelUpItem.InitItem(info);
        }
    }

    private void UpdateSelect(List<ItemInfo> infos)
    {
        var exp = 0;
        if (choiceItemIndexes != null)
        {
            choiceItemIndexes.Clear();
        }
        else
        {
            choiceItemIndexes = new List<short>();
        }
        for (int i = 0; i < infos.Count; i++)
        {
            choiceItemIndexes.Add(infos[i].BagIndex);
            var itemFrame = itemsToUp[i];
            var child = itemFrame.FindChild(LvlUpItem.name + "(Clone)") ??
                        NGUITools.AddChild(itemFrame.gameObject, LvlUpItem).transform;
            var levelUpItem = child.GetComponent<LevelUpItem>();
            levelUpItem.InitItem(infos[i]);
            exp += infos[i].ContribExp * (ItemHelper.IsSameJobType(mainType, mainItemInfo.TmplId, infos[i].BagIndex) ? 5 : 1);
        }
        GetExp = exp;
        selCountValue.text = string.Format("{0}/{1}", choiceItemIndexes.Count, MaxSelCount);
    }

    public void ShowLevelOver()
    {
        itemInfo.Level = cacheLevel;
        itemInfo.CurExp = cachedCurExp;
        cachedBaseWindow.GetComponent<ItemBaseInfoWindow>().StopCoroutine("PreShowLevelUp");
        cachedBaseWindow.RefreshChangedData();
        CleanUp();
    }

    #endregion
}
