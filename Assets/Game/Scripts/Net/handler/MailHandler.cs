using KXSGCodec;

public class MailHandler 
{
    public delegate void MailUpdated(int mailId, sbyte state);
    public delegate void MailDetail(int mailId, string content);
    public static MailUpdated MailIsUpdated;
    public static MailDetail MailDetailed;

    public static void OnMailList(ThriftSCMessage msg)
    {
        var themsg = msg.GetContent() as SCMailListMsg;
        if (themsg != null)
        {
            if(themsg.ListType == MailConstant.ListTypeUpdate)
            {
                var mailListCached = MailModelLocator.Instance.ScMailListMsg.MailList;
                var mailList = themsg.MailList;
                foreach(var info in mailList)
                {
                    var mail = MailModelLocator.Instance.FindMailViaMailId(info.Uuid);
                    if(mail != null)
                    {
                        mailListCached.Remove(mail);  
                    }
                    mailListCached.Add(info);  
                }
            }
            else
            {
                MailModelLocator.Instance.ScMailListMsg = themsg;
                MailModelLocator.AlreadyRequest = true;
            }
            MailConstant.SortMailList(MailModelLocator.Instance.ScMailListMsg.MailList);
            var mailEntry = WindowManager.Instance.Show<UIEmailEntryWindow>(true);
            mailEntry.Refresh();
        }
    }

    public static void OnMailOptionResultMsg(ThriftSCMessage msg)
    {
        var themsg = msg.GetContent() as SCMailOptionResultMsg;
        if (themsg != null)
        {
            switch ((MailConstant.OptionResult)themsg.OptionResult)
            {
                case MailConstant.OptionResult.OutOfDate:
                    PopTextManager.PopTip(LanguageManager.Instance.GetTextValue(MailConstant.MailOutOfDateKey), false);
                    break;
                case MailConstant.OptionResult.Succeed:
                    PopTextManager.PopTip(LanguageManager.Instance.GetTextValue(MailConstant.MailOperSucceedKey), false);
                    break;
                case MailConstant.OptionResult.NotExit:
                    PopTextManager.PopTip(LanguageManager.Instance.GetTextValue(MailConstant.MailNotExitKey), false);
                    break;
                case MailConstant.OptionResult.NoNeedToUpdate:
                    PopTextManager.PopTip(LanguageManager.Instance.GetTextValue(MailConstant.MailNoNeedUpdateKey), false);
                    break;
            }
        }
    }

    public static void OnMailUpdateMsg(ThriftSCMessage msg)
    {
        var themsg = msg.GetContent() as SCMailUpdateMsg;
        if (themsg != null)
        {
            var mail = MailModelLocator.Instance.FindMailViaMailId(themsg.MailId);
            mail.State = themsg.State;
            if (MailIsUpdated != null)
            {
                MailIsUpdated(themsg.MailId, themsg.State);
            }
        }
    }

    public static void OnMailDetailMsg(ThriftSCMessage msg)
    {
        var themsg = msg.GetContent() as SCMailDetailMsg;
        if (themsg != null)
        {
            var mailId = themsg.Uuid;
            var mail = MailModelLocator.Instance.FindMailViaMailId(mailId);
            var content = themsg.Content;
            mail.Content = content;
            if(MailDetailed != null)
            {
                MailDetailed(mailId, content);
            }
        }
    }

    public static void OnMailDeleteMsg(ThriftSCMessage msg)
    {
        var themsg = msg.GetContent() as SCMailDeleteMsg;
        if (themsg != null)
        {
            var mailList = MailModelLocator.Instance.ScMailListMsg.MailList;
            if(mailList != null)
            {
                mailList.RemoveAll(mail => themsg.MailIds.Contains(mail.Uuid));
            }
        }
    }
}
