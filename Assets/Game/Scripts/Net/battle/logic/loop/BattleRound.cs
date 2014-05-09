namespace com.kx.sglm.gs.battle.share.logic.loop
{

	using BattleArmy = com.kx.sglm.gs.battle.share.actor.impl.BattleArmy;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;
	using IBattleExecuter = com.kx.sglm.gs.battle.share.executer.IBattleExecuter;
	using com.kx.sglm.gs.battle.share.logic;

	/// <summary>
	/// 处理一个回合，即战斗内所有BattleTeam的一次完整出手�?这里会产生出手挂起�?挂起由{@link IBattleExecuter}触发�?
	/// 出手结束后，若未死亡，则直接进入下一个回合，如果产生死亡，通知<seealso cref="BattleScene"/>是否需要切�?
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleRound : AbstractBattleLooper<BattleTeamShot, BattleArmy>
	{

		public BattleRound(Battle battle) : base(battle, battle.BattleArmy, battle.BattleArmy)
		{
			//这一层的attacker比较特殊是完整的BattleArmy
		}

		public override void onFinish()
		{
			//TODO 是否有每次回合结束的结算，以后可能会有相关逻辑
			BattleRoundCountRecord _roundRecord = Battle.Record.OrCreateRoundCountRecord;
			if (!Dead)
			{
				CurAttacker.onRoundFinish(_roundRecord);
			}
			Battle.Record.finishCurRoundCountRecord();
		}

		public override bool hasNextSubAction()
		{
			return CurAttacker.hasCurActor();
		}

		public override BattleTeamShot createSubActionByType()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.actor.impl.BattleTeam _attackerTeam = getCurAttacker().getCurActor();
			BattleTeam _attackerTeam = CurAttacker.CurActor;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.actor.impl.BattleTeam _defencerTeam = getCurAttacker().getOppositeTeam(_attackerTeam);
			BattleTeam _defencerTeam = CurAttacker.getOppositeTeam(_attackerTeam);
			BattleTeamShot _teamSlot = new BattleTeamShot(Battle, _attackerTeam, _defencerTeam);
			return _teamSlot;
		}

		public override void addActorIndex()
		{
			CurAttacker.addCurIndex();
		}

		public override void initOnCreateSubAction()
		{
			BattleTeam _battleTeam = CurAttacker.CurActor;
			_battleTeam.resetOnNewAction();
			//TODO: 每回合开始前将team出手index置为0
			bool _needHungUp = Battle.BattleExcuter.needHungUp(this, _battleTeam);
			if (_needHungUp)
			{
				Battle.updateBattleState(BattleState.HUNGUP);
			}
			//处理初始化操作，比如自动PVP时产生出手信息等�?
			Battle.BattleExcuter.onBattleRoundStart(this, _battleTeam);
		}


		public override bool AllActionFinish
		{
			get
			{
				return CurState != BattleState.HUNGUP;
			}
		}

		public override void createDeadth()
		{
			foreach (BattleTeam _side in CurAttacker.ActorList)
			{
				if (!_side.Alive)
				{
					CurAttacker.DeadBattleSide = _side.BattleSide;
					break;
				}
			}
		}

		public override void onStart()
		{
			// TODO Auto-generated method stub

		}

	}

}