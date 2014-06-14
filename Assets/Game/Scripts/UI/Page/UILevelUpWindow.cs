using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using Template;
using UnityEngine;

/// <summary>
/// The window to show level up.
/// </summary>
public class UILevelUpWindow : Window
{
    #region Public Fields

    /// <summary>
    /// The particle system to show the level up.
    /// </summary>
    public GameObject LevelUpEffect;

    #endregion

    #region Private Fields

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
    private HeroInfo heroInfo;
    private HeroTemplate heroTemplate;
    private UIEventListener overOkLis;
    private Transform smallHero;
    private Transform title;
    private Transform levelUp;
    private Transform soulCost;
    private short curLvl;
    private long totalCostSoul;
    private const int MinLvl = 1;
    private bool isLevelOver;
    private readonly int[] additions = new int[5];
    public static readonly Color NonChangedColor = Color.white;
    public static readonly Color ChangedColor = Color.cyan;
    public SCPropertyChangedNumber PropertyChangedNumber;

    #endregion

    #region Window

    public override void OnEnter()
    {
        heroInfo = HeroModelLocator.Instance.FindHero(HeroBaseInfoWindow.CurUuid);
        heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
        curLvl = heroInfo.Lvl;
        InstallHandlers();
        RefreshData();
        isLevelOver = false;
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Mono

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Awake()
    {
        backBtnLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Back").gameObject);
        addLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Add").gameObject);
        subLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Sub").gameObject);
        okLis = UIEventListener.Get(Utils.FindChild(transform, "Button-OK").gameObject);
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

        nextSoul = Utils.FindChild(transform, "NextSoul").FindChild("Value").GetComponent<UILabel>();
        ownedSoul = Utils.FindChild(transform, "OwnedSoul").FindChild("Value").GetComponent<UILabel>();
        usedSoul = Utils.FindChild(transform, "UsedSoul").FindChild("Value").GetComponent<UILabel>();
        smallHero = transform.FindChild("SmallHero");
        title = Utils.FindChild(baseup, "Title");
        levelUp = Utils.FindChild(baseup, "LevelUp");
        soulCost = transform.FindChild("SoulCost");
        NGUITools.SetActive(smallHero.gameObject, false);
        NGUITools.SetActive(levelUp.gameObject, false);
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        backBtnLis.onClick += OnBackBtnClicked;
        addLis.onClick += OnAddBtnClicked;
        subLis.onClick += OnSubLisClicked;
        okLis.onClick += OnOkBtnClicked;
    }

    /// <summary>
    /// UnInstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        backBtnLis.onClick -= OnBackBtnClicked;
        addLis.onClick -= OnAddBtnClicked;
        subLis.onClick -= OnSubLisClicked;
    }

    /// <summary>
    /// Update ui data.
    /// </summary>
    private void RefreshData()
    {
        if (curLvl == heroTemplate.LvlLimit)
        {
            addLis.GetComponent<UISprite>().color = Color.gray;
            addLis.GetComponent<BoxCollider>().enabled = false;
        }
        if (curLvl == heroInfo.Lvl)
        {
            subLis.GetComponent<UISprite>().color = Color.gray;
            subLis.GetComponent<BoxCollider>().enabled = false;
        }
        ownedSoul.text = PlayerModelLocator.Instance.Sprit.ToString();

        baseLvl.text = string.Format("{0}/{1}", curLvl, heroTemplate.LvlLimit);
        baseAtk.text = heroTemplate.Attack.ToString(CultureInfo.InvariantCulture);
        baseHp.text = heroTemplate.HP.ToString(CultureInfo.InvariantCulture);
        baseRec.text = heroTemplate.Recover.ToString(CultureInfo.InvariantCulture);
        baseMp.text = heroTemplate.MP.ToString(CultureInfo.InvariantCulture);
        RefreshLevelData();
    }

    /// <summary>
    /// Update level data.
    /// </summary>
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
        nextSoul.text = GetCostSoul(curLvl, heroTemplate.Star).ToString(CultureInfo.InvariantCulture);
        usedSoul.text = totalCostSoul.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Get the hero's cost soul with special level and rarity.
    /// </summary>
    /// <param name="lvl">The level of the hero.</param>
    /// <param name="starNum">The rarity of the hero.</param>
    /// <returns>The cost soul of the hero.</returns>
    private long GetCostSoul(short lvl, int starNum)
    {
        long soulValue = 0;
        switch (starNum)
        {
            case 1:
                soulValue = HeroModelLocator.Instance.HeroTemplates.LvlUpTmpl[lvl].CostSoulStar1;
                break;
            case 2:
                soulValue = HeroModelLocator.Instance.HeroTemplates.LvlUpTmpl[lvl].CostSoulStar2;
                break;
            case 3:
                soulValue = HeroModelLocator.Instance.HeroTemplates.LvlUpTmpl[lvl].CostSoulStar3;
                break;
            case 4:
                soulValue = HeroModelLocator.Instance.HeroTemplates.LvlUpTmpl[lvl].CostSoulStar4;
                break;
            case 5:
                soulValue = HeroModelLocator.Instance.HeroTemplates.LvlUpTmpl[lvl].CostSoulStar5;
                break;
        }
        return soulValue;
    }

