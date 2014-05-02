namespace com.kx.sglm.gs.battle.actor
{

	using BattleRoundCountRecord = com.kx.sglm.gs.battle.data.record.BattleRoundCountRecord;

	/// <summary>
	/// 战斗参与者
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