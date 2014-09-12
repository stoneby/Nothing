using System;
using System.Collections.Generic;
using KXSGCodec;
using Property;
using Template.Auto.Hero;
using UnityEngine;

public class MailItem : MonoBehaviour
{
    public MailMsgInfo MailMsgInfo { get; private set; }
    public TweenRotation TweenRotation;

    private UILabel senderLabel;
    private UILabel timeLabel;
    private UILabel titleLabel;
    private UILabel attachLabel;
    private UILabel remainTimeLabel;
    private Transform receivedIcon;
    private UIEventListener receiveLis;
    private UIEventListener delLis;
    private UIEventListener detailLis;
    private UISprite mailStateSprite;

    private const string UnReadNoAttach = "MailState0";
    private const string UnReadAttach = "MailState1";
    private const string ReadAttach = "MailState2";
    private const string ReadNoAttach = "MailState3";

    private MailConstant.MailState mailState;
    public MailConstant.MailState MailState
    {
        get { return mailState; }
        set
        {
            var hasAttachment = (MailMsgInfo.Attachments != null && MailMsgInfo.Attachments.Count > 0);
            switch (value)
            {
                case MailConstant.MailState.UnRead:
                    {
                        mailStateSprite.spriteName = hasAttachment ? UnReadAttach : UnReadNoAttach;
                        break;
                    }
                case MailConstant.MailState.Read:
                    {
                        mailStateSprite.spriteName = hasAttachment ? ReadAttach : ReadNoAttach;
                        break;
                    }
                case MailConstant.MailState.Gain:
                    {
                        mailStateSprite.spriteName = ReadNoAttach;
                        NGUITools.SetActive(receiveLis.gameObject, false);
                        NGUITools.SetActive(receivedIcon.gameObject, true);
                        break;
                    }
            }
            mailState = value;
        }
    }

    private TimeSpan timeRemain;
    public TimeSpan TimeRemain
    {
        get
        {
            return timeRemain;
        }
        set
        {
            timeRemain = value;
            remainTimeLabel.text = MailConstant.ConvertTimeSpanToString(timeRemain);
        }
    }

    private const string Multipler = "X";
    private const string Separator = "  ";

    private void Awake()
    {
        mailStateSprite = transform.Find("MailState").GetComponent<UISprite>();
        senderLabel = transform.Find("Sender").GetComponent<UILabel>();
        timeLabel = transform.Find("Time").GetComponent<UILabel>();
        titleLabel = transform.Find("Title").GetComponent<UILabel>();
        attachLabel = transform.Find("Attach").GetComponent<UILabel>();
        remainTimeLabel = transform.Find("RemainTime/Value").GetComponent<UILabel>();
        receivedIcon = transform.Find("ReceiveIcon");
        NGUITools.SetActive(receivedIcon.gameObject, false);
        receiveLis = UIEventListener.Get(transform.Find("Button-Receive").gameObject);
        delLis = UIEventListener.Get(transform.Find("Button-Del").gameObject);
        detailLis = UIEventListener.Get(gameObject);
        ShowDel(false);
    }

    private void OnDel(GameObject go)
    {
        var state = (MailConstant.MailState) MailMsgInfo.State;
        var isUnRead = (state == MailConstant.MailState.UnRead);
        var hasAttachNotReceive = state != MailConstant.MailState.Gain &&
                                  MailMsgInfo.Attachments != null && MailMsgInfo.Attachments.Count > 0;
    
        if(isUnRead || hasAttachNotReceive)
        {
            var content =
                LanguageManager.Instance.GetTextValue(isUnRead
                                                          ? MailConstant.MailDelNoReadKey
                                                          : MailConstant.MailDelAttachKey);
            var assert = WindowManager.Instance.GetWindow<AssertionWindow>();
            assert.Title = "";
            assert.Message = content;
            assert.AssertType = AssertionWindow.Type.OkCancel;
            assert.OkButtonClicked += OnDelOk;
            WindowManager.Instance.Show<AssertionWindow>(true);
            return;
        }
        SendDelMessage();
    }

    private void SendDelMessage()
    {
        var csmsg = new CSOptionMailMsg
                        {
                            OptionId = MailConstant.DeleteOperId,
                            NeedContent = MailConstant.DontNeedContent,
                            Uuid = MailMsgInfo.Uuid,
                            Version = MailMsgInfo.Version
                        };
        NetManager.SendMessage(csmsg);
    }

