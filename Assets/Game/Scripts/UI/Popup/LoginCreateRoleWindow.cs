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
    private UIEventListener btnRandomNameUiEventListener;
    private UIEventListener btnClearWordsUiEventListener;

    #region Window

    public override void OnEnter()
    {
        BtnLoginUIEventListener.onClick += OnLoginButtonClick;
        btnRandomNameUiEventListener.onClick = OnRandomNameButtonClick;
        btnClearWordsUiEventListener.onClick = OnClearWordsButtonClick;
    }

    public override void OnExit()
    {
        if (BtnLoginUIEventListener != null) BtnLoginUIEventListener.onClick -= OnLoginButtonClick;
        btnRandomNameUiEventListener = null;
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Awake()
    {
        BtnLogin = transform.FindChild("Image Button - login").gameObject;

        InputAccount = transform.FindChild("Input - account").gameObject;

        BtnLoginUIEventListener = UIEventListener.Get(BtnLogin);
        btnRandomNameUiEventListener = UIEventListener.Get(transform.FindChild("RandomCharName").gameObject);
        btnClearWordsUiEventListener = UIEventListener.Get(Utils.FindChild(transform, "ClearWords").gameObject);
    }

    #endregion

    private void OnLoginButtonClick(GameObject game)
    {
        var acinput = InputAccount.GetComponent<UIInput>();


        if (acinput.value == "" || acinput.value == "请输入昵称")
        {
            PopTextManager.PopTip("请输入昵称", false);
            return;
        }

        Logger.Log("MessageType.SC_CREATE_PLAYER");
        var csMsg = new CSCreatePlayerMsg();
        //        if (ServiceManager.AccountData == null)
        //        {
        //            ServiceManager.GetDefaultAccount();
        //        }
        csMsg.Name = acinput.value;
        PopTextManager.PopTip("正在创建角色(" + csMsg.Name + ")");
        NetManager.SendMessage(csMsg);
    }

    /// <summary>
    /// Send RandomName request to server.
    /// </summary>
    /// <param name="go"></param>
    private void OnRandomNameButtonClick(GameObject go)
    {
        var msg = new CSRandomCharNameMsg();
        NetManager.SendMessage(msg);
    }

    /// <summary>
    /// Clear inputed words.
    /// </summary>
    /// <param name="go"></param>
    private void OnClearWordsButtonClick(GameObject go)
    {
        var acinput = InputAccount.GetComponent<UIInput>();
        acinput.value = "";

        var label = InputAccount.GetComponentInChildren<UILabel>();
        label.text = "请输入昵称";
    }
}
