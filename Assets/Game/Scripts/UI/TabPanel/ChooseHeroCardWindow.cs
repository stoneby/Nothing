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
    private UILabel oneTimeCost;
    private UILabel tenTimeCost;
    private UILabel timeForFree;
    private UILabel freeThisTime;
    private UILabel timesForHero;
    private UILabel totalFamous;
    private Transform heroRelated;
    private Transform itemRelated;
    private const int TenTimes = 10;
    private bool isFreeTime;
    private SCLotteryList scLotteryList;
    private Transform tenTimesDesc;
    private Transform elevenTimesDesc;
    private AdvertisePlayer player;
    private int lotteryMode = -1;

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
        player.Reset();
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        backLis = UIEventListener.Get(Utils.FindChild(transform, "BackBtn").gameObject);
        buyOneLis = UIEventListener.Get(Utils.FindChild(transform, "BuyOneBtn").gameObject);
        buyTenLis = UIEventListener.Get(Utils.FindChild(transform, "BuyTenBtn").gameObject);
        tabTemplate = Utils.FindChild(transform, "TabTemplate").GetComponent<UISprite>();
        tabTemplate.gameObject.SetActive(false);
        table = Utils.FindChild(transform, "TabButtons").GetComponent<UITable>();
        tabBehaviour = table.GetComponent<TabBehaviour>();
        oneTimeCost = Utils.FindChild(transform,"OneTimeCost").GetComponent<UILabel>();
        tenTimeCost = Utils.FindChild(transform, "TenTimeCost").GetComponent<UILabel>();
        timeForFree = Utils.FindChild(transform, "TimeForFree").GetComponent<UILabel>();
        freeThisTime = Utils.FindChild(transform, "FreeThisTime").GetComponent<UILabel>();
        totalFamous = Utils.FindChild(transform, "TotalRep").GetComponent<UILabel>();
        tenTimesDesc = Utils.FindChild(transform, "TenTimes");
        elevenTimesDesc = Utils.FindChild(transform, "ElevenTimes");
        heroRelated = Utils.FindChild(transform, "HeroRelated");
        itemRelated = Utils.FindChild(transform, "ItemRelated");
        timesForHero = heroRelated.FindChild("TimesForHero").GetComponent<UILabel>();
        player = GetComponentInChildren<AdvertisePlayer>();
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
        lotteryMode = isFreeTime ? LotteryConstant.LotteryModeFree : LotteryConstant.LotteryModeOnceCharge;
        if(isFreeTime)
        {
            SendBuyMessage();
        }
        else
        {
            var str = scLotteryList.LotteryType == LotteryConstant.LotteryTypeHero
                          ? StringTable.OneTimeHeroLotteryConfirm
                          : StringTable.OneTimeItemLotteryConfirm;
            ShowConfirm(string.Format(str, scLotteryList.LotteryCost));
        }
    }

    private void OnCardConfirmCancel(GameObject sender)
    {
        ConfirmWindowClean();
        WindowManager.Instance.Show<AssertionWindow>(false);
    }

    private void OnCardConfirmOk(GameObject sender)
    {
        ConfirmWindowClean();
        SendBuyMessage();
    }

    private void ConfirmWindowClean()
    {
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.OkButtonClicked -= OnCardConfirmOk;
        assertWindow.CancelButtonClicked -= OnCardConfirmCancel;
    }

    private void SendBuyMessage()
    {   
        var msg = new CSLottery
                      {
                          ActivityId = scLotteryList.ListLotteryInfo[tabBehaviour.ActiveTab].ActivityId,
                          LotteryType = scLotteryList.LotteryType,
                          LotteryMode = (sbyte) lotteryMode
                      };
        NetManager.SendMessage(msg);
    }

    private void ShowConfirm(string title)
    {
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.AssertType = AssertionWindow.Type.OkCancel;
        assertWindow.Message = "";
        assertWindow.Title = title;
        assertWindow.OkButtonClicked += OnCardConfirmOk;
        assertWindow.CancelButtonClicked += OnCardConfirmCancel;
        WindowManager.Instance.Show(typeof(AssertionWindow), true);
    }

    private void OnBuyTen(GameObject go)
    {
        lotteryMode = LotteryConstant.LotteryModeTenthCharge;
        var str = scLotteryList.LotteryType == LotteryConstant.LotteryTypeHero
                      ? StringTable.TenTimeHeroLotteryConfirm
                      : StringTable.TenTimeItemLotteryConfirm;
        ShowConfirm(string.Format(str, scLotteryList.LotteryCost * TenTimes));
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

    private void OnTabChanged(int activetab)
    { 
        var isElevenTimes = scLotteryList.ListLotteryInfo[activetab].TenLotteryGiveElevenHero;
        elevenTimesDesc.gameObject.SetActive(isElevenTimes);
        tenTimesDesc.gameObject.SetActive(!isElevenTimes);
        //Just For Demo
        SetAdvIncon(activetab);
    }

    private void SetAdvIncon(int activetab)
    {
        player.Back.GetComponent<UISprite>().spriteName = "Ad" + (2 * activetab + 2);
        player.Front.GetComponent<UISprite>().spriteName = "Ad" + (2 * activetab + 1);
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
            toggleItems.Add(child.GetComponent<UISprite>(), null);
        }
        table.repositionNow = true;
        tabBehaviour.InitTab(toggleItems, 0, "FourTabN", "FourTabD");
        tabBehaviour.TabChanged += OnTabChanged;
        var isHeroChoose = (lotteryList.LotteryType == LotteryConstant.LotteryTypeHero);
        NGUITools.SetActiveChildren(heroRelated.gameObject, isHeroChoose);
        NGUITools.SetActiveChildren(itemRelated.gameObject, !isHeroChoose);
        var oneTimeCostValue = lotteryList.LotteryCost;
        oneTimeCost.text = oneTimeCostValue.ToString(CultureInfo.InvariantCulture);
        tenTimeCost.text = (oneTimeCostValue * TenTimes).ToString(CultureInfo.InvariantCulture);
        timesForHero.text = lotteryList.Get4StarHeroRestTimes.ToString(CultureInfo.InvariantCulture);
        totalFamous.text = PlayerModelLocator.Instance.Famous.ToString(CultureInfo.InvariantCulture);
        var isElevenTimes = lotteryList.ListLotteryInfo[tabBehaviour.ActiveTab].TenLotteryGiveElevenHero;
        elevenTimesDesc.gameObject.SetActive(isElevenTimes);
        tenTimesDesc.gameObject.SetActive(!isElevenTimes);
        player.Play();
        DisplayFreeTime(lotteryList.LastFreeLotteryTime);
    }

    public void RefreshTimes(long lastTime, int restTimes)
    {
        DisplayFreeTime(lastTime);
        timesForHero.text = restTimes.ToString(CultureInfo.InvariantCulture);
    }

    public void RefreshFamous()
    {
        totalFamous.text = PlayerModelLocator.Instance.Famous.ToString(CultureInfo.InvariantCulture);
    }

    #endregion
}
