using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Hero team build window controller.
/// </summary>
public class UITeamBuildWindow : Window
{
    #region private Fields

    private UIEventListener backBtnLis;
    private UIEventListener editBtnLis;
    private UIEventListener curTeamBtnLis;

    private GameObject teamButtons;
    private Transform properties;
    private Transform firstSkill;
    private Transform secondSkill;
    private Transform thirdSkill;

    private readonly List<Transform> heros = new List<Transform>();

    //This is just for demo.
    private const string TeamPrefix = "∂”ŒÈ";

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        var curTeamIndex = HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex;
        var curTeam = HeroModelLocator.Instance.SCHeroList.TeamList[curTeamIndex];
        Refresh(curTeamIndex, curTeam.ListHeroUuid);
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        editBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Edit").gameObject);
        curTeamBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-CurTeam").gameObject);

        teamButtons = Utils.FindChild(transform, "TeamButtons").gameObject;
        properties = Utils.FindChild(transform, "Properties");
        var leaderSkill = Utils.FindChild(transform, "LeaderSkills");
        firstSkill = leaderSkill.FindChild("1st-Skill");
        secondSkill = leaderSkill.FindChild("2nd-Skill");
        thirdSkill = leaderSkill.FindChild("3th-Skill");

        var leaders = Utils.FindChild(transform, "Leaders");
        for (int index = 0; index < leaders.childCount; index++)
        {
            heros.Add(leaders.GetChild(index));
        }
        var members = Utils.FindChild(transform, "Members");
        for (int index = 0; index < members.childCount; index++)
        {
            heros.Add(members.GetChild(index));
        }
    }

    public void Refresh(int currentTeamIndex, List<long> heroUuids)
    {
        curTeamBtnLis.transform.FindChild("Label").GetComponent<UILabel>().text =
                                    TeamPrefix + currentTeamIndex;
        var attack = 0;
        var hp = 0;
        var recover = 0;
        var mp = 0;
        for (int index = 0; index < heros.Count; index++)
        {
            if (index < heroUuids.Count  && heroUuids[index] == UITeamEditWindow.DefaultNonHero)
            {
                heros[index].gameObject.SetActive(false);
                continue;
            }
            if (index < heroUuids.Count)
            {
                heros[index].gameObject.SetActive(true);
                heros[index].GetComponent<HeroInfoPack>().Uuid = heroUuids[index];
                var heroInfo = HeroModelLocator.Instance.FindHero(heroUuids[index]);
                var jobSymobl = Utils.FindChild(heros[index], "JobSymbol").GetComponent<UISprite>();
                var attackLabel = Utils.FindChild(heros[index], "Attack").GetComponent<UILabel>();

                var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
                jobSymobl.spriteName = "icon_zhiye_" + heroTemplate.Job;
                attackLabel.text = heroInfo.Prop[RoleProperties.HERO_ATK].ToString(CultureInfo.InvariantCulture);
                
                attack += heroTemplate.Attack;
                hp += heroTemplate.HP;
                recover += heroTemplate.Recover;
                mp += heroTemplate.MP;
            }
        }
        if (heroUuids.Count < UITeamEditWindow.MaxHeroCount)
        {
            for (int index = heroUuids.Count; index < UITeamEditWindow.MaxHeroCount; index++)
            {
                heros[index].gameObject.SetActive(false);
            }
        }
        var firstInfo = HeroModelLocator.Instance.FindHero(heroUuids[0]);
        var secondInfo = HeroModelLocator.Instance.FindHero(heroUuids[1]);
        var thirdInfo = HeroModelLocator.Instance.FindHero(heroUuids[2]);
        RefreshBaseInfo(firstSkill, firstInfo);
        RefreshBaseInfo(secondSkill, secondInfo);
        RefreshBaseInfo(thirdSkill, thirdInfo);

        Utils.FindChild(properties, "Attack-Value").GetComponent<UILabel>().text = attack.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties, "HP-Value").GetComponent<UILabel>().text = hp.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties, "Recover-Value").GetComponent<UILabel>().text = recover.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties, "MP-Value").GetComponent<UILabel>().text = mp.ToString(CultureInfo.InvariantCulture);
    }

    private void RefreshBaseInfo(Transform parent, HeroInfo heroInfo)
    {
        Utils.FindChild(parent, "Attack-Value").GetComponent<UILabel>().text = heroInfo.Prop[RoleProperties.HERO_ATK].ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(parent, "HP-Value").GetComponent<UILabel>().text = heroInfo.Prop[RoleProperties.HERO_HP].ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(parent, "Recover-Value").GetComponent<UILabel>().text = heroInfo.Prop[RoleProperties.HERO_RECOVER].ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(parent, "MP-Value").GetComponent<UILabel>().text = heroInfo.Prop[RoleProperties.HERO_MP].ToString(CultureInfo.InvariantCulture);
        
        var activeSkill = Utils.FindChild(parent, "Active-Skill");
        var leaderSkill = Utils.FindChild(parent, "Leader-Skill");
        if (activeSkill || leaderSkill)
        {
            var skillTmp = HeroModelLocator.Instance.SkillTemplates.SkillTmpl;
            var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
            var leaderSkillTemp = skillTmp[heroTemplate.LeaderSkill];
            var activeSkillTemp = skillTmp[heroTemplate.ActiveSkill];
            if(leaderSkill)
            {
                Utils.FindChild(leaderSkill, "Leader-Value").GetComponent<UILabel>().text = leaderSkillTemp.Desc;
            }
            if(activeSkill)
            {
                Utils.FindChild(activeSkill, "Active-Value").GetComponent<UILabel>().text = activeSkillTemp.Desc;
            }
        }
    }

    private void InstallHandlers()
    {
        backBtnLis.onClick += OnBackBtnClicked;
        editBtnLis.onClick += OnEditBtnClicked;
        curTeamBtnLis.onClick += OnCurTeamBtClicked;
    }

    private void OnCurTeamBtClicked(GameObject go)
    {
        teamButtons.SetActive(true);
        backBtnLis.gameObject.SetActive(false);
        properties.gameObject.SetActive(false);

        //This is just for demo.
        if(teamButtons.transform.childCount > 0)
        {
            go.SetActive(false);
            return;
        }
        var teamCount = HeroModelLocator.Instance.SCHeroList.TeamList.Count;
        for (int index = 0; index < teamCount; index++)
        {
            var obj = NGUITools.AddChild(teamButtons, go);
            obj.transform.Find("Label").GetComponent<UILabel>().text = TeamPrefix + index;
            UIEventListener.Get(obj).onClick += OnTeamBtnClicked;
        }
        teamButtons.GetComponent<UIGrid>().Reposition();
        go.SetActive(false);
    }

    private void OnTeamBtnClicked(GameObject go)
    {
        var index = 0;
        for(var buttonIndex = 0; buttonIndex < teamButtons.transform.childCount; buttonIndex++)
        {
            var tran = teamButtons.transform.GetChild(buttonIndex);
            index = buttonIndex;
            if(tran == go.transform)
            {
                break;
            }
        }
        backBtnLis.gameObject.SetActive(true);
        properties.gameObject.SetActive(true);
        curTeamBtnLis.gameObject.SetActive(true);
        curTeamBtnLis.transform.FindChild("Label").GetComponent<UILabel>().text
            = go.transform.FindChild("Label").GetComponent<UILabel>().text;
        teamButtons.SetActive(false);
        HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex = (sbyte)index;
        var uUids = HeroModelLocator.Instance.SCHeroList.TeamList[index].ListHeroUuid;
        Refresh(index, uUids);
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick -= OnBackBtnClicked;
        editBtnLis.onClick -= OnEditBtnClicked;
    }

    private void OnEditBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof(UITeamEditWindow), true);
    }

    private void OnBackBtnClicked(GameObject go)
    {
        //Just for demo.
        WindowManager.Instance.Show(typeof(UIHeroItemsPageWindow), true);
        var csmsg = new CSHeroChangeTeam { TeamIndex = HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex };
        NetManager.SendMessage(csmsg);
    }

    #endregion
}
