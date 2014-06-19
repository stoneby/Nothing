using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using UnityEngine;
using System.Collections;

public class ChooseHeroCardWindow : Window
{
    #region Private Fields

    private UIEventListener backLis;
    private UIEventListener buyOneLis;
    private UIEventListener buyTenLis;
    private UISprite tabTemplate;
    private UITable table;
    private TabBehaviour tabBehaviour;
    private GameObject ad1;
    private UILabel oneTimeCost;
    private UILabel tenTimeCost;
    private UILabel timeForFree;
    private UILabel freeThisTime;
    private UILabel timesForHero;
    private Transform heroRelated;
    private Transform itemRelated;
    private const int TenTimes = 10;
    private bool isFreeTime;
    private SCLotteryList scLotteryList;

    #endregion

    #region Window

    public override void OnEnter()
    {
        InstallHandlers();
    }

    public override void OnExit()
    {
        UnInstallHandlers();
        StopAllCoroutines();
        scLotteryList = null;
        var list = table.transform.Cast<Transform>().ToList();
        for (int index = 0; index < list.Count; index++)
        {
            var item = list[index];
            Destroy(item.gameObject);
        }
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        backLis = UIEventListener.Get(transform.FindChild("Buttons/BackBtn").gameObject);
        buyOneLis = UIEventListener.Get(transform.FindChild("Buttons/BuyOneBtn").gameObject);
        buyTenLis = UIEventListener.Get(transform.FindChild("Buttons/BuyTenBtn").gameObject);
        tabTemplate = transform.FindChild("TabTemplate").GetComponent<UISprite>();
        tabTemplate.gameObject.SetActive(false);
        table = transform.FindChild("TabButtons").GetComponent<UITable>();
        tabBehaviour = table.GetComponent<TabBehaviour>();
        oneTimeCost = transform.FindChild("Labels/OneTimeCost").GetComponent<UILabel>();
        tenTimeCost = transform.FindChild("Labels/TenTimeCost").GetComponent<UILabel>();
        timeForFree = transform.FindChild("Labels/TimeForFree").GetComponent<UILabel>();
        freeThisTime = transform.FindChild("Labels/FreeThisTime").GetComponent<UILabel>();
        heroRelated = transform.FindChild("HeroRelated");
        itemRelated = transform.FindChild("ItemRelated");
        timesForHero = heroRelated.FindChild("TimesForHero").GetComponent<UILabel>();
        ad1 = transform.FindChild("Ads/Ad1").gameObject;
    }

    private void InstallHandlers()
    {
        backLis.onClick += OnBack;
        buyOneLis.onClick += OnBuyOne;
        buyTenLis.onClick += OnBuyTen;
    }

    private void UnInstallHandlers()
    {
        backLis.onClick -= OnBack;
        buyOneLis.onClick -= OnBuyOne;
        buyTenLis.onClick -= OnBuyTen;
    }

    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<ChooseHeroCardWindow>(false);
    }

    private void OnBuyOne(GameObject go)
    {
        var lotteryMode = isFreeTime ? LotteryConstant.LotteryModeFree : LotteryConstant.LotteryModeOnceCharge;
        var msg = new CSLottery
                            {
                                ActivityId = scLotteryList.ListLotteryInfo[tabBehaviour.ActiveTab].ActivityId,
                                LotteryType = scLotteryList.LotteryType,
                                LotteryMode = (sbyte)lotteryMode
                            };
        NetManager.SendMessage(msg);
    }

    private void OnBuyTen(GameObject go)
    {
        var msg = new CSLottery
        {
            ActivityId = scLotteryList.ListLotteryInfo[tabBehaviour.ActiveTab].ActivityId,
            LotteryType = scLotteryList.LotteryType,
            LotteryMode = LotteryConstant.LotteryModeTenthCharge
        };
        NetManager.SendMessage(msg);
    }

    private IEnumerator UpdateFreeTime(TimeSpan timeRemain)
    {
        if(timeRemain.TotalSeconds > 0)
        {
            var oneSecond = new TimeSpan(0, 0, 1);
            while (timeRemain.TotalSeconds > 0)
            {
                timeRemain = timeRemain.Subtract(oneSecond);
                if (timeRemain.TotalSeconds > 0)
                {
                    timeForFree.text = Utils.ConvertTimeSpanToString(timeRemain);
                }
                yield return new WaitForSeconds(1);
            }
            timeForFree.gameObject.SetActive(true);
            isFreeTime = true;
        }
    }

    private void DisplayFreeTime(long lastTime)
    {
        StopAllCoroutines();
        var lastFreeTime = Utils.ConvertFromJavaTimestamp(lastTime);
        lastFreeTime = lastFreeTime.AddDays(1);
        var timeRemain = lastFreeTime.Subtract(DateTime.Now);
        isFreeTime = timeRemain.TotalSeconds <= 0;
        timeForFree.gameObject.SetActive(!isFreeTime);
        freeThisTime.gameObject.SetActive(isFreeTime);
        StartCoroutine("UpdateFreeTime", timeRemain);
    }

    #endregion

    #region Public Methods

    public void Refresh(SCLotteryList lotteryList)
    {
        scLotteryList = lotteryList;
        var infos = lotteryList.ListLotteryInfo;
        var toggleItems = new Dictionary<UISprite, GameObject>();
        for (int i = 0; i < infos.Count; i++)
        {
            var info = infos[i];
            var child = Instantiate(tabTemplate.gameObject) as GameObject;
            child.SetActive(true);
            Utils.MoveToParent(table.transform, child.transform);
            child.GetComponentInChildren<UILabel>().text = info.Name;
            toggleItems.Add(child.GetComponent<UISprite>(), ad1);
        }
        table.repositionNow = true;
        tabBehaviour.InitTab(toggleItems, 0, "FourTabN", "FourTabD");
        var isHeroChoose = (lotteryList.LotteryType == LotteryConstant.LotteryTypeHero);
        NGUITools.SetActiveChildren(heroRelated.gameObject, isHeroChoose);
        NGUITools.SetActiveChildren(itemRelated.gameObject, !isHeroChoose);
        var oneTimeCostValue = lotteryList.LotteryCost;
        oneTimeCost.text = oneTimeCostValue.ToString(CultureInfo.InvariantCulture);
        tenTimeCost.text = (oneTimeCostValue * TenTimes).ToString(CultureInfo.InvariantCulture);
        timesForHero.text = lotteryList.Get4StarHeroRestTimes.ToString(CultureInfo.InvariantCulture);
        DisplayFreeTime(lotteryList.LastFreeLotteryTime);
    }

    public void RefreshTimes(long lastTime, int restTimes)
    {
        DisplayFreeTime(lastTime);
        timesForHero.text = restTimes.ToString(CultureInfo.InvariantCulture);
    }

    #endregion
}
