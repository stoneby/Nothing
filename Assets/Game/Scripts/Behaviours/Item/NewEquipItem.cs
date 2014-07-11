using KXSGCodec;
using UnityEngine;
using System.Collections;

public class NewEquipItem : ItemBase
{
    private Transform lockedIcon;
    private Transform curTeamEquiped;
    private Transform otherTeamEquiped;

    public void InitItem(ItemInfo itemInfo)
    {
        BagIndex = itemInfo.BagIndex;
        Quality = ItemModeLocator.Instance.GetQuality(itemInfo.TmplId);
    }

    private sbyte quality;
    public override sbyte Quality
    {
        get
        {
            return base.Quality;
        }
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

}
