using System.Collections.Generic;

namespace com.kx.sglm.gs.battle.share.actor.impl
{


	using IBattleBuffManager = com.kx.sglm.gs.battle.share.buff.IBattleBuffManager;
	using FighterInfo = com.kx.sglm.gs.battle.share.data.FighterInfo;
	using BattleFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleFightRecord;
	using BattleRoundCountRecord = com.kx.sglm.gs.battle.share.data.record.BattleRoundCountRecord;
	using BattleTeamFightRecord = com.kx.sglm.gs.battle.share.data.record.BattleTeamFightRecord;
	using SingleActionRecord = com.kx.sglm.gs.battle.share.data.record.SingleActionRecord;
	using BattleSideEnum = com.kx.sglm.gs.battle.share.enums.BattleSideEnum;
	using FighterState = com.kx.sglm.gs.battle.share.enums.FighterState;
	using HeroColor = com.kx.sglm.gs.battle.share.enums.HeroColor;
	using InnerBattleEvent = com.kx.sglm.gs.battle.share.@event.InnerBattleEvent;
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
		/// 当前攻击 </summary>
		protected internal int attack;

		/// <summary>
		/// 当前回复 </summary>
		protected internal int recover;

		/// <summary>
		/// 当前防御 </summary>
		protected internal int defence;

		/// <summary>
		/// 当前免伤 </summary>
		protected internal int damageFree;

		/// <summary>
		/// 职业 </summary>
		protected internal int job;

		/// <summary>
		/// 武将名字 </summary>
		protected internal string name;

		protected internal bool dead;

		protected internal readonly FighterInfo baseProp;

		protected internal BattleTeam ownerTeam;

		protected internal IBattleBuffManager buffManager;

		protected internal IBattleSkillManager skillManager;

		protected internal List<FighterState> stateList;


		public BattleFighter(BattleTeam ownerTeam, FighterInfo baseProp)
		{
			this.baseProp = baseProp;
			this.dead = false;
			this.stateList = new List<FighterState>();
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
			foreach (FighterState _state in stateList)
			{
				if (!_canAttack)
				{
					break;
				}
				_canAttack = _state.FightAble;
			}
			return _canAttack;

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

		public virtual int AttackVal
		{
			get
			{
				return attack;
			}
		}

		public virtual int DamageFree
		{
			get
			{
				return damageFree;
			}
			set
			{
				this.damageFree = value;
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
				return defence;
			}
			set
			{
				this.defence = value;
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


		public virtual void addBuff()
		{
			// TODO: add buff logic
		}

		public virtual IBattleBuffManager BuffManager
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
		}

		public virtual ISingletonSkillAction FightAction
		{
			get
			{
				return skillManager.AttackAction;
			}
		}

		public virtual void onTeamShotStart(TeamShotStartEvent @event)
		{
			skillManager.onTeamShotStart(@event);
		}

		public virtual int getFighterOtherProp(int key)
		{
			return baseProp.getIntProp(key);
		}



		public virtual int TotalHp
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

		public virtual BattleSideEnum BattleSide
		{
			get
			{
				return ownerTeam.BattleSide;
			}
		}

		public virtual void onRoundFinish(BattleRoundCountRecord roundRecord)
		{
		    if (Dead)
		    {
		        return;
		    }
			skillManager.countDownRound(roundRecord);
			// buffManager.countDownRound();//TODO: 暂时没有BUFF
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


		public virtual int Attack
		{
			get
			{
				return attack;
			}
			set
			{
				this.attack = value;
			}
		}


		public virtual int Recover
		{
			get
			{
				return recover;
			}
			set
			{
				this.recover = value;
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


		public virtual List<FighterState> StateList
		{
			get
			{
				return stateList;
			}
			set
			{
				this.stateList = value;
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

		public virtual void onHandleFightEvent(BattleTeamFightRecord record)
		{
			skillManager.onHandleEvent(record);
		}

	}

}