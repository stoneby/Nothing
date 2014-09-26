using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class SystemModelLocator
{
    private static volatile SystemModelLocator instance;
    private static readonly object SyncRoot = new Object();

    public SCGameNoticeListMsg NoticeListMsg;
    public SCGameNoticeDetailMsg NoticeDetailMsg;

    public NoticeItemControl NoticeItem;

    //public bool NoNeedNotice = false;

    private SystemModelLocator()
    {
    }

    public static SystemModelLocator Instance
    {
        get
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                        instance = new SystemModelLocator();
                }
            }
            return instance;
        }
    }

    
}