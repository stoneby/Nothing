using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using UnityEngine;
using LeaderState = HeroConstant.LeaderState;
using System.Linq;

public class HeroItem : HeroItemBase
{
    public string LeaderSpriteName;
    public string SecondLeaderSpriteName;
    public string ThirdLeaderSpriteName;
    public string MemberSpriteName;
    public string MemberInOtherSpriteName;

    private Transform sortRelatedTran;
    private Transform lockMaskTran;
    private Transform lockedIcon;
    private UISprite leaderStateSprite;

    private LeaderState leaderState;
    public LeaderState LeaderState
    {
        get { return leaderState; }
        set
        {
            leaderState = value;
            switch(leaderState)
            {
                case LeaderState.MainLeader:
                    {
                        leaderStateSprite.spriteName = LeaderSpriteName;
                        break;
                    }

                case LeaderState.SecondLeader:
                    {
                        leaderStateSprite.spriteName = SecondLeaderSpriteName;
                        break;
                    }     
                case LeaderState.ThirdLeader:
                    {
                        leaderStateSprite.spriteName = ThirdLeaderSpriteName;
                        break;
                    }

                case LeaderState.Member:
                    {
                        leaderStateSprite.spriteName = MemberSpriteName;
                        break;
                    } 
                case LeaderState.MemberInOtherTeam:
                    {
                        leaderStateSprite.spriteName = MemberInOtherSpriteName;
                        break;
                    }
            }
            leaderStateSprite.enabled = leaderState != LeaderState.NotInTeam;
            NGUITools.MakePixelPerfect(leaderStateSprite.transform);
        }
    }

    private bool bindState;
    public bool BindState
    {
        get { return bindState; }
        set
        {
            bindState = value;
            lockedIcon.gameObject.SetActive(bindState);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        sortRelatedTran = cachedTran.FindChild("SortRelated");
        lockMaskTran = cachedTran.FindChild("BindMask");
        lockedIcon = cachedTran.FindChild("BindIcon");
        ShowLockMask(false);
        leaderStateSprite = cachedTran.FindChild("LeaderState").GetComponent<UISprite>();
    }


    private void SetLeaderState(HeroInfo heroInfo)
    {
        var heroList = HeroModelLocator.Instance.SCHeroList;
        var curTeamIndex = heroList.CurrentTeamIndex;
        var curTeamUuids = new List<long>();
        var allTeamUuids = new List<long>();
        for (var i = 0; i < heroList.TeamList.Count; i++)
        {
            var uUids = heroList.TeamList[i].ListHeroUuid.Where(id => id != HeroConstant.NoneInitHeroUuid).ToList();
            if (curTeamIndex == i)
            {
                curTeamUuids = uUids.ToList();
            }
            allTeamUuids.AddRange(uUids);
        }
        allTeamUuids = allTeamUuids.Distinct().ToList();
        LeaderState = HeroUtils.GetLeaderState(heroInfo.Uuid, curTeamUuids, allTeamUuids);
    }

    /// <summary>
    /// Show each hero items with the job info.
    /// </summary>
    public void ShowByJob(sbyte job, int atk)
    {
        var jobSymobl = Utils.FindChild(sortRelatedTran, "JobSymbol").GetComponent<UISprite>();
        var attack = Utils.FindChild(sortRelatedTran, "Attack").GetComponent<UILabel>();
        jobSymobl.spriteName = HeroConstant.HeroJobPrefix + job;
        attack.text = atk.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(jobSymobl.gameObject, true);
        NGUITools.SetActive(attack.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the hp info.
    /// </summary>
    public void ShowByHp(int hp)
    {
        var hpTitle = Utils.FindChild(sortRelatedTran, "HP-Title");
        var hpValue = Utils.FindChild(sortRelatedTran, "HP-Value").GetComponent<UILabel>();
        hpValue.text = hp.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(hpTitle.gameObject, true);
        NGUITools.SetActive(hpValue.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the recover info.
    /// </summary>
    public void ShowByRecover(int recover)
    {
        var recoverTitle = Utils.FindChild(sortRelatedTran, "Recover-Title");
        var recoverValue = Utils.FindChild(sortRelatedTran, "Recover-Value").GetComponent<UILabel>();
        recoverValue.text = recover.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(recoverTitle.gameObject, true);
        NGUITools.SetActive(recoverValue.gameObject, true);
    }

    /// <summary>
    /// Show each hero items with the level info.
    /// </summary>
    public void ShowByLvl(short level)
    {
        var lvTitle = Utils.FindChild(sortRelatedTran, "LV-Title");
        var lvValue = Utils.FindChild(sortRelatedTran, "LV-Value").GetComponent<UILabel>();
        lvValue.text = level.ToString(CultureInfo.InvariantCulture);
        NGUITools.SetActiveChildren(sortRelatedTran.gameObject, false);
        NGUITools.SetActive(lvTitle.gameObject, true);
        NGUITools.SetActive(lvValue.gameObject, true);
    }


    public void ShowLockMask(bool show)
    {
        lockMaskTran.gameObject.SetActive(show);
    }

    public override void InitItem(HeroInfo heroInfo, List<long> curTeam, List<long> allTeams)
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        Quality = heroTemplate.Star;
        Uuid = heroInfo.Uuid;
        BindState = heroInfo.Bind;
        LeaderState = HeroUtils.GetLeaderState(Uuid, curTeam, allTeams);
    }
}
