namespace com.kx.sglm.gs.battle.share.actor
{

	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;

	/// <summary>
	/// 战斗参与�?
	/// @author liyuan2
	/// 
	/// </summary>
	public interface IBattleActor : IBattle
	{

		void resetOnNewAction();

		bool Alive {get;}

		void onDead();

		void onRoundFinish(BattleRoundCountRecord roundRecord);

	}

}