    private void OnDelOk(GameObject sender)
    {
        var assert = WindowManager.Instance.GetWindow<AssertionWindow>();
        assert.OkButtonClicked -= OnDelOk;
        SendDelMessage();
    }

    private void OnReceive(GameObject go)
    {
        var mailReceive = WindowManager.Instance.Show<UIMailReceiveWindow>(true);
        mailReceive.Init(MailMsgInfo);
    }

    private void OnDetail(GameObject go)
    {
        if(string.IsNullOrEmpty(MailMsgInfo.Content))
        {
            var csmsg = new CSOptionMailMsg
            {
                OptionId = MailConstant.DisplayOperId,
                NeedContent = MailConstant.NeedContent,
                Uuid = MailMsgInfo.Uuid,
                Version = MailMsgInfo.Version
            };
            NetManager.SendMessage(csmsg);
        }
        else
        {
            var mailDetail = WindowManager.Instance.Show<UIMailDetailWindow>(true);
            mailDetail.Init(MailMsgInfo.Uuid, MailMsgInfo.Content);
        }
    }

    public void Init(MailMsgInfo mailMsgInfo)
    {
        MailMsgInfo = mailMsgInfo;
        if (MailMsgInfo != null)
        {
            senderLabel.text = MailMsgInfo.Sender;
            timeLabel.text = Utils.ConvertFromJavaTimestamp(MailMsgInfo.CreateTime).ToString("yyyy-M-d");
            titleLabel.text = MailMsgInfo.Title;
            attachLabel.text = GetAttachDesc(MailMsgInfo.Attachments);
            DateTime expireTime = Utils.ConvertFromJavaTimestamp(MailMsgInfo.Deadline);
            TimeRemain = expireTime.Subtract(DateTime.Now);
            var hasAttachment = (MailMsgInfo.Attachments != null && MailMsgInfo.Attachments.Count > 0);
            NGUITools.SetActive(receiveLis.gameObject, hasAttachment);
            MailState = (MailConstant.MailState)MailMsgInfo.State;
        }
    }

    private string GetAttachDesc(IEnumerable<MailAttachment> attachments)
    {
        var heroTemps = HeroModelLocator.Instance.HeroTemplates.HeroTmpls;
        var result = "";
        if (attachments != null)
        {
            foreach (var attach in attachments)
            {
                switch (attach.Type)
                {
                    case MailConstant.HeroAttachType:
                        {
                            HeroTemplate heroTemplate;
                            heroTemps.TryGetValue(attach.ItemId, out heroTemplate);
                            if (heroTemplate == null)
                            {
                                Logger.LogError("The hero attach is not found.");
                                return "";
                            }
                            result += (heroTemplate.Name + Multipler + attach.Count + Separator);
                            break;
                        }
                    case MailConstant.ItemAttachType:
                        {
                            result += (ItemModeLocator.Instance.GetName(attach.ItemId) + Multipler + attach.Count + Separator);
                            break;
                        }
                    case MailConstant.CurrencyAttachType:
                        {
                            var currencyText = "";
                            if (attach.ItemId == RoleProperties.ROLEBASE_DIAMOND)
                            {
                                currencyText = LanguageManager.Instance.GetTextValue(MailConstant.DiamondAttachKey);
                            }
                            else if (attach.ItemId == RoleProperties.ROLEBASE_GOLD)
                            {
                                currencyText = LanguageManager.Instance.GetTextValue(MailConstant.CoinsAttachKey);
                            }
                            else if (attach.ItemId == RoleProperties.ROLEBASE_ENERGY)
                            {
                                currencyText = LanguageManager.Instance.GetTextValue(MailConstant.EnergyAttachKey);
                            }
                            result += (currencyText + Multipler + attach.Count + Separator);
                            break;
                        }
                }
            }
        }
        return result;
    }

    public void Play (bool play)
    {
        if (play)
        {
            TweenRotation.PlayForward();
        }
        else
        {
            TweenRotation.enabled = false;
            TweenRotation.transform.localRotation = Quaternion.identity;
        }
    }

    public void ShowDel(bool show)
    {
        NGUITools.SetActive(delLis.gameObject, show);
        if(show)
        {
            receiveLis.onClick = null;
            detailLis.onClick = null;
            delLis.onClick = OnDel;
        }
        else
        {
            receiveLis.onClick = OnReceive;
            detailLis.onClick = OnDetail;
            delLis.onClick = null;
        }
    }

    private void OnDisable()
    {
        ShowDel(false);
    }
}
