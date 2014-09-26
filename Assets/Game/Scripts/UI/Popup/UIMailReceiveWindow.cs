using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIMailReceiveWindow : Window
{
    private MailMsgInfo mailMsgInfo;
    //private const float Interval = 120f;
    private UIGrid mailItems;
    private UIEventListener okLis;
    private UIEventListener dimmerLis;

    #region Window

    public override void OnEnter()
    {
        MailHandler.MailIsUpdated += OnMailUpdate;
    }

    public override void OnExit()
    {
        mailMsgInfo = null;
        Utils.DestoryChildren(mailItems.transform);
        MailHandler.MailIsUpdated -= OnMailUpdate;
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    void Awake()
    {
        mailItems = transform.Find("MailItems").GetComponent<UIGrid>();
        okLis = UIEventListener.Get(transform.Find("OK").gameObject);
        okLis.onClick = OnOk;
        dimmerLis = UIEventListener.Get(transform.Find("Dimmer").gameObject);
        dimmerLis.onClick = OnDimmer;
    }

    private void OnDimmer(GameObject go)
    {
        CloseWindow();
    }

    private void OnOk(GameObject go)
    {
        var csmsg = new CSOptionMailMsg
        {
            OptionId = MailConstant.ReceiveOperId,
            NeedContent = MailConstant.DontNeedContent,
            Uuid = mailMsgInfo.Uuid,
            Version = mailMsgInfo.Version
        };
        NetManager.SendMessage(csmsg);   
    }

    private void OnMailUpdate(int mailid, sbyte state)
    {
        if(mailMsgInfo.Uuid == mailid)
        {
            CloseWindow();
        }
    }

    private void CloseWindow()
    {
        WindowManager.Instance.Show<UIMailReceiveWindow>(false);
    }

    #endregion

    public void Init(MailMsgInfo info)
    {
        mailMsgInfo = info;
        if (mailMsgInfo != null)
        {
            var hasAttachment = mailMsgInfo.Attachments != null && mailMsgInfo.Attachments.Count > 0;
            if (hasAttachment)
            {
                MailConstant.ShowAttachments(mailMsgInfo.Attachments, mailItems.transform, 0);
                mailItems.repositionNow = true;
            }
        }
    }
}
