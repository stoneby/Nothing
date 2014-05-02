namespace com.kx.sglm.gs.battle
{

	using BattleRoundCountRecord = com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord;

	public interface IRoundCounter : IBattle
	{

		void beforeBattleStart(BattleRoundCountRecord roundRecord);

		void countDownRound(BattleRoundCountRecord roundRecord);

	}

}