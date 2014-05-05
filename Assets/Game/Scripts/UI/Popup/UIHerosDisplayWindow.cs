using System.Collections.Generic;
using UnityEngine;
using KXSGCodec;

/// <summary>
/// Heros display window controller.
/// </summary>
public class UIHerosDisplayWindow : Window
{
    #region private Fields

    private GameObject heroTemplate;
    private UIGrid heroItems;
    private UILabel heroNums;
    private UILabel sortLabel;
    private UIEventListener heroViewLis;
    private UIEventListener teamBuildLis;
    private UIEventListener heroSellLis;
    private UIEventListener backBtnLis;
    private UIEventListener createOneLis;
    private Transform heroViewPage;
    private Transform teamBuildPage;
    private Transform heroMixPage;
    private Transform heroSellPage;
    private SCHeroList scHeroList;
    private sbyte cachedOrderType;

    //Just for demo.
    public static bool IsCreateOne;
    public static int CurTemplateId;
    public const string JobPrefix = "icon_zhiye_";


    #endregion

    #region Window

    public override void OnEnter()
    {
        IsCreateOne = false;
        HideOtherExceptMe(heroViewPage);
        cachedOrderType = HeroModelLocator.Instance.SCHeroList.OrderType;
        InstallHandlers(); 
        WindowManager.Instance.Show(typeof(UIHeroItemsPageWindow), true);
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        if (HeroModelLocator.Instance.SCHeroList.OrderType != cachedOrderType)
        {
            var csmsg = new CSHeroChangeOrder {OrderType = scHeroList.OrderType};
            NetManager.SendMessage(csmsg);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initiate some varibles.
    /// </summary>
    private void Awake()
    {
        scHeroList = HeroModelLocator.Instance.SCHeroList;
        heroViewLis = UIEventListener.Get(Utils.FindChild(transform, "Button-HeroView").gameObject);
        teamBuildLis = UIEventListener.Get(Utils.FindChild(transform, "Button-TeamBuild").gameObject);
        heroSellLis = UIEventListener.Get(Utils.FindChild(transform, "Button-HeroSell").gameObject);
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        createOneLis = UIEventListener.Get(Utils.FindChild(transform, "Button-CreateOne").gameObject);

        heroViewPage = Utils.FindChild(transform, "Page-HeroView");
        teamBuildPage = Utils.FindChild(transform, "Page-TeamBuild");
        heroMixPage = Utils.FindChild(transform, "Page-HeroMix");
        heroSellPage = Utils.FindChild(transform, "Page-HeroSell");
    }

    /// <summary>
    /// Install the handlers.
    /// </summary>
    private void InstallHandlers()
    {
        heroViewLis.onClick += OnHeroViewClicked;
        teamBuildLis.onClick += OnTeamBuildClicked;
        heroSellLis.onClick += OnHeroSellClicked;
        backBtnLis.onClick += OnBackClicked;
        createOneLis.onClick += OnCreateOne;
    }

    /// <summary>
    /// Uninstall the handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        heroViewLis.onClick -= OnHeroViewClicked;
        teamBuildLis.onClick -= OnTeamBuildClicked;
        heroSellLis.onClick -= OnHeroSellClicked;
        backBtnLis.onClick -= OnBackClicked;
        createOneLis.onClick -= OnCreateOne;
    }

    private void OnCreateOne(GameObject go)
    {
        IsCreateOne = true;
        var csmsg = new CSHeroCreateOne();
        NetManager.SendMessage(csmsg);
    }

    /// <summary>
    /// The back button click handler.
    /// </summary>
    /// <param name="sender">The sender of click event.</param>
    private void OnBackClicked(GameObject sender)
    {
        WindowManager.Instance.Show(typeof(UIHerosDisplayWindow), false);
        WindowManager.Instance.Show(typeof(UIHeroItemsPageWindow), false);
    }

    /// <summary>
    /// The hero sell button click handler.
    /// </summary>
    /// <param name="sender">The sender of click event.</param>
    private void OnHeroSellClicked(GameObject sender)
    {
        WindowManager.Instance.Show(typeof(UIHeroSellWindow), true);
    }

    /// <summary>
    /// The hero mix button click handler.
    /// </summary>
    /// <param name="sender">The sender of click event.</param>
    private void OnHeroMixClicked(GameObject sender)
    {
        HideOtherExceptMe(heroMixPage);
    }


    /// <summary>
    /// The team build button click handler.
    /// </summary>
    /// <param name="sender">The sender of click event.</param>
    private void OnTeamBuildClicked(GameObject sender)
    {
        WindowManager.Instance.Show(typeof (UITeamBuildWindow), true);
    }


    /// <summary>
    /// The hero view button click handler.
    /// </summary>
    /// <param name="sender">The sender of click event.</param>
    private void OnHeroViewClicked(GameObject sender)
    {
        HideOtherExceptMe(heroViewPage);
    }

    /// <summary>
    /// Hide other pages except current page.
    /// </summary>
    /// <param name="tran">The transfrom of current page.</param>
    private void HideOtherExceptMe(Transform tran)
    {
        var list = new List<Transform>{heroViewPage, teamBuildPage, heroMixPage, heroSellPage};
        for(int i = 0; i < list.Count; i++)
        {
            list[i].gameObject.SetActive(tran == list[i]);
        }
    }

    #endregion
}
