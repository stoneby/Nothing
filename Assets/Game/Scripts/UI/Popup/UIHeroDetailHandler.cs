using System.Collections.Generic;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroDetailHandler : MonoBehaviour
{
    public PropertyUpdater PropertyUpdater;
    private UILabel activeSkillName;
    private UILabel activeSkillDesc; 
    private UILabel leaderSkillName;
    private UILabel leaderSkillDesc;
    private readonly List<UIEventListener> heroSelItemLis = new List<UIEventListener>();
    private sbyte curEquipIndex = -1;
    private Transform baseInfos;
    private GameObject heroSelItemIns;
    private UIHeroCommonWindow commonWindow;

    public CustomGrid Items;

    private UIEventListener selectEquipConfirmListener;
    private UIEventListener selectEquipCancelListener;
    private GameObject SelectEquipContainer;
    private GameObject[] EquipItems;
    private UIEventListener.BoolDelegate setParentFunc;

    public List<ItemInfo> Infos { get; private set; }
    private bool descendSort = true;
    public int CountOfOneGroup = 4;

    public static bool IsLongPressEnter;

    public UIEventListener.BoolDelegate SetParentBoolDelegate
    {
        get { return setParentFunc; }
        set
        {
            setParentFunc = value;
        }
    }

    private void InitWrapContents()
    {
        if (Infos != null) return;
        Infos = ItemModeLocator.Instance.ScAllItemInfos.ItemInfos;
        //var orderType = HeroModelLocator.Instance.OrderType;
        //ItemModeLocator.Instance.SortItemList(orderType, Infos, descendSort);
        var list = new List<List<ItemInfo>>();
        var rows = Mathf.CeilToInt((float)Infos.Count / CountOfOneGroup);
        for (var i = 0; i < rows; i++)
        {
            var infosContainer = new List<ItemInfo>();
            for (var j = 0; j < CountOfOneGroup; j++)
            {
                if (i * CountOfOneGroup + j < Infos.Count)
                {
                    infosContainer.Add(Infos[i * CountOfOneGroup + j]);
                }
            }
            list.Add(infosContainer);
        }
        Items.Init(list);

        var parent = Items.transform;
        for (var i = 0; i < parent.childCount; i++)
        {
            var item = parent.GetChild(i);
            for (var j = 0; j < item.childCount; j++)
            {
                var hero = item.GetChild(j).gameObject;
                var lis = UIEventListener.Get(hero);
                lis.onClick = ClickEquipHandler;
            }
        }
    }

    public sbyte CurEquipIndex
    {
        get { return curEquipIndex; }
        private set { curEquipIndex = value; }
    }

    public GameObject HeroSelItemPrefab;

    #region Private Methods

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.HeroDetailWindow);
        InstallHandlers();
        if (heroSelItemIns != null)
        {
            NGUITools.Destroy(heroSelItemIns);
            heroSelItemIns = null;
        }
        commonWindow.NormalClicked = OnDetail;
        commonWindow.ShowSelMask();
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
        commonWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
        var go = commonWindow.Heros.transform.GetChild(0).GetComponent<WrapHerosItem>().Children[0].gameObject;
        OnDetail(go);

        selectEquipConfirmListener = UIEventListener.Get(transform.FindChild("Container add equip/Image Button confirm").gameObject);
        selectEquipCancelListener = UIEventListener.Get(transform.FindChild("Container add equip/Image Button cancel").gameObject);
        selectEquipConfirmListener.onClick += SelectEquipHandler;
        selectEquipCancelListener.onClick += CancelEquipHandler;
        SelectEquipContainer = transform.FindChild("Container add equip").gameObject;
        SelectEquipContainer.SetActive(false);
        EquipItems = new GameObject[4];
        EquipItems[0] = transform.FindChild("Container items/HeroEquip1").gameObject;
        EquipItems[1] = transform.FindChild("Container items/HeroEquip2").gameObject;
        EquipItems[2] = transform.FindChild("Container items/HeroEquip3").gameObject;
        EquipItems[3] = transform.FindChild("Container items/HeroEquip4").gameObject;
        var col = EquipItems[0].GetComponent<HeroEquipControl>();
        col.PosIndex = 0;
        col = EquipItems[1].GetComponent<HeroEquipControl>();
        col.PosIndex = 1;
        col = EquipItems[2].GetComponent<HeroEquipControl>();
        col.IsLocked = true;
        col.PosIndex = 2;
        col = EquipItems[3].GetComponent<HeroEquipControl>();
        col.IsLocked = true;
        col.PosIndex = 3;
        for (int i = 0; i < EquipItems.Length; i++)
        {
            var item = EquipItems[i].GetComponent<HeroEquipControl>();
            item.ClickedHandler = OpenSelectHandler;
        }
    }

    private GameObject selectPosEquipObj;
    private void OpenSelectHandler(GameObject obj)
    {
        selectPosEquipObj = obj;
        SelectEquipContainer.SetActive(true);
        if (setParentFunc != null)
        {
            setParentFunc(obj, false);
        }
        InitWrapContents();
        Items.gameObject.SetActive(true);
    }

    private void ClickEquipHandler(GameObject obj)
    {
        var item = obj.GetComponent<NewEquipItem>();
        var select = selectPosEquipObj.GetComponent<HeroEquipControl>();
        select.SetSelectData(item.TheItemInfo.Id, item.TheItemInfo.TmplId);
    }

    private void SelectEquipHandler(GameObject obj)
    {
        //var select = selectPosEquipObj.GetComponent<HeroEquipControl>();
        for (int i = 0; i < EquipItems.Length; i++)
        {
            var item = EquipItems[i].GetComponent<HeroEquipControl>();
            if (item.SelectTemplateId > 0)
            {
                var msg = new CSHeroChangeEquip();
                msg.HeroUuid = commonWindow.HeroInfo.Uuid;
                msg.Index = sbyte.Parse(item.PosIndex.ToString());
                msg.EquipUuid = item.Selectuuid;
                NetManager.SendMessage(msg);
                item.SetData(item.Selectuuid, item.SelectTemplateId);
            }
        }
        SelectEquipContainer.SetActive(false);
        if (setParentFunc != null)
        {
            setParentFunc(obj, true);
        }
    }

    private void CancelEquipHandler(GameObject obj)
    {
        SelectEquipContainer.SetActive(false);
        if (setParentFunc != null)
        {
            setParentFunc(obj, true);
        }
        for (int i = 0; i < EquipItems.Length; i++)
        {
            var item = EquipItems[i].GetComponent<HeroEquipControl>();
            item.SetData(item.Uuid, item.TemplateId);
        }
    }

    private void InstallHandlers()
    {
        //for (var i = 0; i < heroSelItemLis.Count; i++)
        //{
        //    var selItemLis = heroSelItemLis[i];
        //    selItemLis.onClick = OnHeroSelItem;
        //}
    }

    private void UnInstallHandlers()
    {
        //for (var i = 0; i < heroSelItemLis.Count; i++)
        //{
        //    var selItemLis = heroSelItemLis[i];
        //    selItemLis.onClick = null;
        //}
    }

    private void OnDetail(GameObject go)
    {
        commonWindow.CurSel = go;
        commonWindow.ShowSelMask(go.transform.position);
        var uuid = go.GetComponent<NewHeroItem>().Uuid;
        commonWindow.HeroInfo = HeroModelLocator.Instance.FindHero(uuid);
        RefreshData();
    }

    private void OnHeroSelItem(GameObject go)
    {
        var eventLis = go.GetComponent<UIEventListener>();
        CurEquipIndex = (sbyte) heroSelItemLis.IndexOf(eventLis);
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

    #endregion

    public void RefreshData(HeroInfo info)
    {
        commonWindow.HeroInfo = info;
        RefreshData();
    }

    public void EquipOver(HeroInfo info)
    {
        RefreshData(info);
        if (heroSelItemIns != null)
        {
            NGUITools.Destroy(heroSelItemIns);
            heroSelItemIns = null;
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
        baseInfos.FindChild("Name").GetComponent<UILabel>().text = heroTemplate.Name;
        var stars = baseInfos.Find("Stars");
        for (int index = stars.childCount - 1; index >= stars.childCount - heroTemplate.Star; index--)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = 0; index < stars.childCount - heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }

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

        for (int i = 0; i < heroInfo.EquipTemplateId.Count; i++)
        {
            if (EquipItems != null && EquipItems[i] != null)
            {
                var item = EquipItems[i].GetComponent<HeroEquipControl>();
                item.SetData(heroInfo.EquipUuid[i], heroInfo.EquipTemplateId[i]);
            }           
        }
        //InitEquipedItems();
    }

    private void InitEquipedItems()
    {
        //var equips = heroInfo.EquipUuid;
        //for (var i = 0; i < equips.Count; i++)
        //{
        //    var itemIcon = heroSelItemLis[i].transform.GetChild(0).GetComponent<UISprite>();
        //    itemIcon.enabled = equips[i] != "";
        //}
        //for (var i = equips.Count; i < heroSelItemLis.Count; i++)
        //{
        //    var itemIcon = heroSelItemLis[i].transform.GetChild(0).GetComponent<UISprite>();
        //    itemIcon.enabled = false;
        //}
    }

    public void RefreshCanEquipItems()
    {
        NGUITools.SetActiveChildren(baseInfos.gameObject, false);
        heroSelItemIns = NGUITools.AddChild(gameObject, HeroSelItemPrefab);
        heroSelItemIns.GetComponent<HeroSelItem>().Refresh();
    } 
}