namespace com.kx.sglm.gs.battle.share.factory.creater
{

	using BattleField = com.kx.sglm.gs.battle.share.logic.loop.BattleField;

	public class BattleSceneCreater
	{



		public static BattleField createBattleField(Battle battle)
		{
			BattleField _battleField = new BattleField(battle);
			return _battleField;
		}


	}

}