using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UILevelUpWindow : Window
{
    public GameObject StarPrefab;
    private UIEventListener skillBtnLis;
    private UIEventListener lvBtnLis;
    private UIEventListener limitBtnLis;
    private UIEventListener backBtnLis;
    private UIEventListener addLis;
    private UIEventListener subLis;
    private UIEventListener okLis;

    private UILabel baseLvl;
    private UILabel baseAtk;
    private UILabel baseHp;
    private UILabel baseRec;
    private UILabel baseMp;  
    
    private UILabel adjLvl;
    private UILabel adjAtk;
    private UILabel adjHp;
    private UILabel adjRec;
    private UILabel adjMp;
   
    private UILabel nextSoul;
    private UILabel ownedSoul;
    private UILabel usedSoul;
    private UILabel heroName;
    private GameObject stars;

    private GameObject property;
    private HeroInfo heroInfo;
    private HeroTemplate heroTemplate;
    private GameObject levelOver;
    private UIEventListener overOkLis;

    private short curLvl;
    private long totalCostSoul;
    public static readonly Color NonChangedColor = Color.white;
    public static readonly Color ChangedColor = Color.cyan;
    public SCPropertyChangedNumber PropertyChangedNumber;

    private readonly int[] additions = new int[5];

    #region Window

    public override void OnEnter()
    {
        heroInfo = HeroModelLocator.Instance.FindHero(UIHeroInfoWindow.Uuid);
        heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        curLvl = heroInfo.Lvl;
        InstallHandlers();
        RefreshData();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        CleanUp();
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        skillBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Skill").gameObject);
        lvBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-LV").gameObject);
        limitBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Limit").gameObject);
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        addLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Add").gameObject);
        subLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sub").gameObject);
        okLis = UIEventListener.Get(Utils.FindChild(transform, "Button-OK").gameObject);
        property = Utils.FindChild(transform, "Property").gameObject;
        var baseup = Utils.FindChild(transform, "BaseUp");
        baseLvl = Utils.FindChild(baseup, "Level-Original").GetComponent<UILabel>();
        baseAtk = Utils.FindChild(baseup, "Attack-Original").GetComponent<UILabel>();
        baseHp = Utils.FindChild(baseup, "HP-Original").GetComponent<UILabel>();
        baseRec = Utils.FindChild(baseup, "Recover-Original").GetComponent<UILabel>();
        baseMp = Utils.FindChild(baseup, "MP-Original").GetComponent<UILabel>();

        adjLvl = Utils.FindChild(baseup, "Level-Later").GetComponent<UILabel>();
        adjAtk = Utils.FindChild(baseup, "Attack-Later").GetComponent<UILabel>();
        adjHp = Utils.FindChild(baseup, "HP-Later").GetComponent<UILabel>();
        adjRec = Utils.FindChild(baseup, "Recover-Later").GetComponent<UILabel>();
        adjMp = Utils.FindChild(baseup, "MP-Later").GetComponent<UILabel>();
        heroName = Utils.FindChild(transform, "Name").GetComponent<UILabel>();
        stars = Utils.FindChild(transform, "Stars").gameObject;

        nextSoul = Utils.FindChild(transform, "NextSoul").FindChild("Value").GetComponent<UILabel>();
        ownedSoul = Utils.FindChild(transform, "OwnedSoul").FindChild("Value").GetComponent<UILabel>();
        usedSoul = Utils.FindChild(transform, "UsedSoul").FindChild("Value").GetComponent<UILabel>();
        levelOver = Utils.FindChild(transform, "LevelOver").gameObject;
        overOkLis = UIEventListener.Get(Utils.FindChild(levelOver.transform, "Button-OK").gameObject);
        NGUITools.SetActiveChildren(levelOver, false);
    }

    private void InstallHandlers()
    {
        skillBtnLis.onClick += OnSkillBtnClicked;
        lvBtnLis.onClick += OnLvBtnClicked;
        limitBtnLis.onClick += OnLimitBtnClicked;
        backBtnLis.onClick += OnBackBtnClicked;
        addLis.onClick += OnAddBtnClicked;
        subLis.onClick += OnSubLisClicked;
        okLis.onClick += OnOkBtnClicked;
        overOkLis.onClick += OnOverOkBtnClicked;
    }

    private void UnInstallHandlers()
    {
        skillBtnLis.onClick -= OnSkillBtnClicked;
        lvBtnLis.onClick -= OnLvBtnClicked;
        limitBtnLis.onClick -= OnLimitBtnClicked;
        backBtnLis.onClick -= OnBackBtnClicked;
        addLis.onClick -= OnAddBtnClicked;
        subLis.onClick -= OnSubLisClicked;
        overOkLis.onClick -= OnOverOkBtnClicked;
    }

    private void RefreshData()
    {
        ownedSoul.text = "150000";
        heroName.text = heroTemplate.Name;
        var spriteWidth = StarPrefab.GetComponent<UISprite>().width;
        for (int index = 0; index < heroTemplate.Star; index++)
        {
            var starObj = NGUITools.AddChild(stars, StarPrefab);
            starObj.transform.localPosition = new Vector3((index + 1) * spriteWidth, 0, 0);
        }
        var atkLabel = Utils.FindChild(property.transform, "Attack-Value").GetComponent<UILabel>();
        var hpLabel = Utils.FindChild(property.transform, "HP-Value").GetComponent<UILabel>();
        var recLabel = Utils.FindChild(property.transform, "Recover-Value").GetComponent<UILabel>();
        var mpLabel = Utils.FindChild(property.transform, "MP-Value").GetComponent<UILabel>();
        atkLabel.text = heroInfo.Prop[RoleProperties.HERO_ATK].ToString(CultureInfo.InvariantCulture);
        hpLabel.text = heroInfo.Prop[RoleProperties.HERO_HP].ToString(CultureInfo.InvariantCulture);
        recLabel.text = heroInfo.Prop[RoleProperties.HERO_RECOVER].ToString(CultureInfo.InvariantCulture);
        mpLabel.text = heroInfo.Prop[RoleProperties.HERO_MP].ToString(CultureInfo.InvariantCulture);

        baseLvl.text = string.Format("{0}/{1}", curLvl, heroTemplate.LvlLimit);
        baseAtk.text = heroTemplate.Attack.ToString(CultureInfo.InvariantCulture);
        baseHp.text = heroTemplate.HP.ToString(CultureInfo.InvariantCulture);
        baseRec.text = heroTemplate.Recover.ToString(CultureInfo.InvariantCulture);
        baseMp.text = heroTemplate.MP.ToString(CultureInfo.InvariantCulture);
        RefreshLevelData();
    }

    private void RefreshLevelData()
    {
        var list = new List<int> { heroInfo.Lvl, heroTemplate.Attack, heroTemplate.HP, heroTemplate.Recover, heroTemplate.MP };
        var labelList = new List<UILabel> {adjLvl, adjAtk, adjHp, adjRec, adjMp};
        for (int index = 0; index < list.Count; index++)
        {
            if(additions[index] == 0)
            {
                labelList[index].color = NonChangedColor;
                labelList[index].text = list[index].ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                labelList[index].color = ChangedColor;
                labelList[index].text = string.Format("{0}(+{1})", additions[index] + list[index], additions[index]);
            }
        }
        nextSoul.text = HeroModelLocator.Instance.HeroTemplates.LvlUpTmpl[curLvl].CostSoul.ToString(CultureInfo.InvariantCulture);
        usedSoul.text = totalCostSoul.ToString(CultureInfo.InvariantCulture);
    }

    public void ShowLevelOver()
    {
        var overLvl = Utils.FindChild(levelOver.transform, "Level-Later").GetComponent<UILabel>();
        var overAtk = Utils.FindChild(levelOver.transform, "Attack-Later").GetComponent<UILabel>();
        var overHp = Utils.FindChild(levelOver.transform, "HP-Later").GetComponent<UILabel>();
        var overRec = Utils.FindChild(levelOver.transform, "Recover-Later").GetComponent<UILabel>();
        var overMp = Utils.FindChild(levelOver.transform, "MP-Later").GetComponent<UILabel>();
        var list = new List<int> { heroInfo.Lvl, heroTemplate.Attack, heroTemplate.HP, heroTemplate.Recover, heroTemplate.MP };
        var labelList = new List<UILabel> { overLvl, overAtk, overHp, overRec, overMp };
        for (int index = 0; index < list.Count; index++)
        {
            if (additions[index] == 0)
            {
                labelList[index].color = NonChangedColor;
                labelList[index].text = list[index].ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                labelList[index].color = ChangedColor;
                labelList[index].text = string.Format("{0}         (+{1})", additions[index] + list[index], additions[index]);
            }
        }
        NGUITools.SetActiveChildren(levelOver, true);
    }

    private void OnOverOkBtnClicked(GameObject go)
    {
        var heroInfoWindow = WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
        heroInfoWindow.GetComponent<UIHeroInfoWindow>().ShowLevelUp(PropertyChangedNumber);
    }

    private void OnOkBtnClicked(GameObject go)
    {
        if(curLvl > heroInfo.Lvl)
        {
            var csmsg = new CSHeroLvlUp { TargetLvl = curLvl, Uuid = UIHeroInfoWindow.Uuid };
            NetManager.SendMessage(csmsg);
        }
    }

    private void OnSkillBtnClicked(GameObject go)
    {
        
    }

    private void OnLvBtnClicked(GameObject go)
    {
        
    }

    private void OnLimitBtnClicked(GameObject go)
    {

    }

    private void OnBackBtnClicked(GameObject go)
    {
        CleanUp();
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
    }

    private void OnAddBtnClicked(GameObject go)
    {
        LevelUp(true);
    }

    private void OnSubLisClicked(GameObject go)
    {
        LevelUp(false);
    }

    private void LevelUp(bool up)
    {
        var flag = up ? 1 : -1;
        if(curLvl == heroInfo.Lvl && flag == -1)
        {
            return;
        }
        totalCostSoul += HeroModelLocator.Instance.HeroTemplates.LvlUpTmpl[up ? curLvl + 1 : curLvl].CostSoul;
        curLvl += (short)flag;
        additions[0] += flag;
        additions[1] += heroTemplate.AttackAddtion * flag;
        additions[2] += heroTemplate.HPAddtion * flag;
        additions[3] += heroTemplate.RecoverAddtion * flag;
        additions[4] += heroTemplate.MPAddtion * flag;
        RefreshLevelData();
    }

    /// <summary>
    /// Do some clean up work before leaving this window.
    /// </summary>
    private void CleanUp()
    {
        var list = stars.transform.Cast<Transform>().ToList();
        for (int index = list.Count - 1; index >= 0; index--)
        {
            Destroy(list[index].gameObject);
        }
    }

    #endregion
}
