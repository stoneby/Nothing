using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using KXSGCodec;
using Property;
using Template;
using Template.Auto.Hero;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UILevelUpHeroWindow : Window
{
    #region Public Fields

    /// <summary>
    /// The particle system to show the level up.
    /// </summary>
    public GameObject LevelUpEffect;

    #endregion

    private UIHeroCommonWindow herosWindow;
    private GameObject heroToLevelUp;
    private GameObject curLevelUpObject;
    private UIDragDropContainer dragDropContainer;
    private GameObject mask;

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
    private UIEventListener backLis;

    private long totalCostSoul;
    private readonly int[] additions = new int[5];

    private HeroInfo HeroInfo
    {
        get { return heroInfo; }
        set
        {
            heroInfo = value;
            ResetData();
            var infoIsNotNull = heroInfo != null;
            if (infoIsNotNull)
            {
                curLvl = heroInfo.Lvl;
                heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[heroInfo.TemplateId];
                var addtionTimes = heroTemplate.LvlLimit - heroInfo.Lvl;
                maxAtk = heroInfo.Prop[RoleProperties.ROLE_ATK] + GetConverted(heroTemplate.AttackAddtion) * addtionTimes;
                maxHp = heroInfo.Prop[RoleProperties.ROLE_HP] + GetConverted(heroTemplate.HPAddtion) * addtionTimes;
                maxRecover = heroInfo.Prop[RoleProperties.ROLE_RECOVER] + GetConverted(heroTemplate.RecoverAddtion) * addtionTimes;
                maxMp = heroInfo.Prop[RoleProperties.ROLE_MP] + GetConverted(heroTemplate.MPAddtion) * addtionTimes;
                CheckButtonEnabled();
            }
            ShowUnNeedObjects(infoIsNotNull);
        }
    }

    private void CheckButtonEnabled()
    {
        //Make sure can not go lower than current level.
        subLis.GetComponent<UIButton>().isEnabled = heroInfo.Lvl != curLvl;
        //If it already reaches the limit, make sure can not level up any more.
        if(heroInfo.Lvl == heroTemplate.LvlLimit)
        {
            addLis.GetComponent<UIButton>().isEnabled = false;
            levelUpLis.GetComponent<UIButton>().isEnabled = false;
        }
        else if(GetCostSoul((short) (curLvl + 1), heroTemplate.Star) < PlayerModelLocator.Instance.Sprit)
        {
            addLis.GetComponent<UIButton>().isEnabled = true;
            levelUpLis.GetComponent<UIButton>().isEnabled = true;
        }
    }

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.LevelUpHeroWindow);
        HeroInfo = null;
        herosWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
        herosWindow.NormalClicked = OnNormalClickForLvlUp;
        InstallHandlers();
        HeroConstant.EnterType = HeroConstant.HeroDetailEnterType.LvlUp;
    }

    public override void OnExit()
    {
        MtaManager.TrackEndPage(MtaType.LevelUpHeroWindow);
        UnInstallHandlers();
        CleanupCurLvlUp();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        dragDropContainer = transform.Find("DragDropContainer").GetComponent<UIDragDropContainer>();
        addLis = UIEventListener.Get(transform.Find("Buttons/Button-Inc").gameObject);
        subLis = UIEventListener.Get(transform.Find("Buttons/Button-Dec").gameObject);
        levelUpLis = UIEventListener.Get(transform.Find("Buttons/Button-LevelUp").gameObject);
        backLis = UIEventListener.Get(transform.Find("Buttons/Button-Back").gameObject);
        levelTitle = Utils.FindChild(transform, "Level").GetComponent<UILabel>();
        levelValue = levelTitle.transform.Find("LevelValue").GetComponent<UILabel>();
        nextCostTitle = Utils.FindChild(transform, "NextSoul").GetComponent<UILabel>();
        nextCostValue = nextCostTitle.transform.Find("NextSoulValue").GetComponent<UILabel>();
        usedSoulTitle = Utils.FindChild(transform, "UsedSoul").GetComponent<UILabel>();
        usedSoulValue = usedSoulTitle.transform.Find("UsedSoulValue").GetComponent<UILabel>();
        ownedSoulValue = Utils.FindChild(transform, "OwnedSoulValue").GetComponent<UILabel>();
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

        mask = Utils.FindChild(transform, "Mask").gameObject;
        mask.SetActive(false);
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        addLis.onClick = OnAddBtnClicked;
        subLis.onClick = OnSubLisClicked;
        levelUpLis.onClick = OnOkBtnClicked;
        backLis.onClick = OnBack;
        CommonHandler.HeroPropertyChanged += OnHeroPeopertyChanged;
        CommonHandler.PlayerPropertyChanged += OnPlayerPropertyChanged;
    }

    /// <summary>
    /// UnInstall all handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        addLis.onClick = null;
        subLis.onClick = null;
        levelUpLis.onClick = null;
        backLis.onClick = null;
        CommonHandler.HeroPropertyChanged -= OnHeroPeopertyChanged;
        CommonHandler.PlayerPropertyChanged -= OnPlayerPropertyChanged;
    }

    private void OnHeroPeopertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        var lvl = scpropertychanged.PropertyChanged[RoleProperties.ROLEBASE_LEVEL];
        HeroInfo.Lvl = (short)lvl;
        levelValue.text = string.Format("{0}/{1}", lvl, heroTemplate.LvlLimit);
        var atkProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_ATK];
        HeroInfo.Prop[RoleProperties.ROLE_ATK] = atkProp;
        atk.text = atkProp.ToString(CultureInfo.InvariantCulture);
        atkSlider.value = (float)atkProp / maxAtk;

        var hpProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_HP];
        HeroInfo.Prop[RoleProperties.ROLE_HP] = hpProp;
        hp.text = hpProp.ToString(CultureInfo.InvariantCulture);
        hpSlider.value = (float)hpProp / maxHp;
        
        var recoverProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_RECOVER];
        HeroInfo.Prop[RoleProperties.ROLE_RECOVER] = recoverProp;
        recover.text = atkProp.ToString(CultureInfo.InvariantCulture);
        recoverSlider.value = (float)recoverProp / maxRecover;

        var mpProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_MP];
        HeroInfo.Prop[RoleProperties.ROLE_MP] = mpProp;
        mp.text = mpProp.ToString(CultureInfo.InvariantCulture);
        mpSlider.value = (float)mpProp / maxMp;

        CheckButtonEnabled();
    }

    private void OnPlayerPropertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        ownedSoulValue.text = PlayerModelLocator.Instance.Sprit.ToString(CultureInfo.InvariantCulture);
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
        else
        {
            atkSlider.value = 1;
            hpSlider.value = 1;
            recoverSlider.value = 1;
            mpSlider.value = 1;
        }
    }

    private void ResetData()
    {
        atkForeShowSlider.value = 0;
        hpForeShowSlider.value = 0;
        recoverForeShowSlider.value = 0;
        mpForeShowSlider.value = 0;
        changedAtk.text = "";
        changedHp.text = "";
        changedRecover.text = "";
        changedMp.text = "";
        totalCostSoul = 0;
        usedSoulValue.text = totalCostSoul.ToString(CultureInfo.InvariantCulture);
        for (var i = 0; i < additions.Length; i++)
        {
            additions[i] = 0;
        }
    }

    private void OnNormalClickForLvlUp(GameObject go)
    {
        heroToLevelUp = go;
        UIHeroSnapShotWindow.CurUuid = go.GetComponent<HeroItemBase>().Uuid;
        var snapShot = WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
        snapShot.InitTemplate("HeroOrItemSnapShot.Levelup", GotoLevelUp);
    }

    private void OnNormalClickForCancel(GameObject go)
    {
        UIHeroSnapShotWindow.CurUuid = go.GetComponent<HeroItemBase>().Uuid;
        var snapShot = WindowManager.Instance.Show<UIHeroSnapShotWindow>(true);
        snapShot.InitTemplate("HeroOrItemSnapShot.CancelLevelup", CancelLevelUp);
    }

    private void GotoLevelUp(GameObject go)
    {
        CleanupCurLvlUp();
        curLevelUpObject = NGUITools.AddChild(dragDropContainer.gameObject, heroToLevelUp);
        curLevelUpObject.GetComponent<HeroItemBase>().Uuid = heroToLevelUp.GetComponent<HeroItemBase>().Uuid;
        var child = NGUITools.AddChild(heroToLevelUp, mask);
        NGUITools.SetActive(child.gameObject, true);
        heroToLevelUp.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        curLevelUpObject.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForCancel;
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
        InitWindow(UIHeroSnapShotWindow.CurUuid);
    }

    private void CancelLevelUp(GameObject go)
    {
        CleanupCurLvlUp();
        WindowManager.Instance.Show<UIHeroSnapShotWindow>(false);
        ResetData();
    }

    private void CleanupCurLvlUp()
    {
        if(curLevelUpObject != null)
        {
            var uuidToCancel = curLevelUpObject.GetComponent<HeroItemBase>().Uuid;
            var heros = herosWindow.Heros.transform;
            for (var i = 0; i < heros.childCount; i++)
            {
                var hero = heros.GetChild(i);
                if (hero.GetComponent<HeroItemBase>().Uuid == uuidToCancel)
                {
                    NGUITools.Destroy(hero.Find("Mask(Clone)").gameObject);
                    hero.GetComponent<NGUILongPress>().OnNormalPress = OnNormalClickForLvlUp;
                }
            }
            NGUITools.Destroy(curLevelUpObject);
            curLevelUpObject = null;
        }
    }

    /// <summary>
    /// Get the hero's cost soul with special level and rarity.
    /// </summary>
    /// <param name="lvl">The level of the hero.</param>
    /// <param name="starNum">The rarity of the hero.</param>
    /// <returns>The cost soul of the hero.</returns>
    private long GetCostSoul(short lvl, int starNum)
    {
        return HeroModelLocator.Instance.HeroTemplates.LvlUpTmpls[lvl].CostSoul[starNum - 1];
    }

    /// <summary>
    /// The callback of clicking ok button.
    /// </summary>
    private void OnOkBtnClicked(GameObject go)
    {
        if (heroInfo != null)
        {
            if(curLvl > heroInfo.Lvl)
            {
                var csmsg = new CSHeroLvlUp {TargetLvl = curLvl, Uuid = heroInfo.Uuid};
                NetManager.SendMessage(csmsg);
            }
        }
    }

    /// <summary>
    ///  The callback of clicking back button.
    /// </summary>
    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<UILevelUpHeroWindow>(false);
        WindowManager.Instance.Show<UIHeroCommonWindow>(false);
    }

    /// <summary>
    /// The callback of clicking increase level button.
    /// </summary>
    private void OnAddBtnClicked(GameObject go)
    {
        LevelUp(true);
        if(curLvl == heroTemplate.LvlLimit || totalCostSoul + GetCostSoul((short)(curLvl + 1), heroTemplate.Star) > PlayerModelLocator.Instance.Sprit)
        {
            addLis.GetComponent<UIButton>().isEnabled = false;
        }
        if(curLvl == heroInfo.Lvl + 1)
        {
            subLis.GetComponent<UIButton>().isEnabled = true;
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
            addLis.GetComponent<UIButton>().isEnabled = true;
        }
        if (curLvl == heroInfo.Lvl)
        {
            subLis.GetComponent<UIButton>().isEnabled = false;
        }
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
            ownedSoulValue.text = PlayerModelLocator.Instance.Sprit.ToString(CultureInfo.InvariantCulture);
        }
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

    #endregion

    public void InitWindow(long uuid)
    {
        HeroInfo = HeroModelLocator.Instance.FindHero(uuid);
        RefreshData();
    }

    /// <summary>
    /// Show level over result.
    /// </summary>
    public void ShowLevelOver()
    {
        StartCoroutine("PlayEffect", 1.5f);
        ResetData();
    }

}
