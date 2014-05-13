using UnityEngine;

/// <summary>
/// The window to display all equips.
/// </summary>
public class UIEquipsDisplayWindowWindow : Window
{
    #region Private Fields

    private UIEventListener equipViewLis;
    private UIEventListener equipBindLis;
    private UIEventListener equipSellLis;
    private UIEventListener backLis;

    #endregion

    #region Window

    public override void OnEnter()
    {
        Toggle(1);
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        equipViewLis = UIEventListener.Get(Utils.FindChild(transform, "Button-EquipView").gameObject);
        equipSellLis = UIEventListener.Get(Utils.FindChild(transform, "Button-EquipSell").gameObject);
        equipBindLis = UIEventListener.Get(Utils.FindChild(transform, "Button-EquipBind").gameObject);
        backLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
    }

    /// <summary>
    /// Install the handlers.
    /// </summary>
    private void InstallHandlers()
    {
        equipViewLis.onClick += OnEquipViewClicked;
        equipSellLis.onClick += OnEquipSellClicked;
        equipBindLis.onClick += OnEquipBindClicked;
        backLis.onClick += OnBackClicked;
    }

    /// <summary>
    /// Uninstall the handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        equipViewLis.onClick -= OnEquipViewClicked;
        equipSellLis.onClick -= OnEquipSellClicked;
        equipBindLis.onClick -= OnEquipBindClicked;
        backLis.onClick -= OnBackClicked;
    }

    /// <summary>
    /// The equip view button click handler.
    /// </summary>
    /// <param name="go">The sender of click event.</param>
    private void OnEquipViewClicked(GameObject go)
    {
        Toggle(1);
    }

    /// <summary>
    /// The equip sell button click handler.
    /// </summary>
    /// <param name="go">The sender of click event.</param>
    private void OnEquipSellClicked(GameObject go)
    {
        Toggle(2);
    }

    /// <summary>
    /// The equip bind button click handler.
    /// </summary>
    /// <param name="go">The sender of click event.</param>
    private void OnEquipBindClicked(GameObject go)
    {
        Toggle(3);
    }

    /// <summary>
    /// The back button click handler.
    /// </summary>
    /// <param name="go">The sender of click event.</param>
    private void OnBackClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof(UIEquipsDisplayWindowWindow), false);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Toggle the button's color.
    /// </summary>
    /// <param name="index">Indicates which button will have the highlight color.</param>
    public void Toggle(int index)
    {
        switch (index)
        {
            case 1:
                equipViewLis.GetComponent<UISprite>().spriteName = HeroBaseInfoWindow.DownBtnSpriteName;
                equipBindLis.GetComponent<UISprite>().spriteName = HeroBaseInfoWindow.NormalBtnSpriteName;
                equipSellLis.GetComponent<UISprite>().spriteName = HeroBaseInfoWindow.NormalBtnSpriteName;
                break;
            case 2:
                equipSellLis.GetComponent<UISprite>().spriteName = HeroBaseInfoWindow.DownBtnSpriteName;
                equipViewLis.GetComponent<UISprite>().spriteName = HeroBaseInfoWindow.NormalBtnSpriteName;
                equipBindLis.GetComponent<UISprite>().spriteName = HeroBaseInfoWindow.NormalBtnSpriteName;
                break;
            case 3:
                equipBindLis.GetComponent<UISprite>().spriteName = HeroBaseInfoWindow.DownBtnSpriteName;
                equipViewLis.GetComponent<UISprite>().spriteName = HeroBaseInfoWindow.NormalBtnSpriteName;
                equipSellLis.GetComponent<UISprite>().spriteName = HeroBaseInfoWindow.NormalBtnSpriteName;
                break;
        }
    }

    #endregion
}
