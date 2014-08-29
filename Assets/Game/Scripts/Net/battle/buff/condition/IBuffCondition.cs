namespace com.kx.sglm.gs.battle.share.buff.condition
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using IBattlePartInfo = com.kx.sglm.gs.battle.share.enums.IBattlePartInfo;

	public abstract class IBuffCondition : IBattlePartInfo
	{

		public abstract bool canOptionBuff(BattleFighter fighter);

	}

}