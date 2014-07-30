using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIItemSnapShotWindow : Window
{
    private UIEventListener cancelLis;
    private UIEventListener viewDetailLis;
    private UIEventListener templateLis;
    private UIEventListener backLis;
    private UILabel templateLabel;
    private UILabel atkLabel;
    private UILabel hpLabel;
    private UILabel recoverLabel;
    private UILabel mpLabel;
    private UILabel level;
    private UILabel nameLabel;
    private UISprite jobIcon;

    public UIEventListener.VoidDelegate TemplateBtnPressed;

    /// <summary>
    /// The current item info.
    /// </summary>
    public static ItemInfo ItemInfo;

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Public Methods

    public void InitTemplate(string key, UIEventListener.VoidDelegate templatePressed, bool isEnabled = true)
    {
        templateLabel.text = LanguageManager.Instance.GetTextValue(key);
        TemplateBtnPressed = templatePressed;
        var button = templateLis.GetComponent<UIButton>();
        if(button != null && button.isEnabled != isEnabled)
        {
            button.isEnabled = isEnabled;
        }
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        cancelLis = UIEventListener.Get(transform.Find("Buttons/CancelBtn").gameObject);
        viewDetailLis = UIEventListener.Get(transform.Find("Buttons/ViewDetailBtn").gameObject);
        templateLis = UIEventListener.Get(transform.Find("Buttons/TemplateBtn").gameObject);
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
        templateLabel = templateLis.GetComponentInChildren<UILabel>();
        var property = transform.Find("Property");
        atkLabel = property.Find("Attack/AttackValue").GetComponent<UILabel>();
        hpLabel = property.Find("HP/HPValue").GetComponent<UILabel>();
        recoverLabel = property.Find("Recover/RecoverValue").GetComponent<UILabel>();
        mpLabel = property.Find("MP/MPValue").GetComponent<UILabel>();
        level = transform.Find("Item/Level/LevelLabel").GetComponent<UILabel>();
        jobIcon = transform.Find("Item/Job/JobIcon").GetComponent<UISprite>();
        nameLabel = transform.Find("Name").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        cancelLis.onClick = OnCancel;
        viewDetailLis.onClick = OnViewDetail;
        templateLis.onClick = OnTemplate;
        backLis.onClick = OnBack;
    }

    private void UnInstallHandlers()
    {
        cancelLis.onClick = null;
        viewDetailLis.onClick = null;
        templateLis.onClick = null;
        backLis.onClick = null;
    }

    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
    }

    private void OnCancel(GameObject go)
    {
        WindowManager.Instance.Show<UIItemSnapShotWindow>(false);
    }

    private void OnViewDetail(GameObject go)
    {
        UIItemDetailWindow.IsLongPressEnter = false;
        var csmsg = new CSQueryItemDetail { BagIndex = ItemInfo.BagIndex };
        NetManager.SendMessage(csmsg);
    }

    private void OnTemplate(GameObject go)
    {
        if (TemplateBtnPressed != null)
        {
            TemplateBtnPressed(go);
        }
    }

    private void Refresh()
    {
        RefreshProperty();
        RefreshItem();
    }

    private void RefreshItem()
    {
        level.text = ItemInfo.Level.ToString(CultureInfo.InvariantCulture);
        var tempId = ItemInfo.TmplId;
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + ItemModeLocator.Instance.GetJob(tempId);
        nameLabel.text = ItemModeLocator.Instance.GetName(tempId);
    }

    private void RefreshProperty()
    {
        RefreshLabel(atkLabel, ItemModeLocator.Instance.GetAttack(ItemInfo.TmplId, ItemInfo.Level));
        RefreshLabel(hpLabel, ItemModeLocator.Instance.GetHp(ItemInfo.TmplId, ItemInfo.Level));
        RefreshLabel(recoverLabel, ItemModeLocator.Instance.GetRecover(ItemInfo.TmplId, ItemInfo.Level));
        RefreshLabel(mpLabel, ItemModeLocator.Instance.GetMp(ItemInfo.TmplId));
    }

    private void RefreshLabel(UILabel label, int value)
    {
        var temp = value >= 0 ? value : 0;
        label.text = temp.ToString(CultureInfo.InvariantCulture);
    }

    #endregion
}
