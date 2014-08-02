using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KXSGCodec;
using Property;
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
    private UILabel totalChips;
    private Transform heroRelated;
    private Transform itemRelated;
    private const int TenTimes = 10;
    private bool isFreeTime;
    private SCLotteryList scLotteryList;
    private Transform tenTimesDesc;
    private Transform elevenTimesDesc;
    private AdvertisePlayer player;
    private int lotteryMode = -1;
    private bool isHeroChoose;

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
        totalChips = Utils.FindChild(transform, "TotalChips").GetComponent<UILabel>();
        tenTimesDesc = Utils.FindChild(transform, "TenTimes");
        elevenTimesDesc = Utils.FindChild(transform, "ElevenTimes");
        heroRelated = Utils.FindChild(transform, "HeroRelated");
        itemRelated = Utils.FindChild(transform, "ItemRelated");
        timesForHero = heroRelated.FindChild("TimesForHero").GetComponent<UILabel>();
        player = GetComponentInChildren<AdvertisePlayer>();
        CommonHandler.PlayerPropertyChanged += OnPlayerPropertyChanged;
    }

    /// <summary>
    /// Used to do some clean work before destorying.
    /// </summary>
    private void OnDestory()
    {
        CommonHandler.PlayerPropertyChanged -= OnPlayerPropertyChanged;
    }

    private void InstallHandlers()
    {
        backLis.onClick = OnBack;
        buyOneLis.onClick = OnBuyOne;
        buyTenLis.onClick = OnBuyTen;
    }

    private void UnInstallHandlers()
    {
        backLis.onClick = null;
        buyOneLis.onClick = null;
        buyTenLis.onClick = null;
    }

    /// <summary>
    /// The call back of the property changed number message of player.
    /// </summary>
    /// <param name="scpropertychanged"></param>
    private void OnPlayerPropertyChanged(SCPropertyChangedNumber scpropertychanged)
    {
        var propertyChanged = scpropertychanged.PropertyChanged;
        if (propertyChanged != null &&
            (propertyChanged.ContainsKey(RoleProperties.ROLEBASE_FAMOUS)
            || propertyChanged.ContainsKey(RoleProperties.ROLEBASE_SUPER_CHIP)))
        {
            RefreshFamousAndChips();
        }
    }

    /// <summary>
    /// The call back of back button.
    /// </summary>
    /// <param name="go">The sender.</param>
    private void OnBack(GameObject go)
    {
        WindowManager.Instance.Show<ChooseHeroCardWindow>(false);
    }

    /// <summary>
    /// The call back of buying one hero or item event.
    /// </summary>
    /// <param name="go">The sender.</param>
    private void OnBuyOne(GameObject go)
    {
        lotteryMode = isFreeTime ? LotteryConstant.LotteryModeFree : LotteryConstant.LotteryModeOnceCharge;
        if(isFreeTime && isHeroChoose)
        {
            SendBuyMessage();
        }
        else
        {
            ShowOneConfirm();
        }
    }

    /// <summary>
    /// The call back of choosing card confirm cancel event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    private void OnCardConfirmCancel(GameObject sender)
    {
        ConfirmWindowClean();
        WindowManager.Instance.Show<AssertionWindow>(false);
    }

    /// <summary>
    /// The call back of choosing card confirm ok event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    private void OnCardConfirmOk(GameObject sender)
    {
        ConfirmWindowClean();
        SendBuyMessage();
    }

    /// <summary>
    /// Clean the varibles of confirm window.
    /// </summary>
    private void ConfirmWindowClean()
    {
        var assertWindow = WindowManager.Instance.GetWindow<AssertionWindow>();
        assertWindow.OkButtonClicked -= OnCardConfirmOk;
        assertWindow.CancelButtonClicked -= OnCardConfirmCancel;
    }

    /// <summary>
    /// Send the buy message from client side to server side.
    /// </summary>
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

    /// <summary>
    /// Show the confirm window with special title.
    /// </summary>
    /// <param name="title">The title of the confirm window to show.</param>
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

    /// <summary>
    /// Show the confirm window for choose ten times.
    /// </summary>
    private void ShowTenConfirm()
    {
        var key = scLotteryList.LotteryType == LotteryConstant.LotteryTypeHero
                  ? LotteryConstant.TenTimeHeroLotteryConfirmKey
                  : LotteryConstant.TenTimeItemLotteryConfirmKey;
        var str = LanguageManager.Instance.GetTextValue(key);
        ShowConfirm(string.Format(str, scLotteryList.LotteryCost * TenTimes));
    }

    /// <summary>
    /// Show the confirm window for choose one time.
    /// </summary>
    private void ShowOneConfirm()
    {
        var key = scLotteryList.LotteryType == LotteryConstant.LotteryTypeHero
                  ? LotteryConstant.OneTimeHeroLotteryConfirmKey
                  : LotteryConstant.OneTimeItemLotteryConfirmKey;
        var str = LanguageManager.Instance.GetTextValue(key);
        ShowConfirm(string.Format(str, scLotteryList.LotteryCost));
    }

    /// <summary>
    /// The handler of the choose ten times card.
    /// </summary>
    /// <param name="go"></param>
    private void OnBuyTen(GameObject go)
    {
        lotteryMode = LotteryConstant.LotteryModeTenthCharge;
        ShowTenConfirm();
    }

    /// <summary>
    /// Start a coroutine to update time remain for free choose card.
    /// </summary>
    /// <param name="timeRemain">The time remain to free choose card.</param>
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

    /// <summary>
    /// Update the free time.
    /// </summary>
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

    /// <summary>
    /// The call back of tab changed event.
    /// </summary>
    /// <param name="activetab">The current active tab.</param>
    private void OnTabChanged(int activetab)
    { 
        var isElevenTimes = scLotteryList.ListLotteryInfo[activetab].TenLotteryGiveElevenHero;
        elevenTimesDesc.gameObject.SetActive(isElevenTimes);
        tenTimesDesc.gameObject.SetActive(!isElevenTimes);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Used to refresh the main window ui.
    /// </summary>
    /// <param name="lotteryList"></param>
    public void Refresh(SCLotteryList lotteryList)
    {
        scLotteryList = lotteryList;
        var infos = lotteryList.ListLotteryInfo;
        var toggleItems = new Dictionary<UISprite, GameObject>();
        var depthAdjust = infos.Count;
        for (var i = 0; i < infos.Count; i++)
        {
            var info = infos[i];
            var child = Instantiate(tabTemplate.gameObject) as GameObject;
            var childSprite = child.GetComponent<UISprite>();
            Utils.AdjustDepth(childSprite, depthAdjust--);
            child.SetActive(true);
            Utils.MoveToParent(table.transform, child.transform);
            child.GetComponentInChildren<UILabel>().text = info.Name;
            toggleItems.Add(childSprite, null);
        }
        table.repositionNow = true;
        tabBehaviour.InitTab(toggleItems, 0, "FourTabN", "FourTabD");
        tabBehaviour.TabChanged += OnTabChanged;
        isHeroChoose = (lotteryList.LotteryType == LotteryConstant.LotteryTypeHero);
        NGUITools.SetActiveChildren(heroRelated.gameObject, isHeroChoose);
        NGUITools.SetActiveChildren(itemRelated.gameObject, !isHeroChoose);
        var oneTimeCostValue = lotteryList.LotteryCost;
        oneTimeCost.text = oneTimeCostValue.ToString(CultureInfo.InvariantCulture);
        tenTimeCost.text = (oneTimeCostValue * TenTimes).ToString(CultureInfo.InvariantCulture);
        totalFamous.text = PlayerModelLocator.Instance.Famous.ToString(CultureInfo.InvariantCulture);
        totalChips.text = PlayerModelLocator.Instance.SuperChip.ToString(CultureInfo.InvariantCulture);
        var isElevenTimes = lotteryList.ListLotteryInfo[tabBehaviour.ActiveTab].TenLotteryGiveElevenHero;
        elevenTimesDesc.gameObject.SetActive(isElevenTimes);
        tenTimesDesc.gameObject.SetActive(!isElevenTimes);
        player.Play();
        RefreshTimes(lotteryList.LastFreeLotteryTime, lotteryList.Get4StarHeroRestTimes);
    }

    /// <summary>
    /// Update the time with the last free time and times for free.
    /// </summary>
    /// <param name="lastTime"></param>
    /// <param name="restTimes"></param>
    public void RefreshTimes(long lastTime, int restTimes)
    {
        if (isHeroChoose)
        {
            DisplayFreeTime(lastTime);
            timesForHero.text = restTimes.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Update the famous and chips on ui.
    /// </summary>
    public void RefreshFamousAndChips()
    {
        totalFamous.text = PlayerModelLocator.Instance.Famous.ToString(CultureInfo.InvariantCulture);
        totalChips.text = PlayerModelLocator.Instance.SuperChip.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Show ui for lottery can not be free.
    /// </summary>
    public void LotteryCannotFree()
    {
        isFreeTime = false;
        lotteryMode = LotteryConstant.LotteryModeOnceCharge;
        ShowOneConfirm();
    }

    #endregion
}
