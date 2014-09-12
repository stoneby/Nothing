using KXSGCodec;
using UnityEngine;

public class ItemBase : MonoBehaviour 
{
    protected Transform cachedTran;
    private UISprite icon;
    private UILabel itemName;
    protected Transform stars;
    private UISprite jobIcon;

    [HideInInspector]
    public ItemInfo TheItemInfo;
    protected sbyte quality;
    public virtual sbyte Quality
    {
        get { return quality; }
        protected set
        {
            quality = value;
            var starCount = ItemHelper.GetStarCount(quality);
            var childCount = stars.transform.childCount;
            for (int index = 0; index < starCount; index++)
            {
                NGUITools.SetActive(stars.FindChild("Star" + (childCount - index - 1)).gameObject, true);
            }
            for (int index = childCount - starCount - 1; index >= 0; index--)
            {
                NGUITools.SetActive(stars.FindChild("Star" + index).gameObject, false);
            }
        }
    }

    public short BagIndex { get; set; }

    protected virtual void Awake()
    {
        cachedTran = transform;
        icon = transform.Find("Icon").GetComponent<UISprite>();
        stars = cachedTran.FindChild("Rarity");
        var nameTran = transform.Find("Name");
        if(nameTran)
        {
            itemName = nameTran.GetComponent<UILabel>();
        }
        var jobTran = transform.Find("Job");
        if(jobTran)
        {
            jobIcon = jobTran.Find("JobIcon").GetComponent<UISprite>();
        }
    }

    public virtual void InitItem(ItemInfo itemInfo)
    {
        TheItemInfo = itemInfo;
        BagIndex = itemInfo.BagIndex;
        InitItem(itemInfo.TmplId);
    }

    public virtual void InitItem(int temId)
    {
        Quality = ItemModeLocator.Instance.GetQuality(temId);
        ItemType.SetHeadByTemplate(icon, temId);
        if(itemName)
        {
            itemName.text = ItemModeLocator.Instance.GetName(temId);
        }
        if(jobIcon)
        {
            var job = ItemModeLocator.Instance.GetJob(temId);
            if(job != -1)
            {
                jobIcon.spriteName = HeroConstant.HeroJobPrefix + job;
            }
            else
            {
                jobIcon.spriteName = ItemType.ArmorJob;
            }
            jobIcon.MakePixelPerfect();
        }
    }
}
