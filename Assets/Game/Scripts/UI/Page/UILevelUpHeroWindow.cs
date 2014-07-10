using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using Property;
using Template;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UILevelUpHeroWindow : Window
{
    private UIHeroCommonWindow herosWindow;
    private GameObject heroToLevelUp;
    private GameObject curLevelUpObject;
    private UIDragDropContainer dragDropContainer;

    private short curLvl;
    private HeroInfo heroInfo;

    private UIEventListener addLis;
    private UIEventListener subLis;
    private UIEventListener levelUpLis;
    private HeroTemplate heroTemplate;
    private UILabel levelTitle;
    private UILabel levelValue;
    private UILabel nextCostTitle;
    private UILabel nextCostValue;
    private UILabel ownedSoulValue;
    private UILabel usedSoulTitle;
    private UILabel usedSoulValue;
    private UILabel atk;
    private UILabel hp;
    private UILabel recover;
    private UILabel mp;
    private UILabel changedAtk;
    private UILabel changedHp;
    private UILabel changedRecover;
    private UILabel changedMp;
    private UISlider atkForeShowSlider;
    private UISlider atkSlider;
    private UISlider hpForeShowSlider;
    private UISlider hpSlider;
    private UISlider recoverForeShowSlider;
    private UISlider recoverSlider;
    private UISlider mpForeShowSlider;
    private UISlider mpSlider;
    private const int ConversionRate = 100;


    private int maxAtk;
    private int maxHp;
    private int maxRecover;
    private int maxMp;

    private long totalCostSoul;
    private readonly int[] additions = new int[5];

    private HeroInfo HeroInfo
    {
        get { return heroInfo; }
        set
        {
            heroInfo = value;
            var infoIsNotNull = heroInfo != null;
            if (infoIsNotNull)
            {
                curLvl = heroInfo.Lvl;
                heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpl[heroInfo.TemplateId];
                var addtionTimes = heroTemplate.LvlLimit - heroInfo.Lvl;
                maxAtk = heroInfo.Prop[RoleProperties.ROLE_ATK] + GetConverted(heroTemplate.AttackAddtion) * addtionTimes;
                maxHp = heroInfo.Prop[RoleProperties.ROLE_HP] + GetConverted(heroTemplate.HPAddtion) * addtionTimes;
                maxRecover = heroInfo.Prop[RoleProperties.ROLE_RECOVER] + GetConverted(heroTemplate.RecoverAddtion) * addtionTimes;
                maxMp = heroInfo.Prop[RoleProperties.ROLE_MP] + GetConverted(heroTemplate.MPAddtion) * addtionTimes;
            }
            ShowUnNeedObjects(infoIsNotNull);
        }
    }

    #region Window

    public override void OnEnter()
    {
        HeroInfo = null;
        herosWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
        herosWindow.NormalClicked = OnNormalClicked;
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        dragDropContainer = transform.Find("DragDropContainer").GetComponent<UIDragDropContainer>();
        addLis = UIEventListener.Get(Utils.FindChild(transform, "IncreaseBtn").gameObject);
        subLis = UIEventListener.Get(Utils.FindChild(transform, "DecreaseBtn").gameObject);
        levelUpLis = UIEventListener.Get(Utils.FindChild(transform, "LevelUpBtn").gameObject);
        levelTitle = Utils.FindChild(transform, "Level").GetComponent<UILabel>();
        levelValue = levelTitle.transform.Find("LevelValue").GetComponent<UILabel>();
        nextCostTitle = Utils.FindChild(transform, "NextSoul").GetComponent<UILabel>();
        nextCostValue = nextCostTitle.transform.Find("NextSoulValue").GetComponent<UILabel>();
        usedSoulTitle = Utils.FindChild(transform, "UsedSoul").GetComponent<UILabel>();
        usedSoulValue = usedSoulTitle.transform.Find("UsedSoulValue").GetComponent<UILabel>();
        var property = transform.Find("Property");
        atk = property.Find("Attack/AttackValue").GetComponent<UILabel>();
        changedAtk = property.Find("Attack/ChangedAtk").GetComponent<UILabel>();
        atkForeShowSlider = property.Find("Attack/ForeshowBar").GetComponent<UISlider>();
        atkSlider = property.Find("Attack/NormalBar").GetComponent<UISlider>();

        hp = property.Find("HP/HPValue").GetComponent<UILabel>();
        changedHp = property.Find("HP/ChangedHp").GetComponent<UILabel>();
        hpForeShowSlider = property.Find("HP/ForeshowBar").GetComponent<UISlider>();
        hpSlider = property.Find("HP/NormalBar").GetComponent<UISlider>();

        recover = property.Find("Recover/RecoverValue").GetComponent<UILabel>();
        changedRecover = property.Find("Recover/ChangedRecover").GetComponent<UILabel>();
        recoverForeShowSlider = property.Find("Recover/ForeshowBar").GetComponent<UISlider>();
        recoverSlider = property.Find("Recover/NormalBar").GetComponent<UISlider>();

        mp = property.Find("MP/MPValue").GetComponent<UILabel>();
        changedMp = property.Find("MP/ChangedMp").GetComponent<UILabel>();
        mpForeShowSlider = property.Find("MP/ForeshowBar").GetComponent<UISlider>();
        mpSlider = property.Find("MP/NormalBar").GetComponent<UISlider>();
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        addLis.onClick = OnAddBtnClicked;
        subLis.onClick = OnSubLisClicked;
        levelUpLis.onClick = OnOkBtnClicked;
    }

    /// <summary>
    /// UnInstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        addLis.onClick = null;
        subLis.onClick = null;
        levelUpLis.onClick = null;
    }

    private void ShowUnNeedObjects(bool show)
    {
        NGUITools.SetActive(levelTitle.gameObject, show);
        NGUITools.SetActive(nextCostTitle.gameObject, show);
        NGUITools.SetActive(usedSoulTitle.gameObject, show);
        if (show)
        {
            levelValue.text = string.Format("{0}/{1}", HeroInfo.Lvl, heroTemplate.LvlLimit);
            var initCostSoul = GetCostSoul(HeroInfo.Lvl, heroTemplate.Star);
            nextCostValue.text = initCostSoul.ToString(CultureInfo.InvariantCulture);
            totalCostSoul = 0;
            usedSoulValue.text = totalCostSoul.ToString(CultureInfo.InvariantCulture);

            atk.text = heroInfo.Prop[RoleProperties.ROLE_ATK].ToString(CultureInfo.InvariantCulture);
            atkSlider.value = (float) heroInfo.Prop[RoleProperties.ROLE_ATK] / maxAtk;

            hp.text = heroInfo.Prop[RoleProperties.ROLE_HP].ToString(CultureInfo.InvariantCulture);
            hpSlider.value = (float)heroInfo.Prop[RoleProperties.ROLE_HP] / maxHp;  
            
            recover.text = heroInfo.Prop[RoleProperties.ROLE_RECOVER].ToString(CultureInfo.InvariantCulture);
            recoverSlider.value = (float)heroInfo.Prop[RoleProperties.ROLE_RECOVER] / maxRecover;

            mp.text = heroInfo.Prop[RoleProperties.ROLE_MP].ToString(CultureInfo.InvariantCulture);
            mpSlider.value = (float)heroInfo.Prop[RoleProperties.ROLE_MP] / maxMp;
        }
    }

    private void OnNormalClicked(GameObject go)
    {
        heroToLevelUp = go;
        UIHeroSnapShotWindow.CurUuid = go.GetComponent<HeroItemBase>().Uuid;
        var snapShot = WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
        snapShot.InitTemplate("HeroSnapShot.Levelup", GotoLevelUp);
    }

    private void GotoLevelUp(GameObject go)
    {
        NGUITools.Destroy(curLevelUpObject);
        curLevelUpObject = NGUITools.AddChild(dragDropContainer.gameObject, heroToLevelUp);
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
        InitWindow(UIHeroSnapShotWindow.CurUuid);
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
        if (curLvl > heroInfo.Lvl)
        {
            var csmsg = new CSHeroLvlUp { TargetLvl = curLvl, Uuid = HeroBaseInfoWindow.CurUuid };
            NetManager.SendMessage(csmsg);
        }
    }

    private void CheckButtonEnabled()
    {
        if (curLvl == heroTemplate.LvlLimit)
        {
            addLis.GetComponent<UIButton>().isEnabled = false;
        }
        else if (curLvl == heroInfo.Lvl)
        {
            subLis.GetComponent<UIButton>().isEnabled = false;
        }
        else
        {
            addLis.GetComponent<UIButton>().isEnabled = true;
            subLis.GetComponent<UIButton>().isEnabled = true;
        }
    }

    /// <summary>
    /// The callback of clicking increase level button.
    /// </summary>
    private void OnAddBtnClicked(GameObject go)
    {
        LevelUp(true);
        CheckButtonEnabled();
    }

    /// <summary>
    /// The callback of clicking decrease level button.
    /// </summary>
    private void OnSubLisClicked(GameObject go)
    {
        LevelUp(false);
        CheckButtonEnabled();
    }

    /// <summary>
    /// Update the array of addition when level up or down.
    /// </summary>
    /// <param name="up">If true, it will level up one level.</param>
    private void LevelUp(bool up)
    {
        var flag = up ? 1 : -1;
        if (curLvl == heroInfo.Lvl && flag == -1)
        {
            return;
        }
        totalCostSoul += (up ? GetCostSoul(curLvl, heroTemplate.Star) : GetCostSoul((short)(curLvl - 1), heroTemplate.Star)) * flag;
        curLvl += (short)flag;
        additions[0] += flag;
        additions[1] += GetConverted(heroTemplate.AttackAddtion) * flag;
        additions[2] += GetConverted(heroTemplate.HPAddtion) * flag;
        additions[3] += GetConverted(heroTemplate.RecoverAddtion) * flag;
        additions[4] += GetConverted(heroTemplate.MPAddtion) * flag;
        RefreshLevelData();
    }

    private int GetConverted(int value)
    {
        return Mathf.RoundToInt((float) value / ConversionRate);
    }

    private void RefreshLevelData()
    {
        levelValue.text = string.Format("{0}/{1}", curLvl, heroTemplate.LvlLimit);
        changedAtk.text = additions[1] > 0 ? "+" + additions[1] : "";
        atkForeShowSlider.value = (float)(HeroInfo.Prop[RoleProperties.ROLE_ATK] + additions[1]) / maxAtk;
        changedHp.text = additions[2] > 0 ? "+" + additions[2] : "";
        hpForeShowSlider.value = (float)(HeroInfo.Prop[RoleProperties.ROLE_HP] + additions[2]) / maxHp;
        changedRecover.text = additions[3] > 0 ? "+" + additions[3] : "";
        recoverForeShowSlider.value = (float) (HeroInfo.Prop[RoleProperties.ROLE_RECOVER] + additions[3]) / maxRecover;
        nextCostValue.text = GetCostSoul(curLvl, heroTemplate.Star).ToString(CultureInfo.InvariantCulture);
        usedSoulValue.text = totalCostSoul.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Update ui data.
    /// </summary>
    private void RefreshData()
    {
        if(heroInfo != null)
        {
            CheckButtonEnabled();
        }
    }

    #endregion

    public void InitWindow(long uuid)
    {
        HeroInfo = HeroModelLocator.Instance.FindHero(uuid);
        RefreshData();
    }
}
