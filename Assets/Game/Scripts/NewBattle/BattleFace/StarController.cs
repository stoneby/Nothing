using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Star achieving controller.
/// </summary>
public class StarController : MonoBehaviour
{
    public List<UISprite> StarList;
    public List<float> RatioList;

    public int CurrentStar { get; private set; }

    public void Reset()
    {
        StarList.ForEach(item => item.gameObject.SetActive(true));
        CurrentStar = RatioList.Count;
    }

    public void Show(float ratio)
    {
        CurrentStar = RatioList.Count;
        for (var i = RatioList.Count - 1; i >= 0; --i)
        {
            if (ratio < RatioList[i])
            {
                StarList[i].gameObject.SetActive(false);
                CurrentStar = i;
            }
        }
    }

    private void Awake()
    {
        if (StarList == null || RatioList == null || StarList.Count != RatioList.Count)
        {
            Logger.LogError("StartList and RatioList should not be null or not equals to.");
        }
    }
}
