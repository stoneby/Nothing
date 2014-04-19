using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoginMainWindow : Window
{
    private GameObject SpriteAccount;
    private GameObject SpriteServers;
    private GameObject BtnPlay;
    private GameObject ServerLabel;
    private GameObject AccountLabel;

    private UIEventListener AccountUIEventListener;
    private UIEventListener ServiceUIEventListener;
    private UIEventListener BtnLoginUIEventListener;

    #region Window

    public override void OnEnter()
    {
        var obj = ServiceManager.GetDefaultAccount();
        if (obj != null)
        {
            var aclabel = AccountLabel.GetComponent<UILabel>();
            aclabel.text = obj.Account;
            ServiceManager.ServerData = ServiceManager.GetServerByUrl(obj.Servers[obj.Servers.Count - 1]);
        }

        var lb = ServerLabel.GetComponent<UILabel>();
        if (ServiceManager.ServerData != null) lb.text = ServiceManager.ServerData.ServerName;

        BtnLoginUIEventListener.onClick += OnLoginButtonClick;
        AccountUIEventListener.onClick += OnAccountClick;
        ServiceUIEventListener.onClick += OnServerClick;

        EventManager.Instance.AddListener<LoginEvent>(OnSCPlayerInfoHandler);
    }

    public override void OnExit()
    {
        if (BtnLoginUIEventListener != null) BtnLoginUIEventListener.onClick -= OnLoginButtonClick;
        if (AccountUIEventListener != null) AccountUIEventListener.onClick -= OnAccountClick;
        if (ServiceUIEventListener != null) ServiceUIEventListener.onClick -= OnServerClick;

        EventManager.Instance.RemoveListener<LoginEvent>(OnSCPlayerInfoHandler);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        AccountLabel = transform.FindChild("Sprite - account/Label - account").gameObject;

        SpriteAccount = transform.FindChild("Sprite - account").gameObject;
        //SpriteAccount.AddComponent<OpenAccountHandler>();

        SpriteServers = transform.FindChild("Sprite - server").gameObject;
        //SpriteServers.AddComponent<OpenServersHandler>();

        BtnPlay = transform.FindChild("Image Button - play").gameObject;
        //BtnPlay.AddComponent<PlayHandler>();

        ServerLabel = transform.FindChild("Sprite - server/Label - server").gameObject;


        BtnLoginUIEventListener = UIEventListener.Get(BtnPlay);
        AccountUIEventListener = UIEventListener.Get(SpriteAccount);
        ServiceUIEventListener = UIEventListener.Get(SpriteServers);
    }

    #endregion

    private void OnLoginButtonClick(GameObject game)
    {
        var obj = ServiceManager.GetDefaultAccount();
        if (obj != null)
        {
            var csMsg = new CSPasswdLoginMsg();
            csMsg.DeviceId = "1";
            csMsg.DeviceType = 2;
            csMsg.Passwd = obj.Password;
            csMsg.AccountName = obj.Account;
            NetManager.SendMessage(csMsg);
        }
        else
        {
            OnAccountClick();
        }
    }

    private void OnAccountClick(GameObject game = null)
    {
        WindowManager.Instance.Show(typeof(LoginAccountWindow), true);
    }

    private void OnServerClick(GameObject game)
    {
        WindowManager.Instance.Show(typeof(LoginServersWindow), true);
    }

    private static void OnSCPlayerInfoHandler(LoginEvent e)
    {
        ServiceManager.SaveAccount();
    }
}
