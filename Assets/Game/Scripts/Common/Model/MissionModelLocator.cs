﻿using System.Collections.Generic;
using KXSGCodec;
using Template;
using UnityEngine;
using System.Collections;

public sealed class MissionModelLocator
{
    private static volatile MissionModelLocator instance;
    private static readonly object SyncRoot = new Object();

    private const string RaidTemlatePath = "Templates/Raid";
    private MissionModelLocator() { }
    public static MissionModelLocator Instance
    {
        get
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                        instance = new MissionModelLocator();
                }
            }
            return instance;
        }
    }

    private Raid raidTemplates;
    public Raid RaidTemplates
    {
        get { return raidTemplates ?? (raidTemplates = Utils.Decode<Raid>(RaidTemlatePath)); }
    }

    public RaidTemplate GetRaidByTemplateId(int templateid)
    {
        if (RaidTemplates != null && RaidTemplates.RaidTmpl != null && RaidTemplates.RaidTmpl.ContainsKey(templateid))
        {
            return RaidTemplates.RaidTmpl[templateid];
        }
        return null;
    }

    public RaidStageTemplate GetRaidStagrByTemplateId(int templateid)
    {
        if (RaidTemplates != null && RaidTemplates.RaidStageTmpl != null && RaidTemplates.RaidStageTmpl.ContainsKey(templateid))
        {
            return RaidTemplates.RaidStageTmpl[templateid];
        }
        return null;
    }

    public int GetStageFinishTimeByTemplateId(int templateid)
    {
        if (RaidLoadingAll != null && RaidLoadingAll.TodayFinishTimes != null && RaidLoadingAll.TodayFinishTimes.ContainsKey(templateid))
        {
            return RaidLoadingAll.TodayFinishTimes[templateid];
        }
        return 0;
    }

    public SCRaidLoadingAll RaidLoadingAll;
    public SCRaidAddtion RaidAddition;

    public RaidAddtionInfo GetAdditionInfoByRaidTemplateID(int raidtype, int templateid)
    {
        if (RaidAddition == null || RaidAddition.AddtionInfo == null) return null;

        for (int i = 0; i < RaidAddition.AddtionInfo.Count; i++)
        {
            if (raidtype == RaidAddition.AddtionInfo[i].RaidType)
            {
                for (int j = 0; j < RaidAddition.AddtionInfo[i].RaidTemplateId.Count; j++)
                {
                    if (templateid == RaidAddition.AddtionInfo[i].RaidTemplateId[j])
                    {
                        return RaidAddition.AddtionInfo[i];
                    }
                }
            }
        }
        return null;
    }

    public Dictionary<int, int> TotalStageCount; 

    public void ComputeStagecount()
    {
        if (TotalStageCount != null) return;
        var raidtemplates = MissionModelLocator.Instance.RaidTemplates.RaidStageTmpl;
        TotalStageCount = new Dictionary<int, int>();
        foreach (KeyValuePair<int, RaidStageTemplate> item in raidtemplates)
        {
            if (TotalStageCount.ContainsKey(item.Value.RaidId))
            {
                TotalStageCount[item.Value.RaidId]++;
            }
            else
            {
                TotalStageCount.Add(item.Value.RaidId, 1);
            }
        }
        
    }

    public int GetStageCountByRaidId(int raidtemplateid)
    {
        if (TotalStageCount == null)
        {
            return 0;
        }
        else if (TotalStageCount.ContainsKey(raidtemplateid))
        {
            return TotalStageCount[raidtemplateid];
        }
        else
        {
            return 0;
        }
    }
    
//    public int 
    public bool IsFriend;
    public FriendInfo FriendData;
    public int SelectedStageId;
}