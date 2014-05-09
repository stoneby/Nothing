using System;

namespace com.kx.sglm.gs.battle.share.skill
{

	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using ISingletonBattleAction = com.kx.sglm.gs.battle.share.singleton.ISingletonBattleAction;
	using NormalMonsterAttackAction = com.kx.sglm.gs.battle.share.singleton.NormalMonsterAttackAction;

	public class MonsterSkillManager : AbstractSkillManager
	{

		/// <summary>
		/// 测试使用的Action </summary>
		private NormalMonsterAttackAction curAction;

		private int leftRound;

		public MonsterSkillManager(BattleFighter fighter) : base(fighter)
		{
			curAction = new NormalMonsterAttackAction();
		}

		public override void countDownRound(BattleRoundCountRecord roundRecord)
		{
			countDownRound();
			recordRoundCount(roundRecord);
		}

		public override void beforeBattleStart(BattleRoundCountRecord roundRecord)
		{
			this.leftRound = curAction.DefaultRound;
			recordRoundCount(roundRecord);
		}

		protected internal virtual void recordRoundCount(BattleRoundCountRecord roundRecord)
		{
			SingleActionRecord _record = roundRecord.OrCreateRecord;
			recordRound(_record);
			roundRecord.finishCurRecord();
		}

		protected internal virtual void recordRound(SingleActionRecord singleRecord)
		{
			BattleRecordHelper.initSingleRecord(Fighter, singleRecord);
			singleRecord.addState(BattleKeyConstants.BATTLE_STATE_MONSTER_SKILL_ROUND, LeftRound);
		}

		public virtual bool needAttack()
		{
			return leftRound == 0;
		}

		public virtual int LeftRound
		{
			get
			{
				return leftRound;
			}
		}

		protected internal virtual void resetLeftRound()
		{
			leftRound = curAction.DefaultRound;
		}

		public override ISingletonBattleAction FightAction
		{
			get
			{
				return curAction;
			}
		}


		public override bool canAttack()
		{
			return leftRound <= 0;
		}

		protected internal virtual void countDownRound()
		{
			leftRound--;
			Console.WriteLine("cur left round is " + leftRound + ", index is " + Fighter.Index);
		}

		public override void beforeAttack(BattleFightRecord record)
		{
			// TODO: add logic
		}

		public override void afterAttack(BattleFightRecord record)
		{
			// 也许会更换技�?
			resetLeftRound();
			recordRound(record.getAttackAction());
		}

	}

}