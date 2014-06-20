using System.Collections.Generic;
using UnityEngine;

public class TabBehaviour : MonoBehaviour
{
    /// <summary>
    /// The all toggle items in the tab panel.
    /// </summary>
    private Dictionary<UISprite, GameObject> toggleItems;

    public delegate void TabChangedDelegate(int activeTab);

    public TabChangedDelegate TabChanged;

    private int activeTab = -1;

    private string hLightSpriteName;
    private string normalSpriteName;

    public int ActiveTab
    {
        get { return activeTab; }
    }

    public void InitTab(Dictionary<UISprite, GameObject> items, int defaultIndex, string normalSpName, string hLightSpName)
    {
        if (items != null && items.Count > defaultIndex)
        {
            hLightSpriteName = hLightSpName;
            normalSpriteName = normalSpName;
            toggleItems = items;
            ShowTab(defaultIndex);
            if (TabChanged != null)
            {
                TabChanged(activeTab);
            }
            foreach (var item in items)
            {
                var sprite = item.Key;
               if (sprite.GetComponent<BoxCollider>() == null)
               {
                   NGUITools.AddWidgetCollider(sprite.gameObject);
               }
               UIEventListener.Get(sprite.gameObject).onClick += ChangeTab; 
            }
        }
    }

    private void ChangeTab(GameObject go)
    {
        var sprite = go.GetComponent<UISprite>();
        var tabIndex = GetIndex(sprite);
        ShowTab(tabIndex);

    }

    public void ShowTab(int tabIndex)
    {
        if (tabIndex >= 0 && tabIndex < toggleItems.Count)
        {
            if(tabIndex != activeTab)
            {
                if (TabChanged != null)
                {
                    TabChanged(tabIndex);
                }
                activeTab = tabIndex;
            }
            int index = 0;
            GameObject objectToShow = null;
            foreach (var item in toggleItems)
            {
                var sprite = item.Key;
                var spriteObject = item.Value;
                if (index == activeTab)
                {
                    sprite.spriteName = hLightSpriteName;
                    objectToShow = item.Value;
                }
                else
                {
                    sprite.spriteName = normalSpriteName;
                    if(spriteObject != null)
                    {
                        spriteObject.SetActive(false);
                    }
                }
                index++;
            }
            if(objectToShow != null)
            {
                objectToShow.SetActive(true);
            }
        }
    }

    private int GetIndex(UISprite tabSprite)
    {
        int index = 0;
        foreach (var item in toggleItems)
        {
            var sprite = item.Key;
            if(sprite == tabSprite)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    public UISprite GetTabKey(int index)
    {
        if(index >= 0 && index < toggleItems.Count)
        {
            var tabIndex = 0;
            foreach (var item in toggleItems)
            {
                if(tabIndex == index)
                {
                    return item.Key;
                }
                tabIndex++;
            }
        }
        return null;
    } 
    
    public GameObject GetTypeValue(int index)
    {
        if(index >= 0 && index < toggleItems.Count)
        {
            var tabIndex = 0;
            foreach (var item in toggleItems)
            {
                if(tabIndex == index)
                {
                    return item.Value;
                }
                tabIndex++;
            }
        }
        return null;
    }

}
