namespace com.kx.sglm.gs.battle.share.actor
{

	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;

	public interface IBattleSideActor : IBattleActor
	{

		void tryDead();

		BattleSideEnum BattleSide {get;}

	}

}