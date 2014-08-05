using KXSGCodec;
using System;
using System.Collections.Generic;
using Template;
using Template.Auto.Raid;
using Object = UnityEngine.Object;

public sealed class MissionModelLocator
{
    public SCRaidLoadingAll RaidLoadingAll;
    public SCRaidAddtion RaidAddition;
    public string DestTime;
    public int NextRaidType = 1;
    public int CurrRaidType = 0;

    public RaidInfo Raid;

    public SCRaidQueryFriend FriendsMsg;

    public FriendVO FriendData;
    public int SelectedStageId;

    public int MissionStep;

    public SCRaidReward BattleReward;

    public int OldExp;
    public int OldLevel;

    public bool ShowAddFriendAlert = false;

    private const int FirstTime = 8;
    private const int SecondTime = 16;
    private const int ThirdTime = 24;

    private static volatile MissionModelLocator instance;
    private static readonly object SyncRoot = new Object();

    private Raid raidTemplates;
    private List<RaidInfo> raidInfoElite;
    private List<RaidInfo> raidInfoMaster;

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

    public Raid RaidTemplates
    {
        get { return raidTemplates ?? (raidTemplates = Utils.Decode<Raid>(RaidTemlatePath)); }
    }

    public RaidTemplate GetRaidByTemplateId(int templateid)
    {
        if (RaidTemplates != null && RaidTemplates.RaidTmpls != null && RaidTemplates.RaidTmpls.ContainsKey(templateid))
        {
            return RaidTemplates.RaidTmpls[templateid];
        }
        return null;
    }

