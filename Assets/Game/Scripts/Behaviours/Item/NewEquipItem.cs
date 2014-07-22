using KXSGCodec;
using UnityEngine;
using System.Collections;

public class NewEquipItem : EquipItem
{
    private Transform sortRelated;
    private Transform stars;

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
            var childCount = stars.transform.childCount;
            for (var index = 0; index < starCount; index++)
            {
                NGUITools.SetActive(stars.FindChild("Star" + (childCount - index - 1)).gameObject, true);
            }
            for (var index = childCount - starCount - 1; index >= 0; index--)
            {
                NGUITools.SetActive(stars.FindChild("Star" + index).gameObject, false);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        sortRelated = transform.Find("SortRelated");
        stars = sortRelated.Find("Rarity");
    }

    public override void ShowByQuality(int star)
    {
        Quality = (sbyte)star;
        NGUITools.SetActiveChildren(sortRelated.gameObject, false);
        NGUITools.SetActive(stars.gameObject, true);
    }

}
