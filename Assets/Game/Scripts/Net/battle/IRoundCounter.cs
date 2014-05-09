namespace com.kx.sglm.gs.battle.share
{

	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;

	public interface IRoundCounter : IBattle
	{

		void beforeBattleStart(BattleRoundCountRecord roundRecord);

		void countDownRound(BattleRoundCountRecord roundRecord);

	}

}