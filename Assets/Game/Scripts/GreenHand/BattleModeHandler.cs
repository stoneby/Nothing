using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.enums;

public interface IBattleMode
{
    void ResetCurrentConfig();

    void SetBattleField(TeamSelectController teamController, TeamSimpleController enemyController, Character[,] characters, string mode);

    int CheckCanAttack(TeamSelectController teamController);

    void StopFingerMove();
}

public class BattleModeHandler
{
    public IBattleMode BattleMode = null;

    public BattleModeHandler()
    {
        if (BattleModelLocator.Instance.BattleType == BattleType.GREENHANDPVE.Index)
        {
            BattleMode = GreenHandGuideHandler.Instance;
        }
        else
        {
            BattleMode = new BattleNormalMode();
        }
    }

    public void ResetCurrentConfig()
    {
        BattleMode.ResetCurrentConfig();
    }

    public void SetBattleField(TeamSelectController teamController, TeamSimpleController enemyController, Character[,] characters, string mode)
    {
        BattleMode.SetBattleField(teamController, enemyController, characters, mode);
    }
    public int CheckCanAttack(TeamSelectController teamController)
    {
        return BattleMode.CheckCanAttack(teamController);
    }

    public void StopFingerMove()
    {
        BattleMode.StopFingerMove();
    }
}
