using System;

namespace com.kx.sglm.gs.battle.share.logic.action
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleTeam = com.kx.sglm.gs.battle.share.actor.impl.BattleTeam;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using BattleState = com.kx.sglm.gs.battle.share.enums.BattleState;
	using FighterType = com.kx.sglm.gs.battle.share.enums.FighterType;
	using BattleTeamShot = com.kx.sglm.gs.battle.share.logic.loop.BattleTeamShot;

	/// <summary>
	/// 针对一个<seealso cref="BattleFighter"/>的一次出手，出手对象是一个<seealso cref="BattleTeam"/>。 这个类开始，从流程逻辑转向具体逻辑。 <seealso cref="BattleAttackAction"/>的一次结束意味着单个武将/怪物出手的结束。</br> 这个类在实际运行中由<seealso cref="BattleTeamShot"/>控制循环
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleAttackAction : AbstractBattleSingleAction, IBattleLoop
	{

		private bool finished;

		protected internal BattleTeam defencerTeam;

		public BattleAttackAction(Battle battle, BattleFighter attacker, BattleTeam defencerTeam) : base(battle, attacker)
		{
			this.defencerTeam = defencerTeam;
		}

	//	@Override
		public virtual void updateBattleState(BattleState battelState, bool updateSub)
		{

			// TODO: add logic here
		}

	//	@Override
		public virtual bool Finished
		{
			get
			{
				return finished;
			}
		}

	//	@Override
		public override void onAction()
		{
			if (!attacker.canAttack())
			{
				//TODO: 可能有其他的逻辑处理
				return;
			}
			Console.WriteLine("#BattleAttackAction.onAction, hero type is " + attacker.getOwnerTeam().FighterType.Index);
			Console.WriteLine("#BattleAttackAction.onAction, hero index is " + attacker.Index);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord _teamReocrd = getBattle().getRecord().getOrCreateTeamFighterRecord();
			BattleTeamFightRecord _teamReocrd = Battle.Record.OrCreateTeamFighterRecord;
			BattleFightRecord _figherRecord = _teamReocrd.OrCreateRecord;
			attacker.onAttack(_figherRecord);
			_teamReocrd.finishCurRecord();
		}

	//	@Override
		public virtual bool Dead
		{
			get
			{
				return !defencerTeam.hasHp();
			}
		}

	//	@Override
		public override void createDeadth()
		{

			defencerTeam.tryDead();

		}

	//	@Override
		public virtual void createNewSubAction()
		{
			// TODO Auto-generated method stub

		}

	//	@Override
		public override bool DeadInTime
		{
			get
			{
				//如果是
				return defencerTeam.FighterType == FighterType.HERO;
			}
		}

	}

}