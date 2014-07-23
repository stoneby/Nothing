using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoginAccountWindow : Window
{
    private GameObject BtnLogin;
    private GameObject BtnRegister;
    private GameObject BtnReturn;

    private GameObject InputAccount;
    private GameObject InputPassword;

    private UIEventListener BtnLoginUIEventListener;
    private UIEventListener BtnRegisterUIEventListener;
    private UIEventListener BtnCloseUIEventListener;

    #region Window

    public override void OnEnter()
    {
        BtnLoginUIEventListener.onClick += OnLoginButtonClick;
        BtnRegisterUIEventListener.onClick += OnRegisterButtonClick;
        BtnCloseUIEventListener.onClick += OnCloseButtonClick;

        EventManager.Instance.AddListener<LoginEvent>(OnSCPlayerInfoHandler);

//        var obj = ServiceManager.GetDefaultAccount();
//        if (obj != null)
//        {
            var acinput = InputAccount.GetComponent<UIInput>();
            var pwinput = InputPassword.GetComponent<UIInput>();
            acinput.value = ServiceManager.DebugUserName;
            pwinput.value = ServiceManager.DebugPassword;
//        }
    }

    public override void OnExit()
    {
        if (BtnLoginUIEventListener != null) BtnLoginUIEventListener.onClick -= OnLoginButtonClick;
        if (BtnRegisterUIEventListener != null) BtnRegisterUIEventListener.onClick -= OnRegisterButtonClick;
        if (BtnCloseUIEventListener != null) BtnCloseUIEventListener.onClick -= OnCloseButtonClick;

        EventManager.Instance.RemoveListener<LoginEvent>(OnSCPlayerInfoHandler);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        BtnLogin = transform.FindChild("Image Button - login").gameObject;

        BtnRegister = transform.FindChild("Image Button - create").gameObject;

        BtnReturn = transform.FindChild("Image Button - close").gameObject;

        InputAccount = transform.FindChild("Input - account").gameObject;
        InputPassword = transform.FindChild("Input - password").gameObject;

        BtnLoginUIEventListener = UIEventListener.Get(BtnLogin);
        BtnRegisterUIEventListener = UIEventListener.Get(BtnRegister);
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

//        if (ServiceManager.AccountData == null)
//        {
//            ServiceManager.AccountData = new AccountVO();
//        }
        ServiceManager.DebugUserName = acinput.value;
        ServiceManager.DebugPassword = pwinput.value;
        ServiceManager.AddServer(ServiceManager.ServerData.Url);
        ServiceManager.IsDebugAccount = 1;

        var csMsg = new CSPasswdLoginMsg();
        csMsg.DeviceId = "1";
        csMsg.DeviceType = 2;
        csMsg.Passwd = pwinput.value;
        csMsg.AccountName = acinput.value;
        NetManager.SendMessage(csMsg);
        
    }

    private void OnRegisterButtonClick(GameObject game)
    {
        WindowManager.Instance.Show(typeof(LoginRegisterWindow), true);
    }

    private void OnCloseButtonClick(GameObject game)
    {
        WindowManager.Instance.Show(typeof(LoginMainWindow), true);
    }

    private static void OnSCPlayerInfoHandler(LoginEvent e)
    {
        Logger.LogWarning("OnSCPlayerInfoHandler get called back, with event-" + e.Message);
        //Logger.LogWarning("Account data: " + ServiceManager.AccountData);
        if (ServiceManager.IsDebugAccount == 1)
        {
            ServiceManager.SetDebugAccount(ServiceManager.DebugUserName, ServiceManager.DebugPassword);
        }
        
        //ServiceManager.SaveAccount();
    }
}
