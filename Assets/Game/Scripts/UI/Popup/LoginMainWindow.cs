using KXSGCodec;
using System.Collections;
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

    private GameObject TexLogo;
    private GameObject ContainerBox;

    private UIEventListener AccountUIEventListener;
    private UIEventListener ServiceUIEventListener;
    private UIEventListener BtnLoginUIEventListener;

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.LoginMainWindow);
        ResetAccountList();

        BtnLoginUIEventListener.onClick += OnLoginButtonClick;
        AccountUIEventListener.onClick += OnAccountClick;
        ServiceUIEventListener.onClick += OnServerClick;

        //EventManager.Instance.AddListener<LoginEvent>(OnSCPlayerInfoHandler);
        if (ServiceManager.IsCheck)
        {
            FeidouManager.DoLogin();
        }
        else
        {
            StartCoroutine(PlayLogo());
        }
    }

    IEnumerator PlayLogo()
    {
        TexLogo.transform.localPosition = new Vector3(0, 0, 0);
        TexLogo.transform.localScale = new Vector3(0.01f,0.01f,1);
        ContainerBox.transform.localPosition = new Vector3(0, 420, 0);
        yield return new WaitForSeconds(0.1f);
        PlayTweenScale(TexLogo, 0.3f, new Vector3(0.01f, 0.01f, 1), new Vector3(1.3f,1.3f,1));
        yield return new WaitForSeconds(0.3f);
        PlayTweenScale(TexLogo, 0.1f, new Vector3(1.3f, 1.3f, 1), new Vector3(1, 1, 1));
//        yield return new WaitForSeconds(0.1f);
//        PlayTweenScale(TexLogo, 0.05f, new Vector3(1, 1, 1), new Vector3(1.2f, 1.2f, 1));
//        yield return new WaitForSeconds(0.05f);
//        PlayTweenScale(TexLogo, 0.05f, new Vector3(1.2f, 1.2f, 1), new Vector3(1, 1, 1));
        yield return new WaitForSeconds(0.2f);
        PlayTweenPosition(TexLogo, 0.4f, new Vector3(0,0,0), new Vector3(0,180,0));
        yield return new WaitForSeconds(0.3f);
        PlayTweenPosition(ContainerBox, 0.4f, new Vector3(0, 420, 0), new Vector3(0, 0, 0));
    }

    void PlayTweenScale(GameObject obj, float playtime, Vector3 from, Vector3 to)
    {
        TweenScale ts = obj.AddComponent<TweenScale>();
        ts.from = from;
        ts.to = to;
        ts.duration = playtime;
        ts.PlayForward();
        Destroy(ts, playtime);
    }

    void PlayTweenPosition(GameObject obj, float playtime, Vector3 from, Vector3 to)
    {
        TweenPosition ts = obj.AddComponent<TweenPosition>();
        ts.from = from;
        ts.to = to;
        ts.duration = playtime;
        ts.PlayForward();
        Destroy(ts, playtime);
    }

    private void ResetAccountList()
    {
         var aclabel = AccountLabel.GetComponent<UILabel>();
        if (ServiceManager.DebugUserName == "" && ServiceManager.UserName == "")
        {
            aclabel.text = "";
        }
        else
        {
            var str = (ServiceManager.IsDebugAccount == 1) ? ServiceManager.DebugUserName : ServiceManager.UserName;
            aclabel.text = "" + str;
        }
        
        aclabel.GetComponent<LocalizeWidget>().enabled = false;

        var lb = ServerLabel.GetComponent<UILabel>();
        if (ServiceManager.ServerData != null)
        {
            lb.text = "" + ServiceManager.ServerData.ServerName;
            lb.GetComponent<LocalizeWidget>().enabled = false;
        }
    }

//    private bool CheckDeleteAccount(Transform obj)
//    {
//        //throw new System.NotImplementedException();
//        //var item = obj.gameObject.GetComponent<AccountItemControl>();
//        //item
//        return true;
//    }

    

    public override void OnExit()
    {
        if (BtnLoginUIEventListener != null) BtnLoginUIEventListener.onClick -= OnLoginButtonClick;
        if (AccountUIEventListener != null) AccountUIEventListener.onClick -= OnAccountClick;
        if (ServiceUIEventListener != null) ServiceUIEventListener.onClick -= OnServerClick;

//        EventManager.Instance.RemoveListener<LoginEvent>(OnSCPlayerInfoHandler);
        MtaManager.TrackEndPage(MtaType.LoginMainWindow);
    }

    #endregion

    #region Mono

    // Use this for initialization
    private void Awake()
    {
        AccountLabel = transform.FindChild("Panel - box/Container - control/Sprite - account/Label - account").gameObject;
        SpriteAccount = transform.FindChild("Panel - box/Container - control/Sprite - account").gameObject;
        SpriteServers = transform.FindChild("Panel - box/Container - control/Sprite - server").gameObject;
        BtnPlay = transform.FindChild("Panel - box/Container - control/Image Button - play").gameObject;
        ServerLabel = transform.FindChild("Panel - box/Container - control/Sprite - server/Label - server").gameObject;

        TexLogo = transform.FindChild("Texture - logo").gameObject;
        ContainerBox = transform.FindChild("Panel - box/Container - control").gameObject;

        

        BtnLoginUIEventListener = UIEventListener.Get(BtnPlay);
        AccountUIEventListener = UIEventListener.Get(SpriteAccount);
        ServiceUIEventListener = UIEventListener.Get(SpriteServers);
    }

    #endregion

    private void OnLoginButtonClick(GameObject game)
    {
        //var obj = ServiceManager.GetDefaultAccount();
        if (ServiceManager.OpenTestAccount)
        {
            if (ServiceManager.DebugUserName != "" && ServiceManager.DebugPassword != "")
            {
                var csMsg = new CSPasswdLoginMsg
                {
                    DeviceId = "1",
                    DeviceType = 2,
                    Passwd = ServiceManager.DebugPassword,
                    AccountName = ServiceManager.DebugUserName
                };
                NetManager.SendMessage(csMsg);
                ServiceManager.AddServer(ServiceManager.ServerData.Url);
            }
            else
            {
                OnAccountClick();
            }
        }
        else
        {
            FeidouManager.DoLogin();
        }
    }

    private void OnAccountClick(GameObject game = null)
    {
        if (ServiceManager.OpenTestAccount)
        {
            ServiceManager.IsDebugAccount = 1;
            WindowManager.Instance.Show(typeof(LoginAccountWindow), true);
        }
        else
        {
            FeidouManager.DoLogin();  
        }
    }

    private void OnServerClick(GameObject game)
    {
        WindowManager.Instance.Show(typeof(LoginServersWindow), true);
    }

//    private void OnSCPlayerInfoHandler(LoginEvent e)
//    {
//        //ServiceManager.SaveAccount();
//    }
}
