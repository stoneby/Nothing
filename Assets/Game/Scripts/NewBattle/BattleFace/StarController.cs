using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Star achieving controller.
/// </summary>
public class StarController : MonoBehaviour
{
    public List<UISprite> StarList;
    public List<float> RatioList;

    public int CurrentStar { get; set; }

    public void Reset()
    {
        StarList.ForEach(item => SetEnable(item, true));
        CurrentStar = RatioList.Count;
    }

    public void Show(float ratio)
    {
        for (var i = RatioList.Count - 1; i >= 0; --i)
        {
            if (ratio < RatioList[i])
            {
                CurrentStar = i;
            }

            if (i >= CurrentStar)
            {
                SetEnable(StarList[i], false);
            }
        }
    }

    private void SetEnable(UISprite star, bool enable)
    {
        star.spriteName = enable ? "StarLight" : "StarDelight";
    }

    private void Awake()
    {
        if (StarList == null || RatioList == null || StarList.Count != RatioList.Count)
        {
            Logger.LogError("StartList and RatioList should not be null or not equals to.");
        }
    }
}
