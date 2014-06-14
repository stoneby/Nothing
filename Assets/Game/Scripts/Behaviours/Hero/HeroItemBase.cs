using KXSGCodec;
using UnityEngine;

public class HeroItemBase : MonoBehaviour 
{
    private UISprite bg;
    private string bgSpritePrifix;
    protected Transform cachedTran;
    private sbyte quality;

    public long Uuid { get; set; }

    public sbyte Quality
    {
        get { return quality; }
        protected set
        {
            quality = value;
            switch (quality)
            {
                case 1:
                    bg.spriteName = bgSpritePrifix + "W";
                    break;
                case 2:
                    bg.spriteName = bgSpritePrifix + "G";
                    break;
                case 3:
                    bg.spriteName = bgSpritePrifix + "B";
                    break;
                case 4:
                    bg.spriteName = bgSpritePrifix + "P";
                    break;
                case 5:
                    bg.spriteName = bgSpritePrifix + "O";
                    break;
            }
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
        bg = cachedTran.FindChild("BG").GetComponent<UISprite>();
        bgSpritePrifix = bg.spriteName.Substring(0, bg.spriteName.Length - 1);
    }

    public virtual void InitItem(HeroInfo heroInfo)
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        Quality = heroTemplate.Star;
        Uuid = heroInfo.Uuid;
    }
}