    /// <summary>
    /// The callback of clicking ok button.
    /// </summary>
    private void OnOkBtnClicked(GameObject go)
    {
        if(curLvl > heroInfo.Lvl)
        {
            var csmsg = new CSHeroLvlUp { TargetLvl = curLvl, Uuid = HeroBaseInfoWindow.CurUuid };
            NetManager.SendMessage(csmsg);
        }
    }

    /// <summary>
    /// The callback of clicking back button.
    /// </summary>
    private void OnBackBtnClicked(GameObject go)
    {
        WindowManager.Instance.Show(typeof(UIHeroInfoWindow), true);
        WindowManager.Instance.GetWindow<HeroBaseInfoWindow>().Toggle(1);
        if(isLevelOver)
        {
            WindowManager.Instance.GetWindow<UIHeroInfoWindow>().ShowLevelUp(PropertyChangedNumber);
            WindowManager.Instance.GetWindow<HeroBaseInfoWindow>().ShowButtons(true);
            NGUITools.SetActive(title.gameObject, true);
            NGUITools.SetActive(smallHero.gameObject, false);
            NGUITools.SetActive(levelUp.gameObject, false);
            NGUITools.SetActive(soulCost.gameObject, true);
            StopCoroutine("PlayEffect");
        }
    }

    /// <summary>
    /// The callback of clicking increase level button.
    /// </summary>
    private void OnAddBtnClicked(GameObject go)
    {
        LevelUp(true);
        if(curLvl == heroTemplate.LvlLimit)
        {
            addLis.GetComponent<UISprite>().color = Color.gray;
            addLis.GetComponent<BoxCollider>().enabled = false;
        }
        if (curLvl == heroInfo.Lvl + 1)
        {
            subLis.GetComponent<UISprite>().color = Color.white;
            subLis.GetComponent<BoxCollider>().enabled = true;
        }
    }

    /// <summary>
    /// The callback of clicking decrease level button.
    /// </summary>
    private void OnSubLisClicked(GameObject go)
    {
        LevelUp(false);
        if (curLvl == heroTemplate.LvlLimit - 1)
        {
            addLis.GetComponent<UISprite>().color = Color.white;
            addLis.GetComponent<BoxCollider>().enabled = true;
        }
        if (curLvl == heroInfo.Lvl)
        {
            subLis.GetComponent<UISprite>().color = Color.gray;
            subLis.GetComponent<BoxCollider>().enabled = false;
        }
    }

    /// <summary>
    /// Update the array of addition when level up or down.
    /// </summary>
    /// <param name="up">If true, it will level up one level.</param>
    private void LevelUp(bool up)
    {
        var flag = up ? 1 : -1;
        if(curLvl == heroInfo.Lvl && flag == -1)
        {
            return;
        }
        totalCostSoul += (up ? GetCostSoul(curLvl, heroTemplate.Star) : GetCostSoul((short)(curLvl - 1), heroTemplate.Star)) * flag;
        curLvl += (short)flag;
        additions[0] += flag;
        additions[1] += heroTemplate.AttackAddtion * flag;
        additions[2] += heroTemplate.HPAddtion * flag;
        additions[3] += heroTemplate.RecoverAddtion * flag;
        additions[4] += heroTemplate.MPAddtion * flag;
        RefreshLevelData();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show level over result.
    /// </summary>
    public void ShowLevelOver()
    {
        isLevelOver = true;
        WindowManager.Instance.GetWindow<HeroBaseInfoWindow>().ShowButtons(false);
        NGUITools.SetActive(title.gameObject, false);
        NGUITools.SetActive(smallHero.gameObject, true);
        NGUITools.SetActive(levelUp.gameObject, true);
        NGUITools.SetActive(soulCost.gameObject, false);
        NGUITools.SetActive(LevelUpEffect.gameObject, true);
        StartCoroutine("PlayEffect", 1.5f);
    }

    private IEnumerator PlayEffect(float time)
    {
        var pss = LevelUpEffect.GetComponents<ParticleSystem>();
        foreach (var system in pss)
        {
            system.Play();
        }
        yield return new WaitForSeconds(time);
        foreach (var system in pss)
        {
            system.Stop();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            NGUITools.SetActive(LevelUpEffect.gameObject, true);
            var pss = LevelUpEffect.GetComponents<ParticleSystem>();
            foreach(var system in pss)
            {
                system.Play();
            }
        }
    }

    #endregion
}
