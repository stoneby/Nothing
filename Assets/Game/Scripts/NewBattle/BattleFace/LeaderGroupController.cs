using System.Collections.Generic;
using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using UnityEngine;

public class LeaderGroupController : MonoBehaviour
{
    public List<LeaderControl> LeaderList;
    public List<int> LeaderIndex;

    /// <summary>
    /// Total leader CD counting.
    /// </summary>
    public int TotalLeaderCD { get; set; }

    public void Init(List<FighterInfo> fighterList)
    {
        if (fighterList == null || fighterList.Count <= BattleModelLocator.MinHerosCount)
        {
            return;
        }

        for (var i = 0; i < LeaderIndex.Count; ++i)
        {
            LeaderList[i].SetData(fighterList[LeaderIndex[i]], LeaderIndex[i]);
        }
    }

    /// <summary>
    /// Set current cd.
    /// </summary>
    /// <param name="cd">Current cd</param>
    public void SetCD(int cd)
    {
        LeaderList.ForEach(item => item.SetCD(cd));
    }

    public void Reset()
    {
        TotalLeaderCD = 0;
        LeaderList.ForEach(item => item.Reset());
    }

    public void Awake()
    {
        if (LeaderList == null || LeaderIndex == null || LeaderList.Count != LeaderIndex.Count)
        {
            Logger.LogError("Please verify that LeaderList and LeaderIndex should not be null or not equal to.");
        }
    }
}
