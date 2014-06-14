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

    private EndlessSwipeEffect endlessSwipeEffect;
    private Vector3 endlessPosCached;
    private UIEventListener editBtnLis;
    private UIEventListener flipLBtnLis;
    private UIEventListener flipRBtnLis;

    private Transform properties;
    private float heroCellWidth;
    private int curTeamIndex = -1;
    private int teamCount;
    private readonly List<Transform> heros = new List<Transform>();
    private UISprite teamSprite;
    private string teamSpritePrefix;

    #endregion

    #region Public Fields

    /// <summary>
    /// The index of current team. Whenever the value changed we will update the state of flip buttons and team icon.
    /// </summary>
    public int CurTeamIndex
    {
        get { return curTeamIndex; }
        private set
        {
            if (curTeamIndex != value)
            {
                if (value < 0 || value > teamCount - 1)
                {
                    return;
                }
                if (curTeamIndex == 0)
                {
                    EnableButton(flipRBtnLis.GetComponent<UISprite>(), true);
                }
                if(curTeamIndex == teamCount - 1)
                {
                    EnableButton(flipLBtnLis.GetComponent<UISprite>(), true);
                }
                if(value == 0)
                {
                    EnableButton(flipRBtnLis.GetComponent<UISprite>(), false);
                }
                if (value == teamCount - 1)
                {
                    EnableButton(flipLBtnLis.GetComponent<UISprite>(), false);
                }
                curTeamIndex = value;
                HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex = (sbyte)curTeamIndex;
                teamSprite.spriteName = teamSpritePrefix + curTeamIndex;
                teamSprite.MakePixelPerfect();
            }
        }
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        CurTeamIndex = HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex;
        endlessSwipeEffect.InitCustomData(CurTeamIndex, HeroModelLocator.Instance.SCHeroList.TeamList.Count);
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        endlessSwipeEffect.transform.position = endlessPosCached;
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        editBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Edit").gameObject);
        flipLBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-FlipL").gameObject);
        flipRBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-FlipR").gameObject);
        properties = Utils.FindChild(transform, "Properties");
        teamCount = HeroModelLocator.Instance.SCHeroList.TeamList.Count;
        teamSprite = Utils.FindChild(transform, "TeamValue").GetComponent<UISprite>();
        teamSpritePrefix = teamSprite.spriteName.Remove(teamSprite.spriteName.Length - 1);
        endlessSwipeEffect = GetComponentInChildren<EndlessSwipeEffect>();
        endlessPosCached = endlessSwipeEffect.transform.position;
        endlessSwipeEffect.UpdateData += UpdateData;
    }

    private void UpdateData()
    {
        CurTeamIndex = endlessSwipeEffect.CurCustomIndex;
        Refresh();
    }

    /// <summary>
    /// Update the heros data.
    /// </summary>
    private void UpdateHeros()
    {
        heros.Clear();
        var centeredObject = endlessSwipeEffect.CurrentItem;
        var leaders = Utils.FindChild(centeredObject, "Leaders");
        for (int index = 0; index < leaders.childCount; index++)
        {
            heros.Add(leaders.GetChild(index));
        }
        var members = Utils.FindChild(centeredObject, "Members");
        for (int index = 0; index < members.childCount; index++)
        {
            heros.Add(members.GetChild(index));
        }
    }

    /// <summary>
    /// Update the window ui.
    /// </summary>
    public void Refresh()
    {
        var heroUuids = HeroModelLocator.Instance.SCHeroList.TeamList[CurTeamIndex].ListHeroUuid;
        UpdateHeros();
        var attack = 0;
        var hp = 0;
        var recover = 0;
        var mp = 0;
        var leaderInfo = new HeroInfo();
        for (int index = 0; index < heros.Count; index++)
        {
            if (index < heroUuids.Count && heroUuids[index] == UITeamEditWindow.DefaultNonHero)
            {
                heros[index].FindChild("Hero").gameObject.SetActive(false);
                continue;
            }
            if (index < heroUuids.Count)
            {
                var heroInfo = HeroModelLocator.Instance.FindHero(heroUuids[index]);
                if (index == 0)
                {
                    leaderInfo = heroInfo;
                }
                //If it is not a leader.
                if (index >= 3)
                {
                    heros[index].FindChild("Hero").gameObject.SetActive(true);
                }
                heros[index].GetComponent<HeroItemBase>().InitItem(heroInfo);
                attack += heroInfo.Prop[RoleProperties.HERO_ATK];
                hp += heroInfo.Prop[RoleProperties.HERO_HP];
                recover += heroInfo.Prop[RoleProperties.HERO_RECOVER];
                mp += heroInfo.Prop[RoleProperties.HERO_MP];
            }
        }
        if (heroUuids.Count < UITeamEditWindow.MaxHeroCount)
        {
            for (var index = heroUuids.Count; index < UITeamEditWindow.MaxHeroCount; index++)
            {
                heros[index].FindChild("Hero").gameObject.SetActive(false);
            }
        }

        Utils.FindChild(properties, "Attack-Value").GetComponent<UILabel>().text = attack.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties, "HP-Value").GetComponent<UILabel>().text = hp.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties, "Recover-Value").GetComponent<UILabel>().text = recover.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties, "MP-Value").GetComponent<UILabel>().text = mp.ToString(CultureInfo.InvariantCulture);
        var leaderTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[leaderInfo.TemplateId];
        var skillTmp = HeroModelLocator.Instance.SkillTemplates.SkillTmpl;
        var leaderSkillTemp = skillTmp[leaderTemplate.LeaderSkill];
        Utils.FindChild(properties, "LSkill-Value").GetComponent<UILabel>().text = leaderSkillTemp.Desc;
    }

    /// <summary>
    /// Install all handers for button click.
    /// </summary>
    private void InstallHandlers()
    {
        editBtnLis.onClick += OnEditBtnClicked;
        flipLBtnLis.onClick += OnFlipLClicked;
        flipRBtnLis.onClick += OnFlipRClicked;
    }

    /// <summary>
    /// UnInstall all handers for button click.
    /// </summary>
    private void UnInstallHandlers()
    {
        editBtnLis.onClick -= OnEditBtnClicked;
        flipLBtnLis.onClick -= OnFlipLClicked;
        flipRBtnLis.onClick -= OnFlipRClicked;
    }

    /// <summary>
    /// The callback of clicking flip left button.
    /// </summary>
    private void OnFlipRClicked(GameObject go)
    {
        endlessSwipeEffect.ExcueSwipe(false);
    }

    /// <summary>
    /// The callback of clicking flip right button.
    /// </summary>
    private void OnFlipLClicked(GameObject go)
    {
        endlessSwipeEffect.ExcueSwipe(true);
    }

    private void OnEditBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof(UITeamEditWindow), true);
    }

    /// <summary>
    /// Enable a button to be clickable or not.
    /// </summary>
    private void EnableButton(UISprite sprite, bool enable)
    {
        var spCollider = sprite.GetComponent<BoxCollider>();
        if (spCollider == null)
        {
            Logger.LogWarning("The button to be set enable has no collider attached.");
            return;
        }
        spCollider.enabled = enable;
        sprite.color = enable ? Color.white : Color.grey;
    }

    #endregion
}
