
using System.Collections.Generic;
using KXSGCodec;

public class NewHeroItem : HeroItem
{
    private UISprite jobIcon;

    public override sbyte Quality
    {
        get
        {
            return base.Quality;
        }
        protected set
        {
            quality = value;
            var stars = cachedTran.FindChild("SortRelated/Rarity");
            var starCount = stars.transform.childCount;
            for (int index = 0; index < value; index++)
            {
                NGUITools.SetActive(stars.FindChild("Star" + (starCount - index - 1)).gameObject, true);
            }
            for (int index = starCount - value - 1; index >= 0; index--)
            {
                NGUITools.SetActive(stars.FindChild("Star" + index).gameObject, false);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        jobIcon = transform.Find("Job/JobIcon").GetComponent<UISprite>();
    }

    public override void ShowByJob(sbyte job, int atk)
    {
        base.ShowByJob(job, atk);
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + job;
    }

    public override void ShowByHp(sbyte job, int hp)
    {
        base.ShowByHp(job, hp);
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + job;
    }

    public override void ShowByRecover(sbyte job, int recover)
    {
        base.ShowByRecover(job, recover);
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + job;
    }

    /// <summary>
    /// Show each hero items with the level info.
    /// </summary>
    public override void ShowByLvl(sbyte job, short level)
    {
        base.ShowByLvl(job, level);
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + job;
    }

    public override void ShowByQuality(sbyte job, int star)
    {
        base.ShowByQuality(job, star);
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + job;
    }

    public override void InitItem(HeroInfo heroInfo)
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[heroInfo.TemplateId];
        Quality = heroTemplate.Star;
        Uuid = heroInfo.Uuid;
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;
    }
}
