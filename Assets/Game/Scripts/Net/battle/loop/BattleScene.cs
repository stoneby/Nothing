namespace com.kx.sglm.gs.battle.logic.loop
{

	using BattleArmy = com.kx.sglm.gs.battle.actor.impl.BattleArmy;
	using BattleEndRecord = com.kx.sglm.gs.battle.data.record.BattleEndRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.data.record.BattleRecordConstants;
	using BattleState = com.kx.sglm.gs.battle.enums.BattleState;
	using com.kx.sglm.gs.battle.logic;

	/// <summary>
	/// 战斗内的一个场景，以普通PVE为例，1波怪就是一个scene。 产生的动作是<seealso cref="BattleRound"/>
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleScene : AbstractBattleLooper<BattleRound, BattleArmy>
	{

		public BattleScene(Battle battle, BattleArmy army) : base(battle, army)
		{
		}

		public override bool Dead
		{
			get
			{
				return !CurAttacker.Alive;
			}
		}


		public override void onFinish()
		{
			BattleEndRecord _record = Record.OrCreateEndRecord;
			_record.EndType = BattleRecordConstants.BATTLE_SCENE_END;
			Record.finishCurEndRecord();
		}

		public override bool hasNextSubAction()
		{
			return CurAttacker.Alive;
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

		public override void onStart()
		{
			// TODO Auto-generated method stub

		}

	}

}