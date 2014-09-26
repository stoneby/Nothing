using System.Collections.Generic;
using KXSGCodec;
using Property;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIMailDetailWindow : Window
{
    private UILabel contentLabel;
    private UILabel senderLabel;
    private UILabel titleLabel;
    private UILabel timeLabel;
    private MailMsgInfo mailMsgInfo;
    private Transform decorate;
    private UIEventListener receiveLis;
    private Transform attachment;
    private Transform attachItems;
    private Transform received;
    private UIEventListener backLis;
    private const float Interval = 120f;

    private string content;
    private string Content
    {
        get { return content; }
        set
        {
            content = value;
            contentLabel.text = content;
        }
    }

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.EmailDetailWindow);
        MailHandler.MailIsUpdated += OnMailUpdate;
    }

    public override void OnExit()
    {
        MtaManager.TrackEndPage(MtaType.EmailDetailWindow);
        mailMsgInfo = null;
        Utils.DestoryChildren(attachItems);
        MailHandler.MailIsUpdated -= OnMailUpdate;
    }

    #endregion

    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        contentLabel = transform.Find("Content").GetComponent<UILabel>();
        senderLabel = transform.Find("Sender").GetComponent<UILabel>();
        titleLabel = transform.Find("Title").GetComponent<UILabel>();
        timeLabel = transform.Find("Time").GetComponent<UILabel>();
        decorate = transform.Find("Decorates");
        receiveLis = UIEventListener.Get(transform.Find("Attachments/Button-Receive").gameObject);
        backLis = UIEventListener.Get(transform.Find("Button-Back").gameObject);
        receiveLis.onClick = OnReceive;
        backLis.onClick = OnClose;
        attachment = transform.Find("Attachments");
        attachItems = attachment.Find("MailItems");
        received = transform.Find("Received");
    }

    private void OnMailUpdate(int mailid, sbyte state)
    {
        if (mailMsgInfo.Uuid == mailid && (MailConstant.MailState)state == MailConstant.MailState.Gain)
        {
            NGUITools.SetActive(receiveLis.gameObject, false);
            NGUITools.SetActive(received.gameObject, true);
            Utils.DestoryChildren(attachItems);
        }

    }

    private void OnClose(GameObject go)
    {
        WindowManager.Instance.Show<UIMailDetailWindow>(false);
    }

    private void OnReceive(GameObject go)
    {
        var mailInfo = mailMsgInfo;
        var mailReceive = WindowManager.Instance.Show<UIMailReceiveWindow>(true);
        mailReceive.Init(mailInfo);
    }

    public void Init(int mailId, string txt)
    {
        Content = txt;
        mailMsgInfo = MailModelLocator.Instance.FindMailViaMailId(mailId);
        if(mailMsgInfo != null)
        {
            senderLabel.text = mailMsgInfo.Sender;
            titleLabel.text = mailMsgInfo.Title;
            timeLabel.text = Utils.ConvertFromJavaTimestamp(mailMsgInfo.CreateTime).ToString("yyyy-M-d");
            var hasAttachment = mailMsgInfo.Attachments != null && mailMsgInfo.Attachments.Count > 0;
            var gain = (MailConstant.MailState) mailMsgInfo.State == MailConstant.MailState.Gain;
            NGUITools.SetActive(attachment.gameObject, hasAttachment);
            NGUITools.SetActive(decorate.gameObject, hasAttachment);
            NGUITools.SetActive(received.gameObject, gain);
            NGUITools.SetActive(receiveLis.gameObject, !gain);
            if (hasAttachment && !gain)
            {
                MailConstant.ShowAttachments(mailMsgInfo.Attachments, attachItems, Interval);
            }
        }
    }

    #endregion
}
