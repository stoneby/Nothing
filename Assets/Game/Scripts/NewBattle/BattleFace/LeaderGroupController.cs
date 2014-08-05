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
            Debug.LogError("figher list should not be null.");
            return;
        }

        for (var i = 0; i < LeaderIndex.Count; ++i)
        {
            var leaderIndex = LeaderIndex[i];
            LeaderList[i].SetData(fighterList[leaderIndex], leaderIndex);
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

    public void PlaySeal(int leaderIndex, bool show)
    {
        var index = LeaderIndex.IndexOf(leaderIndex);
        LeaderList[index].PlaySeal(show);
    }

    public void Awake()
    {
        if (LeaderList == null || LeaderIndex == null || LeaderList.Count != LeaderIndex.Count)
        {
            Logger.LogError("Please verify that LeaderList and LeaderIndex should not be null or not equal to.");
        }
    }
}
