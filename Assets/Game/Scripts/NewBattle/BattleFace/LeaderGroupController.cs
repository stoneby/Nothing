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
    public int TotalLeaderCD
    {
        get { return totalLeaderCD; }
        set
        {
            totalLeaderCD = value;
            UpdateCD(totalLeaderCD);
        }
    }

    private int totalLeaderCD;

    public void Init(List<FighterInfo> fighterList, List<Character> characterList)
    {
        if (fighterList == null)
        {
            Debug.LogError("figher list should not be null.");
            return;
        }

        if (fighterList.Count < BattleModelLocator.MinHerosCount)
        {
            Debug.LogError("Fighter list count: " + fighterList.Count + ", should not less than min heros counts: " + BattleModelLocator.MinHerosCount);
            return;
        }

        characterList.ForEach(character => character.IsLeader = false);

        for (var i = 0; i < LeaderIndex.Count; ++i)
        {
            var leaderIndex = LeaderIndex[i];
            LeaderList[i].SetData(fighterList[leaderIndex], characterList[leaderIndex], leaderIndex);
            LeaderList[i].Initialize();
            characterList[leaderIndex].IsLeader = true;
        }
    }

    public void Reset()
    {
        TotalLeaderCD = 0;
        LeaderList.ForEach(item => item.Reset());
        SetAllLeadersCollider(true);
    }

    /// <summary>
    /// Set all leaders' collider enable or disable.
    /// </summary>
    /// <param name="isEnable"></param>
    public void SetAllLeadersCollider(bool isEnable)
    {
        LeaderList.ForEach(item => item.SetLeaderCollider(isEnable));
    }

    public void PlaySeal(int leaderIndex, bool show)
    {
        var index = LeaderIndex.IndexOf(leaderIndex);
        LeaderList[index].PlaySeal(show);
    }

    /// <summary>
    /// Set current cd.
    /// </summary>
    /// <param name="cd">Current cd</param>
    private void UpdateCD(int cd)
    {
        LeaderList.ForEach(item => item.SetCD(cd));
    }

    private void OnActiveLeaderSkill(LeaderData data)
    {
        totalLeaderCD -= data.BaseCd;
        UpdateCD(totalLeaderCD);
    }

    private void Awake()
    {
        if (LeaderList == null || LeaderIndex == null || LeaderList.Count != LeaderIndex.Count)
        {
            Logger.LogError("Please verify that LeaderList and LeaderIndex should not be null or not equal to.");
        }

        LeaderList.ForEach(leader => leader.OnActiveLeaderSkill += OnActiveLeaderSkill);
    }

    private void OnDestroy()
    {
        LeaderList.ForEach(leader => leader.OnActiveLeaderSkill -= OnActiveLeaderSkill);
    }
}
