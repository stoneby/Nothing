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
	/// ս���ڵ�һ������������ͨPVEΪ����1���־���һ��scene�� �����Ķ�����<seealso cref="BattleRound"/><br>
	/// ����ս����˵��һ��<seealso cref="BattleScene"/>�������һ��������ս��
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
		/// ��ÿ��ս��������ʼ����ע��������Ҫ��ʱ��
		/// </summary>
		protected internal virtual void regiestEventHandler()
		{

		}

		public virtual void handleBattleInnerEvent(InnerBattleEvent @event)
		{
			//TODO: �Ļع۲���
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
				//TODO: �Ժ���ܻ��лغ����ƴ��
				return Dead;
			}
		}

		public override void createDeadth()
		{

		}


	}

}