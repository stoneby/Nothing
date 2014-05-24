using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class to handle the tab panel.
/// </summary>
public class TabPanelBase : Window
{
    #region Public Fields

    /// <summary>
    /// The pair of sprite and window.
    /// </summary>
    [Serializable]
    public class TabPanelItem
    {
        public UISprite UiSprite;
        public Window Window;
    }

    /// <summary>
    /// The all toggle items in the tab panel.
    /// </summary>
    public List<TabPanelItem> ToggleItems;

    /// <summary>
    /// The current tab panel item.
    /// </summary>
    [HideInInspector]
    public TabPanelItem CurItem;

    /// <summary>
    /// The default index of highlight item when start the tab panel.
    /// </summary>
    public int DefaultItemIndex = 0;

    /// <summary>
    /// The sprite name of the high light of button.
    /// </summary>
    public string HlightSpriteName;

    #endregion

    #region Private Fields

    private string normalSpriteName;
    private readonly List<TabPanelItem> usedItems = new List<TabPanelItem>();

    #endregion

    #region Window

    public override void OnEnter()
    {
        CurItem = new TabPanelItem
                      {
                          UiSprite = ToggleItems[DefaultItemIndex].UiSprite,
                          Window = WindowManager.Instance.GetWindow(Utils.PrefabNameToWindow(ToggleItems[DefaultItemIndex].Window.name))
                      };
        CurItem.Window.gameObject.SetActive(true);
        normalSpriteName = CurItem.UiSprite.spriteName;
        CurItem.UiSprite.spriteName = HlightSpriteName;
        usedItems.Clear();
        usedItems.Add(CurItem);
    }

    public override void OnExit()
    {
        CurItem.UiSprite.spriteName = normalSpriteName;
        CurItem.Window.gameObject.SetActive(false);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Awake()
    {
        CheckValid();
        for (int i = 0; i < ToggleItems.Count; i++)
        {
            var sprite = ToggleItems[i].UiSprite;
            var spriteObject = sprite.gameObject;
            if (sprite.GetComponent<BoxCollider>() == null)
            {
                NGUITools.AddWidgetCollider(spriteObject);
            }
            UIEventListener.Get(spriteObject).onClick += OnToggleItem;
        }
    }

    /// <summary>
    /// The callback of each button click event in the tab panel.
    /// </summary>
    /// <param name="go">The sender of click event.</param>
    private void OnToggleItem(GameObject go)
    {
        var sprite = go.GetComponent<UISprite>();
        if(sprite != CurItem.UiSprite)
        {
            CurItem.UiSprite.spriteName = normalSpriteName;
            CurItem.Window.gameObject.SetActive(false);
            var spriteList = new List<UISprite>();
            for (var i = 0; i < usedItems.Count; i++)
            {
                spriteList.Add(usedItems[i].UiSprite);
            }
            if (!spriteList.Contains(sprite))
            {
                var item = FindPanelItem(sprite);
                if (item != null)
                {
                    CurItem = new TabPanelItem { UiSprite = sprite, Window = WindowManager.Instance.GetWindow(Utils.PrefabNameToWindow(item.Window.name))};
                    CurItem.UiSprite.spriteName = HlightSpriteName;
                    CurItem.Window.gameObject.SetActive(true);
                    usedItems.Add(CurItem);
                }
            }
            else
            {
                CurItem = usedItems[spriteList.IndexOf(sprite)];
                CurItem.Window.gameObject.SetActive(true);
                CurItem.UiSprite.spriteName = HlightSpriteName;
            }
        }
    }

    /// <summary>
    /// Find the tab panel item in the toggles items.
    /// </summary>
    /// <param name="sprite">The sprite of the toggle button.</param>
    /// <returns>The tab panel item found.</returns>
    private TabPanelItem FindPanelItem(UISprite sprite)
    {
        for (var i = 0; i < ToggleItems.Count; i++)
        {
            if(ToggleItems[i].UiSprite == sprite)
            {
                return ToggleItems[i];
            }
        }
        return null;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Check the valid of the toggle items.
    /// </summary>
    protected virtual void CheckValid()
    {
        var count = ToggleItems.Count;
        if (count > 0)
        {
            var windowGroupType = ToggleItems[0].Window.WindowGroup;
            for (var i = 1; i < count; i++)
            {
                if (windowGroupType != ToggleItems[i].Window.WindowGroup)
                {
                    Logger.LogError("The windows used in the same tab panel must be the same window group.");
                }
            }
        }
    }

    #endregion
}
