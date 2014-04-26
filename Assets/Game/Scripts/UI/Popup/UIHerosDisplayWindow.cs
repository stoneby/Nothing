using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Heros display window controller.
/// </summary>
public class UIHerosDisplayWindow : Window
{
    #region private Fields

    private GameObject heroTemplate;
    private UIGrid heroItems;
    private UILabel heroNums;
    private UIEventListener heroViewLis;
    private UIEventListener teamBuildLis;
    private UIEventListener heroMixLis;
    private UIEventListener heroSellLis;
    private UIEventListener backBtnLis;
    private Transform heroViewPage;
    private Transform teamBuildPage;
    private Transform heroMixPage;
    private Transform heroSellPage;

    public const string JobPrefix = "icon_zhiye_";

    //Just for demo.
    public static int CurTemplateId;
    #endregion

    #region Window

    public override void OnEnter()
    {
        HideOtherExceptMe(heroViewPage);
        InstallHandlers(); 
        var scHeroList = HeroModelLocator.Instance.SCHeroList;
        //ÎªÍâÍødemo.
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, 100);
        var heroItemsPage = WindowManager.Instance.Show(typeof(UIHeroItemsPageWindow), true);
        heroItemsPage.GetComponent<UIHeroItemsPageWindow>().RowToShow = 3;
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initiate some varibles.
    /// </summary>
    private void Awake()
    {
        heroViewLis = UIEventListener.Get(Utils.FindChild(transform, "Button-HeroView").gameObject);
        teamBuildLis = UIEventListener.Get(Utils.FindChild(transform, "Button-TeamBuild").gameObject);
        heroMixLis = UIEventListener.Get(Utils.FindChild(transform, "Button-HeroMix").gameObject);
        heroSellLis = UIEventListener.Get(Utils.FindChild(transform, "Button-HeroSell").gameObject);
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);

        heroViewPage = Utils.FindChild(transform, "Page-HeroView");
        teamBuildPage = Utils.FindChild(transform, "Page-TeamBuild");
        heroMixPage = Utils.FindChild(transform, "Page-HeroMix");
        heroSellPage = Utils.FindChild(transform, "Page-HeroSell");
        heroNums = Utils.FindChild(heroViewPage.transform, "HeroNums").GetComponent<UILabel>();
    }

    /// <summary>
    /// Install the handlers.
    /// </summary>
    private void InstallHandlers()
    {
        heroViewLis.onClick += OnHeroViewClicked;
        teamBuildLis.onClick += OnTeamBuildClicked;
        heroMixLis.onClick += OnHeroMixClicked;
        heroSellLis.onClick += OnHeroSellClicked;
        backBtnLis.onClick += OnBackClicked;
    }

    /// <summary>
    /// Uninstall the handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        heroViewLis.onClick -= OnHeroViewClicked;
        teamBuildLis.onClick -= OnTeamBuildClicked;
        heroMixLis.onClick -= OnHeroMixClicked;
        heroSellLis.onClick -= OnHeroSellClicked;
        backBtnLis.onClick -= OnBackClicked;
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
        HideOtherExceptMe(heroSellPage);
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
