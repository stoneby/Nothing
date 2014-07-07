using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIGrid))]
public class BarItemControl : MonoBehaviour
{
    public BarItem ItemPrefab;
    public string PoolName;
    private readonly List<BarItem> barItems = new List<BarItem>();
    private UIGrid grid;
    public List<EventDelegate> ItemDelegates;
    public List<string> BarNames;
    public bool HideWhenPress = true;

    public virtual void Init()
    {
        var count = ItemDelegates.Count;
        if (count != BarNames.Count)
        {
            Logger.LogError("The bar item count is not equal to the event delegate count");
        }
        barItems.Clear();
        for (var i = 0; i < count; i++)
        {
            var item = PoolManager.Pools[PoolName].Spawn(ItemPrefab.transform);
            Utils.MoveToParent(gameObject.transform, item);
            NGUITools.SetActive(item.gameObject, true);
            var barItem = item.GetComponent<BarItem>();
            barItems.Add(barItem);
            barItem.Init(BarNames[i]);
            UIEventListener.Get(barItem.gameObject).onClick = OnBarItemClicked;
        }
        AdjustPos();
    }

    protected void OnBarItemClicked(GameObject go)
    {
        if(HideWhenPress)
        {
            CleanUp();
        }
        var index = barItems.IndexOf(go.GetComponent<BarItem>());
        EventDelegate.Execute(new List<EventDelegate> {ItemDelegates[index]});
    }

    protected virtual void Awake()
    {
        grid = GetComponent<UIGrid>();
    }

    protected virtual void AdjustPos()
    {
        grid.Reposition();
        grid.transform.localPosition += (barItems.Count - 1) * 0.5f * grid.cellWidth * Vector3.left;
    }

    public virtual void CleanUp()
    {
        for (var i = 0; i < barItems.Count; i++)
        {
            var barItem = barItems[i];
            barItem.CleanUp();
            if (PoolManager.Pools.ContainsKey(PoolName))
            {
                barItem.transform.parent = PoolManager.Pools[PoolName].transform;
                PoolManager.Pools[PoolName].Despawn(barItem.transform);
                UIEventListener.Get(barItem.gameObject).onClick = null;
            }
        }
    }
}
