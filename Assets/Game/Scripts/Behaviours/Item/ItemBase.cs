using KXSGCodec;
using UnityEngine;

public class ItemBase : MonoBehaviour 
{
    protected Transform cachedTran;
    private UISprite icon;

    public ItemInfo TheItemInfo;

    private sbyte quality;
    public virtual sbyte Quality
    {
        get { return quality; }
        protected set
        {
            quality = value;
            var starCount = Mathf.CeilToInt((float)quality / ItemType.QualitiesPerStar);
            var stars = cachedTran.FindChild("Rarity");
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
        icon.spriteName = "Item_" + ItemModeLocator.Instance.GetIconId(temId);
    }
}
