using System.Linq;
using KXSGCodec;
using System.Collections.Generic;
using Property;
using Template;
using UnityEngine;

public sealed class HeroModelLocator
{
    #region Private Field

    private static volatile HeroModelLocator instance;
    private static readonly object SyncRoot = new Object();
    private const string HeroTemlatePath = "Templates/Hero";
    private const string SkillTemlatePath = "Templates/Skill";

    #endregion

    #region Constructs

    private HeroModelLocator()
    {
        
    }

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

    public int GetHeroPos;//获取武将编队列表的位置标识
    public SCHeroList SCHeroList;

    private Hero heroTemplates;
    public Hero HeroTemplates
    {
        get { return heroTemplates ?? (heroTemplates = Utils.Decode<Hero>(HeroTemlatePath)); }
    }

    private Skill skillTemplates;
    public Skill SkillTemplates
    {
        get { return skillTemplates ?? (skillTemplates = Utils.Decode<Skill>(SkillTemlatePath)); }
    }

    public SkillTemplate GetLeaderSkillTemplateById(int templateid)
    {
        return SkillTemplates.SkillTmpl[templateid];
    }

    #endregion

    #region Public Methods

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
    /// <param name="sortType">The specific order type.</param>
    /// <param name="heros">The list of hero info to be sorted.</param>
    public void SortHeroList(short sortType, List<HeroInfo> heros)
    {
        switch (sortType)
        {
            //按入手顺序排序
            case 0:
                heros.Sort(CompareHeroByTime);
                break;

            //按武将职业排序
            case 1:
                heros.Sort(CompareHeroByJob);
                break;

            //按武将稀有度排序
            case 2:
                heros.Sort(CompareHeroByRarity);
                break;

            //按照队伍顺序排序
            case 3:
                break;

            //按攻击力排序
            case 4:
                heros.Sort(CompareHeroByAttack);
                break;

            //按HP排序
            case 5:
                heros.Sort(CompareHeroByHp);
                break;

            //按回复力排序
            case 6:
                heros.Sort(CompareHeroByRecover);
                break;

            //按等级排序
            case 7:
                heros.Sort(CompareHeroByLv);
                break;
        }
    }

    public HeroTemplate GetHeroByTemplateId(int templateid)
    {
        var hero = HeroTemplates;
        if (hero != null && hero.HeroTmpl != null && hero.HeroTmpl.ContainsKey(templateid))
        {
            return hero.HeroTmpl[templateid];
        }
        return null;
    }
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
        int compareResult = p2.CreateTime.CompareTo(p1.CreateTime);
        if (compareResult == 0)
        {
            return p2.TemplateId.CompareTo(p1.TemplateId);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by job.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareHeroByJob(HeroInfo p1, HeroInfo p2)
    {
        var heroTemp = HeroTemplates.HeroTmpl;
        int compareResult = heroTemp[p2.TemplateId].Job.CompareTo(heroTemp[p1.TemplateId].Job);
        if (compareResult == 0)
        {
            return p2.TemplateId.CompareTo(p1.TemplateId);
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
        var heroTemp = HeroTemplates.HeroTmpl;
        int compareResult = heroTemp[p2.TemplateId].Star.CompareTo(heroTemp[p1.TemplateId].Star);
        if (compareResult == 0)
        {
            return p2.TemplateId.CompareTo(p1.TemplateId);
        }
        return compareResult;
    }

    /// <summary>
    /// The comparation of hero info by attack.
    /// </summary>
    /// <param name="p1">The left hero info.</param>
    /// <param name="p2">The right hero info.</param>
    /// <returns>The result of the comparation</returns>
    private int CompareHeroByAttack(HeroInfo p1, HeroInfo p2)
    {
        int compareResult = p2.Prop[RoleProperties.HERO_ATK].CompareTo(p1.Prop[RoleProperties.HERO_ATK]);
        if (compareResult == 0)
        {
            return p2.TemplateId.CompareTo(p1.TemplateId);
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
        int compareResult = p2.Prop[RoleProperties.HERO_HP].CompareTo(p1.Prop[RoleProperties.HERO_HP]);
        if (compareResult == 0)
        {
            return p2.TemplateId.CompareTo(p1.TemplateId);
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
        int compareResult = p2.Prop[RoleProperties.HERO_RECOVER].CompareTo(p1.Prop[RoleProperties.HERO_RECOVER]);
        if (compareResult == 0)
        {
            return p2.TemplateId.CompareTo(p1.TemplateId);
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
            return p2.TemplateId.CompareTo(p1.TemplateId);
        }
        return compareResult;
    }

    public List<HeroInfo> FilterByJob(sbyte job, List<HeroInfo> heros)
    {
        return job == -1 ? heros : heros.Where(t => heroTemplates.HeroTmpl[t.TemplateId].Job == job).ToList();
    }

    #endregion
}
