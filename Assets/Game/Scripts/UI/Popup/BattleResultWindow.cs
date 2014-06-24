using UnityEngine;

/// <summary>
/// Battle window base controller.
/// </summary>
public abstract class BattleResultWindow : Window
{
    #region Public Fields

    public BattleResult OnBattleResult;
    public delegate void BattleResult(bool win);

    public GameObject Background;

    #endregion

    #region Private Fields

    protected UIEventListener BackgroundListener;

    #endregion

    #region Window

    public override void OnEnter()
    {
        Logger.Log("I am OnEnter with type - " + GetType().Name);
        BackgroundListener.onClick += OnBackgroundClick;
    }

    public override void OnExit()
    {
        Logger.Log("I am OnExit with type - " + GetType().Name);
        BackgroundListener.onClick -= OnBackgroundClick;
    }

    #endregion

    #region Public Methods

    public void Close()
    {
        var currentScreen = WindowManager.Instance.CurrentWindowMap[WindowGroupType.Screen];
        var battlemanager = currentScreen.GetComponent<InitBattleField>();
        battlemanager.ResetBattle();

        WindowManager.Instance.Show(WindowGroupType.Popup, false);
        //WindowManager.Instance.Show(typeof(UIMainScreenWindow), true);
        WindowManager.Instance.Show(typeof(MainMenuBarWindow), true);
        WindowManager.Instance.Show(typeof(MissionTabWindow), true);
    }

    #endregion

    #region Protected Methods

    protected abstract void OnBackgroundClick(GameObject sender);

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        BackgroundListener = UIEventListener.Get(Background);
    }

    #endregion
}
