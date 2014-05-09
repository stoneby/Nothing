namespace com.kx.sglm.gs.battle.share.singleton
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;

	public interface ISingletonBattleAction
	{

		void onAction(BattleFighter attacker, BattleTeam defencerTeam, BattleFightRecord record);

	}

}