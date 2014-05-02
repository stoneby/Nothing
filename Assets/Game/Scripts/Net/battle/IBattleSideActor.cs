namespace com.kx.sglm.gs.battle.actor
{

	using BattleSideEnum = com.kx.sglm.gs.battle.enums.BattleSideEnum;

	public interface IBattleSideActor : IBattleActor
	{

		void tryDead();

		BattleSideEnum BattleSide {get;}

	}

}