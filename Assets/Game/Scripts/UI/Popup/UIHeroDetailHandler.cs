using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using Property;
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
    public List<ItemInfo> Infos { get; private set; }

    #endregion

    #region Private Fileds

    private UIEventListener lockLis;
    private UISprite lockSprite;
    private UILabel activeSkillName;
    private UILabel activeSkillDesc; 
    private UILabel leaderSkillName;
    private UILabel leaderSkillDesc;
    private UILabel nameLabel;
    private UISprite icon;
    private Transform baseInfos;
    private HeroSelItem heroSelItem;
    private UIHeroCommonWindow commonWindow;
    private GameObject[] equipItems;
    private const int MaxEquipCount = 4;
    private bool isEnterSelItem;

    #endregion

    #region Private Methods

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.HeroDetailWindow);
        InstallHandlers();
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
        UnInstallHandlers();
        NGUITools.SetActiveChildren(baseInfos.gameObject, true);
    }

    // Use this for initialization
    void Awake()
    {
        NGUITools.SetActive(PropertyUpdater.gameObject, true);
        var activeSkill = Utils.FindChild(transform, "ActiveSkill");
        activeSkillName = activeSkill.Find("Name").GetComponent<UILabel>();
        activeSkillDesc = activeSkill.Find("Desc").GetComponent<UILabel>();
        var leaderSkill = Utils.FindChild(transform, "LeaderSkill");
        leaderSkillName = leaderSkill.Find("Name").GetComponent<UILabel>();
        leaderSkillDesc = leaderSkill.Find("Desc").GetComponent<UILabel>();
        baseInfos = transform.Find("BaseInfo");
        lockLis = UIEventListener.Get(baseInfos.Find("Lock").gameObject);
        lockSprite = lockLis.GetComponent<UISprite>();
        nameLabel = baseInfos.Find("Name").GetComponent<UILabel>();
        icon = baseInfos.Find("Icon").GetComponent<UISprite>();
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
        selectPosEquipObj = obj;
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
        for (var i = 0; i < equipItems.Length; i++)
        {
            var item = equipItems[i].GetComponent<HeroEquipControl>();
            if (item.TemplateId >= 1)
            {
                var msg = new CSHeroChangeEquip
                              {
                                  HeroUuid = commonWindow.HeroInfo.Uuid,
                                  Index = sbyte.Parse(item.PosIndex.ToString()),
                                  EquipUuid = item.Uuid
                              };
                NetManager.SendMessage(msg);
                item.SetData(item.Uuid, item.TemplateId);
            }
        }
    }

    private void CancelEquipHandler(GameObject obj)
    {
        ResetAfterSelItem();
        InitEquipedItems();
        for (var i = 0; i < equipItems.Length; i++)
        {
            var item = equipItems[i].GetComponent<HeroEquipControl>();
            item.SetData(item.Uuid, item.TemplateId);
        }
    }

    private void ResetAfterSelItem()
    {
        NGUITools.SetActive(commonWindow.Heros.gameObject, true);
        commonWindow.ShowSelMask(true);
        isEnterSelItem = false;
    }

    private void InstallHandlers()
    {
        lockLis.onClick = OnLockClicked;
        CommonHandler.HeroPropertyChanged += OnHeroPropertyChanged;
    }

    private void UnInstallHandlers()
    {
        lockLis.onClick = null;
        CommonHandler.HeroPropertyChanged -= OnHeroPropertyChanged;
    }

    private void OnHeroPropertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        var heroInfo = commonWindow.HeroInfo;
        var atkProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_ATK];
        heroInfo.Prop[RoleProperties.ROLE_ATK] = atkProp;
        var hpProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_HP];
        heroInfo.Prop[RoleProperties.ROLE_HP] = hpProp;
        var recoverProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_RECOVER];
        heroInfo.Prop[RoleProperties.ROLE_RECOVER] = recoverProp;
        var mpProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_MP];
        heroInfo.Prop[RoleProperties.ROLE_MP] = mpProp;
        PropertyUpdater.UpdateProperty(heroInfo.Lvl, commonWindow.HeroTemplate.LvlLimit, atkProp, hpProp, recoverProp, mpProp);
        commonWindow.Refresh();
    }

    private void OnLockClicked(GameObject go)
    {
        commonWindow.ReverseLockState();
        commonWindow.ShowLockState(lockSprite);
    }

    private void OnDetail(GameObject go)
    {
        commonWindow.CurSelPos = UISellHeroHandler.GetPosition(go);
        commonWindow.ShowSelMask(go.transform.position);
        commonWindow.ShowLockState(lockSprite);
        RefreshData();
    }

    /// <summary>
    /// Update ui data.
    /// </summary>
    private void RefreshData()
    {
        var heroTemplate = commonWindow.HeroTemplate;
        var heroInfo = commonWindow.HeroInfo;
        nameLabel.text = heroTemplate.Name;
        var stars = baseInfos.Find("Stars");
        for (int index = stars.childCount - 1; index >= stars.childCount - heroTemplate.Star; index--)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = 0; index < stars.childCount - heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
        HeroConstant.SetHeadByIndex(icon, heroTemplate.Icon - 1);
        var level = heroInfo.Lvl;
        var maxLevel = heroTemplate.LvlLimit;
        var atkTemp = heroInfo.Prop[RoleProperties.ROLE_ATK];
        var hpTemp = heroInfo.Prop[RoleProperties.ROLE_HP];
        var recoverTemp = heroInfo.Prop[RoleProperties.ROLE_RECOVER];
        var mpTemp = heroInfo.Prop[RoleProperties.ROLE_MP];
        PropertyUpdater.UpdateProperty(level, maxLevel, atkTemp, hpTemp, recoverTemp, mpTemp);

        var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;
        if (skillTmp.ContainsKey(heroTemplate.SpSkill))
        {
            var activeSkillTemp = skillTmp[heroTemplate.ActiveSkill];
            activeSkillName.text = activeSkillTemp.Name;
            activeSkillDesc.text = activeSkillTemp.Desc;
        }
        else
        {
            activeSkillName.text = "-";
            activeSkillDesc.text = "-";
        }

        if (skillTmp.ContainsKey(heroTemplate.LeaderSkill))
        {
            var leaderSkillTemp = skillTmp[heroTemplate.LeaderSkill];
            leaderSkillName.text = leaderSkillTemp.Name;
            leaderSkillDesc.text = leaderSkillTemp.Desc;
        }
        else
        {
            leaderSkillName.text = "-";
            leaderSkillDesc.text = "-";
        }
        InitEquipedItems();
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
            heroSelItem.CancelClicked = CancelEquipHandler;
            heroSelItem.ConfirmClicked = ConfirmEquipHandler;
            NGUITools.SetActive(commonWindow.Heros.gameObject, false);
            commonWindow.ShowSelMask(false);
            isEnterSelItem = true;
        }
        var curUuid = selectPosEquipObj.GetComponent<HeroEquipControl>().Uuid;
        heroSelItem.Refresh(curUuid, commonWindow.HeroInfo.EquipUuid.Except(new List<string> { curUuid }).ToList());
    }

    private void OnEquipItemClicked(GameObject go)
    {
        var newInfo = (go != null) ? go.GetComponent<NewEquipItem>().TheItemInfo : null;
        var oldUuid = selectPosEquipObj.GetComponent<HeroEquipControl>().Uuid;
        var oldInfo = ItemModeLocator.Instance.FindItem(oldUuid);
        var oldAtk = 0;
        var oldHp = 0;
        var oldRecover = 0;
        var oldMp = 0;
        var newAtk = 0;
        var newHp = 0;
        var newRecover = 0;
        var newMp = 0;
        if (oldInfo != null)
        {
            ItemHelper.GetProproties(oldInfo, out oldAtk, out oldHp, out oldRecover, out oldMp);
        }
        if (newInfo != null)
        {
            ItemHelper.GetProproties(newInfo, out newAtk, out newHp, out newRecover, out newMp);
        }
        PropertyUpdater.PreShowChangedProperty(0, newAtk - oldAtk, newHp - oldHp, newRecover - oldRecover, newMp - oldMp);
        var select = selectPosEquipObj.GetComponent<HeroEquipControl>();
        var id = newInfo != null ? newInfo.Id : "";
        var tempId = newInfo != null ? newInfo.TmplId : 1;
        select.SetData(id, tempId);
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
            NGUITools.SetActiveChildren(baseInfos.gameObject, true);
        }
    }

    #endregion
}