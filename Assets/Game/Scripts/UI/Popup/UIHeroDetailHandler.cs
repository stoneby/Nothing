using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using Property;
using Template.Auto.Hero;
using Template.Auto.Skill;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroDetailHandler : MonoBehaviour
{
    #region Public Fields

    public int CountOfOneGroup = 4;
    public GameObject HeroSelItemPrefab;
    public PropertyUpdater PropertyUpdater;
    public HeroBaseInfoRefresher HeroBaseInfoRefresher;
    public List<ItemInfo> Infos { get; private set; }

    #endregion

    #region Private Fileds

    private UILabel activeSkillName;
    private UILabel activeSkillDesc; 
    private UILabel leaderSkillName;
    private UILabel leaderSkillDesc;
    private HeroSelItem heroSelItem;
    private UIHeroCommonWindow commonWindow;
    private GameObject[] equipItems;
    private const int MaxEquipCount = 4;
    private bool isEnterSelItem;
    private const int MayChangePropCount = 4;
    private readonly int[] changedProps = new int[MayChangePropCount];
    private string cachedEquipUuid;

    #endregion

    #region Private Methods

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.HeroDetailWindow);
        CommonHandler.HeroPropertyChanged += OnHeroPropertyChanged;
        if (heroSelItem != null)
        {
            NGUITools.Destroy(heroSelItem);
            heroSelItem = null;
        }
        commonWindow.NormalClicked = OnDetail;
        commonWindow.ShowSelMask(true);
        RefreshData();
    }

    private void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.HeroDetailWindow);
        CommonHandler.HeroPropertyChanged -= OnHeroPropertyChanged;
    }

    // Use this for initialization
    void Awake()
    {
        var activeSkill = Utils.FindChild(transform, "ActiveSkill");
        activeSkillName = activeSkill.Find("Name").GetComponent<UILabel>();
        activeSkillDesc = activeSkill.Find("Desc").GetComponent<UILabel>();
        var leaderSkill = Utils.FindChild(transform, "LeaderSkill");
        leaderSkillName = leaderSkill.Find("Name").GetComponent<UILabel>();
        leaderSkillDesc = leaderSkill.Find("Desc").GetComponent<UILabel>();
        commonWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
        equipItems = new GameObject[MaxEquipCount];
        const string equipPrefix = "Container items/HeroEquip";
        for (var i = 0; i < MaxEquipCount; i++)
        {
            equipItems[i] = transform.FindChild(equipPrefix + i).gameObject;
        }
        foreach(var item in equipItems.Select(equipItem => equipItem.GetComponent<HeroEquipControl>()))
        {
            item.ClickedHandler = OpenSelectHandler;
        }
    }

    private GameObject selectPosEquipObj;
    private void OpenSelectHandler(GameObject obj)
    {
        if(selectPosEquipObj)
        {
            var heroEquipControl = selectPosEquipObj.GetComponent<HeroEquipControl>();
            SendChangeEquipMessage(heroEquipControl);
        }
        selectPosEquipObj = obj;
        cachedEquipUuid = obj.GetComponent<HeroEquipControl>().Uuid;
        if (!ItemModeLocator.AlreadyMainRequest)
        {
            ItemModeLocator.Instance.GetItemPos = ItemType.GetItemInHeroInfo;
            var csmsg = new CSQueryAllItems { BagType = ItemType.MainItemBagType };
            NetManager.SendMessage(csmsg);
        }
        else
        {
            RefreshCanEquipItems();
        }
    }

    private void ConfirmEquipHandler(GameObject obj)
    {
        ResetAfterSelItem();
        if (selectPosEquipObj)
        {
            var heroEquipControl = selectPosEquipObj.GetComponent<HeroEquipControl>();
            SendChangeEquipMessage(heroEquipControl);
        }
    }

    private void SendChangeEquipMessage(HeroEquipControl heroEquipControl)
    {
        if (heroEquipControl.Uuid != cachedEquipUuid)
        {
            var msg = new CSHeroChangeEquip
                          {
                              HeroUuid = commonWindow.HeroInfo.Uuid,
                              Index = sbyte.Parse(heroEquipControl.PosIndex.ToString()),
                              EquipUuid = heroEquipControl.Uuid
                          };
            NetManager.SendMessage(msg);
            cachedEquipUuid = "";
            selectPosEquipObj = null;
        }
    }

    private void ResetAfterSelItem()
    {
        NGUITools.SetActive(commonWindow.Heros.gameObject, true);
        commonWindow.ShowSelMask(true);
        isEnterSelItem = false;
        for (var i = 0; i < changedProps.Length; i++)
        {
            changedProps[i] = 0;
        }
    }

    private void OnHeroPropertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        var heroInfo = commonWindow.HeroInfo;
        PropertyUpdater.UpdateProperty(heroInfo.Lvl, commonWindow.HeroTemplate.LvlLimit,
                                       heroInfo.Prop[RoleProperties.ROLE_ATK], heroInfo.Prop[RoleProperties.ROLE_HP],
                                       heroInfo.Prop[RoleProperties.ROLE_RECOVER], heroInfo.Prop[RoleProperties.ROLE_MP]);
        if(NGUITools.GetActive(commonWindow.Heros.gameObject))
        {
            commonWindow.Refresh();
        }
    }


    private void OnDetail(GameObject go)
    {
        commonWindow.CurSelPos = UISellHeroHandler.GetPosition(go);
        cachedEquipUuid = "";
        selectPosEquipObj = null;
        RefreshData();
    }

    /// <summary>
    /// Update ui data.
    /// </summary>
    private void RefreshData()
    {
        var heroTemplate = commonWindow.HeroTemplate;
        var heroInfo = commonWindow.HeroInfo;
        var level = heroInfo.Lvl;
        var maxLevel = heroTemplate.LvlLimit;
        var atkTemp = heroInfo.Prop[RoleProperties.ROLE_ATK];
        var hpTemp = heroInfo.Prop[RoleProperties.ROLE_HP];
        var recoverTemp = heroInfo.Prop[RoleProperties.ROLE_RECOVER];
        var mpTemp = heroInfo.Prop[RoleProperties.ROLE_MP];
        PropertyUpdater.UpdateProperty(level, maxLevel, atkTemp, hpTemp, recoverTemp, mpTemp);
        HeroBaseInfoRefresher.Refresh(heroInfo);
        RefreshSkills(heroTemplate);
        InitEquipedItems();
    }

    private void RefreshSkills(HeroTemplate heroTemplate)
    {
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;
        HeroBattleSkillTemplate skillTemp;
        skillTmp.TryGetValue(heroTemplate.ActiveSkill, out skillTemp);
        var contains = skillTemp != null;
        activeSkillName.text = contains ? skillTemp.Name : "-";
        activeSkillDesc.text = contains ? skillTemp.Desc : "-";
        skillTmp.TryGetValue(heroTemplate.LeaderSkill, out skillTemp);
        contains = skillTemp != null;
        leaderSkillName.text = contains ? skillTemp.Name : "-";
        leaderSkillDesc.text = contains ? skillTemp.Desc : "-";
    }

    private void InitEquipedItems()
    {
        var heroInfo = commonWindow.HeroInfo;
        for (int i = 0; i < heroInfo.EquipTemplateId.Count; i++)
        {
            if (equipItems != null && equipItems[i] != null)
            {
                var item = equipItems[i].GetComponent<HeroEquipControl>();
                item.SetData(heroInfo.EquipUuid[i], heroInfo.EquipTemplateId[i]);
            }
        }
    }

    public void RefreshCanEquipItems()
    {
        if (isEnterSelItem == false)
        {
            heroSelItem = NGUITools.AddChild(gameObject, HeroSelItemPrefab).GetComponent<HeroSelItem>();
            heroSelItem.EquipItemClicked = OnEquipItemClicked;
            heroSelItem.ConfirmClicked = ConfirmEquipHandler;
            NGUITools.SetActive(commonWindow.Heros.gameObject, false);
            commonWindow.ShowSelMask(false);
            isEnterSelItem = true;
        }
        var curUuid = "";
        var others = new List<string>();
        foreach (var equip in equipItems)
        {
            if(equip == selectPosEquipObj)
            {
                curUuid = selectPosEquipObj.GetComponent<HeroEquipControl>().Uuid;
            }
            else
            {
                others.Add(equip.GetComponent<HeroEquipControl>().Uuid);
            }
        }
        heroSelItem.Refresh(curUuid, others);
    }

    private void OnEquipItemClicked(GameObject go)
    {
        var newInfo = (go != null) ? go.GetComponent<NewEquipItem>().TheItemInfo : null;
        var oldUuid = selectPosEquipObj.GetComponent<HeroEquipControl>().Uuid;
        var oldInfo = ItemModeLocator.Instance.FindItem(oldUuid);
        int oldAtk = 0, oldHp = 0, oldRecover = 0, oldMp = 0;
        int newAtk = 0, newHp = 0, newRecover = 0, newMp = 0;
        if (oldInfo != null)
        {
            ItemHelper.GetProproties(oldInfo, out oldAtk, out oldHp, out oldRecover, out oldMp);
        }
        if (newInfo != null)
        {
            ItemHelper.GetProproties(newInfo, out newAtk, out newHp, out newRecover, out newMp);
        }
        changedProps[0] += (newAtk - oldAtk);
        changedProps[1] += (newHp - oldHp);
        changedProps[2] += (newRecover - oldRecover);
        changedProps[3] += (newMp - oldMp);
        PropertyUpdater.PreShowChangedProperty(0, changedProps[0], changedProps[1], changedProps[2], changedProps[3]);
        var select = selectPosEquipObj.GetComponent<HeroEquipControl>();
        var id = newInfo != null ? newInfo.Id : "";
        var tempId = newInfo != null ? newInfo.TmplId : 1;
        select.SetData(id, tempId);
        RefreshCanEquipItems();
    }

    #endregion

    #region Public Methods

    public void RefreshData(HeroInfo info)
    {
        commonWindow.HeroInfo = info;
        RefreshData();
    }

    public void EquipOver(HeroInfo info)
    {
        RefreshData(info);
        if (heroSelItem != null)
        {
            NGUITools.Destroy(heroSelItem);
            heroSelItem = null;
        }
    }

    #endregion
}