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
        StarList.ForEach(item => item.gameObject.SetActive(true));
        CurrentStar = RatioList.Count;
    }

    public void Show(float ratio)
    {
        if (CurrentStar == null && RatioList != null)
        {
            CurrentStar = RatioList.Count;
        } 
        
        for (var i = RatioList.Count - 1; i >= 0; --i)
        {
            if (ratio < RatioList[i])
            {
                CurrentStar = i;
            }

            if (i >= CurrentStar)
            {
                StarList[i].gameObject.SetActive(false);
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
