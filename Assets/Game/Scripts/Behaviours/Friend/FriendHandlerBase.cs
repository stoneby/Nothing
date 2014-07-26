using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

public class FriendHandlerBase : MonoBehaviour
{
    public Transform FriendItem;
    private UIGrid items;
   
    protected UIGrid Items
    {
        get { return items ?? transform.Find("Scroll View/Grid").GetComponent<UIGrid>(); }
    }

    /// <summary>
    /// Fill in the item game objects.
    /// </summary> 
    protected virtual void UpdateItemList(int itemCount)
    {
        var childCount = Items.transform.childCount;
        if (childCount != itemCount)
        {
            var isAdd = childCount < itemCount;
            HeroUtils.AddOrDelItems(Items.transform, FriendItem, isAdd, Mathf.Abs(itemCount - childCount),
                                    "FriendRelated",
                                    null);
            Items.repositionNow = true;
        }
    }

    public virtual void Refresh(List<FriendInfo> infos)
    {
        UpdateItemList(infos.Count);
        for (int i = 0; i < infos.Count; i++)
        {
            var child = Items.transform.GetChild(i);
            child.GetComponent<FriendItem>().Init(infos[i]);
        }
    }
}
