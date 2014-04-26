using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
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
    private GameObject properties;

    private List<Transform> heros = new List<Transform>();

    //This is just for demo.
    private const string TeamPrefix = "∂”ŒÈ";

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        curTeamBtnLis.transform.FindChild("Label").GetComponent<UILabel>().text
            = TeamPrefix + HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex;
        Refresh();
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
        properties = Utils.FindChild(transform, "Properties").gameObject;

        var leaders = Utils.FindChild(transform, "Leaders");
        for (int i = 0; i < leaders.childCount; i++)
        {
            heros.Add(leaders.GetChild(i));
        }
        var members = Utils.FindChild(transform, "1stMembers");
        for (int i = 0; i < members.childCount; i++)
        {
            heros.Add(members.GetChild(i));
        }
        members = Utils.FindChild(transform, "2ndMembers");
        for (int i = 0; i < members.childCount; i++)
        {
            heros.Add(members.GetChild(i));
        }
    }

    private void Refresh()
    {
        var curTeamList =
            HeroModelLocator.Instance.SCHeroList.TeamList[HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex];
        var attack = 0;
        var hp = 0;
        var recover = 0;
        var mp = 0;
        for (int i = 0; i < heros.Count; i++)
        {
            if(i < curTeamList.ListHeroUuid.Count)
            {
                heros[i].gameObject.SetActive(true);
                var uUid = curTeamList.ListHeroUuid[i];
                var heroInfo = HeroModelLocator.Instance.SCHeroList.HeroList.Find(info => info.Uuid == uUid);
                var jobSymobl = Utils.FindChild(heros[i], "JobSymbol").GetComponent<UISprite>();
                var attackLabel = Utils.FindChild(heros[i], "Attack").GetComponent<UILabel>();

                var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
                jobSymobl.spriteName = "icon_zhiye_" + heroTemplate.Job;
                attackLabel.text = heroTemplate.Attack.ToString(CultureInfo.InvariantCulture);
                
                attack += heroTemplate.Attack;
                hp += heroTemplate.HP;
                recover += heroTemplate.Recover;
                mp += heroTemplate.MP;
            }
            else
            {
                heros[i].gameObject.SetActive(false);
            }
        }
        Utils.FindChild(properties.transform, "Attack-Value").GetComponent<UILabel>().text = attack.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties.transform, "HP-Value").GetComponent<UILabel>().text = hp.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties.transform, "Recover-Value").GetComponent<UILabel>().text = recover.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties.transform, "MP-Value").GetComponent<UILabel>().text = mp.ToString(CultureInfo.InvariantCulture);
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
        for (int i = 0; i < teamCount; i++)
        {
            var obj = Instantiate(go) as GameObject;
            obj.transform.parent = teamButtons.transform;
            obj.transform.Find("Label").GetComponent<UILabel>().text = TeamPrefix + i;
            UIEventListener.Get(obj).onClick += OnTeamBtnClicked;
        }
        teamButtons.GetComponent<UIGrid>().Reposition();
        go.SetActive(false);
    }

    private void OnTeamBtnClicked(GameObject go)
    {
        var index = 0;
        for(var i = 0; i < teamButtons.transform.childCount; i++)
        {
            var tran = teamButtons.transform.GetChild(i);
            index = i;
            if(tran == go.transform)
            {
                break;
            }
        }
        var csmsg = new CSHeroChangeTeam { TeamIndex = (sbyte)index };
        NetManager.SendMessage(csmsg);

        HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex = (sbyte) index;
        backBtnLis.gameObject.SetActive(true);
        properties.gameObject.SetActive(true);
        curTeamBtnLis.gameObject.SetActive(true);
        curTeamBtnLis.transform.FindChild("Label").GetComponent<UILabel>().text
            = go.transform.FindChild("Label").GetComponent<UILabel>().text;
        teamButtons.SetActive(false);
        Refresh();
    }

    private void UnInstallHandlers()
    {
        backBtnLis.onClick -= OnBackBtnClicked;
        editBtnLis.onClick -= OnEditBtnClicked;
    }

    private void OnEditBtnClicked(GameObject go)
    {
        
    }

    private void OnBackBtnClicked(GameObject go)
    {
        //Just for demo.
        //WindowManager.Instance.Show(typeof (UITeamBuildWindow), false);
        gameObject.SetActive(false);
    }

    #endregion
}
