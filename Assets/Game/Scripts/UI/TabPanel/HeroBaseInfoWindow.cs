using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class HeroBaseInfoWindow : Window
{
    private UIEventListener skillBtnLis;
    private UIEventListener lvBtnLis;
    private UIEventListener limitBtnLis;

    private HeroInfo heroInfo;
    private HeroTemplate heroTemplate;
    public static long CurUuid;

    #region Window

    public override void OnEnter()
    {
        Toggle(1);
        heroInfo = HeroModelLocator.Instance.FindHero(CurUuid);
        heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        Refresh();
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        skillBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Skill").gameObject);
        lvBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-LV").gameObject);
        limitBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Limit").gameObject);
    }

    private void InstallHandlers()
    {
        skillBtnLis.onClick += OnSkillBtnClicked;
        lvBtnLis.onClick += OnLvBtnClicked;
        limitBtnLis.onClick += OnLimitBtnClicked;
    }

    private void UnInstallHandlers()
    {
        skillBtnLis.onClick -= OnSkillBtnClicked;
        lvBtnLis.onClick -= OnLvBtnClicked;
        limitBtnLis.onClick -= OnLimitBtnClicked;
    }

    private void Refresh()
    {
        transform.FindChild("Name").GetComponent<UILabel>().text = heroTemplate.Name;
        var stars = transform.FindChild("Stars");
        for (int index = 0; index < heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = heroTemplate.Star; index < stars.childCount; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
        }
        Utils.FindChild(transform, "LV-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", heroInfo.Lvl, heroTemplate.LvlLimit);
        Utils.FindChild(transform, "Limit-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", heroInfo.BreakTimes, heroTemplate.BreakLimit);
        Utils.FindChild(transform, "Luck-Value").GetComponent<UILabel>().text = heroTemplate.Lucky.ToString();
        Utils.FindChild(transform, "Job-Value").GetComponent<UISprite>().spriteName = UIHerosDisplayWindow.JobPrefix + heroTemplate.Job;
    }

    private void OnLimitBtnClicked(GameObject go)
    {
        
    }

    private void OnLvBtnClicked(GameObject go)
    {
        Toggle(2);
        WindowManager.Instance.Show(typeof(UILevelUpWindow), true);
    }

    private void OnSkillBtnClicked(GameObject go)
    {
        Toggle(1);
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
    }

    public void ShowButtons(bool show)
    {
        NGUITools.SetActive(skillBtnLis.gameObject, show);
        NGUITools.SetActive(lvBtnLis.gameObject, show);
        NGUITools.SetActive(limitBtnLis.gameObject, show);
    }

    public void Toggle(int index)
    {
        switch(index)
        {
            case 1:
                skillBtnLis.GetComponent<UISprite>().spriteName = "BtnD";
                lvBtnLis.GetComponent<UISprite>().spriteName = "BtnN";
                limitBtnLis.GetComponent<UISprite>().spriteName = "BtnN";
                break;
            case 2:
                lvBtnLis.GetComponent<UISprite>().spriteName = "BtnD";
                skillBtnLis.GetComponent<UISprite>().spriteName = "BtnN";
                limitBtnLis.GetComponent<UISprite>().spriteName = "BtnN";
                break;
            case 3:
                limitBtnLis.GetComponent<UISprite>().spriteName = "BtnD";
                skillBtnLis.GetComponent<UISprite>().spriteName = "BtnN";
                lvBtnLis.GetComponent<UISprite>().spriteName = "BtnN";
                break;
        }
    }

    #endregion
}
