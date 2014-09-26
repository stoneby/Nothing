using UnityEngine;
using System.Collections;

public class BattleNormalMode : IBattleMode
{
    public void ResetCurrentConfig()
    {
        
    }

    public void SetBattleField(TeamSelectController teamController, TeamSimpleController enemyController, Character[,] characters, string mode)
    {
        
    }

    public int CheckCanAttack(TeamSelectController teamController)
    {
        return -1;
    }

    public void StopFingerMove()
    {
        
    }
}
