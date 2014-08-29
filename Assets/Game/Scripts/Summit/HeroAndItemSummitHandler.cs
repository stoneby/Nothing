using KXSGCodec;
using Property;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class HeroAndItemSummitHandler : MonoBehaviour
{
    #region Private Fields

    private UIEventListener buyOneLis;
    private UIEventListener buyTenLis;
    private UILabel oneTimeCost;
    private UILabel tenTimeCost;
    private UILabel timeForFree;
    private UILabel freeThisTime;
    private UILabel timesForHero;
    private UILabel totalFamous;
    private UILabel totalIngot;
    private UISprite heroDescNotFree;
    private UISprite heroDescFree;
    private Transform heroRelated;
    private Transform itemRelated;
    private const int TenTimes = 10;
    private bool isFreeTime;
    private SCLotteryList scLotteryList;
    private Transform tenTimesDesc;
    private Transform elevenTimesDesc;
    private AdvertisePlayer heroPlayer;
    private AdvertisePlayer itemPlayer;
    private int lotteryMode = -1;
    private bool isHeroChoose;

    #endregion

    #region Window

    private void OnEnable()
    {
        InstallHandlers();
    }

    private void OnDisable()
    {
        UnInstallHandlers();
        StopAllCoroutines();
        scLotteryList = null;
        heroPlayer.Reset();
        itemPlayer.Reset();
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        buyOneLis = UIEventListener.Get(Utils.FindChild(transform, "BuyOneBtn").gameObject);
        buyTenLis = UIEventListener.Get(Utils.FindChild(transform, "BuyTenBtn").gameObject);
        oneTimeCost = Utils.FindChild(transform, "OneTimeCost").GetComponent<UILabel>();
        tenTimeCost = Utils.FindChild(transform, "TenTimeCost").GetComponent<UILabel>();
        timeForFree = Utils.FindChild(transform, "TimeForFree").GetComponent<UILabel>();
        freeThisTime = Utils.FindChild(transform, "FreeThisTime").GetComponent<UILabel>();
        totalFamous = Utils.FindChild(transform, "TotalRep").GetComponent<UILabel>();
        totalIngot = Utils.FindChild(transform, "TotalIngot").GetComponent<UILabel>();
        tenTimesDesc = Utils.FindChild(transform, "10Time");
        elevenTimesDesc = Utils.FindChild(transform, "11Time");
        heroRelated = Utils.FindChild(transform, "HeroRelated");
        itemRelated = Utils.FindChild(transform, "ItemRelated");
        timesForHero = heroRelated.FindChild("TimesForHero").GetComponent<UILabel>();
        heroDescNotFree = heroRelated.FindChild("HeroDesc1").GetComponent<UISprite>();
        heroDescFree = heroRelated.FindChild("HeroDesc1_2").GetComponent<UISprite>();
        heroPlayer = heroRelated.GetComponentInChildren<AdvertisePlayer>();
        itemPlayer = itemRelated.GetComponentInChildren<AdvertisePlayer>();
    }

    private void InstallHandlers()
    {
        buyOneLis.onClick = OnBuyOne;
        buyTenLis.onClick = OnBuyTen;
        CommonHandler.PlayerPropertyChanged += RefreshFamousAndIngot;
    }

    private void UnInstallHandlers()
    {
        buyOneLis.onClick = null;
        buyTenLis.onClick = null;
        CommonHandler.PlayerPropertyChanged -= RefreshFamousAndIngot;
    }

    /// <summary>
    /// The call back of buying one hero or item event.
    /// </summary>
    /// <param name="go">The sender.</param>
    private void OnBuyOne(GameObject go)
    {
        if (isHeroChoose)
        {
            lotteryMode = isFreeTime ? LotteryConstant.LotteryModeFree : LotteryConstant.LotteryModeOnceCharge;
        }
        else
        {
            lotteryMode = LotteryConstant.LotteryModeOnceCharge;
        }
        if (isFreeTime && isHeroChoose)
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

    }

    /// <summary>
    /// Send the buy message from client side to server side.
    /// </summary>
    private void SendBuyMessage()
    {
        var msg = new CSLottery();
        msg.ActivityId = isHeroChoose
            ? scLotteryList.ListLotteryHeroInfo[0].ActivityId
            : scLotteryList.ListLotteryItemInfo[0].ActivityId;
        msg.LotteryType = isHeroChoose ? (sbyte)1 : (sbyte)2;
        msg.LotteryMode = (sbyte)lotteryMode;

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
        assertWindow.OkButtonClicked = OnCardConfirmOk;
        assertWindow.CancelButtonClicked = OnCardConfirmCancel;
        WindowManager.Instance.Show(typeof(AssertionWindow), true);
    }

    /// <summary>
    /// Show the confirm window for choose ten times.
    /// </summary>
    private void ShowTenConfirm()
    {
        var key = isHeroChoose
                  ? LotteryConstant.TenTimeHeroLotteryConfirmKey
                  : LotteryConstant.TenTimeItemLotteryConfirmKey;
        var str = LanguageManager.Instance.GetTextValue(key);
        ShowConfirm(string.Format(str, (isHeroChoose ? scLotteryList.LotteryHeroCost : scLotteryList.LotteryItemCost) * TenTimes));
    }

    /// <summary>
    /// Show the confirm window for choose one time.
    /// </summary>
    private void ShowOneConfirm()
    {
        var key = isHeroChoose
                  ? LotteryConstant.OneTimeHeroLotteryConfirmKey
                  : LotteryConstant.OneTimeItemLotteryConfirmKey;
        var str = LanguageManager.Instance.GetTextValue(key);
        ShowConfirm(string.Format(str, (isHeroChoose ? scLotteryList.LotteryHeroCost : scLotteryList.LotteryItemCost)));
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
        if (timeRemain.TotalSeconds > 0)
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
        if (gameObject.activeSelf == true)
        {
            StartCoroutine("UpdateFreeTime", timeRemain);
        }
    }

    /// <summary>
    /// Update the famous and chips on ui.
    /// </summary>
    private void RefreshFamousAndIngot(SCPropertyChangedNumber scpropertychanged)
    {
        totalFamous.text = PlayerModelLocator.Instance.Famous.ToString(CultureInfo.InvariantCulture);
        totalIngot.text = PlayerModelLocator.Instance.Diamond.ToString(CultureInfo.InvariantCulture);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Used to refresh the main window ui.
    /// </summary>
    /// <param name="lotteryList"></param>
    /// <param name="isChooseHeroCard"></param>
    public void Refresh(SCLotteryList lotteryList, bool isChooseHeroCard)
    {
        isHeroChoose = isChooseHeroCard;
        scLotteryList = lotteryList;

        if (isChooseHeroCard)
        {
            NGUITools.SetActiveChildren(heroRelated.gameObject, isHeroChoose);
            NGUITools.SetActiveChildren(itemRelated.gameObject, false);

            var oneTimeCostValue = lotteryList.LotteryHeroCost;
            oneTimeCost.text = oneTimeCostValue.ToString(CultureInfo.InvariantCulture);
            tenTimeCost.text = (oneTimeCostValue * TenTimes).ToString(CultureInfo.InvariantCulture);
            var isElevenTimes = lotteryList.ListLotteryHeroInfo[0].TenLotteryGiveElevenHero;
            elevenTimesDesc.gameObject.SetActive(isElevenTimes);
            tenTimesDesc.gameObject.SetActive(!isElevenTimes);
            if (heroPlayer.Textures.Count != 0 && heroPlayer.Textures.Count != 1)
            {
                heroPlayer.Play();
            }
            RefreshTimes(lotteryList.LastFreeLotteryTime, lotteryList.Get4StarHeroRestTimes);
        }
        else
        {
            NGUITools.SetActiveChildren(itemRelated.gameObject, !isHeroChoose);
            NGUITools.SetActiveChildren(heroRelated.gameObject, false);

            var oneTimeCostValue = lotteryList.LotteryItemCost;
            oneTimeCost.text = oneTimeCostValue.ToString(CultureInfo.InvariantCulture);
            tenTimeCost.text = (oneTimeCostValue * TenTimes).ToString(CultureInfo.InvariantCulture);
            var isElevenTimes = lotteryList.ListLotteryHeroInfo[0].TenLotteryGiveElevenHero;
            elevenTimesDesc.gameObject.SetActive(isElevenTimes);
            tenTimesDesc.gameObject.SetActive(!isElevenTimes);
            if (itemPlayer.Textures.Count != 0 && itemPlayer.Textures.Count != 1)
            {
                itemPlayer.Play();
            }
            RefreshTimes(lotteryList.LastFreeLotteryTime, lotteryList.Get4StarHeroRestTimes);
        }
        totalFamous.text = PlayerModelLocator.Instance.Famous.ToString(CultureInfo.InvariantCulture);
        totalIngot.text = PlayerModelLocator.Instance.Diamond.ToString(CultureInfo.InvariantCulture);
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
            if (int.Parse(timesForHero.text) == 0)
            {
                timesForHero.gameObject.SetActive(false);
                heroDescNotFree.gameObject.SetActive(false);
                heroDescFree.gameObject.SetActive(true);
            }
            else if (int.Parse(timesForHero.text) > 0)
            {
                timesForHero.gameObject.SetActive(true);
                heroDescNotFree.gameObject.SetActive(true);
                heroDescFree.gameObject.SetActive(false);
            }
            else
            {
                Logger.LogError("Not correct timesForHero Num.");
                return;
            }
            WindowManager.Instance.GetWindow<ChooseCardWindow>().ScLotteryList.LastFreeLotteryTime = lastTime;
        }
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
