using UnityEngine;

public class ItemBase : MonoBehaviour 
{
    protected Transform cachedTran;
    private UISprite bg;

    private const int StarsPerLevel = 3;
    private sbyte quality;
    public sbyte Quality
    {
        get { return quality; }
        protected set
        {
            quality = value;
            var type = Mathf.CeilToInt((float)quality / StarsPerLevel);
            switch (type)
            {
                case 1:
                    bg.spriteName = "FrameW";
                    break;
                case 2:
                    bg.spriteName = "FrameG";
                    break;
                case 3:
                    bg.spriteName = "FrameB";
                    break;
                case 4:
                    bg.spriteName = "FrameP";
                    break;
                case 5:
                    bg.spriteName = "FrameO";
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

    public short BagIndex { get; set; }

    protected virtual void Awake()
    {
        cachedTran = transform;
        bg = cachedTran.FindChild("BG").GetComponent<UISprite>();
    }
}
