using System;
using KXSGCodec;
using Object = UnityEngine.Object;

public class FriendModelLocator 
{
    private static volatile FriendModelLocator instance;
    private static readonly object SyncRoot = new Object();
    private DateTime allUpdateTime = new DateTime(1970, 1, 1);
    private DateTime applyUpdateTime = new DateTime(1970, 1, 1);
    private DateTime recieveUpdateTime = new DateTime(1970, 1, 1);

    #region Public Fields

    public enum FriendListType
    {
        LoadingAll,
        Apply,
        Receive
    }

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

    public float MessageOutOfDateSeconds = 120f;

    public int ExtendFriendTimes { get;  set; }

    private SCFriendLoadingAll scFriendLoadingAll;
    public SCFriendLoadingAll ScFriendLoadingAll
    {
        get
        {
            return scFriendLoadingAll;
        }
        set
        {
            scFriendLoadingAll = value;
            allUpdateTime = DateTime.Now;
        }
    }

    private SCFriendApplyList scFriendApplyList;
    public SCFriendApplyList ScFriendApplyList
    {
        get
        {
            return scFriendApplyList;
        }
        set
        {
            scFriendApplyList = value;
            applyUpdateTime = DateTime.Now;
        }
    }

    private SCFriendRecieveEnergyList scFriendRecieveEnergyList;
    public SCFriendRecieveEnergyList ScFriendRecieveEnergyList
    {
        get
        {
            return scFriendRecieveEnergyList;
        }
        set
        {
            scFriendRecieveEnergyList = value;
            recieveUpdateTime = DateTime.Now;
        }
    }

    public FriendInfo FindInfo(long uuid)
    {
        if(ScFriendLoadingAll == null || ScFriendLoadingAll.FriendList == null)
        {
            return null;
        }
        return ScFriendLoadingAll.FriendList.Find(item => item.FriendUuid == uuid);
    }

    public bool IsFriendListOutOfDate(FriendListType friendListType)
    {
        switch(friendListType)
        {
                case FriendListType.LoadingAll:
                {
                    return (DateTime.Now - allUpdateTime).TotalSeconds > MessageOutOfDateSeconds;
                }
                case FriendListType.Apply:
                {
                    return (DateTime.Now - applyUpdateTime).TotalSeconds > MessageOutOfDateSeconds;
                }
                case FriendListType.Receive:
                {
                    return (DateTime.Now - recieveUpdateTime).TotalSeconds > MessageOutOfDateSeconds;
                }
        }
        return false;
    }

    public void Clear()
    {
        ExtendFriendTimes = 0;
        allUpdateTime = new DateTime(1970, 1, 1);
        applyUpdateTime = new DateTime(1970, 1, 1);
        recieveUpdateTime = new DateTime(1970, 1, 1);
    }

    #endregion
}
