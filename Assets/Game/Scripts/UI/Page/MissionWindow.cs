using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class MissionWindow : Window
{
    private GameObject MapContainer;
//    private GameObject RaidContainer;
    private GameObject FriendContainer;

    private GameObject LevelLabel;
    private GameObject ExpLabel;
    private GameObject EnergyLabel;
    private GameObject EnergyProgress;
    private GameObject ExpProgress;

    private GameObject MapEvent;
    private GameObject MapInfoLabel;
    private GameObject ItemsContainer;

    private GameObject EventSprite;
    private GameObject EventNameLabel;
    private GameObject EventTimeLabel;

    private GameObject RewardBox;
    private bool HasReward;

    private GameObject BtnReturn;
    private UIEventListener BtnCloseUIEventListener;
    private UIEventListener BtnRewardUIEventListener;

    private List<RaidInfo> Raids; 

    #region Window

    public override void OnEnter()
    {
        if (MissionModelLocator.Instance.CurrRaidType != MissionModelLocator.Instance.NextRaidType)
        {
            MissionModelLocator.Instance.CurrRaidType = MissionModelLocator.Instance.NextRaidType;
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

        EventManager.Instance.AddListener<FriendEvent>(OnFriendHandler);

        BtnCloseUIEventListener.onClick += OnCloseButtonClick;
        BtnRewardUIEventListener.onClick += OnRewardButtonClick;

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
        ItemsContainer.SetActive(true);
        FriendContainer.SetActive(false);
        RewardBox.SetActive(false);
        MissionModelLocator.Instance.ComputeStagecount();

        Raids = MissionModelLocator.Instance.GetCurrentRaids();
        var table = ItemsContainer.GetComponent<KxVScrollRender>();
        List<RaidInfo> temp;
        if (MissionModelLocator.Instance.CurrRaidType == RaidType.RaidNormal)
        {
            temp = new List<RaidInfo>(Raids.OrderByDescending(raidInfo => raidInfo.TemplateId));
        }
        else
        {
            temp = new List<RaidInfo>(Raids.OrderBy(raidInfo => raidInfo.TemplateId));
        }

//        for (int i = 0; i < 10; i++)
//        {
//            for (int j = 0; j < Raids.Count; j++)
//            {
//                temp.Add(Raids[j]);
//            }
//        }

        table.Init(temp, "Prefabs/Component/MissionItem", 537, 521, 537, 160, OnItemClicktHandler);

        var lb = MapInfoLabel.GetComponent<UILabel>();
        lb.text = "";
    }


    private List<RaidStageInfo> GetStages(RaidInfo raid)
    {
        Raids = MissionModelLocator.Instance.GetCurrentRaids();
        for (int i = 0; i < Raids.Count; i++)
        {
            if (Raids[i].TemplateId == raid.TemplateId)
            {
                return Raids[i].StateInfo;
            }
        }
        return raid.StateInfo;
    }

    private void SetStageList(RaidInfo raid)
    {
        MissionModelLocator.Instance.MissionStep = RaidType.StepStageList;
        MapContainer.SetActive(true);
        ItemsContainer.SetActive(true);
        FriendContainer.SetActive(false);
        RewardBox.SetActive(true);
        var boximage = RewardBox.transform.FindChild("Background").gameObject;
        var boxbg = boximage.GetComponent<UISprite>();
        HasReward = false;
        if (raid.StateInfo.Count <= 1)
        {
            boxbg.color = new Color(0.33f, 0.33f, 0.33f, 1);
        }
        else if (raid.StateInfo[raid.StateInfo.Count - 1].Star <= 0)
        {
            boxbg.color = new Color(0.33f, 0.33f, 0.33f, 1);
        }
        else if (!MissionModelLocator.Instance.RaidLoadingAll.HasAwardInfo.Contains(raid.TemplateId))
        {
           
            HasReward = MissionModelLocator.Instance.HasRaidReward(raid.TemplateId);
            if (HasReward)
            {
                boxbg.color = new Color(255, 255, 255, 1);
            }
            else
            {
                boxbg.color = new Color(0.33f, 0.33f, 0.33f, 1);
            }
        }
        else
        {
            RewardBox.SetActive(false);
        }

        var table = ItemsContainer.GetComponent<KxVScrollRender>();
        var stages = GetStages(raid);
        List<RaidStageInfo> temp;
        if (MissionModelLocator.Instance.CurrRaidType == RaidType.RaidNormal)
        {
            temp = new List<RaidStageInfo>(stages.OrderByDescending(stageinfo => stageinfo.TemplateId));
        }
        else
        {
            temp = new List<RaidStageInfo>(stages.OrderBy(stageinfo => stageinfo.TemplateId));
        }
        table.Init(temp, "Prefabs/Component/MissionItem", 537, 521, 537, 160, OnItemClicktHandler);
        

        var lb = MapInfoLabel.GetComponent<UILabel>();
        var raidtemp = MissionModelLocator.Instance.GetRaidByTemplateId(raid.TemplateId);
        
        lb.text = raidtemp.Name + "\n      " + raidtemp.RaidDesc;
        
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
            }
            lbtime.text = MissionModelLocator.Instance.DestTime;
        }
    }

    private void SetFriendList(SCRaidQueryFriend friends)
    {
        MissionModelLocator.Instance.MissionStep = RaidType.StepFriendList;
        MapContainer.SetActive(false);
        ItemsContainer.SetActive(false);
        FriendContainer.SetActive(true);

        var friendlist = new List<FriendVO>();

        for (int i = 0; i < friends.BattleFriend.Count; i++)
        {
            var friend = new FriendVO();
            friend.Data = friends.BattleFriend[i];
            friend.IsFriend = true;
            friendlist.Add(friend);
        }

        for (int i = 0; i < friends.BattleGuest.Count; i++)
        {
            var friend = new FriendVO();
            friend.Data = friends.BattleGuest[i];
            friend.IsFriend = false;
            friendlist.Add(friend);
        }

        var box = FriendContainer.GetComponent<KxVListRender>();
        box.Init(friendlist, "Prefabs/Component/FriendItem", 1034, 522, 1034, 160, OnFriendSelected);
    }

    private void OnFriendSelected(GameObject obj)
    {
        var control = obj.GetComponent<FriendItemControl>();

        MissionModelLocator.Instance.FriendData = control.FriendData;
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

    public override void OnExit()
    {
//        EventManager.Instance.RemoveListener<MissionItemEvent>(OnItemClicktHandler);
        EventManager.Instance.RemoveListener<FriendEvent>(OnFriendHandler);

        if (BtnCloseUIEventListener != null) BtnCloseUIEventListener.onClick -= OnCloseButtonClick;
        if (BtnRewardUIEventListener != null) BtnRewardUIEventListener.onClick -= OnRewardButtonClick;
        
    }

    #endregion

    #region Mono
    void Awake()
    {
        MapContainer = transform.FindChild("Map Container").gameObject;
        MapInfoLabel = transform.FindChild("Map Container/Map Info Label").gameObject;
//        RaidContainer = transform.FindChild("Items Container").gameObject;
        FriendContainer = transform.FindChild("VList Friend").gameObject;
        ItemsContainer = transform.FindChild("VScrollList").gameObject;

        MapEvent = transform.FindChild("Map Container/Event Container").gameObject;

        EnergyLabel = transform.FindChild("Top Bar Container/Energy Label").gameObject;
        LevelLabel = transform.FindChild("Top Bar Container/Level Label").gameObject;
        ExpLabel = transform.FindChild("Top Bar Container/Exp Label").gameObject;
        EnergyProgress = transform.FindChild("Top Bar Container/Energy Progress Bar").gameObject;
        ExpProgress = transform.FindChild("Top Bar Container/Exp Progress Bar").gameObject;

        BtnReturn = transform.FindChild("Image Button return").gameObject;

        EventSprite = transform.FindChild("Map Container/Event Container/Event Sprite").gameObject;
        EventNameLabel = transform.FindChild("Map Container/Event Container/Event Label").gameObject;
        EventTimeLabel = transform.FindChild("Map Container/Event Container/Left Label").gameObject;

        RewardBox = transform.FindChild("Map Container/Image Button box").gameObject;

        BtnCloseUIEventListener = UIEventListener.Get(BtnReturn);
        BtnRewardUIEventListener = UIEventListener.Get(RewardBox);
    }
    // Use this for initialization

    #endregion

//    private MissionItemEvent MissionEvent;
    private void OnItemClicktHandler(GameObject obj)
    {
        var control = obj.GetComponent<MissionItemControl>();
        if (control.IsRaid)
        {
            MissionModelLocator.Instance.Raid = control.RaidData;
            SetStageList(MissionModelLocator.Instance.Raid);
        }
        else
        {
//            if (PlayerModelLocator.Instance.Energy < control.StageTemp.CostEnergy)
//            {
//                PopTextManager.PopTip(LanguageManager.Instance.GetTextValue("Poptip.HaveNotEnoughEnemy"));
//            }
//            else 
            if (MissionModelLocator.Instance.RaidLoadingAll != null && 
                MissionModelLocator.Instance.RaidLoadingAll.TodayFinishTimes != null && 
                MissionModelLocator.Instance.RaidLoadingAll.TodayFinishTimes.ContainsKey(control.StageTemp.Id) &&
                MissionModelLocator.Instance.RaidLoadingAll.TodayFinishTimes[control.StageTemp.Id] >= control.StageTemp.DailyLimitTimes)
            {
                var text = LanguageManager.Instance.GetTextValue("Poptip.Limit");
                PopTextManager.PopTip(text);
            }
            else
            {
                MissionModelLocator.Instance.SelectedStageId = control.StageTemp.Id;
                NetManager.SendMessage(new CSRaidQueryFriend());
            }
        }
    }

    private void OnFriendHandler(FriendEvent e)
    {
        MissionModelLocator.Instance.FriendsMsg = e.RaidFriend;
        SetFriendList(MissionModelLocator.Instance.FriendsMsg);
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

    private void OnRewardButtonClick(GameObject game)
    {
        if (HasReward)
        {
            var csMsg = new CSRaidReceiveAwards();
            csMsg.RaidId = MissionModelLocator.Instance.Raid.TemplateId;
            NetManager.SendMessage(csMsg);
            HasReward = false;
            RewardBox.SetActive(false);
        }
    }
}
