using System;
using System.Linq;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ChooseCardEffectWindow : Window
{
    public GameObject HeroBase;
    public GameObject ItemBase;
    public GameObject HeroDetailInfo;
    public GameObject ItemDetailInfo;
    public GameObject Grid;
    public int MaxPerLine = 5;
    public float CellWidth = 180f;
    public float CellHeight = 180f;
    public ChooseCardEffectController OneChooseCardEffect;
    public ChooseCardEffectController TenChooseCardEffect;

    private SCLottery lotteryCached;
    private UIEventListener chooseLis;
    private UIEventListener closeLis;
    private bool isChooseHero;
    private int index;
    private Transform chooseTen;
    private Transform chooseOnce;
    private Transform reputation;
    private Transform diamond;
    private UILabel cost;
    private GameObject detailClone;
    private Transform title;

    #region Window

    public override void OnEnter()
    {
        GlobalWindowSoundController.Instance.PlayOpenSound();
        ShowElements(false);
        NGUITools.Destroy(detailClone);
    }

    public override void OnExit()
    {
        GlobalWindowSoundController.Instance.PlayCloseSound();
        CleanUp();
    }

    public void CleanUp()
    {
        var list = (from Transform o in Grid.transform select o.gameObject).ToList();
        for(int i = list.Count - 1; i >= 0; i--)
        {
            NGUITools.Destroy(list[i]);
        }
        var chooseEffect = (lotteryCached.LotteryMode == LotteryConstant.LotteryModeTenthCharge)
                               ? TenChooseCardEffect
                               : OneChooseCardEffect;
        chooseEffect.CleanMainEffect();
        ShowElements(false);
    }

    #endregion

    #region public Methods

    public void Refresh(SCLottery lottery)
    {
        lotteryCached = lottery;
        RefreshChooseButton(lotteryCached.LotteryMode, lotteryCached.LotteryType);
        if (lotteryCached.LotteryMode == LotteryConstant.LotteryModeTenthCharge)
        {
            TenChooseCardEffect.OnCardMoveComplete = OnCardMoveComplete;
            TenChooseCardEffect.OnEffectComplete = OnEffectComplete;
            TenChooseCardEffect.Play();
        }
        else
        {
            OneChooseCardEffect.OnCardMoveComplete = OnCardMoveComplete;
            OneChooseCardEffect.OnEffectComplete = OnEffectComplete;
            OneChooseCardEffect.Play();
        }
        isChooseHero = (lotteryCached.LotteryType == LotteryConstant.LotteryTypeHero);
    }

    private void ShowElements(bool show)
    {
        NGUITools.SetActive(chooseLis.gameObject, show);
        NGUITools.SetActive(closeLis.gameObject, show);
        NGUITools.SetActive(title.gameObject, show);
    }

    private void RefreshChooseButton(sbyte lotteryMode, sbyte lotteryType)
    {
        var isTenLottery = (lotteryMode == LotteryConstant.LotteryModeTenthCharge);
        var isLotteryHero = (lotteryType == LotteryConstant.LotteryTypeHero);
        NGUITools.SetActive(chooseTen.gameObject, isTenLottery);
        NGUITools.SetActive(chooseOnce.gameObject, !isTenLottery);
        NGUITools.SetActive(diamond.gameObject, isLotteryHero);
        NGUITools.SetActive(reputation.gameObject, !isLotteryHero);
        var lotteryList = WindowManager.Instance.GetWindow<ChooseCardWindow>().ScLotteryList;
        var oneTimeCost = isLotteryHero ? lotteryList.LotteryHeroCost : lotteryList.LotteryItemCost;
        const int tenTimes = 10;
        const int oneTime = 1;
        var bonus = isTenLottery ? tenTimes : oneTime;
        cost.text = (oneTimeCost * bonus).ToString();
    }

    private void OnCardMoveComplete()
    {
        if(lotteryCached.LotteryMode == LotteryConstant.LotteryModeTenthCharge)
        {
            var localPos = new Vector3(CellWidth * (index % MaxPerLine), -CellHeight * (index / MaxPerLine), 0);
            var pos = Grid.transform.TransformPoint(localPos);
            var child = Instantiate(isChooseHero ? HeroBase : ItemBase, pos, Quaternion.identity) as GameObject;
            child.transform.parent = Grid.transform;
            child.transform.localScale = Vector3.one;
            if (isChooseHero)
            {
                var hero = child.GetComponent<HeroItemBase>();
                if (index < lotteryCached.RewardItem.Count)
                {
                    var heroInfo = HeroModelLocator.Instance.FindHero(Convert.ToInt64(lotteryCached.RewardItem[index].Uuid));
                    if (heroInfo != null)
                    {
                        hero.InitItem(heroInfo);
                    }
                }
            }
            else
            {
                var item = child.GetComponent<ItemBase>();
                if (index < lotteryCached.RewardItem.Count)
                {
                    var itemInfo = ItemModeLocator.Instance.FindItem(lotteryCached.RewardItem[index].Uuid);
                    if (itemInfo != null)
                    {
                        item.InitItem(itemInfo);
                    }
                }
            }
            index++;
        }
        else
        {
            if (isChooseHero)
            {
                var heroInfo = HeroModelLocator.Instance.FindHero(Convert.ToInt64(lotteryCached.RewardItem[index].Uuid));
                ShowHeroDetail(heroInfo);
            }
            else
            {
                var itemInfo = ItemModeLocator.Instance.FindItem(lotteryCached.RewardItem[index].Uuid);
                ShowItemDetail(itemInfo);
            }
        }
    }

    private void ShowHeroDetail(HeroInfo heroInfo)
    {
        detailClone = NGUITools.AddChild(gameObject, HeroDetailInfo);
        var heroDetailControl = detailClone.GetComponent<HeroDetailInfoControl>();
        heroDetailControl.Refresh(heroInfo);
    }

    private void ShowItemDetail(ItemInfo itemInfo)
    {
        detailClone = NGUITools.AddChild(gameObject, ItemDetailInfo);
        var itemDetailControl = detailClone.GetComponent<ItemDetailControl>();
        itemDetailControl.Refresh(itemInfo);
    }

    private void OnEffectComplete()
    {
        index = 0;
        ShowElements(true);
        var isTenChoose = lotteryCached.LotteryMode == LotteryConstant.LotteryModeTenthCharge;
        NGUITools.SetActive(title.gameObject, isTenChoose);
        foreach(Transform child in Grid.transform)
        {
            var longPress = child.GetComponent<NGUILongPress>();
            longPress.OnLongPress = OnLongPress;
            longPress.OnLongPressFinish = OnLongPressFinish;
        }
        if(!isTenChoose)
        {
            if (isChooseHero)
            {
                var heroInfo = HeroModelLocator.Instance.FindHero(Convert.ToInt64(lotteryCached.RewardItem[index].Uuid));
                ShowHeroDetail(heroInfo);
            }
            else
            {
                var itemInfo = ItemModeLocator.Instance.FindItem(lotteryCached.RewardItem[index].Uuid);
                ShowItemDetail(itemInfo);
            }
        }
    }

    private void OnLongPress(GameObject go)
    {
        if (isChooseHero)
        {
            var heroInfo = go.GetComponent<HeroItemBase>().HeroInfo;
            ShowHeroDetail(heroInfo);
        }
        else
        {
            var bagIndex = go.GetComponent<ItemBase>().BagIndex;
            var info = ItemModeLocator.Instance.FindItem(bagIndex);
            ShowItemDetail(info);
        }
        if(detailClone)
        {
            var floatWindowShower = detailClone.GetComponent<FloatWindowShower>();
            floatWindowShower.Show();
        }
    }

    private void OnLongPressFinish(GameObject go)
    {
        NGUITools.Destroy(detailClone);
    }

    private void Awake()
    {
        var buttons = transform.Find("Panel/Buttons");
        chooseLis = UIEventListener.Get(buttons.Find("Button-Choose").gameObject);
        closeLis = UIEventListener.Get(buttons.Find("Button-Close").gameObject);
        title = transform.Find("Panel/Title");
        var chooseTran = chooseLis.transform;
        chooseTen = chooseTran.Find("ChooseTen");
        chooseOnce = chooseTran.Find("ChooseOnce");
        diamond = chooseTran.Find("Diamond");
        reputation = chooseTran.Find("Reputation");
        cost = chooseTran.Find("Cost").GetComponent<UILabel>();
        chooseLis.onClick = OnChoose;
        closeLis.onClick = OnClose;
    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardEffectWindow>(false);
    }

    private void OnChoose(GameObject go)
    {
        var csmsg = new CSLottery
                        {
                            ActivityId = lotteryCached.ActivityId,
                            LotteryMode = lotteryCached.LotteryMode,
                            LotteryType = lotteryCached.LotteryType
                        };
        NetManager.SendMessage(csmsg);
        NGUITools.Destroy(detailClone);
    }

    #endregion
}
