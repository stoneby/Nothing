using System;
using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.actor.impl
{

	using MathUtils = com.kx.sglm.core.util.MathUtils;
	using BattleBuffManager = com.kx.sglm.gs.battle.share.buff.BattleBuffManager;
	using BuffInfo = com.kx.sglm.gs.battle.share.buff.BuffInfo;
	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using SingleFighterRecord = com.kx.sglm.gs.battle.share.data.record.SingleFighterRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
	using BeforeAttackEvent = com.kx.sglm.gs.battle.share.@event.impl.BeforeAttackEvent;
	using SceneStartEvent = com.kx.sglm.gs.battle.share.@event.impl.SceneStartEvent;
	using TeamShotStartEvent = com.kx.sglm.gs.battle.share.@event.impl.TeamShotStartEvent;
	using IBattleSkillManager = com.kx.sglm.gs.battle.share.skill.IBattleSkillManager;
	using ISingletonSkillAction = com.kx.sglm.gs.battle.share.skill.ISingletonSkillAction;
	using RoleAProperty = com.kx.sglm.gs.hero.properties.RoleAProperty;

	/// <summary>
	/// 战斗武将，包括Monster和Hero
	/// 
	/// @author liyuan2
	/// 
	/// </summary>
	public class BattleFighter : IBattleSideActor
	{

		/// <summary>
		/// 战斗武将的位置，初始化后不再变化 </summary>
		protected internal int index;

		/// <summary>
		/// 当前血量 </summary>
		protected internal int curHp;

		/// <summary>
		/// 武将名字 </summary>
		protected internal string name;

		protected internal bool dead;

		protected internal readonly FighterInfo baseProp;

		protected internal BattleTeam ownerTeam;

		protected internal BattleBuffManager buffManager;

		protected internal IBattleSkillManager skillManager;

		protected internal FighterStateManager stateManager;

		protected internal BattleFighterPropty fighterProp;

		public BattleFighter(BattleTeam ownerTeam, FighterInfo baseProp)
		{
			this.baseProp = baseProp;
			this.dead = false;
			this.stateManager = new FighterStateManager(this);
			this.fighterProp = new BattleFighterPropty(this);
		}

		public virtual IBattleSkillManager SkillManager
		{
			set
			{
				this.skillManager = value;
			}
			get
			{
				return skillManager;
			}
		}

		public virtual bool canAttack()
		{
			bool _canAttack = !dead;
			if (_canAttack)
			{
				_canAttack = skillManager.canAttack();
			}
			if (_canAttack)
			{
				_canAttack = stateManager.canStateAttack();
			}
			return _canAttack;

		}

		public virtual void updateStateRecord(SingleFighterRecord record)
		{
			stateManager.updateStateRecord(record);
		}

		public virtual bool hasState(int state)
		{
			return stateManager.hasState(state);
		}

		public virtual void attackerAfterSkillAction(BattleFightRecord record)
		{
			skillManager.afterAttack(record);
		}

		public virtual void afterDefence(BattleFighter attacker, BattleFightRecord record)
		{
			buffManager.onDefence(attacker, record);
		}

		public virtual void useActiveSkill()
		{
			skillManager.onActiveOption();
		}

		public virtual void activeBuff(int buffFlag)
		{
			buffManager.activeAllBuff(buffFlag);
		}

		public virtual void onCostHp(int costHp)
		{
			this.curHp -= costHp;
		}

		protected internal virtual int CurHp
		{
			get
			{
				return curHp;
			}
			set
			{
				this.curHp = value;
			}
		}

		public virtual int FighterCurHp
		{
			get
			{
				return getOwnerTeam().getFighterCurHp(this);
			}
		}

		public virtual int FighterTotalHp
		{
			get
			{
				return getOwnerTeam().getFighterTotalHp(this);
			}
		}

		public virtual void changeFighterHp(int changeHp)
		{
			getOwnerTeam().changeHp(changeHp, this);
		}

		public virtual int FighterCurHpPercent
		{
			get
			{
				float _leftHp = FighterCurHp;
				float _totalHp = FighterTotalHp;
				float _percent = _leftHp / _totalHp;
				return (int)(_percent * BattleConstants.BATTLE_RATIO_BASE);
			}
		}

		public virtual BattleTeam getOwnerTeam()
		{
			return ownerTeam;
		}

		public virtual int FighterStateFlag
		{
			get
			{
				return stateManager.FighterStateFlag;
			}
		}

		public virtual BattleFighterState getFighterState(int buffId)
		{
			return stateManager.getFighterState(buffId);
		}

		public virtual int AttackVal
		{
			get
			{
				return fighterProp.BattleProp.getAsInt(RoleAProperty.ATK);
			}
		}

		public virtual int DamageMuti
		{
			get
			{
				return fighterProp.BattleProp.getAsInt(RoleAProperty.INCRDAMAGE);
			}
		}

		public virtual int DamageFree
		{
			get
			{
				return fighterProp.BattleProp.getAsInt(RoleAProperty.DECRDAMAGE);
			}
		}

		public virtual void resetOnNewAction()
		{
			// TODO: reset prop
		}

		public virtual bool hasHp()
		{
			return CurHp > 0;
		}

		/// <summary>
		/// 在攻击时，如果一只fighter死亡，攻击队列并不会更改目标，所以使用了<seealso cref="#hasHp()"/>和<seealso cref="#isDead()"/>，处理不同的需求
		/// 
		/// @return
		/// </summary>
		public virtual bool Dead
		{
			get
			{
				return dead;
			}
			set
			{
				this.dead = value;
			}
		}

		public virtual int Defence
		{
			get
			{
				return fighterProp.BattleProp.getAsInt(RoleAProperty.DEFENSE);
			}
		}

		public virtual int Job
		{
			get
			{
				string _jobStr = baseProp.getProp(BattleKeyConstants.BATTLE_KEY_HERO_JOB);
				return _jobStr.Length == 0 ? 0 : Convert.ToInt32(_jobStr);
			}
		}

		public virtual void addBuff(int buffId)
		{
			BuffInfo _info = new BuffInfo(buffId, null);
			addBuff(_info);

		}

		public virtual void addBuff(BuffInfo buffInfo)
		{
			if (!hasHp())
			{
				return;
			}
			buffManager.addBuff(buffInfo);
		}

		public virtual BattleBuffManager BuffManager
		{
			get
			{
				return buffManager;
			}
			set
			{
				this.buffManager = value;
			}
		}

		public virtual void onAttack(BattleFightRecord fightRecord)
		{
			skillManager.onAttack(fightRecord);
			buffManager.onAttack(fightRecord);
		}

		public virtual ISingletonSkillAction FightAction
		{
			get
			{
				return skillManager.AttackAction;
			}
		}

		public virtual void activeSceneStartSkill(SceneStartEvent @event)
		{
			skillManager.onSceneStart(@event);

		}

		public virtual void activeSceneStartBuff(SceneStartEvent @event)
		{
			buffManager.activeAllBuff(BattleConstants.BUFF_ALL_FALG);
			effectAllBuff();
		}

		public virtual void onSceneStop()
		{
			buffManager.onSceneStop();
	//		stateManager.clearState();
			stateManager.clearState();
			recalcProp();
		}

		/// <summary>
		/// 分为对方出手和己方出手，可能对方出手的回合己方也会有动作 </summary>
		/// <param name="event"> </param>
		public virtual void onTeamShotStart(TeamShotStartEvent @event)
		{
			skillManager.onTeamShotStart(@event);
		}

		public virtual void recalcProp()
		{
			fighterProp.recalcBattleProp();
		}

		public virtual void onHandleFightInputAction(BattleTeamFightRecord record)
		{
			skillManager.onHandleInputAction(record);
		}

		public virtual void onRoundFinish(BattleRoundCountRecord roundRecord)
		{
			if (Dead)
			{
				return;
			}
			skillManager.countDownRound(roundRecord);
			buffManager.onRoundFinish(roundRecord);
		}

		public virtual int getFighterOtherProp(int key)
		{
			return baseProp.getIntProp(key);
		}



		protected internal virtual int TotalHp
		{
			get
			{
				return (int)fighterProp.BattleProp.get(RoleAProperty.HP);
			}
		}

		public virtual int TotalMp
		{
			get
			{
				return (int)fighterProp.BattleProp.get(RoleAProperty.MP);
			}
		}

		internal virtual void changeCurHp(int addHp)
		{
			int _totalHp = addHp + CurHp;
			if (_totalHp >= TotalHp)
			{
				_totalHp = TotalHp;
			}
			if (_totalHp < 0)
			{
				_totalHp = 0;
			}
			CurHp = _totalHp;
		}


		public virtual void onDead()
		{
			// TODO Auto-generated method stub

		}

		public virtual Battle Battle
		{
			get
			{
				return ownerTeam.Battle;
			}
		}

		public virtual void tryDead()
		{
			if (curHp == 0)
			{
				Dead = true;
			}
		}

		public virtual void randomSpMax()
		{
			float _lucky = fighterProp.BattleProp.get(RoleAProperty.LUCY);
			if (MathUtils.randomRate(_lucky, BattleConstants.BATTLE_RATIO_BASE))
			{
				addSpMaxBuff();
			}
		}

		public virtual void addSpMaxBuff()
		{
			addBuff(BattleActionService.Service.SpMaxBuffId);
		}

		public virtual BattleSideEnum BattleSide
		{
			get
			{
				return ownerTeam.BattleSide;
			}
		}


		public virtual void refreshState()
		{
			stateManager.backupState();
			stateManager.clearState();
		}

		public virtual void addState(BattleFighterState state)
		{
			stateManager.addState(state);
		}


		public virtual void removeStateFlag(int stateId)
		{
			stateManager.removeState(stateId);
		}

		public virtual int Side
		{
			get
			{
				return getOwnerTeam().BattleSide.Index;
			}
		}

		public virtual int Index
		{
			get
			{
				return index;
			}
			set
			{
				this.index = value;
			}
		}


		public virtual int Recover
		{
			get
			{
				return fighterProp.BattleProp.getAsInt(RoleAProperty.RECOVER);
			}
			set
			{
				fighterProp.BaseProp.set(RoleAProperty.RECOVER, value);
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
			}
		}


		public virtual int LeaderSkillId
		{
			get
			{
				return BaseProp.LeaderSkillId;
			}
		}

		public virtual FighterInfo BaseProp
		{
			get
			{
				return baseProp;
			}
		}


		public virtual void setOwnerTeam(BattleTeam ownerTeam)
		{
			this.ownerTeam = ownerTeam;
		}


		public virtual void changeColor(HeroColor color, SingleActionRecord singleRecord)
		{
			getOwnerTeam().changeFightColor(index, color, singleRecord);
		}

		public virtual void handleEvent(InnerBattleEvent @event)
		{
			// TODO Auto-generated method stub

		}

		public virtual int TemplateId
		{
			get
			{
				return baseProp.getIntProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE);
			}
		}

		public virtual BattleFighterPropty FighterProp
		{
			get
			{
				return fighterProp;
			}
		}

		public virtual int BattleTotalHp
		{
			get
			{
				return Hero ? getOwnerTeam().TotalHp : TotalHp;
			}
		}

		public virtual bool Hero
		{
			get
			{
				return getOwnerTeam().FighterType.Hero;
			}
		}

		public virtual int AliveCostMaxHp
		{
			get
			{
				return BattleTotalHp - BattleConstants.FIGHTER_ALIVE_MIN_HP;
			}
		}

		public virtual int Attack
		{
			set
			{
				fighterProp.BaseProp.set(RoleAProperty.ATK, value);
			}
		}


		public virtual bool Leader
		{
			get
			{
				return MathUtils.hasFlagIndex(BattleConstants.FIGHTER_LEADER_SKILL_FLAG, Index);
			}
		}

		public virtual bool ActiveFighter
		{
			get
			{
				return getOwnerTeam().isActiveFighter(this);
			}
		}

		public virtual bool isSameFighter(BattleFighter fighter)
		{
			return Index == fighter.Index;
		}

		public virtual Dictionary<int, int> BattleProp
		{
			get
			{
				return fighterProp.BattleProp.PropMaps;
			}
		}

		public virtual void beforeAttack(BeforeAttackEvent @event)
		{
			skillManager.beforeAttack(@event);
		}

		public virtual void effectAllBuff()
		{
			buffManager.recalcPropEffectBuffs();
		}

	}

}