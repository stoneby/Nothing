using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class BattleLostWindow : Window
{
    private GameObject LabelLevel;
    private GameObject ExpBar;
    private GameObject BtnRetrun;
    private GameObject BtnToItem;
    private GameObject BtnToHero;
    private GameObject BtnBackToRaid;

    private UIEventListener ReturnUIEventListener;
    private UIEventListener ToEquipUIEventListener;
    private UIEventListener BackToRaidUIEventListener;
    private UIEventListener ToHeroUIEventListener;

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.BattleFailWindow);
        //base.OnEnter();
        //Logger.Log("I am OnEnter with type - " + GetType().Name);

        ReturnUIEventListener.onClick += ReturnHandler;
        ToEquipUIEventListener.onClick += ToEquipHandler;
        BackToRaidUIEventListener.onClick += BackToRaidHandler;
        ToHeroUIEventListener.onClick += ToHeroHandler;

        ShowExp();
    }

    private void ShowExp()
    {
        var lb = LabelLevel.GetComponent<UILabel>();
        lb.text = "Lv." + PlayerModelLocator.Instance.Level;

        var bar = ExpBar.GetComponent<UIProgressBar>();
        var temp = LevelModelLocator.Instance.GetLevelByTemplateId(PlayerModelLocator.Instance.Level + 1);
        var v = (float)(PlayerModelLocator.Instance.Exp) / temp.MaxExp;
        bar.value = v;
    }

    public override void OnExit()
    {
        if (ReturnUIEventListener != null) ReturnUIEventListener.onClick -= ReturnHandler;
        if (ToEquipUIEventListener != null) ToEquipUIEventListener.onClick -= ToEquipHandler;
        if (BackToRaidUIEventListener != null) BackToRaidUIEventListener.onClick -= BackToRaidHandler;
        if (ToHeroUIEventListener != null) ToHeroUIEventListener.onClick -= ToHeroHandler;

        //base.OnExit();
        //Logger.Log("I am OnExit with type - " + GetType().Name);
        MtaManager.TrackEndPage(MtaType.BattleFailWindow);
    }

    private void ReturnHandler(GameObject obj)
    {
        var currentScreen = WindowManager.Instance.CurrentWindowMap[WindowGroupType.Screen];
        var battlemanager = currentScreen.GetComponent<InitBattleField>();
        battlemanager.ResetBattle();

        // [NOTE:] Clear history window at this point.
        WindowManager.Instance.ClearHistory();

        WindowManager.Instance.Show(WindowGroupType.Popup, false);
        WindowManager.Instance.Show(typeof(UIMainScreenWindow), true);
        WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
        //WindowManager.Instance.Show(typeof(MissionTabWindow), true);
    }

    private void ToEquipHandler(GameObject obj)
    {

    }

    private void BackToRaidHandler(GameObject obj)
    {
        var currentScreen = WindowManager.Instance.CurrentWindowMap[WindowGroupType.Screen];
        var battlemanager = currentScreen.GetComponent<InitBattleField>();
        battlemanager.ResetBattle();

        // [NOTE:] Clear history window at this point.
        WindowManager.Instance.ClearHistory();

        WindowManager.Instance.Show(WindowGroupType.Popup, false);
        WindowManager.Instance.Show(typeof(UIMainScreenWindow), true);
        WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
        WindowManager.Instance.Show(typeof(MissionTabWindow), true);
    }

    private void ToHeroHandler(GameObject obj)
    {

    }

    #endregion

    #region Battle Result Window

//    protected override void OnBackgroundClick(GameObject sender)
//    {
//        Close();
//
//        if (OnBattleResult != null)
//        {
//            OnBattleResult(false);
//        }
//    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        LabelLevel = transform.FindChild("Label level").gameObject;
        ExpBar = transform.FindChild("Progress Bar exp").gameObject;

        BtnBackToRaid = transform.FindChild("Container normal/Button raid").gameObject;
        BtnRetrun = transform.FindChild("Container normal/Button next").gameObject;
        BtnToItem = transform.FindChild("Container normal/Button equip").gameObject;
        BtnToHero = transform.FindChild("Container normal/Button hero").gameObject;

        BackToRaidUIEventListener = UIEventListener.Get(BtnBackToRaid);
        ReturnUIEventListener = UIEventListener.Get(BtnRetrun);
        ToEquipUIEventListener = UIEventListener.Get(BtnToItem);
        ToHeroUIEventListener = UIEventListener.Get(BtnToHero);

        ShowExp();
    }

    #endregion
}
