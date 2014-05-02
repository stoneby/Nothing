namespace com.kx.sglm.gs.battle.factory
{

	using BattleSource = com.kx.sglm.gs.battle.data.BattleSource;

	public interface IBattleFactory
	{

		Battle createBattle(BattleSource source);
	}

}