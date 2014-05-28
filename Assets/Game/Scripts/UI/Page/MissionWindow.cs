using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class MissionWindow : Window
{
    private GameObject MapContainer;
    private GameObject RaidContainer;
    private GameObject FriendContainer;

    private GameObject LevelLabel;
    private GameObject ExpLabel;
    private GameObject EnergyLabel;
    private GameObject EnergyProgress;
    private GameObject ExpProgress;

    private GameObject MapEvent;
    private GameObject ItemsTable;
    private GameObject FriendsTable;

    private GameObject EventSprite;
    private GameObject EventNameLabel;
    private GameObject EventTimeLabel;

    private GameObject ItemPrefab;
    private GameObject FriendItemPrefab;

    private GameObject BtnReturn;
    private UIEventListener BtnCloseUIEventListener;

    private List<RaidInfo> Raids; 

    #region Window

    public override void OnEnter()
    {
        if (MissionModelLocator.Instance.CurrRaidType != MissionModelLocator.Instance.RaidType)
        {
            MissionModelLocator.Instance.CurrRaidType = MissionModelLocator.Instance.RaidType;
            MissionModelLocator.Instance.MissionStep = RaidType.StepRaidList;
        }
        switch (MissionModelLocator.Instance.MissionStep)
        {
            case RaidType.StepRaidList:
                SetMissionList();
                break;
            case RaidType.StepStageList:
                SetStageList(MissionModelLocator.Instance.Raid);
                break;
            case RaidType.StepFriendList:
                SetFriendList(MissionModelLocator.Instance.FriendsMsg);
                break;
            default:
                SetMissionList();
                break;
        }

        EventManager.Instance.AddListener<MissionItemEvent>(OnItemClicktHandler);
        EventManager.Instance.AddListener<FriendEvent>(OnFriendHandler);
        EventManager.Instance.AddListener<FriendClickEvent>(OnFriendItemClickHandler);

        BtnCloseUIEventListener.onClick += OnCloseButtonClick;

        var lb = LevelLabel.GetComponent<UILabel>();
        lb.text = PlayerModelLocator.Instance.Level.ToString();

        var temp = LevelModelLocator.Instance.GetLevelByTemplateId(PlayerModelLocator.Instance.Level + 1);

        lb = EnergyLabel.GetComponent<UILabel>();
        lb.text = PlayerModelLocator.Instance.Energy + "/" + temp.MaxEnergy;

        var pro = EnergyProgress.GetComponent<UIProgressBar>();
        pro.value = (float)PlayerModelLocator.Instance.Energy / temp.MaxEnergy;

        lb = ExpLabel.GetComponent<UILabel>();
        lb.text = ((int)(100 * PlayerModelLocator.Instance.Exp / temp.MaxExp)) + "%";

        pro = ExpProgress.GetComponent<UIProgressBar>();
        pro.value = (float)PlayerModelLocator.Instance.Exp / temp.MaxExp;
    }

    private void SetMissionList()
    {
        MissionModelLocator.Instance.MissionStep = RaidType.StepRaidList;
        MapEvent.SetActive(false);
        MapContainer.SetActive(true);
        RaidContainer.SetActive(true);
        FriendContainer.SetActive(false);
        MissionModelLocator.Instance.ComputeStagecount();
        
        switch (MissionModelLocator.Instance.CurrRaidType)
        {
            case RaidType.RaidNormal:
                Raids = MissionModelLocator.Instance.RaidLoadingAll.RaidInfoNormal;
                break;
            case RaidType.RaidElite:
                Raids = MissionModelLocator.Instance.RaidLoadingAll.RaidInfoElite;
                break;
            case RaidType.RaidHero:
                Raids = MissionModelLocator.Instance.RaidLoadingAll.RaidInfoMaster;
                break;
            default:
                Raids = MissionModelLocator.Instance.RaidLoadingAll.RaidInfoNormal;
                break;
        }
        var table = ItemsTable.GetComponent<UITable>();
        int itemindex = 0;
        GameObject item;
        for (int i = Raids.Count - 1; i >= 0; i--)
        {
            var raid = Raids[i];
            //raid.TemplateId;
            
            if (itemindex < table.children.Count)
            {
                item = table.children[itemindex].gameObject;
                item.SetActive(true);
            }
            else
            {
                item = NGUITools.AddChild(ItemsTable, ItemPrefab);
            }
           
            var col = item.GetComponent<MissionItemControl>();
            col.InitRaid(i, MissionModelLocator.Instance.GetRaidByTemplateId(raid.TemplateId), raid,
                MissionModelLocator.Instance.GetAdditionInfoByRaidTemplateID(raid.TemplateId));

            itemindex++;
        }

        while (itemindex < table.children.Count)
        {
            item = table.children[itemindex].gameObject;
            item.SetActive(false);
            itemindex ++;
        }
    }

    private void SetStageList(RaidInfo raid)
    {
        MissionModelLocator.Instance.MissionStep = RaidType.StepStageList;
        MapContainer.SetActive(true);
        RaidContainer.SetActive(true);
        FriendContainer.SetActive(false);
        var table = ItemsTable.GetComponent<UITable>();
        int itemindex = 0;
        GameObject item;
        var stages = raid.StateInfo;
        var addition = MissionModelLocator.Instance.GetAdditionInfoByRaidTemplateID(raid.TemplateId);
        if (addition != null)
        {
            MapEvent.SetActive(true);
            var sp = EventSprite.GetComponent<UISprite>();
            var lbname = EventNameLabel.GetComponent<UILabel>();
            var lbtime = EventTimeLabel.GetComponent<UILabel>();
            //金币150%、武魂150%、体力消耗50%、掉落概率150%
            switch (addition.AddtionType)
            {
                case RaidType.RaidAddtionTypeDrop:
                    sp.spriteName = "icon_box";
                    lbname.text = "x1.5";
                    break;
                case RaidType.RaidAddtionTypeEnergy:
                    sp.spriteName = "icon_energy";
                    lbname.text = "x0.5";
                    break;
                case RaidType.RaidAddtionTypeGold:
                    sp.spriteName = "icon_gold";
                    lbname.text = "x1.5";
                    break;
                case RaidType.RaidAddtionTypeSprit:
                    sp.spriteName = "icon_wuhun";
                    lbname.text = "x1.5";
                    break;
                default:
                    break;
            }


        }
        for (int i = stages.Count - 1; i >= 0; i--)
        {
            var stage = stages[i];
            //raid.TemplateId;

            if (itemindex < table.children.Count)
            {
                item = table.children[itemindex].gameObject;
                item.SetActive(true);
            }
            else
            {
                item = NGUITools.AddChild(ItemsTable, ItemPrefab);
            }

            var col = item.GetComponent<MissionItemControl>();
            col.InitStage(i, MissionModelLocator.Instance.GetRaidStagrByTemplateId(stage.TemplateId), stage);

            itemindex++;
        }

        while (itemindex < table.children.Count)
        {
            item = table.children[itemindex].gameObject;
            item.SetActive(false);
            itemindex++;
        }
    }

    private void SetFriendList(SCRaidQueryFriend friends)
    {
        MissionModelLocator.Instance.MissionStep = RaidType.StepFriendList;
        MapContainer.SetActive(false);
        RaidContainer.SetActive(false);
        FriendContainer.SetActive(true);

        var table = FriendsTable.GetComponent<UITable>();
        int itemindex = 0;
        GameObject item;

        for (int i = 0; i < friends.BattleFriend.Count; i++)
        {
            var friend = friends.BattleFriend[i];
            //raid.TemplateId;

            if (itemindex < table.children.Count)
            {
                item = table.children[itemindex].gameObject;
                item.SetActive(true);
            }
            else
            {
                item = NGUITools.AddChild(FriendsTable, FriendItemPrefab);
            }

            var col = item.GetComponent<FriendItemControl>();
            col.Init(friend, true);

            itemindex++;
        }

        for (int i = 0; i < friends.BattleGuest.Count; i++)
        {
            var friend = friends.BattleGuest[i];
            //raid.TemplateId;

            if (itemindex < table.children.Count)
            {
                item = table.children[itemindex].gameObject;
                item.SetActive(true);
            }
            else
            {
                item = NGUITools.AddChild(FriendsTable, FriendItemPrefab);
            }

            var col = item.GetComponent<FriendItemControl>();
            col.Init(friend, false);

            itemindex++;
        }

        while (itemindex < table.children.Count)
        {
            item = table.children[itemindex].gameObject;
            item.SetActive(false);
            itemindex++;
        }
    }

    public override void OnExit()
    {
        EventManager.Instance.RemoveListener<MissionItemEvent>(OnItemClicktHandler);
        EventManager.Instance.RemoveListener<FriendEvent>(OnFriendHandler);
        EventManager.Instance.RemoveListener<FriendClickEvent>(OnFriendItemClickHandler);

        if (BtnCloseUIEventListener != null) BtnCloseUIEventListener.onClick -= OnCloseButtonClick;
    }

    #endregion

    #region Mono
    void Awake()
    {
        MapContainer = transform.FindChild("Map Container").gameObject;
        RaidContainer = transform.FindChild("Items Container").gameObject;
        FriendContainer = transform.FindChild("Friend Container").gameObject;

        MapEvent = transform.FindChild("Map Container/Event Container").gameObject;
        ItemsTable = transform.FindChild("Items Container/Scroll View/Table").gameObject;
        ItemPrefab = Resources.Load("Prefabs/Component/MissionItem") as GameObject;

        FriendsTable = transform.FindChild("Friend Container/Scroll View/Table").gameObject;
        FriendItemPrefab = Resources.Load("Prefabs/Component/FriendItem") as GameObject;

        EnergyLabel = transform.FindChild("Top Bar Container/Energy Label").gameObject;
        LevelLabel = transform.FindChild("Top Bar Container/Level Label").gameObject;
        ExpLabel = transform.FindChild("Top Bar Container/Exp Label").gameObject;
        EnergyProgress = transform.FindChild("Top Bar Container/Energy Progress Bar").gameObject;
        ExpProgress = transform.FindChild("Top Bar Container/Exp Progress Bar").gameObject;

        BtnReturn = transform.FindChild("Image Button return").gameObject;

        EventSprite = transform.FindChild("Map Container/Event Container/Event Sprite").gameObject;
        EventNameLabel = transform.FindChild("Map Container/Event Container/Event Label").gameObject;
        EventTimeLabel = transform.FindChild("Map Container/Event Container/Left Label").gameObject;

        BtnCloseUIEventListener = UIEventListener.Get(BtnReturn);
    }
    // Use this for initialization
    void Start()
    {
    }

    #endregion

    private MissionItemEvent MissionEvent;
    private void OnItemClicktHandler(MissionItemEvent e)
    {
        if (e.IsRaidClicked)
        {
            MissionModelLocator.Instance.Raid = Raids[e.RaidIndex];
            SetStageList(MissionModelLocator.Instance.Raid);
            
        }
        else
        {
            MissionEvent = e;
            MissionModelLocator.Instance.SelectedStageId = e.StageId;
            NetManager.SendMessage(new CSRaidQueryFriend());
            // var csmsg = new CSRaidQueryFriend();
            

        }
    }

    private void OnFriendHandler(FriendEvent e)
    {
        MissionModelLocator.Instance.FriendsMsg = e.RaidFriend;
        SetFriendList(MissionModelLocator.Instance.FriendsMsg);
    }

    private void OnFriendItemClickHandler(FriendClickEvent e)
    {
        MissionModelLocator.Instance.FriendData = e.FriendData;
        MissionModelLocator.Instance.IsFriend = e.IsFriend;
        if (HeroModelLocator.Instance.SCHeroList == null)
        {
            HeroModelLocator.Instance.GetHeroPos = RaidType.GetHeroInBattle;
            var csmsg = new CSHeroList();
            NetManager.SendMessage(csmsg);
        }
        else
        {
            WindowManager.Instance.Show(typeof(BattleConfirmTabWindow), true);
        }
        MissionModelLocator.Instance.MissionStep = RaidType.StepConfirm;
    }

    private void OnCloseButtonClick(GameObject game)
    {
        switch (MissionModelLocator.Instance.MissionStep)
        {
            case RaidType.StepRaidList:
                var curTabWindow = WindowManager.Instance.CurrentWindowMap[WindowGroupType.TabPanel];
                curTabWindow.gameObject.SetActive(false);
                break;
            case RaidType.StepStageList:
                MissionModelLocator.Instance.MissionStep = RaidType.StepRaidList;
                SetMissionList();
                break;
            case RaidType.StepFriendList:
                MissionModelLocator.Instance.MissionStep = RaidType.StepStageList;
                SetStageList(MissionModelLocator.Instance.Raid);
                break;
            default:
                SetMissionList();
                break;
        }
    }
}
