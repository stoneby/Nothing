using System.Collections.Generic;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class SetBattleWindow : Window
{
    //team
    private GameObject BtnLeft;
    private GameObject BtnRight;
    private GameObject ContainerTeam;
    //info datas
    private GameObject LabelAtk;
    private GameObject LabelLeaderSkill;
    private GameObject LabelHp;
    private GameObject LabelFriendSkill;
    private GameObject LabelRecover;
    private GameObject LabelMp;
    //global
    private GameObject LabelTitle;
    private GameObject BtnClose;
    private GameObject BtnBattle;
    private GameObject LabelCount;
    private GameObject LabelEnegry;
    //friend
    private GameObject ContainerSelectFriends;
    private GameObject SelectFriendItem1;
    private GameObject SelectFriendItem2;
    private GameObject SelectFriendItem3;
    private GameObject HListFriends;

    private UIEventListener BattleUIEventListener;
    private UIEventListener LeftUIEventListener;
    private UIEventListener RightUIEventListener;
    private UIEventListener ReturnUIEventListener;

    private bool HaveInit = false;

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.RaidConfirmScreen);

        BattleUIEventListener.onClick += OnBattleButtonClick;
        LeftUIEventListener.onClick += OnLeftClick;
        RightUIEventListener.onClick += OnRightClick;
        ReturnUIEventListener.onClick += OnReturnClick;

