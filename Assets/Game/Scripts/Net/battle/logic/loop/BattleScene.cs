using System;

namespace com.kx.sglm.gs.battle.share.logic.loop
{

	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleEndRecord = com.kx.sglm.gs.battle.share.data.record.BattleEndRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
	using com.kx.sglm.gs.battle.share.logic;

	/// <summary>
	/// 战斗内的一个场景，以普通PVE为例，1波怪就是一个scene。 产生的动作是<seealso cref="BattleRound"/><br>
	/// 对于战斗来说，一个<seealso cref="BattleScene"/>可以完成一场完整的战斗
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleScene : AbstractBattleLooper<BattleRound, BattleArmy>
	{

	//	private BattleEventHandler eventHandler;

		public BattleScene(Battle battle, BattleArmy army) : base(battle, army)
		{
	//		eventHandler = new BattleEventHandler();
		}


		/// <summary>
		/// 在每次战斗场景开始是先注册所有需要的时间
		/// </summary>
		protected internal virtual void regiestEventHandler()
		{

		}

		public virtual void handleBattleInnerEvent(InnerBattleEvent @event)
		{
			//TODO: 改回观察者
	//		eventHandler.handleEvent(event);
		}

		public override void onStart()
		{
			BattleRoundCountRecord _record = Record.OrCreateRoundCountRecord;
			Battle.BattleExcuter.beforeBattleStart(this, _record);
	//		regiestEventHandler();
			Record.finishCurRoundCountRecord();
            Record.FinishDebugRecord();
        }

		public override bool Dead
		{
			get
			{
				return !CurAttacker.hasHp();
			}
		}


		public override void onFinish()
		{
			BattleEndRecord _record = Record.OrCreateEndRecord;
			Console.WriteLine("#BattleScene.onFinish()------BattleSceneEnd");
			_record.EndType = BattleRecordConstants.BATTLE_SCENE_END;
			Record.finishCurEndRecord();
		}

		public override bool hasNextSubAction()
		{
			return CurAttacker.hasHp();
		}

		public override BattleRound createSubActionByType()
		{
			return new BattleRound(Battle);
		}

		public override void addActorIndex()
		{
			//no actor here
		}

		public override void initOnCreateSubAction()
		{
			updateBattleState(BattleState.RUNTIME, false);
			CurAttacker.resetOnNewAction();
		}


		public override bool AllActionFinish
		{
			get
			{
				//TODO: 以后可能会有回合限制大概
				return Dead;
			}
		}

		public override void createDeadth()
		{

		}


	}

}