    public RaidStageTemplate GetRaidStagrByTemplateId(int templateid)
    {
        if (RaidTemplates != null && RaidTemplates.RaidStageTmpls != null && RaidTemplates.RaidStageTmpls.ContainsKey(templateid))
        {
            return RaidTemplates.RaidStageTmpls[templateid];
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

    public void AddFinishTime(int templateid)
    {
        if (RaidLoadingAll.TodayFinishTimes == null)
        {
            RaidLoadingAll.TodayFinishTimes = new Dictionary<int, sbyte>();
        }
        if (RaidLoadingAll != null && RaidLoadingAll.TodayFinishTimes != null)
        {
            if (RaidLoadingAll.TodayFinishTimes.ContainsKey(templateid))
            {
                RaidLoadingAll.TodayFinishTimes[templateid]++;
            }
            else
            {
                RaidLoadingAll.TodayFinishTimes.Add(templateid, 1);
            }
        }
    }

    public RaidAddtionInfo GetAdditionInfoByRaidTemplateID(int templateid)
    {
        if (RaidAddition == null || RaidAddition.AddtionInfo == null) return null;
        var date = DateTime.Now;
        
		int desttime;
		int flag;
		if (date.Hour < FirstTime)
		{
			desttime = FirstTime;
			flag = 0;
		}
		else if (date.Hour < SecondTime)
		{
			desttime = SecondTime;
			flag = 1;
		}
		else
		{
			desttime = ThirdTime;
			flag = 2;
		}

		int v = desttime - date.Hour;
		if (v > 1)
		{
			DestTime = "剩余" + v + "小时";
		}
		else
		{
			v = 60 - date.Minute;
			if (v > 1)
			{
                DestTime = "剩余" + v + "分钟";
			}
			else
			{
                DestTime = "剩余1分钟";
			}
		}

		if (CurrRaidType == RaidAddition.AddtionInfo[flag].RaidType)
        {
			for (int j = 0; j < RaidAddition.AddtionInfo[flag].RaidTemplateId.Count; j++)
            {
				if (templateid == RaidAddition.AddtionInfo[flag].RaidTemplateId[j])
                {
					return RaidAddition.AddtionInfo[flag];
				}
			}
         }

        return null;
    }

    public List<RaidInfo> GetRaidsByType(int thetype)
    {
        var raidtemplates = RaidTemplates.RaidTmpls;
        var newraids = new List<RaidInfo>();
        var raids = (thetype == RaidType.RaidElite) ? RaidLoadingAll.RaidInfoElite : RaidLoadingAll.RaidInfoMaster;
        foreach (KeyValuePair<int, RaidTemplate> item in raidtemplates)
        {
            if (item.Value.RaidType == thetype && PlayerModelLocator.Instance.Level >= item.Value.OpenLvl)
            {
                RaidInfo raid = null;
                for (int i = 0; i < raids.Count; i++)
                {
                    if (raids[i].TemplateId == item.Value.Id)
                    {
                        raid = raids[i];
                        break;
                    }
                }
                if (raid == null)
                {
                    raid = new RaidInfo();
                    raid.TemplateId = item.Value.Id;
                    raid.StateInfo = new List<RaidStageInfo>();
                }

                foreach (KeyValuePair<int, RaidStageTemplate> stageitem in RaidTemplates.RaidStageTmpls)
                {
                    if (stageitem.Value.RaidId == raid.TemplateId)
                    {
                        var flag = true;
                        for (int i = 0; i < raid.StateInfo.Count; i++)
                        {
                            if (raid.StateInfo[i].TemplateId == stageitem.Value.Id)
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (flag)
                        {
                            var stage = new RaidStageInfo();
                            stage.Star = 0;
                            stage.TemplateId = stageitem.Value.Id;
                            raid.StateInfo.Add(stage);
                        }
                    }
                }
                
                newraids.Add(raid);
            }
        }

        return newraids;
    }

    public List<RaidInfo> GetCurrentRaids()
    {
        switch (CurrRaidType)
        {
            case RaidType.RaidNormal:
                return RaidLoadingAll.RaidInfoNormal;
            case RaidType.RaidElite:
                return GetRaidsByType(RaidType.RaidElite);
            case RaidType.RaidHero:
                return GetRaidsByType(RaidType.RaidHero);
            default:
                return RaidLoadingAll.RaidInfoNormal;
        }
    }

    public void SetCurrentRaids(List<RaidInfo> raids)
    {
        switch (CurrRaidType)
        {
            case RaidType.RaidNormal:
                RaidLoadingAll.RaidInfoNormal = raids;
                break;
            case RaidType.RaidElite:
                RaidLoadingAll.RaidInfoElite = raids;
                break;
            case RaidType.RaidHero:
                RaidLoadingAll.RaidInfoMaster = raids;
                break;
            default:
                RaidLoadingAll.RaidInfoNormal = raids;
                break;
        }
    }

    public Dictionary<int, int> TotalStageCount;
    public Dictionary<int, int> TotalStarCount; 

    public void ComputeStagecount()
    {
        if (TotalStageCount != null)
        {
            return;
        }

        var raidtemplates = Instance.RaidTemplates.RaidStageTmpls;
        TotalStageCount = new Dictionary<int, int>();
        TotalStarCount = new Dictionary<int, int>();

        foreach (KeyValuePair<int, RaidStageTemplate> item in raidtemplates)
        {
            if (TotalStageCount.ContainsKey(item.Value.RaidId))
            {
                TotalStageCount[item.Value.RaidId] += 3;
            }
            else
            {
                TotalStageCount.Add(item.Value.RaidId, 3);
                TotalStarCount.Add(item.Value.RaidId, 0);
            }
        }

        ComputeStarCount(RaidLoadingAll.RaidInfoNormal);
        ComputeStarCount(RaidLoadingAll.RaidInfoElite);
        ComputeStarCount(RaidLoadingAll.RaidInfoMaster);
    }

    public void ComputeStarCount(List<RaidInfo> raids)
    {
        for (int i = 0; i < raids.Count; i++)
        {
            if (TotalStarCount.ContainsKey(raids[i].TemplateId))
            {
                for (int j = 0; j < raids[i].StateInfo.Count; j++)
                {
                    TotalStarCount[raids[i].TemplateId] +=
                        raids[i].StateInfo[j].Star;
                }
            }
        }
    }

    public int GetRaidStarCount(int raidtemplateid)
    {
        if (TotalStarCount == null)
        {
            return 0;
        }
        return TotalStarCount.ContainsKey(raidtemplateid) ? TotalStarCount[raidtemplateid] : 0;
    }

    public string GetStageCountByRaidId(int raidtemplateid)
    {
        if (TotalStageCount == null)
        {
            return "0/0";
        }
        if (TotalStageCount.ContainsKey(raidtemplateid))
        {
            return TotalStarCount[raidtemplateid] + "/" + TotalStageCount[raidtemplateid];
        }
        return "0/0";
    }

    public bool HasRaidReward(int raidtemplateid)
    {
        if (TotalStageCount == null)
        {
            return false;
        }
        if (TotalStageCount.ContainsKey(raidtemplateid))
        {
            return TotalStarCount[raidtemplateid] == TotalStageCount[raidtemplateid];
        }
        return false;
    }

    public void AddNewStage(SCRaidNewStage stage)
    {
        var stageinfo = new RaidStageInfo();
        stageinfo.Star = 0;
        stageinfo.TemplateId = stage.StageTemplateId;

        bool flag = true;
        var raids = GetCurrentRaids();
        for (int i = 0; i < raids.Count; i++)
        {
            if (raids[i].TemplateId == stage.RaidTemplateId)
            {
                raids[i].StateInfo.Add(stageinfo);
                flag = false;
            }
        }
        if (flag)
        {
            MissionStep = RaidType.StepRaidList;
            var raid = new RaidInfo();
            raid.TemplateId = stage.RaidTemplateId;
            raid.StateInfo = new List<RaidStageInfo>();
            raid.StateInfo.Add(stageinfo);
            raids.Add(raid);
        }
        SetCurrentRaids(raids);
    }

    public void AddStar(int count)
    {
        Logger.Log("-------------------------------" + Raid.TemplateId);
        if (TotalStarCount.ContainsKey(Raid.TemplateId))
        {
            var raids = GetCurrentRaids();
            for (int i = 0; i < raids.Count; i++)
            {
                if (raids[i].TemplateId == Raid.TemplateId)
                {
                    for (int j = 0; j < raids[i].StateInfo.Count; j++)
                    {
                        if (raids[i].StateInfo[j].TemplateId == SelectedStageId)
                        {
                            var v = (sbyte) count - raids[i].StateInfo[j].Star;
                            if (v > 0)
                            {
                                TotalStarCount[Raid.TemplateId] += v;
                                raids[i].StateInfo[j].Star = (sbyte)count;
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            SetCurrentRaids(raids);
        }
    }
}