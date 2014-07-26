using System;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class TenLotteryResultDispWindow : Window
{
    #region Private Fields

    private UIEventListener viewOkLis;
    private Transform rewardItemsTran;
    //For Demo
    private Transform lastItem;

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
        rewardItemsTran = transform.FindChild("RewardItems");
        lastItem = rewardItemsTran.FindChild("LastItem");
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
        WindowManager.Instance.Show<TenLotteryResultDispWindow>(false);
    }


    private void RefreshReward(Transform addedItem, HeroInfo info)
    {
        SetHeroIcon(addedItem, true);
        var jobIcon = Utils.FindChild(addedItem, "JobIcon").GetComponent<UISprite>();
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[info.TemplateId];
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;

        var stars = addedItem.FindChild("Rarity");
        var starCount = stars.transform.childCount;
        for (int index = 0; index < heroTemplate.Star; index++)
        {
            NGUITools.SetActive(stars.FindChild("Star" + (starCount - index - 1)).gameObject, true);
        }
        for (int index = starCount - heroTemplate.Star - 1; index >= 0; index--)
        {
            NGUITools.SetActive(stars.FindChild("Star" + index).gameObject, false);
        }
    }

    private static void SetHeroIcon(Transform addedItem, bool isHeroIcon)
    {
        var heroIcon = Utils.FindChild(addedItem, "HeroIcon").GetComponent<UISprite>();
        var itemIcon = Utils.FindChild(addedItem, "ItemIcon").GetComponent<UISprite>();
        heroIcon.enabled = isHeroIcon;
        itemIcon.enabled = !isHeroIcon;
    }

    private void RefreshItem(Transform addedItem, ItemInfo info)
    {
        SetHeroIcon(addedItem, false);
        var stars = addedItem.FindChild("Rarity");
        var quality = ItemModeLocator.Instance.GetQuality(info.TmplId);
        quality = (sbyte)(quality / ItemType.QualitiesPerStar);
        var starCount = stars.transform.childCount;
        for (int index = 0; index <= quality; index++)
        {
            NGUITools.SetActive(stars.FindChild("Star" + (starCount - index - 1)).gameObject, true);
        }
        for (int index = starCount - quality - 2; index >= 0; index--)
        {
            NGUITools.SetActive(stars.FindChild("Star" + index).gameObject, false);
        }
    }

    #endregion

    #region Public Methods

    public void Refresh(SCLottery lottery)
    {
        var rewardItems = lottery.RewardItem;
        NGUITools.SetActiveChildren(rewardItemsTran.gameObject, true);
        if (lottery.LotteryType == LotteryConstant.LotteryTypeHero)
        {
            for (int i = 0; i < rewardItems.Count; i++)
            {
                long uuid = Convert.ToInt64(lottery.RewardItem[i].Uuid);
                var info = HeroModelLocator.Instance.FindHero(uuid);
                RefreshReward(rewardItemsTran.GetChild(i), info);
            }
        }
        else if (lottery.LotteryType == LotteryConstant.LotteryTypeItem)
        {
            for (int i = 0; i < rewardItems.Count; i++)
            {
                var info = ItemModeLocator.Instance.FindItem(lottery.RewardItem[i].Uuid);
                RefreshItem(rewardItemsTran.GetChild(i), info);
            }
        }
        if (rewardItemsTran.transform.childCount > rewardItems.Count)
        {
            lastItem.gameObject.SetActive(false);
        }
    }

    #endregion
}
