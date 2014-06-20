using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class ChooseCardWindow : Window
{
    #region Private Fields

    private UIEventListener dimmerLis;
    private UIEventListener chooseItemLis;
    private UIEventListener chooseHeroLis;
    private UIEventListener fragCombineLis;

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
        dimmerLis = UIEventListener.Get(transform.FindChild("Dimmer").gameObject);
        chooseItemLis = UIEventListener.Get(transform.Find("ItemChooseCard/Frame").gameObject);
        chooseHeroLis = UIEventListener.Get(transform.Find("HeroChooseCard/Frame").gameObject);
        fragCombineLis = UIEventListener.Get(transform.Find("FragmentCombine/Frame").gameObject);
    }

    private void InstallHandlers()
    {
        dimmerLis.onClick += OnDimmer;
        chooseItemLis.onClick += ChooseItemCard;
        chooseHeroLis.onClick += ChooseHeroCard;
        fragCombineLis.onClick += OnFragCombine;
    }

    private void UnInstallHandlers()
    {
        dimmerLis.onClick -= OnDimmer;
        chooseItemLis.onClick -= ChooseItemCard;
        chooseHeroLis.onClick -= ChooseHeroCard;
        fragCombineLis.onClick -= OnFragCombine;
    }

    private void OnDimmer(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardWindow>(false);
    }

    private void ChooseItemCard(GameObject go)
    {
        var msg = new CSLotteryList
        {
            LotteryType = LotteryConstant.LotteryTypeItem
        };
        NetManager.SendMessage(msg);
    }

    private void ChooseHeroCard(GameObject go)
    {
        var msg = new CSLotteryList
                                {
                                    LotteryType = LotteryConstant.LotteryTypeHero
                                };
        NetManager.SendMessage(msg);
    }

    private void OnFragCombine(GameObject go)
    {
        WindowManager.Instance.Show<ChooseCardWindow>(false);
    }

    #endregion
}
