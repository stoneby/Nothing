using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class BattleConfirmWindowWindow : Window
{
    #region Window

    private GameObject BtnBattle;
    private GameObject BtnLeft;
    private GameObject BtnRight;
    private GameObject BtnReturn;

    private GameObject HorContainer;

    private GameObject FriendItem;

    private GameObject LabelAtk;
    private GameObject LabelHp;
    private GameObject LabelRecover;
    private GameObject LabelMp;
    private GameObject LabelLeader;
    private GameObject LabelFriendLeader;

    private UIEventListener BattleUIEventListener;
    private UIEventListener LeftUIEventListener;
    private UIEventListener RightUIEventListener;
    private UIEventListener ReturnUIEventListener;

    public override void OnEnter()
    {

        var friend = FriendItem.GetComponent<AtkHeroItemControl>();
        friend.SetData(MissionModelLocator.Instance.FriendData.Data, MissionModelLocator.Instance.FriendData.IsFriend);
        var hero =
            HeroModelLocator.Instance.GetHeroByTemplateId(
                MissionModelLocator.Instance.FriendData.Data.HeroProp[0].HeroTemplateId);
        var lb = LabelFriendLeader.GetComponent<UILabel>();
        lb.text = HeroModelLocator.Instance.GetLeaderSkillTemplateById(hero.LeaderSkill).Desc;

        var hor = HorContainer.GetComponent<KxListRender>();
        hor.Init(HeroModelLocator.Instance.SCHeroList.TeamList, "Prefabs/Component/AtkHeroGroup", 0, 630, 430, OnSelectedhandler);



        BattleUIEventListener.onClick += OnBattleButtonClick;
        LeftUIEventListener.onClick += OnLeftClick;
        RightUIEventListener.onClick += OnRightClick;
        ReturnUIEventListener.onClick += OnReturnClick;
    }

    public override void OnExit()
    {
        if (BattleUIEventListener != null) BattleUIEventListener.onClick -= OnBattleButtonClick;
        if (LeftUIEventListener != null) LeftUIEventListener.onClick -= OnLeftClick;
        if (RightUIEventListener != null) RightUIEventListener.onClick -= OnRightClick;
        if (ReturnUIEventListener != null) ReturnUIEventListener.onClick -= OnReturnClick;
    }

    #endregion

    #region Mono

    void Awake()
    {
        BtnBattle = transform.FindChild("Image Button battle").gameObject;
        BtnLeft = transform.FindChild("Image Button left").gameObject;
        BtnRight = transform.FindChild("Image Button right").gameObject;
        BtnReturn = transform.FindChild("Image Button return").gameObject;

        FriendItem = transform.FindChild("AtkHeroItem").gameObject;
        HorContainer = transform.FindChild("Swipe Container").gameObject;
        LabelAtk = transform.FindChild("Container labels/Label atk").gameObject;
        LabelHp = transform.FindChild("Container labels/Label hp").gameObject;
        LabelRecover = transform.FindChild("Container labels/Label refresh").gameObject;
        LabelMp = transform.FindChild("Container labels/Label qili").gameObject;
        LabelLeader = transform.FindChild("Container labels/Label leader").gameObject;
        LabelFriendLeader = transform.FindChild("Container labels/Label friend leader").gameObject;

        BattleUIEventListener = UIEventListener.Get(BtnBattle);
        LeftUIEventListener = UIEventListener.Get(BtnLeft);
        RightUIEventListener = UIEventListener.Get(BtnRight);
        ReturnUIEventListener = UIEventListener.Get(BtnReturn);
    }

    // Use this for initialization
    void Start()
    {


    }

    #endregion

    private void OnBattleButtonClick(GameObject game)
    {
        var csMsg = new CSRaidBattleStartMsg();
        csMsg.RaidId = MissionModelLocator.Instance.SelectedStageId;
        csMsg.FriendId = MissionModelLocator.Instance.FriendData.Data.FriendUuid;
        var hor = HorContainer.GetComponent<KxListRender>();
        csMsg.TeamIndex = (sbyte)hor.CurrentIndex;
        NetManager.SendMessage(csMsg);
        MissionModelLocator.Instance.MissionStep = RaidType.StepStageList;
    }

    private void OnLeftClick(GameObject game)
    {
        var hor = HorContainer.GetComponent<KxListRender>();
        hor.MoveLeft();
    }

    private void OnRightClick(GameObject game)
    {
        var hor = HorContainer.GetComponent<KxListRender>();
        hor.MoveRight();
    }

    private void OnReturnClick(GameObject game)
    {
        MissionModelLocator.Instance.MissionStep = RaidType.StepFriendList;
        WindowManager.Instance.Show(typeof(MissionTabWindow), true);
    }

    private void OnSelectedhandler(GameObject obj = null)
    {
        FriendItem.GetComponent<AtkHeroItemControl>();
        HorContainer = transform.FindChild("Swipe Container").gameObject;
        var hor = HorContainer.GetComponent<KxListRender>();
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
                lb = LabelLeader.GetComponent<UILabel>();
                lb.text = HeroModelLocator.Instance.GetLeaderSkillTemplateById(
                    HeroModelLocator.Instance.GetHeroByTemplateId(hero.TemplateId).LeaderSkill).Desc;
            }
            if (hero == null) continue;
            if (hero.Prop.ContainsKey(RoleProperties.HERO_ATK))
            {
                v0 += hero.Prop[RoleProperties.HERO_ATK];
            }
            if (hero.Prop.ContainsKey(RoleProperties.HERO_HP))
            {
                v1 += hero.Prop[RoleProperties.HERO_HP];
            }
            if (hero.Prop.ContainsKey(RoleProperties.HERO_RECOVER))
            {
                v2 += hero.Prop[RoleProperties.HERO_RECOVER];
            }
            if (hero.Prop.ContainsKey(RoleProperties.HERO_MP))
            {
                v3 += hero.Prop[RoleProperties.HERO_MP];
            }
        }

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
