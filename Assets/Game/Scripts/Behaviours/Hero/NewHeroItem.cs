public class NewHeroItem : HeroItem
{
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
}
