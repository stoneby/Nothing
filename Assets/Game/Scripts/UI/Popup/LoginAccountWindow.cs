using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoginAccountWindow : Window
{
    private GameObject BtnLogin;
    private GameObject BtnReturn;

    private GameObject InputAccount;
    private GameObject InputPassword;

    private UIEventListener BtnLoginUIEventListener;
    private UIEventListener BtnCloseUIEventListener;

    #region Window

    public override void OnEnter()
    {
        BtnLoginUIEventListener.onClick += OnLoginButtonClick;
        BtnCloseUIEventListener.onClick += OnCloseButtonClick;

        EventManager.Instance.AddListener<LoginEvent>(OnSCPlayerInfoHandler);

        var acinput = InputAccount.GetComponent<UIInput>();
        var pwinput = InputPassword.GetComponent<UIInput>();
        acinput.value = ServiceManager.DebugUserName;
        pwinput.value = ServiceManager.DebugPassword;
    }

    public override void OnExit()
    {
        if (BtnLoginUIEventListener != null) BtnLoginUIEventListener.onClick -= OnLoginButtonClick;
        if (BtnCloseUIEventListener != null) BtnCloseUIEventListener.onClick -= OnCloseButtonClick;

        EventManager.Instance.RemoveListener<LoginEvent>(OnSCPlayerInfoHandler);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        BtnLogin = transform.FindChild("Image Button - login").gameObject;

        BtnReturn = transform.FindChild("Image Button - close").gameObject;

        InputAccount = transform.FindChild("Input - account").gameObject;
        InputPassword = transform.FindChild("Input - password").gameObject;

        BtnLoginUIEventListener = UIEventListener.Get(BtnLogin);
        BtnCloseUIEventListener = UIEventListener.Get(BtnReturn);
    }

    #endregion

    private void OnLoginButtonClick(GameObject game)
    {
        var acinput = InputAccount.GetComponent<UIInput>();

        var pwinput = InputPassword.GetComponent<UIInput>();

        if (acinput.value == "" || acinput.value == "«Î ‰»Î’À∫≈")
        {
            return;
        }

        if (pwinput.value == "" || pwinput.value == "«Î ‰»Î√‹¬Î")
        {
            return;
        }

        ServiceManager.DebugUserName = acinput.value;
        ServiceManager.DebugPassword = pwinput.value;
        ServiceManager.AddServer(ServiceManager.ServerData.Url);
        ServiceManager.IsDebugAccount = 1;

        if (!HttpResourceManager.Instance.IsLoadTemplateFinished)
        {
            Logger.Log("Start loading template.");
            HttpResourceManager.Instance.LoadTemplate();
            WindowManager.Instance.Show<LoadingWaitWindow>(true);
        }

        var csMsg = new CSPasswdLoginMsg();
        csMsg.DeviceId = "1";
        csMsg.DeviceType = 2;
        csMsg.DeviceModel = SystemInfo.deviceModel;
        csMsg.Passwd = pwinput.value;
        csMsg.AccountName = acinput.value;
        NetManager.SendMessage(csMsg);
    }

    private void OnCloseButtonClick(GameObject game)
    {
        WindowManager.Instance.Show(typeof(LoginMainWindow), true);
    }

    private static void OnSCPlayerInfoHandler(LoginEvent e)
    {
        Logger.LogWarning("OnSCPlayerInfoHandler get called back, with event-" + e.Message);

        if (ServiceManager.IsDebugAccount == 1)
        {
            ServiceManager.SetDebugAccount(ServiceManager.DebugUserName, ServiceManager.DebugPassword);
        }
    }
}
