using System;
using System.Runtime.InteropServices;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ChooseCardSuccWindow : Window
{
    #region Public Fields

    public SCLottery StoredScLotteryMsg;
    public SCHeroFristLoginGive StoredHeroFristLoginGiveMsg;

    #endregion

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
    private UISprite heroIconSprite;
    private GameObject itemIcon;
    private UISprite itemIconSprite;
    private UISprite jobIcon;
    private Transform rarity;

    private UILabel name;
    private UILabel attackValue;
    private UILabel hpValue;
    private UILabel mpValue;
    private UILabel recoverValue;

    private int nowRewardItemIndex = 0;

    #endregion

    #region Public Methods

    public void Refresh(int rewardItemIndex = 0)
    {
        var rewardItemCount = StoredScLotteryMsg.RewardItem.Count;

        //Set detail info in window for HeroLottery or ItemLottery.
        if (StoredScLotteryMsg.LotteryType == 1)
        {
            SetSuccMode(true);

            long uuid = Convert.ToInt64(StoredScLotteryMsg.RewardItem[rewardItemIndex].Uuid);
            var info = HeroModelLocator.Instance.FindHero(uuid);
            var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[info.TemplateId];

            HeroConstant.SetHeadByIndex(heroIconSprite, heroTemplate.Icon - 1);
            jobIcon.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;
            DeActiveAll();
            for (int i = 1; i <= 5; i++)
            {
                NGUITools.SetActive(rarity.FindChild("Star" + i.ToString()).gameObject, i <= heroTemplate.Star);
            }

            name.text = heroTemplate.Name;
            attackValue.text = heroTemplate.Attack.ToString();
            hpValue.text = heroTemplate.HP.ToString();
            mpValue.text = heroTemplate.MP.ToString();
            recoverValue.text = heroTemplate.Recover.ToString();

            if (HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls.ContainsKey(heroTemplate.LeaderSkill))
            {
                skillName1.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.LeaderSkill].Name;
                skillContent1.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.LeaderSkill].Desc;
            }
            if (HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls.ContainsKey(heroTemplate.ActiveSkill))
            {
                skillName2.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.ActiveSkill].Name;
                skillContent2.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.ActiveSkill].Desc;
            }
        }
        else if (StoredScLotteryMsg.LotteryType == 2)
        {
            SetSuccMode(false);

            var info = ItemModeLocator.Instance.FindItem(StoredScLotteryMsg.RewardItem[rewardItemIndex].Uuid);
            var tempId = info.TmplId;
            var quality = ItemModeLocator.Instance.GetQuality(tempId);
            quality = (sbyte)ItemHelper.GetStarCount(quality);
            ItemType.SetHeadByTemplate(itemIconSprite, info.TmplId);
            DeActiveAll();
            DeActiveAll();
            for (int i = 1; i <= 5; i++)
            {
                NGUITools.SetActive(rarity.FindChild("Star" + i.ToString()).gameObject, i <= quality);
            }

            name.text = ItemModeLocator.Instance.GetName(tempId);
            SetTextValue(attackValue, ItemModeLocator.Instance.GetAttack(tempId, info.Level));
            SetTextValue(hpValue, ItemModeLocator.Instance.GetHp(tempId, info.Level));
            SetTextValue(mpValue, ItemModeLocator.Instance.GetMp(tempId));
            SetTextValue(recoverValue, ItemModeLocator.Instance.GetRecover(tempId, info.Level));
        }
        else
        {
            Logger.LogError("Not correct LotteryType.");
            return;
        }

        //Set OKLis for 1 lottery or 10 lotteries.
        if (StoredScLotteryMsg.LotteryMode == 1 || StoredScLotteryMsg.LotteryMode == 2)
        {
            viewOKLis.onClick = ViewOKClose;
        }
        else if (StoredScLotteryMsg.LotteryMode == 3)
        {
            if (rewardItemIndex < rewardItemCount - 1)
            {
                nowRewardItemIndex = rewardItemIndex + 1;
                viewOKLis.onClick = ViewOKAnotherRewardItem;
            }
            else if (rewardItemIndex == rewardItemCount - 1)
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
        }
    }

    private static void SetTextValue(UILabel lable, int atkValue)
    {
        lable.text = atkValue >= 0 ? atkValue.ToString() : "-";
    }

    public void ShowHeroFirstGive(int rewardItemIndex = 0)
    {
        var rewardItemCount = StoredHeroFristLoginGiveMsg.HeroInfos.Count;
        SetSuccMode(true);

        var heroInfo = StoredHeroFristLoginGiveMsg.HeroInfos[rewardItemIndex];
        var heroTemplate = HeroModelLocator.Instance.HeroTemplates.HeroTmpls[heroInfo.TemplateId];
        HeroConstant.SetHeadByIndex(heroIconSprite, heroTemplate.Icon - 1);
        jobIcon.spriteName = HeroConstant.HeroJobPrefix + heroTemplate.Job;
        for (int i = 1; i <= 5; i++)
        {
            NGUITools.SetActive(rarity.FindChild("Star" + i.ToString()).gameObject, i <= heroTemplate.Star);
        }

        name.text = heroTemplate.Name;
        attackValue.text = heroTemplate.Attack.ToString();
        hpValue.text = heroTemplate.HP.ToString();
        mpValue.text = heroTemplate.MP.ToString();
        recoverValue.text = heroTemplate.Recover.ToString();

        if (HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls.ContainsKey(heroTemplate.LeaderSkill))
        {
            skillName1.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.LeaderSkill].Name;
            skillContent1.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.LeaderSkill].Desc;
        }
        if (HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls.ContainsKey(heroTemplate.ActiveSkill))
        {
            skillName2.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.ActiveSkill].Name;
            skillContent2.text = HeroModelLocator.Instance.SkillTemplates.HeroBattleSkillTmpls[heroTemplate.ActiveSkill].Desc;
        }

        if (rewardItemIndex < rewardItemCount - 1)
        {
            nowRewardItemIndex = rewardItemIndex + 1;
            viewOKLis.onClick = ViewOKAnotherHeroInFirstGive;
        }
        else if (rewardItemIndex == rewardItemCount - 1)
        {
            viewOKLis.onClick = ViewOKClose;
        }
        else
        {
            Logger.LogError("Not correct rewardItemIndex.");
        }
    }

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
        //var window = WindowManager.Instance.GetWindow<ChooseCardWindow>();
        //if (window != null)
        //{
        //    window.heroAndItemSummitHandler.RefreshFamousAndIngot();
        //}
        WindowManager.Instance.Show<ChooseCardSuccWindow>(false);

        //Set GreenHand info.
        if (!GreenHandGuideHandler.Instance.GiveHeroFinishFlag)
        {
            GreenHandGuideHandler.Instance.GiveHeroFinishFlag = true;
            GreenHandGuideHandler.Instance.SendEndMessage(2);
            GreenHandGuideHandler.Instance.ExecuteGreenHandFlag();
        }
    }

    private void ViewOKAnotherRewardItem(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardSuccWindow>(false);
        var window = WindowManager.Instance.Show<ChooseCardSuccWindow>(true);
        window.Refresh(nowRewardItemIndex);
    }

    private void ViewOKAnotherHeroInFirstGive(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardSuccWindow>(false);
        Logger.Log("!!!!!!!!!!!!!!!!!Frame count in window another:" + Time.frameCount);
        var window = WindowManager.Instance.Show<ChooseCardSuccWindow>(true);
        window.ShowHeroFirstGive(nowRewardItemIndex);
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

    private void DeActiveAll()
    {
        foreach (Transform item in rarity)
        {
            item.gameObject.SetActive(false);
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
        skillName1 = Utils.FindChild(transform, "Name1").GetComponent<UILabel>();
        skillName2 = Utils.FindChild(transform, "Name2").GetComponent<UILabel>();
        skillContent1 = Utils.FindChild(transform, "Content1").GetComponent<UILabel>();
        skillContent2 = Utils.FindChild(transform, "Content2").GetComponent<UILabel>();

        heroIcon = Utils.FindChild(transform, "HeroIcon").gameObject;
        heroIconSprite = heroIcon.GetComponent<UISprite>();
        itemIcon = Utils.FindChild(transform, "ItemIcon").gameObject;
        itemIconSprite = itemIcon.GetComponent<UISprite>();
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
