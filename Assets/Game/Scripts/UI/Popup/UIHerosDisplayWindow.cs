using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Heros display window controller.
/// </summary>
public class UIHerosDisplayWindow : Window
{
    #region private Fields

    private GameObject heroTemplate;
    private GameObject heroItems;
    private UIEventListener heroViewLis;
    private UIEventListener teamBuildLis;
    private UIEventListener heroMixLis;
    private UIEventListener heroSellLis;
    private UIEventListener backBtnLis;
    private Transform heroViewPage;
    private Transform teamBuildPage;
    private Transform heroMixPage;
    private Transform heroSellPage;

    #endregion

    #region Window

    public override void OnEnter()
    {
        HideOtherExceptMe(heroViewPage);
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

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
        heroItems = Utils.FindChild(heroViewPage.transform, "HeroItems").gameObject;
        for (int i = 0; i < 50; i++)
        {
            var obj = Instantiate(heroTemplate) as GameObject;
            obj.transform.parent = heroItems.transform;
        }
        heroItems.GetComponent<UIGrid>().Reposition();
        heroTemplate.SetActive(false);
    }

    private void InstallHandlers()
    {
        heroViewLis.onClick += OnHeroViewClicked;
        teamBuildLis.onClick += OnTeamBuildClicked;
        heroMixLis.onClick += OnHeroMixClicked;
        heroSellLis.onClick += OnHeroSellClicked;
        backBtnLis.onClick += OnBackClicked;
    }

    private void UnInstallHandlers()
    {
        heroViewLis.onClick -= OnHeroViewClicked;
        teamBuildLis.onClick -= OnTeamBuildClicked;
        heroMixLis.onClick -= OnHeroMixClicked;
        heroSellLis.onClick -= OnHeroSellClicked;
        backBtnLis.onClick -= OnBackClicked;
    }

    private void OnBackClicked(GameObject sender)
    {
        WindowManager.Instance.Show(typeof(UIHerosDisplayWindow), false);
    }

    private void OnHeroSellClicked(GameObject sender)
    {
        HideOtherExceptMe(heroSellPage);
    }

    private void OnHeroMixClicked(GameObject sender)
    {
        HideOtherExceptMe(heroMixPage);
    }

    private void OnTeamBuildClicked(GameObject sender)
    {
        WindowManager.Instance.Show(typeof (UITeamBuildWindow), true);
    }

    private void OnHeroViewClicked(GameObject sender)
    {
        HideOtherExceptMe(heroViewPage);
    }

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
