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
	/// ���һ��<seealso cref="BattleFighter"/>��һ�γ��֣����ֶ�����һ��<seealso cref="BattleTeam"/>�� ����࿪ʼ���������߼�ת������߼��� <seealso cref="BattleAttackAction"/>��һ�ν�����ζ�ŵ����佫/������ֵĽ�����</br> �������ʵ����������<seealso cref="BattleTeamShot"/>����ѭ��
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
				//TODO: �������������߼�����
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
				//�����
				return defencerTeam.FighterType == FighterType.HERO;
			}
		}

	}

}