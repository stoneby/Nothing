using com.kx.sglm.gs.battle.share.data;
using System.Collections.Generic;

public class LevelManager
{
    /// <summary>
    /// Current level index.
    /// </summary>
    public int CurrentLevel { get; set; }

    /// <summary>
    /// Current level monster base index.
    /// </summary>
    public int CurrentLevelMonsterBaseIndex { get; set; }

    public List<FighterInfo> CurrentMonsterList
    {
        get
        {
            return currentMonsterList;
        }
    }

    private readonly List<FighterInfo> currentMonsterList = new List<FighterInfo>();

    /// <summary>
    /// All monsters to the whole level.
    /// </summary>
    public List<FighterInfo> MonsterList { get; set; }

    /// <summary>
    /// Group of monsters.
    /// </summary>
    public List<int> MonsterGroupList { get; set; }

    /// <summary>
    /// Total level includes.
    /// </summary>
    public int TotalLevel { get { return MonsterGroupList.Count; } }

    public bool IsLastLevel()
    {
        Logger.LogWarning("This is the very last level, total level is: " + TotalLevel + ", current level is: " + CurrentLevel);
        return CurrentLevel == TotalLevel - 1;
    }

    public bool HasNextLevel()
    {
        return CurrentLevel < TotalLevel - 1;
    }

    public void Validate()
    {
        var monsterCount = MonsterList.Count;
        var monsterCountToCheck = 0;
        MonsterGroupList.ForEach(group =>
        {
            monsterCountToCheck += group;
        });
        if (monsterCount != monsterCountToCheck)
        {
            Logger.LogError("MonsterList's count: " + monsterCount + " should be the same as MonsterGroupList counted count: " + monsterCountToCheck);
        }
    }

    public void InitLevel()
    {
        if (CurrentLevel < 0 || CurrentLevel >= MonsterGroupList.Count)
        {
            CurrentLevel = 0;
        }

        if (CurrentLevelMonsterBaseIndex < 0 || CurrentLevelMonsterBaseIndex >= MonsterList.Count)
        {
            CurrentLevelMonsterBaseIndex = 0;
        }

        GenerateCurrentLevelMonsterList();
    }

    public void GoNextLevel()
    {
        if (!HasNextLevel())
        {
            Logger.LogWarning("We do not have any levels any more, total level is: " + TotalLevel);
            return;
        }

        CurrentLevelMonsterBaseIndex += MonsterGroupList[CurrentLevel];
        ++CurrentLevel;

        GenerateCurrentLevelMonsterList();
    }

    public void StorePersistent(Dictionary<string, string> persistenceInfo)
    {
        //TopData
        persistenceInfo.Add("TopData", CurrentLevel.ToString());
        //enemy list
        persistenceInfo.Add("EnemyModelIndex", (CurrentLevelMonsterBaseIndex).ToString());
    }

    public void RestorePersistent(Dictionary<string, string> persistenceInfo)
    {
        //Sync TopData
        CurrentLevel = int.Parse(persistenceInfo["TopData"]);
        //Sync Monster Index
        CurrentLevelMonsterBaseIndex = int.Parse(persistenceInfo["EnemyModelIndex"]);
    }

    private void GenerateCurrentLevelMonsterList()
    {
        currentMonsterList.Clear();
        for (var i = CurrentLevelMonsterBaseIndex; i < CurrentLevelMonsterBaseIndex + MonsterGroupList[CurrentLevel]; ++i)
        {
            currentMonsterList.Add(MonsterList[i]);
        }
    }
}
