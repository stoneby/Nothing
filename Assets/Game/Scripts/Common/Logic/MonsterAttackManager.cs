using System.Collections.Generic;

/// <summary>
/// Manager monster list attack value and hit count.
/// </summary>
public class MonsterAttackManager
{
    private readonly Dictionary<MonsterControl, List<float>> monsterAttackDict = new Dictionary<MonsterControl, List<float>>();

    public void AddMonster(MonsterControl monster, float hurt)
    {
        if (!monsterAttackDict.ContainsKey(monster))
        {
            monsterAttackDict.Add(monster, new List<float>());
        }

        monsterAttackDict[monster].Add(hurt);
    }

    public int GetHitCounter(MonsterControl monster)
    {
        return monsterAttackDict[monster].Count;
    }

    public float GetTotalHurt(MonsterControl monster)
    {
        var result = 0f;
        monsterAttackDict[monster].ForEach(value => result += value);
        return result;
    }

    public void Cleanup()
    {
        monsterAttackDict.Clear();
    }
}