//        var cma = Camera.main.GetComponent<UICamera>();
//        if (cma != null)
//        {
//            cma.eventType = UICamera.EventType.UI;
//        }
        
        SetContent();
    }

    public override void OnExit()
    {
        if (BattleUIEventListener != null) BattleUIEventListener.onClick -= OnBattleButtonClick;
        if (LeftUIEventListener != null) LeftUIEventListener.onClick -= OnLeftClick;
        if (RightUIEventListener != null) RightUIEventListener.onClick -= OnRightClick;
        if (ReturnUIEventListener != null) ReturnUIEventListener.onClick -= OnReturnClick;
        MtaManager.TrackEndPage(MtaType.RaidConfirmScreen);
    }

    private void SetContent()
    {
        if (!HaveInit) return;

        var hor = ContainerTeam.GetComponent<KxListRender>();
        hor.Init(HeroModelLocator.Instance.SCHeroList.TeamList, "Prefabs/Component/AtkHeroGroup",
            TeamMemberManager.Instance.CurTeamIndex, 750, 260, OnSelectedhandler);
        //hor.CurrentIndex = 3;
        //

        SetFriendList(MissionModelLocator.Instance.FriendsMsg);

        var lb = LabelTitle.GetComponent<UILabel>();
        lb.text = MissionModelLocator.Instance.BattleRaidTemplate.RaidName + "-" + MissionModelLocator.Instance.BattleStageTemplate.StageName;

        lb = LabelEnegry.GetComponent<UILabel>();
        lb.text = MissionModelLocator.Instance.BattleStageTemplate.CostEnergy.ToString();

        lb = LabelCount.GetComponent<UILabel>();
        //UIRaid.Count
        lb.text = LanguageManager.Instance.GetTextValue("UIRaid.Count") + ":" + 
            MissionModelLocator.Instance.GetStageFinishTimeByTemplateId(MissionModelLocator.Instance.BattleStageTemplate.Id)
                        + "/" + MissionModelLocator.Instance.BattleStageTemplate.DailyLimitTimes;
    }

    private void SetSelectFriendHeros(FriendVO thedata)
    {
        MissionModelLocator.Instance.FriendData = thedata;
        SetSelectFriendItem(SelectFriendItem1, 0, 2);
        SetSelectFriendItem(SelectFriendItem2, 1, 3);
        SetSelectFriendItem(SelectFriendItem3, 2, 3);

        if (MissionModelLocator.Instance.FriendData != null &&
            MissionModelLocator.Instance.FriendData.Data.HeroProp.Count > 0)
        {
            var hero = HeroModelLocator.Instance.GetHeroByTemplateId(
                MissionModelLocator.Instance.FriendData.Data.HeroProp[0].HeroTemplateId);
            var lb = LabelFriendSkill.GetComponent<UILabel>();
            lb.text = (hero.LeaderSkill == 0) ? "" :HeroModelLocator.Instance.GetLeaderSkillTemplateById(hero.LeaderSkill).Desc;
            OnSelectedhandler();
        }
    }

    private void SetSelectFriendItem(GameObject theobj, int theindex, int thetype)
    {
        var item = theobj.GetComponent<RaidHeadControl>();
        if (MissionModelLocator.Instance.FriendData != null && MissionModelLocator.Instance.FriendData.Data.HeroProp.Count > theindex)
        {
            item.SetFriendData(MissionModelLocator.Instance.FriendData.Data.HeroProp[theindex], thetype);
        }
        else
        {
            item.SetFriendData(null, thetype);
        }
    }

    #endregion

    #region Mono

    void Awake()
    {
        BtnBattle = transform.FindChild("Image Button battle").gameObject;
        BtnLeft = transform.FindChild("Sprite team/Image Button left").gameObject;
        BtnRight = transform.FindChild("Sprite team/Image Button right").gameObject;
        BtnClose = transform.FindChild("Image Button close").gameObject;
        ContainerTeam = transform.FindChild("Swipe Container").gameObject;
        LabelTitle = transform.FindChild("Label title").gameObject;

        LabelAtk = transform.FindChild("Container labels/Label atk").gameObject;
        LabelHp = transform.FindChild("Container labels/Label hp").gameObject;
        LabelRecover = transform.FindChild("Container labels/Label refresh").gameObject;
        LabelMp = transform.FindChild("Container labels/Label qili").gameObject;
        LabelLeaderSkill = transform.FindChild("Container labels/Label leader").gameObject;
        LabelFriendSkill = transform.FindChild("Container labels/Label friend leader").gameObject;

        LabelEnegry = transform.FindChild("Image Button battle/Container active/Label").gameObject;
        LabelCount = transform.FindChild("Label count").gameObject;

        ContainerSelectFriends = transform.FindChild("Sprite team/Container select friends").gameObject;
        SelectFriendItem1 = transform.FindChild("Sprite team/Container select friends/RaidHeadItem1").gameObject;
        SelectFriendItem2 = transform.FindChild("Sprite team/Container select friends/RaidHeadItem2").gameObject;
        SelectFriendItem3 = transform.FindChild("Sprite team/Container select friends/RaidHeadItem3").gameObject;
        HListFriends = transform.FindChild("HList").gameObject;

        BattleUIEventListener = UIEventListener.Get(BtnBattle);
        LeftUIEventListener = UIEventListener.Get(BtnLeft);
        RightUIEventListener = UIEventListener.Get(BtnRight);
        ReturnUIEventListener = UIEventListener.Get(BtnClose);

        HaveInit = true;
        SetContent();
    }

    private void SetFriendList(SCRaidQueryFriend friends)
    {
        var friendlist = new List<FriendVO>();

        if (friends != null && friends.BattleFriend != null)
        {
            for (int i = 0; i < friends.BattleFriend.Count; i++)
            {
                var friend = new FriendVO();
                friend.Data = friends.BattleFriend[i];
                friend.IsFriend = true;
                friendlist.Add(friend);
            }
        }

        if (friends != null && friends.BattleGuest != null)
        {
            for (int i = 0; i < friends.BattleGuest.Count; i++)
            {
                var friend = new FriendVO();
                friend.Data = friends.BattleGuest[i];
                friend.IsFriend = false;
                friendlist.Add(friend);
            }
        }

        var box = HListFriends.GetComponent<KxHListRender>();
        box.Init(friendlist, "Prefabs/Component/RaidFriendItem", 1024, 170, 130, 150, OnFriendSelected);

        if (friendlist.Count > 0)
        {
            SetSelectFriendHeros(friendlist[0]);
        }
        else
        {
            SetSelectFriendHeros(null);
        }
    }

    private void OnFriendSelected(GameObject obj)
    {
        var item = obj.GetComponent<FriendHeadControl>();
        SetSelectFriendHeros(item.FriendData);
    }


    #endregion

    private void OnBattleButtonClick(GameObject game)
    {
        var csMsg = new CSRaidBattleStartMsg
        {
            RaidId = MissionModelLocator.Instance.BattleStageTemplate.Id,
            FriendId = MissionModelLocator.Instance.FriendData.Data.FriendUuid
        };
        var hor = ContainerTeam.GetComponent<KxListRender>();
        csMsg.TeamIndex = (sbyte)hor.CurrentIndex;
        NetManager.SendMessage(csMsg);
        MissionModelLocator.Instance.MissionStep = RaidType.StepStageList;
        MissionModelLocator.Instance.ShowAddFriendAlert = false;
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("raid", MissionModelLocator.Instance.BattleStageTemplate.Id.ToString());
        dict.Add("friend", MissionModelLocator.Instance.FriendData.Data.FriendUuid.ToString());
        MtaManager.TrackCustomKVEvent(MtaType.VKEventBattle, dict);
    }

    private void OnLeftClick(GameObject game)
    {
        var hor = ContainerTeam.GetComponent<KxListRender>();
        hor.MoveLeft();
    }

    private void OnRightClick(GameObject game)
    {
        var hor = ContainerTeam.GetComponent<KxListRender>();
        hor.MoveRight();
    }

    private void OnReturnClick(GameObject game)
    {
        WindowManager.Instance.Show(typeof(SetBattleWindow), false);
        WindowManager.Instance.Show(typeof(RaidsWindow), true);
//        var cma = Camera.main.GetComponent<UICamera>();
//        if (cma != null)
//        {
//            cma.eventType = UICamera.EventType.Unity2D;
//        }
    }

    private void OnSelectedhandler(GameObject obj = null)
    {
        //FriendItem.GetComponent<AtkHeroItemControl>();
        //ContainerTeam = transform.FindChild("Swipe Container").gameObject;
        var hor = ContainerTeam.GetComponent<KxListRender>();
        var team = HeroModelLocator.Instance.SCHeroList.TeamList[hor.CurrentIndex];
        int v0 = 0;
        int v1 = 0;
        int v2 = 0;
        int v3 = 0;

        UILabel lb;

        for (int i = 0; i < team.ListHeroUuid.Count; i++)
        {
            HeroInfo hero = HeroModelLocator.Instance.FindHero(team.ListHeroUuid[i]);
            if (i == 0)
            {
                lb = LabelLeaderSkill.GetComponent<UILabel>();
                var temp = HeroModelLocator.Instance.GetHeroByTemplateId(hero.TemplateId);
                lb.text = (temp == null || temp.LeaderSkill == 0) ? "" : HeroModelLocator.Instance.GetLeaderSkillTemplateById(temp.LeaderSkill).Desc;
            }
            if (hero == null) continue;
            if (hero.Prop.ContainsKey(RoleProperties.ROLE_ATK))
            {
                v0 += hero.Prop[RoleProperties.ROLE_ATK];
            }
            if (hero.Prop.ContainsKey(RoleProperties.ROLE_HP))
            {
                v1 += hero.Prop[RoleProperties.ROLE_HP];
            }
            if (hero.Prop.ContainsKey(RoleProperties.ROLE_RECOVER))
            {
                v2 += hero.Prop[RoleProperties.ROLE_RECOVER];
            }
            if (hero.Prop.ContainsKey(RoleProperties.ROLE_MP))
            {
                v3 += hero.Prop[RoleProperties.ROLE_MP];
            }
        }

        var select = SelectFriendItem1.GetComponent<RaidHeadControl>();
        v0 += select.Attrack;
        v1 += select.Hp;
        v2 += select.Recover;
        v3 += select.Mp;
        select = SelectFriendItem2.GetComponent<RaidHeadControl>();
        v0 += select.Attrack;
        v1 += select.Hp;
        v2 += select.Recover;
        v3 += select.Mp;
        select = SelectFriendItem3.GetComponent<RaidHeadControl>();
        v0 += select.Attrack;
        v1 += select.Hp;
        v2 += select.Recover;
        v3 += select.Mp;

        lb = LabelAtk.GetComponent<UILabel>();
        lb.text = v0.ToString();

        lb = LabelHp.GetComponent<UILabel>();
        lb.text = v1.ToString();

        lb = LabelRecover.GetComponent<UILabel>();
        lb.text = v2.ToString();

        lb = LabelMp.GetComponent<UILabel>();
        lb.text = v3.ToString();
    }
}
