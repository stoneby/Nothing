using System.Globalization;
using System.Linq;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIHeroInfoWindow : Window
{
    private UIEventListener skillBtnLis;
    private UIEventListener lvBtnLis;
    private UIEventListener limitBtnLis;
    private UIEventListener mixBtnLis;
    private UIEventListener backBtnLis;

    public static int TemplateId;

    private GameObject star;
    private GameObject stars;

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
        RefreshData();
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
        mixBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Mix").gameObject);
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);

        star = Utils.FindChild(transform, "Star").gameObject;
        stars = Utils.FindChild(transform, "Stars").gameObject;

        star.SetActive(false);
    }


    private void RefreshData()
    {
        Utils.FindChild(transform, "Name").GetComponent<UILabel>().text =
            HeroModelLocator.Instance.HeroTemplates.HeroTmpl[TemplateId].Name;

        var starCount = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[TemplateId].Star;
        if (starCount > 1)
        {
            var spriteWidth = star.GetComponent<UISprite>().width;
            for (int i = 0; i < starCount - 1; i++)
            {
                var starObj = Instantiate(star) as GameObject;
                starObj.SetActive(true);
                starObj.transform.parent = stars.transform;
                starObj.transform.localPosition = new Vector3((i + 1) * spriteWidth, 0, 0);
                starObj.transform.localRotation = Quaternion.identity;
                starObj.transform.localScale = Vector3.one;
            }
        }
        var heroInfo = HeroModelLocator.Instance.SCHeroList.HeroList.Find(info => info.TemplateId == TemplateId);
        var heroTemp = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[TemplateId];
        var leaderSkillTemp = HeroModelLocator.Instance.SkillTemplates.SkillTmpl[heroTemp.LeaderSkill];
        var activeSkillTemp = HeroModelLocator.Instance.SkillTemplates.SkillTmpl[heroTemp.ActiveSkill];
        var spSkillTemp = HeroModelLocator.Instance.SkillTemplates.SkillTmpl[heroTemp.SpSkill];

        Utils.FindChild(transform, "AttackValue").GetComponent<UILabel>().text = heroInfo.Prop.ATK.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(transform, "HPValue").GetComponent<UILabel>().text = heroInfo.Prop.HP.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(transform, "RecoverValue").GetComponent<UILabel>().text = heroInfo.Prop.RECOVER.ToString(CultureInfo.InvariantCulture);
        Utils.FindChild(transform, "MPValue").GetComponent<UILabel>().text = heroInfo.Prop.MP.ToString(CultureInfo.InvariantCulture);

        Utils.FindChild(transform, "LV-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", heroInfo.Lvl, heroTemp.LvlLimit);
        Utils.FindChild(transform, "Limit-Value").GetComponent<UILabel>().text = string.Format("{0}/{1}", heroInfo.BreakTimes, heroTemp.BreakLimit);
        Utils.FindChild(transform, "Luck-Value").GetComponent<UILabel>().text = "20";
        Utils.FindChild(transform, "Job-Value").GetComponent<UISprite>().spriteName = UIHerosDisplayWindow.JobPrefix + heroTemp.Job;

        var activeSkill = Utils.FindChild(transform, "Skill-Active");
        var leaderSkill = Utils.FindChild(transform, "Skill-Leader");
        var spSkill = Utils.FindChild(transform, "Skill-SP");

        activeSkill.FindChild("Name").GetComponent<UILabel>().text = activeSkillTemp.Name;
        activeSkill.FindChild("Desc").GetComponent<UILabel>().text = activeSkillTemp.Desc;
        activeSkill.FindChild("Cost").GetComponent<UILabel>().text = activeSkillTemp.CostMp +"气";

        leaderSkill.FindChild("Name").GetComponent<UILabel>().text = leaderSkillTemp.Name;
        leaderSkill.FindChild("Desc").GetComponent<UILabel>().text = leaderSkillTemp.Desc;
        
        spSkill.FindChild("Name").GetComponent<UILabel>().text = spSkillTemp.Name;
        spSkill.FindChild("LV").GetComponent<UILabel>().text = "LV." + (spSkillTemp.Id % 10);
        spSkill.FindChild("Probability").GetComponent<UILabel>().text = "发动几率      " + spSkillTemp.OccorRate + "%";

    }

    private void InstallHandlers()
    {
        skillBtnLis.onClick += OnSkillBtnClicked;
        lvBtnLis.onClick += OnLvBtnClicked;
        limitBtnLis.onClick += OnLimitBtnClicked;
        mixBtnLis.onClick += OnMixBtnClicked;
        backBtnLis.onClick += OnBackBtnClicked;
    }

    private void UnInstallHandlers()
    {
        skillBtnLis.onClick -= OnSkillBtnClicked;
        lvBtnLis.onClick -= OnLvBtnClicked;
        limitBtnLis.onClick -= OnLimitBtnClicked;
        mixBtnLis.onClick -= OnMixBtnClicked;
        backBtnLis.onClick -= OnBackBtnClicked;
    }

    private void OnBackBtnClicked(GameObject go)
    {
        var list = stars.transform.Cast<Transform>().ToList();
        for (int i = list.Count - 1; i >= 0; i--)
        {
            Destroy(list[i].gameObject);
        }
        gameObject.SetActive(false);
    }

    private void OnMixBtnClicked(GameObject go)
    {

    }

    private void OnLimitBtnClicked(GameObject go)
    {

    }

    private void OnLvBtnClicked(GameObject go)
    {

    }

    private void OnSkillBtnClicked(GameObject go)
    {

    }

    #endregion
}
