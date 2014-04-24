using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

    private const string jobPrefix = "icon_zhiye_";


    #endregion

    #region Window

    public override void OnEnter()
    {
        HideOtherExceptMe(heroViewPage);
        InstallHandlers(); 
        StartCoroutine(FillHeroList());
        var scHeroList = HeroModelLocator.Instance.SCHeroList;
        heroNums.text = string.Format("{0}/{1}", scHeroList.HeroList.Count, scHeroList.HeroCountLimit);
        Refresh();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Refresh the hero list.
    /// </summary>
    private void Refresh()
    {
        var orderType = HeroModelLocator.Instance.SCHeroList.OrderType;
        HeroModelLocator.Instance.SortHeroList(orderType, HeroModelLocator.Instance.SCHeroList.HeroList);
        for (int i = 0; i < HeroModelLocator.Instance.SCHeroList.HeroList.Count; i++)
        {
            var item = heroItems.transform.GetChild(i);
            ShowHero(orderType, item, HeroModelLocator.Instance.SCHeroList.HeroList[i].TemplateId);
        }
    }

    /// <summary>
    /// Show the info of the hero.
    /// </summary>
    /// <param name="orderType">The order type of </param>
    /// <param name="heroTran"></param>
    /// <param name="templateId"></param>
    private void ShowHero(short orderType, Transform heroTran, int templateId)
    {
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[templateId];
        switch (orderType)
        {
            //»Î ÷À≥–Ú≈≈–Ú
            case 0:
                break;

            //Œ‰Ω´÷∞“µ≈≈–Ú
            case 1:

                var jobSymobl = Utils.FindChild(heroTran, "JobSymbol").GetComponent<UISprite>();
                var attack = Utils.FindChild(heroTran, "Attack").GetComponent<UILabel>();
                jobSymobl.spriteName = jobPrefix + heroTemplate.Job;
                attack.text = heroTemplate.Attack.ToString(CultureInfo.InvariantCulture);
                break;

            //Œ‰Ω´œ°”–∂»≈≈–Ú
            case 3:

                break;

            //’’∂”ŒÈÀ≥–Ú≈≈–Ú
            case 4:
                break;

            //π•ª˜¡¶≈≈–Ú
            case 5:

                break;

            //HP≈≈–Ú
            case 6:
  
                break;

            //ªÿ∏¥¡¶≈≈–Ú
            case 7:

                break;

            //µ»º∂≈≈–Ú
            case 8:
                break;   
        }
    }

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

        heroTemplate = Utils.FindChild(transform, "HeroTemplate").gameObject;
        heroItems = Utils.FindChild(heroViewPage.transform, "HeroItems").GetComponent<UIGrid>();
        heroNums = Utils.FindChild(heroViewPage.transform, "HeroNums").GetComponent<UILabel>();
    }
    
    /// <summary>
    /// Fill in the hero game objects.
    /// </summary> 
    private IEnumerator FillHeroList()
    {
        var heroCount = HeroModelLocator.Instance.SCHeroList.HeroList.Count;
        for (int i = 0; i < heroCount; i++)
        {
            var item = Instantiate(heroTemplate) as GameObject;
            item.transform.parent = heroItems.transform;
        }
        heroItems.Reposition();
        heroTemplate.SetActive(false);
        yield return new WaitForEndOfFrame();
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
