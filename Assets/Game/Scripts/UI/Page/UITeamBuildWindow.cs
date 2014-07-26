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
    private UIEventListener editBtnLis;
    private UIEventListener flipLBtnLis;
    private UIEventListener flipRBtnLis;

    private Transform properties;
    private float heroCellWidth;
    private int curTeamIndex = -1;
    private const int LeaderCount = 3;
    private int teamCount;
    private readonly List<Transform> heros = new List<Transform>();
    private UISprite teamSprite;
    private string teamSpritePrefix;
    private readonly List<Vector3> endlessItemPostions = new List<Vector3>();

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
                    flipRBtnLis.GetComponent<UIButton>().isEnabled = true;
                }
                if(curTeamIndex == teamCount - 1)
                {
                    flipLBtnLis.GetComponent<UIButton>().isEnabled = true;
                }
                if(value == 0)
                {
                    flipRBtnLis.GetComponent<UIButton>().isEnabled = false;
                }
                if (value == teamCount - 1)
                {
                    flipLBtnLis.GetComponent<UIButton>().isEnabled = false;
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
        // Enable finger guester.
        if (FingerGestures.Instance != null)
        {
            FingerGestures.Instance.enabled = true;
        }

        InstallHandlers();
        ResetEndlessItemsPos();
        CurTeamIndex = HeroModelLocator.Instance.SCHeroList.CurrentTeamIndex;
        endlessSwipeEffect.InitCustomData(CurTeamIndex, HeroModelLocator.Instance.SCHeroList.TeamList.Count);
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();

        // Disable finger guester.
        if (FingerGestures.Instance != null)
        {
            FingerGestures.Instance.enabled = false;
        }
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
        endlessSwipeEffect.UpdateData += UpdateData;
        CacheEndlessItemsPos();
    }

    /// <summary>
    /// Cache the local positions of endless swipe and its children.
    /// </summary>
    private void CacheEndlessItemsPos()
    {
        var endlessTran = endlessSwipeEffect.transform;
        endlessItemPostions.Add(endlessTran.localPosition);
        for(int i = 0; i < endlessTran.childCount; i++)
        {
            endlessItemPostions.Add(endlessSwipeEffect.transform.GetChild(i).localPosition);
        }
    }

    /// <summary>
    /// Reset the local positions of endless swipe and its children.
    /// </summary>
    private void ResetEndlessItemsPos()
    {
        //The first cached for endless swipe effect's transform local position, and others for its children.
        endlessSwipeEffect.transform.localPosition = endlessItemPostions[0];
        for (int i = 0; i < endlessSwipeEffect.transform.childCount; i++)
        {
            endlessSwipeEffect.transform.GetChild(i).localPosition = endlessItemPostions[i + 1];
        }
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
            if (index < heroUuids.Count && heroUuids[index] == HeroConstant.NoneInitHeroUuid)
            {
                heros[index].FindChild("Hero").gameObject.SetActive(false);
                continue;
            }
            if (index < heroUuids.Count)
            {
                var heroInfo = HeroModelLocator.Instance.FindHero(heroUuids[index]);
                if (index == HeroConstant.LeaderPosInTeam)
                {
                    leaderInfo = heroInfo;
                }
                //If it is not a leader.
                if (index >= LeaderCount)
                {
                    heros[index].FindChild("Hero").gameObject.SetActive(true);
                }
                heros[index].GetComponent<HeroItemBase>().InitItem(heroInfo);
                attack += heroInfo.Prop[RoleProperties.ROLE_ATK];
                hp += heroInfo.Prop[RoleProperties.ROLE_HP];
                recover += heroInfo.Prop[RoleProperties.ROLE_RECOVER];
                mp += heroInfo.Prop[RoleProperties.ROLE_MP];
            }
        }
        if (heroUuids.Count < HeroConstant.MaxHerosPerTeam)
        {
            for (var index = heroUuids.Count; index < HeroConstant.MaxHerosPerTeam; index++)
            {
                heros[index].FindChild("Hero").gameObject.SetActive(false);
            }
        }

        Utils.FindChild(properties, "Attack-Value").GetComponent<UILabel>().text = attack.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties, "HP-Value").GetComponent<UILabel>().text = hp.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties, "Recover-Value").GetComponent<UILabel>().text = recover.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(properties, "MP-Value").GetComponent<UILabel>().text = mp.ToString(CultureInfo.InvariantCulture);
        var heroTemp = HeroModelLocator.Instance.HeroTemplates.HeroTmpls;
        if(heroTemp.ContainsKey(leaderInfo.TemplateId))
        {
            var leaderTemplate = heroTemp[leaderInfo.TemplateId];
            var skillTmp = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls;
            if(skillTmp.ContainsKey(leaderTemplate.LeaderSkill))
            {
                var leaderSkillTemp = skillTmp[leaderTemplate.LeaderSkill];
                Utils.FindChild(properties, "LSkill-Value").GetComponent<UILabel>().text = leaderSkillTemp.BaseTmpl.Desc;
            }
        }
    }

    /// <summary>
    /// Install all handers for button click.
    /// </summary>
    private void InstallHandlers()
    {
        editBtnLis.onClick = OnEditBtnClicked;
        flipLBtnLis.onClick = OnFlipLClicked;
        flipRBtnLis.onClick = OnFlipRClicked;
    }

    /// <summary>
    /// UnInstall all handers for button click.
    /// </summary>
    private void UnInstallHandlers()
    {
        editBtnLis.onClick = null;
        flipLBtnLis.onClick = null;
        flipRBtnLis.onClick = null;
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

    #endregion
}
