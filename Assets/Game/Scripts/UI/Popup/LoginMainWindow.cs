using System.Collections.Generic;
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
    private GameObject BtnOpenSelectAccount;

    private GameObject SpriteSelectAccount;
    private GameObject LabelDefault;
    private GameObject SelectAccountContainer;

    private GameObject AccountItemPrefab;

//    private List<GameObject> AccountList; 

    private UIEventListener AccountUIEventListener;
    private UIEventListener ServiceUIEventListener;
    private UIEventListener BtnLoginUIEventListener;
    private UIEventListener BtnSelectAccountUIEventListener;

    #region Window

    public override void OnEnter()
    {

        ResetAccountList();

        if (ServiceManager.AccountArray != null && ServiceManager.AccountArray.Count > 1)
        {
            BtnOpenSelectAccount.SetActive(true);
        }
        else
        {
            BtnOpenSelectAccount.SetActive(false);
        }
        SpriteSelectAccount.SetActive(false);


        BtnLoginUIEventListener.onClick += OnLoginButtonClick;
        AccountUIEventListener.onClick += OnAccountClick;
        ServiceUIEventListener.onClick += OnServerClick;
        BtnSelectAccountUIEventListener.onClick += OnSelectAccountClick;

        EventManager.Instance.AddListener<LoginEvent>(OnSCPlayerInfoHandler);
        EventManager.Instance.AddListener<SelectAccountEvent>(OnSelectAccountHandler);
    }

    private void ResetAccountList()
    {
        var obj = ServiceManager.GetDefaultAccount();
        if (obj != null)
        {
            var aclabel = AccountLabel.GetComponent<UILabel>();
            aclabel.text = obj.Account;
            aclabel.GetComponent<LocalizeWidget>().enabled = false;
            aclabel = LabelDefault.GetComponent<UILabel>();
            aclabel.text = obj.Account;
            ServiceManager.ServerData = ServiceManager.GetServerByUrl(obj.Servers[obj.Servers.Count - 1]);
        }

        var lb = ServerLabel.GetComponent<UILabel>();
        if (ServiceManager.ServerData != null)
        {
            lb.text = ServiceManager.ServerData.ServerName;
            lb.GetComponent<LocalizeWidget>().enabled = false;
        }

        if (ServiceManager.AccountArray != null)
        {
            int theindex = 0;
            var table = SelectAccountContainer.GetComponent<UITable>();
            GameObject item;
            for (int i = ServiceManager.AccountArray.Count - 1; i >= 0; i--)
            {
                if (ServiceManager.AccountArray[i].Account != obj.Account)
                {
                    if (theindex < table.children.Count)
                    {
                        item = table.children[theindex].gameObject;
                        item.SetActive(true);
                    }
                    else
                    {
                        item = NGUITools.AddChild(SelectAccountContainer, AccountItemPrefab);
                    }
                    var col = item.GetComponent<AccountItemControl>();
                    col.SetData(ServiceManager.AccountArray[i]);
                    //AccountList.Add(item);
                    theindex++;
                }
            }

            while (theindex < table.children.Count)
            {
                item = table.children[theindex].gameObject;
                item.SetActive(false);
                theindex++;
            }
        }
    }

    private bool CheckDeleteAccount(Transform obj)
    {
        //throw new System.NotImplementedException();
        //var item = obj.gameObject.GetComponent<AccountItemControl>();
        //item
        return true;
    }

    

    public override void OnExit()
    {
        if (BtnLoginUIEventListener != null) BtnLoginUIEventListener.onClick -= OnLoginButtonClick;
        if (AccountUIEventListener != null) AccountUIEventListener.onClick -= OnAccountClick;
        if (ServiceUIEventListener != null) ServiceUIEventListener.onClick -= OnServerClick;
        if (BtnSelectAccountUIEventListener != null) BtnSelectAccountUIEventListener.onClick -= OnSelectAccountClick;

        EventManager.Instance.RemoveListener<LoginEvent>(OnSCPlayerInfoHandler);
        EventManager.Instance.RemoveListener<SelectAccountEvent>(OnSelectAccountHandler);


        GlobalUIEventManager.Instance.EventListener.onClick -= OnFallThroughClick;
    }

    #endregion

    #region Mono

    // Use this for initialization
    private void Awake()
    {
        AccountLabel = transform.FindChild("Sprite - account/Label - account").gameObject;

        SpriteAccount = transform.FindChild("Sprite - account").gameObject;
        //SpriteAccount.AddComponent<OpenAccountHandler>();

        SpriteServers = transform.FindChild("Sprite - server").gameObject;
        //SpriteServers.AddComponent<OpenServersHandler>();

        BtnPlay = transform.FindChild("Image Button - play").gameObject;
        //BtnPlay.AddComponent<PlayHandler>();

        ServerLabel = transform.FindChild("Sprite - server/Label - server").gameObject;

        BtnOpenSelectAccount = transform.FindChild("Sprite - account/Button - down").gameObject;
        SpriteSelectAccount = transform.FindChild("Sprite - select account").gameObject;
        LabelDefault = transform.FindChild("Sprite - select account/Label - default").gameObject;
        SelectAccountContainer = transform.FindChild("Sprite - select account/Scroll View/Table").gameObject;

        AccountItemPrefab = Resources.Load("Prefabs/Component/AccountItem") as GameObject;

        BtnLoginUIEventListener = UIEventListener.Get(BtnPlay);
        AccountUIEventListener = UIEventListener.Get(SpriteAccount);
        ServiceUIEventListener = UIEventListener.Get(SpriteServers);
        BtnSelectAccountUIEventListener = UIEventListener.Get(BtnOpenSelectAccount);
    }

    private void Start()
    {
        GlobalUIEventManager.Instance.EventListener.onClick += OnFallThroughClick;
    }

    #endregion

    private void OnLoginButtonClick(GameObject game)
    {
        var obj = ServiceManager.GetDefaultAccount();
        if (obj != null)
        {
            var csMsg = new CSPasswdLoginMsg
            {
                DeviceId = "1",
                DeviceType = 2,
                Passwd = obj.Password,
                AccountName = obj.Account
            };
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

    private void OnSCPlayerInfoHandler(LoginEvent e)
    {
        ServiceManager.SaveAccount();
    }

    private void OnSelectAccountHandler(SelectAccountEvent e)
    {
        if (e.type == SelectAccountEvent.TYPE_CHANGE)
        {
            ServiceManager.AddAccount(e.account);
            ResetAccountList();
            SpriteSelectAccount.SetActive(false);
            SpriteAccount.SetActive(true);
            SpriteServers.SetActive(true);
            BtnPlay.SetActive(true);
        }
        else
        {
            ServiceManager.DeleteAccount(e.account);
            ResetAccountList();
            
        }

//        var grid = SelectAccountContainer.GetComponent<UIGrid>();
//        grid.Reposition();
    }

    private void OnSelectAccountClick(GameObject game = null)
    {
        SpriteSelectAccount.SetActive(true);
        SpriteAccount.SetActive(false);
        SpriteServers.SetActive(false);
        BtnPlay.SetActive(false);
    }

    private void OnFallThroughClick(GameObject sender)
    {
        SpriteSelectAccount.SetActive(false);
        SpriteAccount.SetActive(true);
        SpriteServers.SetActive(true);
        BtnPlay.SetActive(true);
    }
}
