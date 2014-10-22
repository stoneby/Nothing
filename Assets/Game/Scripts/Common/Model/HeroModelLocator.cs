using KXSGCodec;
using Property;
using System;
using System.Collections.Generic;
using System.Linq;
using Template.Auto.Hero;
using Template.Auto.Skill;
using Object = UnityEngine.Object;
using OrderType = ItemHelper.OrderType;

public sealed class HeroModelLocator
{
    #region Private Field

    private static volatile HeroModelLocator instance;
    private static readonly object SyncRoot = new Object();

    #endregion

    #region Public Fields

    public static HeroModelLocator Instance
    {
        get
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                    {
                        instance = new HeroModelLocator();
                    }
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Order type
    /// </summary>
    public OrderType OrderType;

    public int GetHeroPos;
    public SCHeroList SCHeroList;

    private Hero heroTemplates;
    public Hero HeroTemplates
    {
        get { return heroTemplates ?? (heroTemplates = Utils.Decode<Hero>(ResourcePath.FileHero)); }
    }

    private Skill skillTemplates;
    public Skill SkillTemplates
    {
        get { return skillTemplates ?? (skillTemplates = Utils.Decode<Skill>(ResourcePath.FileSkill)); }
    }

    public static bool AlreadyRequest;

    #endregion

    #region Private Methods

    /// <summary>
    /// The comparation of hero info by time.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareHeroByTime(HeroInfo p1, HeroInfo p2)
    {
        return p2.Uuid.CompareTo(p1.Uuid);
    }

    /// <summary>
    /// The comparation of hero info by job.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareHeroByJob(HeroInfo p1, HeroInfo p2)
    {
        var heroTemp = HeroTemplates.HeroTmpls;
        int compareResult = heroTemp[p1.TemplateId].Job.CompareTo(heroTemp[p2.TemplateId].Job);
        if (compareResult == 0)
        {
            return CompareHeroByTemplateId(p1, p2);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by rarity.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareHeroByRarity(HeroInfo p1, HeroInfo p2)
    {
        var heroTemp = HeroTemplates.HeroTmpls;
        int compareResult = heroTemp[p2.TemplateId].Star.CompareTo(heroTemp[p1.TemplateId].Star);
        if (compareResult == 0)
        {
            return CompareHeroByTemplateId(p1, p2);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by template id.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    private int CompareHeroByTemplateId(HeroInfo p1, HeroInfo p2)
    {
        return p2.TemplateId.CompareTo(p1.TemplateId);
    }

    /// <summary>
    /// The comparation of hero info by attack.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareHeroByAttack(HeroInfo p1, HeroInfo p2)
    {
        int compareResult = p2.Prop[RoleProperties.ROLE_ATK].CompareTo(p1.Prop[RoleProperties.ROLE_ATK]);
        if (compareResult == 0)
        {
            return CompareHeroByTemplateId(p1, p2);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by hp.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareHeroByHp(HeroInfo p1, HeroInfo p2)
    {
        int compareResult = p2.Prop[RoleProperties.ROLE_HP].CompareTo(p1.Prop[RoleProperties.ROLE_HP]);
        if (compareResult == 0)
        {
            return CompareHeroByTemplateId(p1, p2);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by recover.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareHeroByRecover(HeroInfo p1, HeroInfo p2)
    {
        int compareResult = p2.Prop[RoleProperties.ROLE_RECOVER].CompareTo(p1.Prop[RoleProperties.ROLE_RECOVER]);
        if (compareResult == 0)
        {
            return CompareHeroByTemplateId(p1, p2);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by level.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareHeroByLv(HeroInfo p1, HeroInfo p2)
    {
        int compareResult = p2.Lvl.CompareTo(p1.Lvl);
        if (compareResult == 0)
        {
            return CompareHeroByTemplateId(p1, p2);
        }
        return compareResult;
    }

    /// <summary>
    ///  Sort hero infos by team
    /// </summary>
    /// <param name="infos">The hero infos to be sorted.</param>
    private void SortHeroByTeam(List<HeroInfo> infos)
    {
        var sortedInfos = new List<HeroInfo>();
        var priorityUuids = GetTeamPriorityUuids().ToList();
        for (var i = 0; i < priorityUuids.Count; i++)
        {
            var priorityUuid = priorityUuids[i];
            for (var j = 0; j < infos.Count; j++)
            {
                var info = infos[j];
                if (info.Uuid == priorityUuid)
                {
                    sortedInfos.Add(info);
                    break;
                }
            }
        }
        var nonPriorityInfos = infos.Where(info => !priorityUuids.Contains(info.Uuid)).ToList();
        nonPriorityInfos.Sort(CompareHeroByTemplateId);
        infos.Clear();
        infos.AddRange(sortedInfos);
        infos.AddRange(nonPriorityInfos);
    }

    /// <summary>
    /// Get the priority uuids of those hero infos according to the team list.
    /// </summary>
    /// <returns>The priority uuids which is team list.</returns>
    private IEnumerable<long> GetTeamPriorityUuids()
    {
        var teamList = SCHeroList.TeamList;
        var priorityUuids = teamList[SCHeroList.CurrentTeamIndex].ListHeroUuid.Where(uuid => uuid != HeroConstant.NoneInitHeroUuid).ToList();
        for (var i = 0; i < teamList.Count; i++)
        {
            if (i == SCHeroList.CurrentTeamIndex)
            {
                continue;
            }
            priorityUuids.AddRange(teamList[i].ListHeroUuid.Where(uuid => uuid != HeroConstant.NoneInitHeroUuid).ToList());
        }
        return priorityUuids.Distinct();
    }

    #endregion

    #region Public Methods

    public bool IsHeroFull()
    {
        if(SCHeroList == null || SCHeroList.HeroList == null)
        {
            return false;
        }
        return SCHeroList.HeroList.Count >= PlayerModelLocator.Instance.HeroMax;
    }

    public List<HeroInfo> FilterByJob(sbyte job, List<HeroInfo> heros)
    {
        return job == -1 ? heros : heros.Where(t => heroTemplates.HeroTmpls[t.TemplateId].Job == job).ToList();
    }

    public HeroBattleSkillTemplate GetLeaderSkillTemplateById(int templateid)
    {
        HeroBattleSkillTemplate result;
        if (SkillTemplates.HeroBattleSkillTmpls.TryGetValue(templateid, out result))
        {
            return result;
        }
        return null;
        //throw new Exception("Template id: " + templateid + " could not be found in skill template ");
    }

    /// <summary>
    /// Find the hero info in the hero list through the hero uid.
    /// </summary>
    /// <param name="heroUuid">The uid of the hero.</param>
    /// <returns>The hero info found out.</returns>
    public HeroInfo FindHero(long heroUuid)
    {
        return SCHeroList.HeroList.Find(info => info.Uuid == heroUuid);
    }

    /// <summary>
    /// Sort the list of hero info by specific order type.
    /// </summary>
    /// <param name="orderType">The specific order type.</param>
    /// <param name="heros">The list of hero info to be sorted.</param>
    /// <param name="isDescend">Descend or ascend of the sorting.</param>
    public void SortHeroList(OrderType orderType, List<HeroInfo> heros, bool isDescend = true)
    {
        if (heros == null || heros.Count <= 1)
        {
            return;
        }
        switch (orderType)
        {
            case OrderType.Time:
                heros.Sort(CompareHeroByTime);
                break;

            case OrderType.Job:
                heros.Sort(CompareHeroByJob);
                break;

            case OrderType.Rarity:
                heros.Sort(CompareHeroByRarity);
                break;

            case OrderType.Team:
                SortHeroByTeam(heros);
                break;

            case OrderType.Attack:
                heros.Sort(CompareHeroByAttack);
                break;

            case OrderType.Health:
                heros.Sort(CompareHeroByHp);
                break;

            case OrderType.Recover:
                heros.Sort(CompareHeroByRecover);
                break;

            case OrderType.Level:
                heros.Sort(CompareHeroByLv);
                break;
        }
        if(isDescend == false)
        {
            heros.Reverse();
        }
    }

    public HeroTemplate GetHeroByTemplateId(int templateid)
    {
        var hero = HeroTemplates;
        if (hero != null && hero.HeroTmpls != null && hero.HeroTmpls.ContainsKey(templateid))
        {
            return hero.HeroTmpls[templateid];
        }
        return null;
    }

    public void Clear()
    {
        OrderType = OrderType.Time;
        GetHeroPos = 0;
        SCHeroList = null;
        AlreadyRequest = false;
    }

    #endregion
}
