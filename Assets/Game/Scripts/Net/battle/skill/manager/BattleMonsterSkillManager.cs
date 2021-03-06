using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.manager
{


	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using MonsterAI = com.kx.sglm.gs.battle.share.ai.MonsterAI;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BeforeAttackEvent = com.kx.sglm.gs.battle.share.@event.impl.BeforeAttackEvent;
	using SceneStartEvent = com.kx.sglm.gs.battle.share.@event.impl.SceneStartEvent;
	using TeamShotStartEvent = com.kx.sglm.gs.battle.share.@event.impl.TeamShotStartEvent;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using BaseMonsterSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseMonsterSkillAction;

	/// <summary>
	/// 战斗内怪物技能管理器
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleMonsterSkillManager : AbstractBattleSkillManager
	{

		/// <summary>
		/// 怪物AI管理 </summary>
		private MonsterAI monsterAI;
		/// <summary>
		/// 怪物技能列表 </summary>
		private Dictionary<int, BaseMonsterSkillAction> monsterSkillMap;
		private BaseMonsterSkillAction curAction;
		private int leftRound;
		private bool inExtraRound;

		public BattleMonsterSkillManager(BattleFighter fighter) : base(fighter)
		{
			monsterSkillMap = new Dictionary<int, BaseMonsterSkillAction>();
			init();
		}

		public override void init()
		{
			monsterSkillMap = new Dictionary<int, BaseMonsterSkillAction>();
			int _aiId = Owner.BaseProp.AiId;
			monsterAI = SkillService.getMonsterAI(_aiId);
			Dictionary<int, BaseMonsterSkillAction> _aiSKills = SkillService.getSkillActions(monsterAI);
			foreach (KeyValuePair<int, BaseMonsterSkillAction> _entry in _aiSKills)
			{
				monsterSkillMap[_entry.Key] = _entry.Value;
			}
			resetLeftRound();
		}

		public override void countDownRound(BattleRoundCountRecord roundRecord)
		{
			countDownRound();
			recordRoundCount(roundRecord);
		}

		public override void beforeBattleStart(BattleRoundCountRecord roundRecord)
		{
			this.leftRound = Owner.getFighterOtherProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DEFAULT_CD);
			recordRoundCount(roundRecord);
		}

		public override void beforeAttack(BeforeAttackEvent @event)
		{
			// TODO: add logic
		}

		public override void afterAttack(BattleFightRecord record)
		{
			// 也许会更换技能
			resetLeftRound();
			recordRound(record.OrCreateAttack);
		}

		public override void onSceneStart(SceneStartEvent @event)
		{
			if (hasShield())
			{
				Owner.addBuff(monsterAI.ShieldBuffId);
			}
		}

		public override void onAttack(BattleFightRecord fightRecord)
		{
			// 假设所有的技能都有额外CD，先更新
			if (!inExtraRound)
			{
				addExtraCDRound(fightRecord);
				InExtraRound = true;
			}
			// 如果当前剩余回合为0，执行动作
			if (CDZero)
			{
				action(curAction, fightRecord);
				InExtraRound = false;
			}

		}

		protected internal virtual bool needAddExtraRound()
		{
			return !inExtraRound && curAction.hasExtraRound();
		}

		protected internal virtual void addExtraCDRound(BattleFightRecord fightRecord)
		{
			// TODO：如果当前没有CD，就不用变了，因为走到#onFightAction方法的时候CD必然是零
			leftRound = curAction.ExtraCD;
			if (!CDZero)
			{
				recordRound(fightRecord.OrCreateAttack);
			}
		}

		protected internal virtual bool CDZero
		{
			get
			{
				return LeftRound <= 0;
			}
		}

		protected internal virtual bool InExtraRound
		{
			set
			{
				this.inExtraRound = value;
			}
		}

		protected internal virtual void calcCurAction()
		{
			if (inExtraRound)
			{
				//如果是额外回合中，不重算
				return;
			}
			int _skillId = monsterAI.calcCurSkill(Owner);
			curAction = getAction(_skillId);
		}

		protected internal virtual void recordRoundCount(BattleRoundCountRecord roundRecord)
		{
			SingleActionRecord _record = roundRecord.OrCreateRecord;
			recordRound(_record);
			roundRecord.finishCurRecord();
		}

		protected internal virtual void recordRound(SingleActionRecord singleRecord)
		{
			BattleRecordHelper.initSingleRecord(Owner, singleRecord);
			singleRecord.addProp(BattleRecordConstants.BATTLE_MONSTER_SKILL_ROUND, LeftRound);
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
			leftRound = DefaultCDRound;
		}

		public override ISingletonSkillAction AttackAction
		{
			get
			{
				return curAction;
			}
		}

		protected internal virtual int DefaultCDRound
		{
			get
			{
				return Owner.getFighterOtherProp(BattleKeyConstants.BATTLE_PROP_MONSTER_DEFAULT_CD);
			}
		}

		public override bool canAttack()
		{
			return CDZero;
		}

		protected internal virtual void countDownRound()
		{
			leftRound--;
			Console.WriteLine("cur left round is " + leftRound + ", index is " + Owner.Index);
		}

		protected internal virtual BaseMonsterSkillAction getAction(int skillId)
		{
			return monsterSkillMap[skillId];
		}

		public virtual MonsterAI MonsterAI
		{
			set
			{
				this.monsterAI = value;
			}
		}

		public override void onTeamShotStart(TeamShotStartEvent @event)
		{
			calcCurAction();
		}

		public override void onHandleInputAction(BattleTeamFightRecord record)
		{
			// TODO Auto-generated method stub

		}

		public virtual bool hasShield()
		{
			return monsterAI.ShieldBuffId != 0;
		}

		public virtual void setSheildLeftRound()
		{
			leftRound = monsterAI.MonsterShield.BreakShieldCd;
		}

		protected internal override ISingletonSkillAction getSkill(int skillId)
		{
			return SkillService.getMonsterSkillAction(skillId);
		}

	}

}