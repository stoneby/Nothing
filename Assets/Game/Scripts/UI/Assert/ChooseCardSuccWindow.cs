using System;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ChooseCardSuccWindow : Window
{
    #region Private Fields

    private UIEventListener viewOKLis;

    private GameObject flagRed;
    private GameObject flagBlue;

    private GameObject skill1;
    private GameObject skill2;
    private UILabel skillName1;
    private UILabel skillContent1;
    private UILabel skillName2;
    private UILabel skillContent2;

    private GameObject heroIcon;
    private GameObject itemIcon;
    private UISprite jobIcon;
    private Transform rarity;

    private UILabel name;
    private UILabel attackValue;
    private UILabel hpValue;
    private UILabel mpValue;
    private UILabel recoverValue;

    private int nowRewardItemIndex = 0;

    #endregion

    #region Private Methods

    private void InstallHandlers()
    {
        viewOKLis.onClick = ViewOKClose;
    }

    private void UnInstallHandlers()
    {
        viewOKLis.onClick = null;
    }

    private void ViewOKClose(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardSuccWindow>(false);
    }

    private void ViewOKAnother(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardSuccWindow>(false);
        var window = WindowManager.Instance.Show<ChooseCardSuccWindow>(true);
        window.Refresh(nowRewardItemIndex);
    }

    /// <summary>
    /// Set ChooseHeroCard Mode or ChooseItemCard Mode.
    /// </summary>
    /// <param name="isHeroSucc">true if is ChooseHeroCard Mode</param>
    private void SetSuccMode(bool isHeroSucc)
    {
        flagRed.SetActive(isHeroSucc);
        flagBlue.SetActive(!isHeroSucc);
        heroIcon.SetActive(isHeroSucc);
        itemIcon.SetActive(!isHeroSucc);
        skill1.SetActive(isHeroSucc);
        skill2.SetActive(isHeroSucc);
    }

    #endregion

    #region Public Fields

    public SCLottery storedScLotteryMsg;

    #endregion

    #region Public Methods

    public void Refresh(int rewardItemIndex = 0)
    {
        var rewardItemCount = storedScLotteryMsg.RewardItem.Count;

        //Set detail info in window for HeroLottery or ItemLottery.
        if (storedScLotteryMsg.LotteryType == 1)
        {
            SetSuccMode(true);

            long uuid = Convert.ToInt64(storedScLotteryMsg.RewardItem[rewardItemIndex].Uuid);
            var info = HeroModelLocator.Instance.FindHero(uuid);
            var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[info.TemplateId];

            jobIcon.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;
            for (int i = 1; i <= heroTemplate.Star; i++)
            {
                NGUITools.SetActive(rarity.FindChild("Star" + i.ToString()).gameObject, true);
            }

            name.text = heroTemplate.Name;
            attackValue.text = heroTemplate.Attack.ToString();
            hpValue.text = heroTemplate.HP.ToString();
            mpValue.text = heroTemplate.MP.ToString();
            recoverValue.text = heroTemplate.Recover.ToString();

            skillContent1.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.LeaderSkill].Desc;
            skillContent2.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.ActiveSkill].Desc;
        }
        else if (storedScLotteryMsg.LotteryType == 2)
        {
            SetSuccMode(false);

            var info = ItemModeLocator.Instance.FindItem(storedScLotteryMsg.RewardItem[rewardItemIndex].Uuid);
            var quality = ItemModeLocator.Instance.GetQuality(info.TmplId);
            quality = (sbyte)(quality / ItemType.QualitiesPerStar);

            for (int index = 1; index <= quality; index++)
            {
                NGUITools.SetActive(rarity.FindChild("Star" + index.ToString()).gameObject, true);
            }

            name.text = ItemModeLocator.Instance.GetName(info.TmplId);
            attackValue.text = ItemModeLocator.Instance.GetAttack(info.TmplId,info.Level).ToString();
            hpValue.text = ItemModeLocator.Instance.GetHp(info.TmplId, info.Level).ToString();
            mpValue.text = ItemModeLocator.Instance.GetMp(info.TmplId).ToString();
            recoverValue.text = ItemModeLocator.Instance.GetRecover(info.TmplId, info.Level).ToString();
        }
        else
        {
            Logger.LogError("Not correct LotteryType.");
            return;
        }

        //Set OKLis for 1 lottery or 10 lotteries.
        if (storedScLotteryMsg.LotteryMode == 1 || storedScLotteryMsg.LotteryMode == 2)
        {
            viewOKLis.onClick = ViewOKClose;
        }
        else if (storedScLotteryMsg.LotteryMode == 3)
        {
            if (rewardItemIndex < rewardItemCount-1)
            {
                nowRewardItemIndex = rewardItemIndex + 1;
                viewOKLis.onClick = ViewOKAnother;
            }
            else if(rewardItemIndex==rewardItemCount-1)
            {
                viewOKLis.onClick = ViewOKClose;
            }
            else
            {
                Logger.LogError("Not correct rewardItemIndex.");
            }
        }
        else
        {
            Logger.LogError("Not correct LotteryMode.");
            return;
        }
    }

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

    #region Mono

    // Use this for initialization
    void Awake()
    {
        viewOKLis = UIEventListener.Get(Utils.FindChild(transform, "ViewOK").gameObject);

        flagRed = Utils.FindChild(transform, "FlagRed").gameObject;
        flagBlue = Utils.FindChild(transform, "FlagBlue").gameObject;

        skill1 = Utils.FindChild(transform, "Skill1").gameObject;
        skill2 = Utils.FindChild(transform, "Skill2").gameObject;
        skillName1 = skill1.GetComponent<UILabel>();
        skillName2 = skill2.GetComponent<UILabel>();
        skillContent1 = Utils.FindChild(transform, "Content1").GetComponent<UILabel>();
        skillContent2 = Utils.FindChild(transform, "Content2").GetComponent<UILabel>();

        heroIcon = Utils.FindChild(transform, "HeroIcon").gameObject;
        itemIcon = Utils.FindChild(transform, "ItemIcon").gameObject;
        jobIcon = Utils.FindChild(transform, "JobIcon").GetComponent<UISprite>();
        rarity = Utils.FindChild(transform, "Rarity").transform;

        name = Utils.FindChild(transform, "Name").GetComponent<UILabel>();
        attackValue = Utils.FindChild(transform, "AttackValue").GetComponent<UILabel>();
        hpValue = Utils.FindChild(transform, "HPValue").GetComponent<UILabel>();
        mpValue = Utils.FindChild(transform, "MPValue").GetComponent<UILabel>();
        recoverValue = Utils.FindChild(transform, "RecoverValue").GetComponent<UILabel>();
    }

    #endregion
}
