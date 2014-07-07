
using System.Collections.Generic;
using KXSGCodec;

public class NewHeroItem : HeroItemBase
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
            var stars = cachedTran.FindChild("Rarity");
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

    public override void InitItem(HeroInfo heroInfo)
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        Quality = heroTemplate.Star;
        Uuid = heroInfo.Uuid;
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;
    }
}
