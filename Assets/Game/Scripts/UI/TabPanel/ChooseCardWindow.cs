using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ChooseCardWindow : Window
{
    #region Private Fields

    private UIEventListener closeLis;

    #endregion

    #region Private Methods
    
    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardWindow>(false);
    }

    #endregion

    #region Public Fields

    public HeroAndItemSummitHandler heroAndItemSummitHandler;
    public ActivityHandler activityHandler;
    public FragmentCombineHandler fragmentCombineHandler;

    #endregion

    #region Public Methods

    public void ChooseHeroCard()
    {
        Utils.FindChild(transform, "HeroAndItemSummit").gameObject.SetActive(true);
        var msg = new CSLotteryList
        {
            LotteryType = LotteryConstant.LotteryTypeHero
        };
        NetManager.SendMessage(msg);
    }

    public void ChooseItemCard()
    {
        Utils.FindChild(transform, "HeroAndItemSummit").gameObject.SetActive(true);
        var msg = new CSLotteryList
        {
            LotteryType = LotteryConstant.LotteryTypeItem
        };
        NetManager.SendMessage(msg);
    }

    public void OnActivity()
    {
        Utils.FindChild(transform, "Activity").gameObject.SetActive(true);
    }

    public void OnFragCombine()
    {
        Utils.FindChild(transform, "FragCombine").gameObject.SetActive(true);
        CSLotteryComposeList csMsg = new CSLotteryComposeList();
        NetManager.SendMessage(csMsg);
    }

    #endregion

    #region Window

    public override void OnEnter()
    {
        closeLis.onClick = OnClose;       
    }

    public override void OnExit()
    {
        closeLis.onClick = null;
    }

    // Use this for initialization
    void Awake()
    {
        closeLis = UIEventListener.Get(Utils.FindChild(transform, "Button-Close").gameObject);
    }

    #endregion
}
