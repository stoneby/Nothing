using System;

namespace com.kx.sglm.gs.battle.share.logic.loop
{

	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleEndRecord = com.kx.sglm.gs.battle.share.data.record.BattleEndRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using com.kx.sglm.gs.battle.share.logic;

	/// <summary>
	/// 战斗场景，管理总体的战斗逻辑，更多的一个壳子的作用，产生的循环体是<seealso cref="BattleScene"/>。
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleField : AbstractBattleLooper<BattleScene, BattleArmy>
	{

		public BattleField(Battle battle) : base(battle)
		{
		}

		public virtual BattleArmy BattleArmy
		{
			get
			{
				return CurSubAction == null ? null : CurSubAction.CurAttacker;
			}
		}



		public override void onFinish()
		{
			//当一个BattleField完成的时候战斗就完成了
			BattleEndRecord _record = Record.OrCreateEndRecord;
			_record.EndType = BattleRecordConstants.BATTLE_ALL_END;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int _winner = getBattleArmy().getDeadBattleSide().getDefaultOpponent().getIndex();
			int _winner = BattleArmy.DeadBattleSide.DefaultOpponent.Index;
			_record.addProp(BattleRecordConstants.BATTLE_END_WIN_SIDE, _winner);
			updateBattleState(BattleState.STOP, true);
			Console.WriteLine("#BattleField.onFinish()------BattleFieldEnd");
			Battle.finish();
			Record.finishCurEndRecord();

		}

		public override bool hasNextSubAction()
		{
			return Excuter.hasNextBattleScene();
		}

		public override BattleScene createSubActionByType()
		{
			return Excuter.createNextBattleScene();
		}

		public override void addActorIndex()
		{

		}

		public override void initOnCreateSubAction()
		{
			updateBattleState(BattleState.RUNTIME, false);
		}


		public override bool AllActionFinish
		{
			get
			{
				return !Excuter.hasNextBattleScene();
			}
		}

		public override void createDeadth()
		{

		}

		public override bool Dead
		{
			get
			{
				return Excuter.BattleDead;
			}
		}

		public virtual IBattleExecuter Excuter
		{
			get
			{
				return Battle.BattleExcuter;
			}
		}

		public override void onStart()
		{
			// TODO Auto-generated method stub

		}


	}

}