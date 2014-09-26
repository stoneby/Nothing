using System.Collections.Generic;
using KXSGCodec;
using Property;
using Template.Auto.Skill;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIFriendDetailWindow : Window
{
    #region Private Fields

    private UIEventListener closeLis;
    private UILabel atkLabel;
    private UILabel hpLabel;
    private UILabel recoverLabel;
    private UILabel mpLabel;
    private UILabel leaderSkName;
    private UILabel leaderSkDesc;

    private readonly List<UISprite> leaderJobs = new List<UISprite>();
    private readonly List<UISprite> leaderIcons = new List<UISprite>();
    private readonly List<UILabel> leaderAtks = new List<UILabel>();

    #endregion

    #region Private Property

    private int Atk
    {
        set
        {
            atkLabel.text = value.ToString();
        }
    }
    
    private int Hp
    {
        set
        {
            hpLabel.text = value.ToString();
        }
    }
    
    private int Recover
    {
        set
        {
            recoverLabel.text = value.ToString();
        }
    }
    
    private int Mp
    {
        set
        {
            mpLabel.text = value.ToString();
        }
    }

    private FriendInfo FriendInfo
    {
        set
        {
            if(value != null)
            {
                var atkValue = FriendUtils.GetProp(value, RoleProperties.ROLE_ATK);
                var hpValue = FriendUtils.GetProp(value, RoleProperties.ROLE_HP);
                var recoverValue = FriendUtils.GetProp(value, RoleProperties.ROLE_RECOVER);
                var mpValue = FriendUtils.GetProp(value, RoleProperties.ROLE_MP);
                SetPropertyValue(atkValue, hpValue, recoverValue, mpValue);
                var leaderTempId = value.HeroProp[0].HeroTemplateId;
                var leaderSkillId = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[leaderTempId].LeaderSkill;
                var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;
                if (skillTmp.ContainsKey(leaderSkillId))
                {
                    var leaderSkill = skillTmp[leaderSkillId];
                    leaderSkName.text = leaderSkill.Name;
                    leaderSkDesc.text = leaderSkill.Desc;
                }
                RefreshLeaders(value);
            }
        }
    }

    #endregion

    #region Public Methods

    public void Init(FriendInfo info)
    {
        FriendInfo = info;
    }

    #endregion

    #region Private Methods

    private void RefreshLeaders(FriendInfo info)
    {
        var heroTemps = HeroModelLocator.Instance.HeroTemplates.HeroTmpls;
        for(int i = 0; i < info.HeroProp.Count; i++)
        {
            var heroTempId = info.HeroProp[i].HeroTemplateId;
            HeroConstant.SetHeadByTemplate(leaderIcons[i], heroTempId);
            leaderJobs[i].spriteName = HeroConstant.HeroJobPrefix + heroTemps[heroTempId].Job;
            leaderAtks[i].text = info.HeroProp[i].Prop[RoleProperties.ROLE_ATK].ToString();
        }
    }

    private void SetPropertyValue(int atkValue, int hpValue, int recoverValue, int mpValue)
    {
        Atk = atkValue;
        Hp = hpValue;
        Recover = recoverValue;
        Mp = mpValue;
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        closeLis.onClick = OnClose;
    }

    public override void OnExit()
    {
        closeLis.onClick = null;
    }

    #endregion

    #region Mono

    // Use this for initialization
    private void Awake()
    {
        closeLis = UIEventListener.Get(transform.Find("Dimmer").gameObject);
        var property = transform.Find("Property");
        atkLabel = property.Find("Attack/Value").GetComponent<UILabel>();
        hpLabel = property.Find("Hp/Value").GetComponent<UILabel>();
        recoverLabel = property.Find("Recover/Value").GetComponent<UILabel>();
        mpLabel = property.Find("Mp/Value").GetComponent<UILabel>();
        leaderSkName = transform.Find("Skill/Value").GetComponent<UILabel>();
        leaderSkDesc = Utils.FindChild(transform, "Desc").GetComponent<UILabel>();

        leaderAtks.Add(transform.Find("Leader1/Attack/Value").GetComponent<UILabel>());
        leaderAtks.Add(transform.Find("Leader2/Attack/Value").GetComponent<UILabel>());
        leaderAtks.Add(transform.Find("Leader3/Attack/Value").GetComponent<UILabel>());

        leaderJobs.Add(transform.Find("Leader1/JobBG/JobIcon").GetComponent<UISprite>());
        leaderJobs.Add(transform.Find("Leader2/JobBG/JobIcon").GetComponent<UISprite>());
        leaderJobs.Add(transform.Find("Leader3/JobBG/JobIcon").GetComponent<UISprite>()); 
        
        leaderIcons.Add(transform.Find("Leader1/HeroIcon").GetComponent<UISprite>());
        leaderIcons.Add(transform.Find("Leader2/HeroIcon").GetComponent<UISprite>());
        leaderIcons.Add(transform.Find("Leader3/HeroIcon").GetComponent<UISprite>());
    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<UIFriendDetailWindow>(false);
    }

    #endregion
}
