namespace com.kx.sglm.gs.battle.share.actor
{

	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using IBattleInnerEventHandler = com.kx.sglm.gs.battle.share.@event.IBattleInnerEventHandler;
	using SceneStartEvent = com.kx.sglm.gs.battle.share.@event.impl.SceneStartEvent;

	/// <summary>
	/// 战斗参与者
	/// @author liyuan2
	/// 
	/// </summary>
	public interface IBattleActor : IBattle, IBattleInnerEventHandler
	{

		void resetOnNewAction();

		bool hasHp();

		void onDead();

		void onSceneStart(SceneStartEvent @event);

		void onSceneStop();

		void onRoundFinish(BattleRoundCountRecord roundRecord);

	}

}