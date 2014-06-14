namespace com.kx.sglm.gs.battle.share.actor
{

	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using IBattleInnerEventHandler = com.kx.sglm.gs.battle.share.@event.IBattleInnerEventHandler;

	/// <summary>
	/// ζζεδΈθ?
	/// @author liyuan2
	/// 
	/// </summary>
	public interface IBattleActor : IBattle, IBattleInnerEventHandler
	{

		void resetOnNewAction();

		bool hasHp();

		void onDead();

		void onRoundFinish(BattleRoundCountRecord roundRecord);

	}

}