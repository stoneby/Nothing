using System.Collections.Generic;
using UnityEngine;

public class WrapHerosItem : WrapItemBase
{
    public List<Transform> Children;
    public int Row;

    private readonly List<Transform> sellMasks = new List<Transform>();
    private readonly List<Transform> cannotSellMasks = new List<Transform>();

    private void Awake()
    {
        foreach (var child in Children)
        {
            var mask = child.Find("SellMask");
            sellMasks.Add(mask);
            NGUITools.SetActive(mask.gameObject, false);
            var cannotSellMask = child.Find("CanNotSellMask");
            cannotSellMasks.Add(cannotSellMask);
            NGUITools.SetActive(cannotSellMask.gameObject, false);
        }
    }

    public void ShowSellMask(int col, bool show)
    {
        if (col < 0 || col >= sellMasks.Count)
        {
            Logger.LogError("The column index is out of range.");
            return;
        }
        NGUITools.SetActive(sellMasks[col].gameObject, show);
    }

    public void ShowSellMasks(bool show)
    {
        foreach (var mask in sellMasks)
        {
            NGUITools.SetActive(mask.gameObject, show);
        }
    } 
    
    public void ShowCanNotSellMask(int col, bool show)
    {
        if (col < 0 || col >= cannotSellMasks.Count)
        {
            Logger.LogError("The column index is out of range.");
            return;
        }
        NGUITools.SetActive(cannotSellMasks[col].gameObject, show);
    }

    public void ShowCanNotSellMasks(bool show)
    {
        foreach (var mask in cannotSellMasks)
        {
            NGUITools.SetActive(mask.gameObject, show);
        }
    }

    public override void SetData(object data, int index)
    {
        Row = index;
        var myData = data as List<long>;
        for (var i = 0; i < myData.Count; i++)
        {
            var child = Children[i];
            NGUITools.SetActive(child.gameObject, true);
            var heroItem = child.GetComponentInChildren<NewHeroItem>();
            var heroInfo = HeroModelLocator.Instance.FindHero(myData[i]);
            heroItem.InitItem(heroInfo, TeamMemberManager.Instance.CurTeam, TeamMemberManager.Instance.Teams);
            HeroUtils.ShowHero(HeroModelLocator.Instance.OrderType, heroItem, heroInfo);
        }
        for (int i = myData.Count; i < Children.Count; i++)
        {
            NGUITools.SetActive(Children[i].gameObject, false);
        }    
    }
}
