using System;
using System.Globalization;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class InfoShowingWindow : Window
{
    #region Private Fields

    private UIEventListener viewOkLis;
    private UISprite heroIcon;
    private UISprite itemIcon;

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        viewOkLis = UIEventListener.Get(transform.FindChild("ViewOK").gameObject);
        heroIcon = Utils.FindChild(transform, "HeroIcon").GetComponent<UISprite>();
        itemIcon = Utils.FindChild(transform, "ItemIcon").GetComponent<UISprite>();
    }

    private void InstallHandlers()
    {
        viewOkLis.onClick = OnViewOk;
    }

    private void UnInstallHandlers()
    {
        viewOkLis.onClick = null;
    }

    private void OnViewOk(GameObject go)
    {
        WindowManager.Instance.Show<InfoShowingWindow>(false);
    }

    private void RefreshHero(HeroInfo info)
    {
        heroIcon.enabled = true;
        itemIcon.enabled = false;
        var jobIcon = Utils.FindChild(transform, "JobIcon").GetComponent<UISprite>();
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[info.TemplateId];
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;

        var stars = transform.FindChild("RewardItem/Rarity");
        var starCount = stars.transform.childCount;
        for (int index = 0; index < heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.FindChild("Star" + (starCount - index - 1)).gameObject, true);
        }
        for (int index = starCount - heroTemplate.Star - 1; index >= 0; index--)
        {
            NGUITools.SetActive(stars.FindChild("Star" + index).gameObject, false);
        }

        RefreshProperty(heroTemplate.Attack, heroTemplate.HP, heroTemplate.Recover, heroTemplate.MP);
    }

    private void RefreshItem(ItemInfo info)
    {
        heroIcon.enabled = false;
        itemIcon.enabled = true;
        ShowItemJob(info);
        var stars = transform.FindChild("RewardItem/Rarity");
        var quality = ItemModeLocator.Instance.GetQuality(info.TmplId);
        quality = (sbyte)(quality / ItemType.QualitiesPerStar);
        var starCount = stars.transform.childCount;
        for (int index = 0; index < quality; index++)
        {
            NGUITools.SetActive(stars.FindChild("Star" + (starCount - index - 1)).gameObject, true);
        }
        for (int index = starCount - quality - 1; index >= 0; index--)
        {
            NGUITools.SetActive(stars.FindChild("Star" + index).gameObject, false);
        }

        var atk = ItemModeLocator.Instance.GetAttack(info.TmplId, info.Level);
        var hp = ItemModeLocator.Instance.GetHp(info.TmplId, info.Level);
        var recover = ItemModeLocator.Instance.GetRecover(info.TmplId, info.Level);
        var mp = ItemModeLocator.Instance.GetMp(info.TmplId);
        RefreshProperty(atk, hp, recover, mp);
    }

    private void ShowItemJob(ItemInfo info)
    {
        var jobIcon = Utils.FindChild(transform, "JobIcon").GetComponent<UISprite>();
        var job = ItemModeLocator.Instance.GetJob(info.TmplId);
        if (job == -1)
        {

        }
        else
        {
            jobIcon.spriteName = HeroConstant.HeroJobPrefix + job;
        }
    }

    private void RefreshProperty(int atk, int hp, int recover, int mp)
    {
        if (atk >= 0 && hp >= 0 && recover >= 0 && mp >= 0)
        {
            var properties = transform.FindChild("Properties");
            var atkLabel = Utils.FindChild(properties, "AttackValue").GetComponent<UILabel>();
            var hpLabel = Utils.FindChild(properties, "HPValue").GetComponent<UILabel>();
            var recoverLabel = Utils.FindChild(properties, "RecoverValue").GetComponent<UILabel>();
            var mpLabel = Utils.FindChild(properties, "MPValue").GetComponent<UILabel>();

            atkLabel.text = atk.ToString(CultureInfo.InvariantCulture);
            hpLabel.text = hp.ToString(CultureInfo.InvariantCulture);
            recoverLabel.text = recover.ToString(CultureInfo.InvariantCulture);
            mpLabel.text = mp.ToString(CultureInfo.InvariantCulture);
        }
    }

    #endregion

    #region Public Methods

    public void Refresh(ItemInfo item, HeroInfo hero)
    {
        if (item != null)
        {
            RefreshItem(item);
        }
        if (hero != null)
        {
            RefreshHero(hero);
        }
    }

    #endregion
}
