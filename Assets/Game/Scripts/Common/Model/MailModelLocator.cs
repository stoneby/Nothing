using System.Collections.Generic;
using KXSGCodec;
using Object = UnityEngine.Object;

public class MailModelLocator  
{
    #region Private Field

    private static volatile MailModelLocator instance;
    private static readonly object SyncRoot = new Object();

    #endregion

    public static MailModelLocator Instance
    {
        get
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                    {
                        instance = new MailModelLocator();
                    }
                }
            }
            return instance;
        }
    }

    public SCMailListMsg ScMailListMsg { get; set; }

    public static bool AlreadyRequest;

    public MailMsgInfo FindMailViaMailId(int mailId)
    {
        if(ScMailListMsg == null || ScMailListMsg.MailList == null)
        {
            return null;
        }
        return ScMailListMsg.MailList.Find(mail => mail.Uuid == mailId);
    }
}