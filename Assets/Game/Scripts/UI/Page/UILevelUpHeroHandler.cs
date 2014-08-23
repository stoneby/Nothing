using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
using Template;
using Template.Auto.Hero;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UILevelUpHeroHandler : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// The particle system to show the level up.
    /// </summary>
    public GameObject LevelUpEffect;

    public PropertyUpdater PropertyUpdater;

    #endregion

    #region Private Fields

    private UIHeroCommonWindow commonWindow;
    private short curLvl = -1;
    private UIEventListener addLis;
    private UIEventListener subLis;
    private UIEventListener levelUpLis;
    private UILabel nextCostTitle;
    private UILabel nextCostValue;
    private UILabel ownedSoulValue;
    private UILabel usedSoulTitle;
    private UILabel usedSoulValue;
    private const int ConversionRate = 100;
    private HeroInfo HeroInfo
    {
        get
        {
            if(commonWindow == null)
            {
                return null;
            }
            return commonWindow.HeroInfo;
        }
    }

    private HeroTemplate HeroTemplate
    {
        get
        {
            if (commonWindow == null)
            {
                return null;
            }
            return commonWindow.HeroTemplate;
        }
    }
    private Transform baseInfos;

    private long totalCostSoul;
    private readonly List<int> additions = new List<int>{0, 0, 0, 0, 0};

    #endregion

    #region Private Methods

    private void CheckButtonEnabled()
    {
        //Make sure can not go lower than current level.
        subLis.GetComponent<UIButton>().isEnabled = HeroInfo.Lvl != curLvl;
        //If it already reaches the limit, make sure can not level up any more.
        if (HeroInfo.Lvl == HeroTemplate.LvlLimit)
        {
            addLis.GetComponent<UIButton>().isEnabled = false;
            levelUpLis.GetComponent<UIButton>().isEnabled = false;
        }
        else if (GetCostSoul((short)(curLvl + 1), HeroTemplate.Star) < PlayerModelLocator.Instance.Sprit)
        {
            addLis.GetComponent<UIButton>().isEnabled = true;
            levelUpLis.GetComponent<UIButton>().isEnabled = true;
        }
    }

    private void OnEnable()
    {
        MtaManager.TrackBeginPage(MtaType.LevelUpHeroWindow);
        if (HeroInfo != null)
        {
            commonWindow.NormalClicked = OnNormalClick;
            commonWindow.ShowSelMask();
            InstallHandlers();
            ResetData();
            curLvl = HeroInfo.Lvl;
            CheckButtonEnabled();
            RefreshData();
        }
    }

    private void OnDisable()
    {
        MtaManager.TrackEndPage(MtaType.LevelUpHeroWindow);
        UnInstallHandlers();
    }

    // Use this for initialization
    private void Awake()
    {
        addLis = UIEventListener.Get(transform.Find("Buttons/Button-Add").gameObject);
        subLis = UIEventListener.Get(transform.Find("Buttons/Button-Sub").gameObject);
        levelUpLis = UIEventListener.Get(transform.Find("Buttons/Button-LvlUp").gameObject);
        nextCostTitle = Utils.FindChild(transform, "NextSoul").GetComponent<UILabel>();
        nextCostValue = nextCostTitle.transform.Find("NextSoulValue").GetComponent<UILabel>();
        usedSoulTitle = Utils.FindChild(transform, "UsedSoul").GetComponent<UILabel>();
        usedSoulValue = usedSoulTitle.transform.Find("UsedSoulValue").GetComponent<UILabel>();
        ownedSoulValue = Utils.FindChild(transform, "OwnedSoulValue").GetComponent<UILabel>();
        commonWindow = WindowManager.Instance.GetWindow<UIHeroCommonWindow>();
        baseInfos = transform.Find("BaseInfo");
        //As the property updater's Awake has the dependency with the window's OnEnter, we make it be called first.
        NGUITools.SetActive(PropertyUpdater.gameObject, true);
    }

    /// <summary>
    /// Install all handlers.
    /// </summary>
    private void InstallHandlers()
    {
        addLis.onClick = OnAddBtnClicked;
        subLis.onClick = OnSubLisClicked;
        levelUpLis.onClick = OnOkBtnClicked;
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
        CommonHandler.HeroPropertyChanged -= OnHeroPeopertyChanged;
        CommonHandler.PlayerPropertyChanged -= OnPlayerPropertyChanged;
    }

    private void OnHeroPeopertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        var level = scpropertychanged.PropertyChanged[RoleProperties.ROLEBASE_LEVEL];
        curLvl = HeroInfo.Lvl = (short)level;
        var atkProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_ATK];
        HeroInfo.Prop[RoleProperties.ROLE_ATK] = atkProp;
        var hpProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_HP];
        HeroInfo.Prop[RoleProperties.ROLE_HP] = hpProp;
        var recoverProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_RECOVER];
        HeroInfo.Prop[RoleProperties.ROLE_RECOVER] = recoverProp;
        var mpProp = scpropertychanged.PropertyChanged[RoleProperties.ROLE_MP];
        HeroInfo.Prop[RoleProperties.ROLE_MP] = mpProp;
        PropertyUpdater.UpdateProperty(level, HeroTemplate.LvlLimit, atkProp, hpProp, recoverProp, mpProp);

        CheckButtonEnabled();

        List<long> curTeamUuids;
        List<long> allTeamUuids;
        HeroModelLocator.Instance.GetTeamUuids(out curTeamUuids, out allTeamUuids, 1);
        var heroItem = commonWindow.CurSel.GetComponent<NewHeroItem>();
        heroItem.InitItem(HeroInfo, curTeamUuids, allTeamUuids);
        HeroUtils.ShowHero(HeroModelLocator.Instance.OrderType, heroItem, HeroInfo);
    }

    private void OnPlayerPropertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        ownedSoulValue.text = PlayerModelLocator.Instance.Sprit.ToString(CultureInfo.InvariantCulture);
    }

    private void ResetData()
    {
        totalCostSoul = 0;
        curLvl = -1;
        usedSoulValue.text = totalCostSoul.ToString(CultureInfo.InvariantCulture);
        for (var i = 0; i < additions.Count; i++)
        {
            additions[i] = 0;
        }
    }

    private void OnNormalClick(GameObject go)
    {
        ResetData();
        commonWindow.ShowSelMask(go.transform.position);
        commonWindow.CurSel = go;
        var uuid = go.GetComponent<NewHeroItem>().Uuid;
        commonWindow.HeroInfo = HeroModelLocator.Instance.FindHero(uuid);
        curLvl = HeroInfo.Lvl;
        CheckButtonEnabled();
        RefreshData();
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
        if (HeroInfo != null)
        {
            if (curLvl > HeroInfo.Lvl)
            {
                var csmsg = new CSHeroLvlUp { TargetLvl = curLvl, Uuid = HeroInfo.Uuid };
                NetManager.SendMessage(csmsg);
            }
        }
    }

    /// <summary>
    /// The callback of clicking increase level button.
    /// </summary>
    private void OnAddBtnClicked(GameObject go)
    {
        LevelUp(true);
        if (curLvl == HeroTemplate.LvlLimit || totalCostSoul + GetCostSoul((short)(curLvl + 1), HeroTemplate.Star) > PlayerModelLocator.Instance.Sprit)
        {
            addLis.GetComponent<UIButton>().isEnabled = false;
        }
        if(curLvl == HeroInfo.Lvl + 1)
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
        if (curLvl == HeroTemplate.LvlLimit - 1)
        {
            addLis.GetComponent<UIButton>().isEnabled = true;
        }
        if (curLvl == HeroInfo.Lvl)
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
        if (curLvl == HeroInfo.Lvl && flag == -1)
        {
            return;
        }
        totalCostSoul += (up ? GetCostSoul(curLvl, HeroTemplate.Star) : GetCostSoul((short)(curLvl - 1), HeroTemplate.Star)) * flag;
        curLvl += (short)flag;
        additions[0] += flag;
        additions[1] += GetConverted(HeroTemplate.AttackAddtion) * flag;
        additions[2] += GetConverted(HeroTemplate.HPAddtion) * flag;
        additions[3] += GetConverted(HeroTemplate.RecoverAddtion) * flag;
        additions[4] += GetConverted(HeroTemplate.MPAddtion) * flag;
        RefreshLevelData();
    }

    private int GetConverted(int value)
    {
        return Mathf.RoundToInt((float) value / ConversionRate);
    }

    private void RefreshLevelData()
    {
        var changed = additions.Any(addition => addition > 0);
        if(!changed)
        {
            var atk = HeroInfo.Prop[RoleProperties.ROLE_ATK];
            var hp = HeroInfo.Prop[RoleProperties.ROLE_HP];
            var recover = HeroInfo.Prop[RoleProperties.ROLE_RECOVER];
            var mp = HeroInfo.Prop[RoleProperties.ROLE_MP];
            PropertyUpdater.UpdateProperty(HeroInfo.Lvl, HeroTemplate.LvlLimit, atk, hp, recover, mp);
        }
        else
        {
            PropertyUpdater.PreShowChangedProperty(additions[0], additions[1], additions[2], additions[3], additions[4]);
        }
        nextCostValue.text = GetCostSoul(curLvl, HeroTemplate.Star).ToString(CultureInfo.InvariantCulture);
        usedSoulValue.text = totalCostSoul.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Update ui data.
    /// </summary>
    private void RefreshData()
    {
        RefreshLevelData();
        if(HeroInfo != null)
        {
            ownedSoulValue.text = PlayerModelLocator.Instance.Sprit.ToString(CultureInfo.InvariantCulture);
        }
        baseInfos.FindChild("Name").GetComponent<UILabel>().text = HeroTemplate.Name;
        var stars = baseInfos.Find("Stars");
        for (int index = stars.childCount - 1; index >= stars.childCount - HeroTemplate.Star; index--)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, true);
        }
        for (int index = 0; index < stars.childCount - HeroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.GetChild(index).gameObject, false);
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

    #region Public Methods

    /// <summary>
    /// Show level over result.
    /// </summary>
    public void ShowLevelOver()
    {
        StartCoroutine("PlayEffect", 1.5f);
        ResetData();
    }

    #endregion
}
