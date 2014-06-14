using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.skill.manager
{


	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleFighter = com.kx.sglm.gs.battle.share.actor.impl.BattleFighter;
	using HeroTeam = com.kx.sglm.gs.battle.share.actor.impl.HeroTeam;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRecord = com.kx.sglm.gs.battle.share.data.record.BattleRecord;
	using BattleRecordConstants = com.kx.sglm.gs.battle.share.data.record.BattleRecordConstants;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleSkillRecord = com.kx.sglm.gs.battle.share.data.record.BattleSkillRecord;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using SceneStartEvent = com.kx.sglm.gs.battle.share.@event.impl.SceneStartEvent;
	using TeamShotStartEvent = com.kx.sglm.gs.battle.share.@event.impl.TeamShotStartEvent;
	using BattleRecordHelper = com.kx.sglm.gs.battle.share.helper.BattleRecordHelper;
	using BaseHeroBattleSkillAction = com.kx.sglm.gs.battle.share.skill.action.BaseHeroBattleSkillAction;
	using SkillActionHolder = com.kx.sglm.gs.battle.share.skill.model.SkillActionHolder;
	using ArrayUtils = com.kx.sglm.gs.battle.share.utils.ArrayUtils;

	/// <summary>
	/// 英雄战斗内技能管理器
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleHeroSkillManager : AbstractBattleSkillManager
	{

		private HeroTeam ownerTeam;

		private BaseHeroBattleSkillAction attackAction;

		private BaseHeroBattleSkillAction activeAction;

		private SkillActionHolder[] actionHolderArr;

		public BattleHeroSkillManager(BattleFighter fighter, HeroTeam ownerTeam) : base(fighter)
		{
			// TODO: 这里之后是单例的Action
			this.ownerTeam = ownerTeam;
			init();
			// TODO: 以后重构
		}

		public override void init()
		{
			attackAction = SkillService.NormalHeroAttack;
			initHolder();
			initAllSkill();
		}

		protected internal virtual void initHolder()
		{
			this.actionHolderArr = new SkillActionHolder[BattleEventConstants.SIZE];
			for (int _i = 0; _i < BattleEventConstants.SIZE; _i++)
			{
				actionHolderArr[_i] = new SkillActionHolder(_i);
			}
		}

		protected internal virtual void initAllSkill()
		{
			List<int> _skillIds = Fighter.BaseProp.SkillIdList;
			List<BaseHeroBattleSkillAction> _allSkills = SkillService.getSkillAction(_skillIds);
			foreach (BaseHeroBattleSkillAction _action in _allSkills)
			{
				SkillActionHolder _holder = actionHolderArr[_action.TriggerId];
				_holder.addSkillAction(_action);
				if (_action.TriggerId == BattleEventConstants.BATTLE_HUNG_UP)
				{
					activeAction = _action;
				}
			}
			if (activeAction == null)
			{
				Logger.Log("no active action");
			}
		}

		protected internal virtual List<BaseHeroBattleSkillAction> getSkillAction(int trigger, BattleFighter attacker)
		{
			List<BaseHeroBattleSkillAction> _actionList = new List<BaseHeroBattleSkillAction>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.skill.model.SkillActionHolder _holder = actionHolderArr[trigger];
			SkillActionHolder _holder = actionHolderArr[trigger];
			if (_holder != null)
			{
				_actionList.AddRange(_holder.getActAction(attacker));
			}
			return _actionList;
		}

		public virtual void calcAttackBattleAction(BattleTeamFightRecord record)
		{
			BaseHeroBattleSkillAction _attackAct = null;
			List<BaseHeroBattleSkillAction> _actList = getToActList(BattleEventConstants.BATTLE_FIGHTER_ATTACK);
			if (_actList.Count == 0)
			{
				_attackAct = NormalAttack;
			}
			else
			{
				_attackAct = _actList[0];
			}
			attackAction = _attackAct;
			recordSPSkillInfo(record);
		}

		protected internal virtual void recordSPSkillInfo(BattleTeamFightRecord record)
		{
			if (!attackAction.NormalAction)
			{
				record.addSkillFighter(Fighter.TemplateId);
			}
		}

		public virtual List<BaseHeroBattleSkillAction> getToActList(int eventIndex)
		{
			List<BaseHeroBattleSkillAction> _actionList = new List<BaseHeroBattleSkillAction>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final com.kx.sglm.gs.battle.share.skill.model.SkillActionHolder _holder = getActionHolder(eventIndex);
			SkillActionHolder _holder = getActionHolder(eventIndex);
			if (_holder != null)
			{
				_actionList.AddRange(_holder.getActAction(Fighter));
			}

			return _actionList;
		}

		public override ISingletonSkillAction AttackAction
		{
			get
			{
				HeroColor _color = ownerTeam.CurFightColor;
				ISingletonSkillAction _curAction = null;
				if (_color.Recover)
				{
					_curAction = NormalRecover;
				}
				else
				{
					_curAction = attackAction;
				}
				return _curAction;
			}
		}

		public override bool canAttack()
		{
			return true;
		}

		public override void onAttack(BattleFightRecord fightRecord)
		{
			action(AttackAction, fightRecord);
		}

		protected internal virtual SkillActionHolder getActionHolder(int eventIndex)
		{
			SkillActionHolder _holder = null;
			if (!ArrayUtils.isRightArrayIndex(eventIndex, actionHolderArr))
			{
				return _holder;
			}
			_holder = actionHolderArr[eventIndex];
			return _holder;
		}

		public virtual BaseHeroBattleSkillAction ActiveAction
		{
			set
			{
				this.activeAction = value;
			}
		}

		public override void onActiveOption()
		{
			if (!canActiveSkill())
			{
				return;
			}
			BattleRecord _record = Battle.Record;
			BattleSkillRecord _actSkillRecord = _record.OrCreateSkillRecord;
			BattleRecordHelper.initBattelSkill(_actSkillRecord, Fighter);
			BattleFightRecord _fightRecord = _actSkillRecord.OrCreateFightRecord;
			if (!activeAction.canOption(Fighter))
			{
				Logger.Log("cannot.option.activeSkill");
				return;
			}
			ownerTeam.costCurMp(activeAction.CostMp);
			_actSkillRecord.addProp(BattleRecordConstants.BATTLE_HERO_PROP_MP, ownerTeam.CurMp);
			action(activeAction, _fightRecord);
			_record.finishCurSkillRecord();
		}


		protected internal virtual bool canActiveSkill()
		{
			return hasEnoughMP() && ActiveFighter;
		}

		protected internal virtual bool hasEnoughMP()
		{
			return Fighter.getOwnerTeam().CurMp >= activeAction.CostMp;
		}

		protected internal virtual bool ActiveFighter
		{
			get
			{
				return MathUtils.hasFlagIndex(BattleConstants.FIGHTER_ACTIVE_SKILL_FLAG, Fighter.Index);
			}
		}

		public override void beforeAttack(BattleFightRecord record)
		{
			// TODO Auto-generated method stub

		}

		public override void afterAttack(BattleFightRecord record)
		{
			// TODO Auto-generated method stub

		}

		public override void countDownRound(BattleRoundCountRecord roundRecord)
		{

		}

		public override void beforeBattleStart(BattleRoundCountRecord roundRecord)
		{

		}

		public override void onSceneStart(SceneStartEvent @event)
		{

		}

		public override void onTeamShotStart(TeamShotStartEvent @event)
		{
		}

		public virtual BaseHeroBattleSkillAction NormalAttack
		{
			get
			{
				return SkillService.NormalHeroAttack;
			}
		}

		public virtual BaseHeroBattleSkillAction NormalRecover
		{
			get
			{
				return SkillService.NormalHeroRecover;
			}
		}

		public override void onHandleEvent(BattleTeamFightRecord record)
		{
			calcAttackBattleAction(record);
		}

	}

}