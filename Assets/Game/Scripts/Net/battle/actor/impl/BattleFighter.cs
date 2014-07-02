namespace com.kx.sglm.gs.battle.share.actor.impl
{

	using BattleBuffManager = com.kx.sglm.gs.battle.share.buff.BattleBuffManager;
	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
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
		/// 职业 </summary>
		protected internal int job;

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

		public virtual void updateStateRecord(SingleActionRecord record)
		{
			stateManager.updateStateRecord(record);
		}

		public virtual void afterAttack(BattleFightRecord record)
		{
			skillManager.afterAttack(record);
		}

		public virtual void afterDefence(BattleFightRecord record)
		{

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

		public virtual int CurHp
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

		public virtual BattleTeam getOwnerTeam()
		{
			return ownerTeam;
		}

		public virtual int FighterState
		{
			get
			{
				return stateManager.FighterStateFlag;
			}
		}

		public virtual int AttackVal
		{
			get
			{
				return fighterProp.BattleProp.getAsInt(RoleAProperty.ATK);
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
				return job;
			}
			set
			{
				this.job = value;
			}
		}


		public virtual void addBuff(int buffId)
		{
			buffManager.addBuff(buffId);
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

		public virtual void onSceneStart(SceneStartEvent @event)
		{
			skillManager.onSceneStart(@event);
			buffManager.activeAllBuff(BattleConstants.BUFF_ALL_FALG);
		}


		public virtual void onSceneStop()
		{
			buffManager.clearAllBuff();
		}


		public virtual void onTeamShotStart(TeamShotStartEvent @event)
		{
			skillManager.onTeamShotStart(@event);
			buffManager.onTeamShotStart(@event);
			FighterProp.recalcBattleProp();
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


		public virtual int FighterTotalHp
		{
			get
			{
				if (!baseProp.BattleProperty.ContainsKey(RoleAProperty.HP))
				{
					Logger.Log("error");
				}
				return baseProp.BattleProperty[RoleAProperty.HP];
			}
		}

		public virtual int TotalMp
		{
			get
			{
				return baseProp.BattleProperty[RoleAProperty.MP];
			}
		}

		public virtual void changeCurHp(int addHp)
		{
			int _totalHp = addHp + CurHp;
			if (_totalHp >= FighterTotalHp)
			{
				_totalHp = FighterTotalHp;
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

		public virtual BattleSideEnum BattleSide
		{
			get
			{
				return ownerTeam.BattleSide;
			}
		}


		public virtual void clearState()
		{
			stateManager.clearState();
		}

		public virtual void addState(BattleFighterState state)
		{
			stateManager.addState(state);
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
				return Hero ? getOwnerTeam().TotalHp : FighterTotalHp;
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


	}

}