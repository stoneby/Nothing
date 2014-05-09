namespace com.kx.sglm.gs.battle.share.factory
{

	using BattleSource = com.kx.sglm.gs.battle.share.data.BattleSource;

	public interface IBattleFactory
	{

		Battle createBattle(BattleSource source);
	}

}