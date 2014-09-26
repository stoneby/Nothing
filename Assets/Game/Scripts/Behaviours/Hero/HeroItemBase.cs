using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class HeroItemBase : MonoBehaviour 
{
    protected Transform cachedTran;
    protected sbyte quality;
    private UISprite icon;
    private UISprite jobIcon;
    private UILabel heroName;

    [HideInInspector]
    public HeroInfo HeroInfo;

    public virtual sbyte Quality
    {
        get { return quality; }
        protected set
        {
            quality = value;
            var stars = cachedTran.FindChild("Rarity");
            var starCount = stars.transform.childCount;
            for (int index = 0; index < quality; index++)
            {
                NGUITools.SetActive(stars.FindChild("Star" + (starCount - index - 1)).gameObject, true);
            }
            for (int index = starCount - quality - 1; index >= 0; index--)
            {
                NGUITools.SetActive(stars.FindChild("Star" + index).gameObject, false);
            }
        }
    }

    protected virtual void Awake()
    {
        cachedTran = transform;
        icon = transform.Find("Icon").GetComponent<UISprite>();
        var jobTran = transform.Find("Job");
        if (jobTran)
        {
            jobIcon = jobTran.Find("JobIcon").GetComponent<UISprite>();
        }
        var nameTran = transform.Find("Name");
        if (nameTran)
        {
            heroName = nameTran.GetComponent<UILabel>();
        }
    }

    public virtual void InitItem(HeroInfo heroInfo)
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[heroInfo.TemplateId];
        Quality = heroTemplate.Star;
        HeroInfo = heroInfo;
        HeroConstant.SetHeadByIndex(icon, heroTemplate.Icon - 1);
        if (jobIcon)
        {
            jobIcon.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;
            jobIcon.MakePixelPerfect();
        }
        if (heroName)
        {
            heroName.text = heroTemplate.Name;
        }
    } 
    
    public virtual void InitItem(HeroInfo heroInfo, List<long> curTeam, List<long> allTeams)
    {
        InitItem(heroInfo);
    }

    public virtual void Show(int tempId)
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[tempId];
        Quality = heroTemplate.Star;
        HeroConstant.SetHeadByIndex(icon, heroTemplate.Icon - 1);
        if (jobIcon)
        {
            jobIcon.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;
            jobIcon.MakePixelPerfect();
        }
        if(heroName)
        {
            heroName.text = heroTemplate.Name;
        }
    }
}
