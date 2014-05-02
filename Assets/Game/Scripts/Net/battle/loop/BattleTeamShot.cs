namespace com.kx.sglm.gs.battle.logic.loop
{

	using BattleFighter = com.kx.sglm.gs.battle.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.actor.impl.BattleTeam;
	using com.kx.sglm.gs.battle.logic;
	using BattleAttackAction = com.kx.sglm.gs.battle.logic.action.BattleAttackAction;

	/// <summary>
	/// 单方队伍出手循环控制子类， 对手是一个<seealso cref="BattleTeam"/>。 该类内循环一个<seealso cref="BattleTeam"/>内所有要出手的对象，创建
	/// <seealso cref="BattleAttackAction"/>进行下一步操作。 出手全部完成后需要进行RoundCounter的结算
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleTeamShot : AbstractBattleLooper<BattleAttackAction, BattleTeam>
	{

		public BattleTeamShot(Battle battle, BattleTeam attackTeam, BattleTeam defencerTeam) : base(battle, attackTeam, defencerTeam)
		{

		}


		public override void onStart()
		{
			Battle.BattleExcuter.onBattleTeamShotStart(this);
		}

		public override void onFinish()
		{
			// 结算RoundCounter
			CurAttacker.calcRoundCounter();
			CurDefencer.calcRoundCounter();
			// 如果是玩家处理下一批武将的入场
			Battle.BattleExcuter.onBattleTeamShotFinish(this);
			Record.finishCurTeamRecord();
		}

		public override bool hasNextSubAction()
		{
			return CurAttacker.hasFightFighter();
		}

		public override BattleAttackAction createSubActionByType()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.actor.impl.BattleFighter _curFighter = getCurAttacker().getCurFighter();
			BattleFighter _curFighter = CurAttacker.CurFighter;
			// TODO: 这里是否存在同一个Team在每次出手内攻击不同的Team，可以之后向策划确认，暂时不支持
			BattleAttackAction _fightAction = new BattleAttackAction(Battle, _curFighter, CurDefencer);

			return _fightAction;
		}

		public override void addActorIndex()
		{
			CurAttacker.addCurFightIndex();

		}

		public override void initOnCreateSubAction()
		{
			//初始化战报
		}


		public override bool AllActionFinish
		{
			get
			{
				return CurAttacker.hasCurActor();
			}
		}

		public override void createDeadth()
		{
			CurAttacker.tryDead();
			CurDefencer.tryDead();
		}


	}

}