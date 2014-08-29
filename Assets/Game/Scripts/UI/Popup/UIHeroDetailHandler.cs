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
    private UIEventListener lockLis;
    private UISprite lockSprite;

    public PropertyUpdater PropertyUpdater;
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
    public List<ItemInfo> Infos { get; private set; }
    public int CountOfOneGroup = 4;
    private const int MaxEquipCount = 4;
    public static bool IsLongPressEnter;
    private bool isEnterSelItem;

    public GameObject HeroSelItemPrefab;

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
        if (ItemModeLocator.Instance.ScAllItemInfos == null)
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
            if (item.TemplateId > 0)
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
    }

    private void UnInstallHandlers()
    {
        lockLis.onClick = null;
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

    #endregion

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

        if (skillTmp.ContainsKey(heroTemplate.LeaderSkill))
        {
            var leaderSkillTemp = skillTmp[heroTemplate.LeaderSkill];
            leaderSkillName.text = leaderSkillTemp.Name;
            leaderSkillDesc.text = leaderSkillTemp.Desc;
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
            heroSelItem.Refresh();
            heroSelItem.EquipItemClicked = OnEquipItemClicked;
            heroSelItem.CancelClicked = CancelEquipHandler;
            heroSelItem.ConfirmClicked = ConfirmEquipHandler;
            NGUITools.SetActive(commonWindow.Heros.gameObject, false);
            commonWindow.ShowSelMask(false);
            isEnterSelItem = true;
        }
        ShowDefaultMask();
    }

    private void ShowDefaultMask()
    {
        var item = selectPosEquipObj.GetComponent<HeroEquipControl>();
        var heroSelItems = heroSelItem.Items.transform;
        for (var i = 0; i < heroSelItems.childCount; i++)
        {
            var itemContent = heroSelItems.GetChild(i).GetComponent<WrapItemContent>();
            foreach (var child in itemContent.Children)
            {
                var equipItem = child.GetComponent<NewEquipItem>();
                if(equipItem.TheItemInfo.Id == item.Uuid)
                {
                    itemContent.ShowSellMask(itemContent.Children.IndexOf(child), true);
                }
            }    
        }
    }

    private void OnEquipItemClicked(GameObject go)
    {
        var item = go.GetComponent<NewEquipItem>();
        var select = selectPosEquipObj.GetComponent<HeroEquipControl>();
        select.SetData(item.TheItemInfo.Id, item.TheItemInfo.TmplId);
    }
}