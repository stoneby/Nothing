using System;
using System.Collections;
using System.Collections.Generic;
using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class UIEmailEntryWindow : Window
{
    public CloseButtonControl CloseButtonControl;
    public Transform MailItem;
    private UIGrid mails;
    private UIToggle sysToggle;
    private UIToggle priToggle;
    private UILabel mailCount;
    private readonly List<MailItem> mailList = new List<MailItem>();
    private List<MailMsgInfo> mailMsgInfos;
    private UIEventListener menuLis;
    private bool play;

    protected UIGrid Mails
    {
        get { return mails ?? transform.Find("Scroll View/Grid").GetComponent<UIGrid>(); }
    }

    /// <summary>
    /// Fill in the item game objects.
    /// </summary> 
    protected virtual void UpdateItemList(int itemCount)
    {
        var childCount = Mails.transform.childCount;
        if (childCount != itemCount)
        {
            var isAdd = childCount < itemCount;
            HeroUtils.AddOrDelItems(Mails.transform, MailItem, isAdd, Mathf.Abs(itemCount - childCount),
                                    "MailRelated",
                                    null);
            Mails.repositionNow = true;
        }
    }

    public void Refresh()
    {
        mailList.Clear();
        UpdateItemList(mailMsgInfos.Count);
        for (int i = 0; i < mailMsgInfos.Count; i++)
        {
            var mailItem = Mails.transform.GetChild(i).GetComponent<MailItem>();
            mailList.Add(mailItem);
            mailItem.Init(mailMsgInfos[i]);
        }
        mailCount.text = string.Format("{0}/{1}", mailMsgInfos.Count,
                                       ItemModeLocator.Instance.ServerConfigMsg.MailShowMax);
    }

    #region Window

    public override void OnEnter()
    {
        MtaManager.TrackBeginPage(MtaType.EmailEntryWindow);
        play = true;
        InstallHandlers();
        mailMsgInfos = MailModelLocator.Instance.ScMailListMsg.MailList;
        if(mailMsgInfos != null)
        {
            Refresh();
        }
        StartCoroutine("UpdateDeadlines");
    }

    public override void OnExit()
    {
        MtaManager.TrackEndPage(MtaType.EmailEntryWindow);
        UnInstallHandlers();
        StopCoroutine("UpdateDeadlines");
        WindowManager.Instance.Show<UIMailDetailWindow>(false);
        ShowDels(false);
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        sysToggle = transform.Find("Toggles/Toggle-Sys").GetComponent<UIToggle>();
        priToggle = transform.Find("Toggles/Toggle-Pri").GetComponent<UIToggle>();
        menuLis = UIEventListener.Get(transform.Find("Menu").gameObject);
        mailCount = transform.Find("MailCount").GetComponent<UILabel>();
    }

    private void InstallHandlers()
    {
        CloseButtonControl.OnCloseWindow = OnClose;
        EventDelegate.Add(sysToggle.onChange, TabChanged);
        EventDelegate.Add(priToggle.onChange, TabChanged);
        menuLis.onClick += OnMenu;
        MailHandler.MailIsUpdated += OnMailUpdate;
        MailHandler.MailDetailed += OnMailDetailed;
    }

    private void UnInstallHandlers()
    {
        CloseButtonControl.OnCloseWindow = null;
        EventDelegate.Remove(sysToggle.onChange, TabChanged);
        EventDelegate.Remove(priToggle.onChange, TabChanged);
        menuLis.onClick -= OnMenu;
        MailHandler.MailIsUpdated -= OnMailUpdate;
        MailHandler.MailDetailed -= OnMailDetailed;
    }

    private void OnMailDetailed(int mailid, string content)
    {
        var mailItem = mailList.Find(mail => mail.MailMsgInfo.Uuid == mailid);
        mailItem.MailMsgInfo.Content = content;
        var mailDetail = WindowManager.Instance.Show<UIMailDetailWindow>(true);
        mailDetail.Init(mailid, content);
    }

    private void OnMailUpdate(int mailid, sbyte state)
    {
        var mailItem = mailList.Find(mail => mail.MailMsgInfo.Uuid == mailid);
        mailItem.MailState = (MailConstant.MailState)state;
    }

    private void OnMenu(GameObject go)
    {
        ShowDels(play);
        play = !play;
    }

    private void ShowDels(bool show)
    {
        foreach(var mailItem in mailList)
        {
            mailItem.Play(show);
            mailItem.ShowDel(show);
        }
    }

    private void TabChanged()
    {
        var curToggle = UIToggle.current;
        if(curToggle == sysToggle)
        {
        
        }
        else if (curToggle == priToggle)
        {
        
        }
    }

    public void OnClose()
    {
        WindowManager.Instance.Show<UIMainScreenWindow>(true);
    }

    /// <summary>
    /// Update the expire time of each buy back items per second.
    /// </summary>
    private IEnumerator UpdateDeadlines()
    {
        var oneSecond = new TimeSpan(0, 0, 1);
        while (mailList.Count > 0)
        {
            yield return new WaitForSeconds(1);
            for (var i = 0; i < mailList.Count; i++)
            {
                if (mailList[i].TimeRemain.TotalSeconds > 1)
                {
                    mailList[i].TimeRemain = mailList[i].TimeRemain.Subtract(oneSecond);
                }
            }
        }
    }

    #endregion
}
