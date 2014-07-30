using System.Collections.Generic;
using UnityEngine;
using KXSGCodec;

public class FriendModelLocator 
{
    private static volatile FriendModelLocator instance;
    private static readonly object SyncRoot = new Object();

    #region Public Fields

    public static FriendModelLocator Instance
    {
        get
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                        instance = new FriendModelLocator();
                }
            }
            return instance;
        }
    }

    public int ExtendFriendTimes { get;  set; }

    private SCFriendLoadingAll friendLoadingAll;
    public SCFriendLoadingAll ScFriendLoadingAll { get; set; }

    public List<FriendInfo> ApplyListCached;

    public static bool AlreadyRequest;

    public FriendInfo FindInfo(long uuid)
    {
        if(ScFriendLoadingAll == null || ScFriendLoadingAll.FriendList == null)
        {
            return null;
        }
        return ScFriendLoadingAll.FriendList.Find(item => item.FriendUuid == uuid);
    }

    #endregion
}
