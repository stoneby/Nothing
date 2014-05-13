using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class LoginCreateRoleWindow : Window
{
    private GameObject BtnLogin;

    private GameObject InputAccount;

    private UIEventListener BtnLoginUIEventListener;

    #region Window

    public override void OnEnter()
    {
        BtnLoginUIEventListener.onClick += OnLoginButtonClick;
    }

    public override void OnExit()
    {
        if (BtnLoginUIEventListener != null) BtnLoginUIEventListener.onClick -= OnLoginButtonClick;
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        BtnLogin = transform.FindChild("Image Button - login").gameObject;

        InputAccount = transform.FindChild("Input - account").gameObject;

        BtnLoginUIEventListener = UIEventListener.Get(BtnLogin);
    }

    #endregion

    private void OnLoginButtonClick(GameObject game)
    {
        var acinput = InputAccount.GetComponent<UIInput>();


        if (acinput.value == "" || acinput.value == "请输入昵称")
        {
            PopTextManager.PopTip("请输入昵称");
            return;
        }

        Logger.Log("MessageType.SC_CREATE_PLAYER");
        var csMsg = new CSCreatePlayerMsg();
        var act = ServiceManager.AccountData ?? ServiceManager.GetDefaultAccount();
        csMsg.Name = acinput.value;
        PopTextManager.PopTip("登录成功，正在创建角色(" + csMsg.Name + ")");
        NetManager.SendMessage(csMsg);

//        var csMsg = new CSPasswdLoginMsg();
//        csMsg.DeviceId = "1";
//        csMsg.DeviceType = 2;
//        csMsg.Passwd = pwinput.value;
//        csMsg.AccountName = acinput.value;
//        NetManager.SendMessage(csMsg);

    }
}
