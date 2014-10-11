using System.Collections.Generic;
using System.Linq;
using KXSGCodec;
using Template.Auto.Quest;
using Template.Auto.Reward;
using Template.Auto.Sign;
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
    private Sign signTemplates;
    private Quest questTemplates;
    private Reward rewardTemplates;

    public int RewardId;
    public int QuestId;

    //
    //private  signTemplates;

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

    public Reward RewardTemplates
    {
        get { return rewardTemplates ?? (rewardTemplates = Utils.Decode<Reward>(ResourcePath.FileReward)); }
    }

    public Sign SighTemplates
    {
        get { return signTemplates ?? (signTemplates = Utils.Decode<Sign>(ResourcePath.FileSign)); }
    }

    public SCSignLoad SignLoadMsg;

    public Quest QuestTemplates
    {
        get { return questTemplates ?? (questTemplates = Utils.Decode<Quest>(ResourcePath.FileQuest)); }
    }

    public SCQuest QuestMsg;
}